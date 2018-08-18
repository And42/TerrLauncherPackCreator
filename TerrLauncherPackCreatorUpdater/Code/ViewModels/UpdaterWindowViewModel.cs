using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Shell;
using CommonLibrary;
using CommonLibrary.CommonUtils;
using Ionic.Zip;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreatorUpdater.Code.Implementations;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;

namespace TerrLauncherPackCreatorUpdater.Code.ViewModels
{
    public class UpdaterWindowViewModel
    {
        private readonly TaskbarItemInfo _taskbarItemInfo;

        private DownloadSpeedEvaluator _downloadSpeedEvaluator;
        private WebClient _webClient;
        private bool _started;

        public Property<int> CurrentProgress { get; }
        public Property<long> SpeedInBytes { get; }

        public UpdaterWindowViewModel(TaskbarItemInfo taskbarItemInfo)
        {
            _taskbarItemInfo = taskbarItemInfo ?? throw new ArgumentNullException(nameof(taskbarItemInfo));

            CurrentProgress = new Property<int>();
            SpeedInBytes = new Property<long>();

            taskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            CurrentProgress.PropertyChanged += CurrentProgressOnPropertyChanged;
        }

        public void StartLoading()
        {
            if (_started)
                return;

            _started = true;

            _webClient = new WebClient();

            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _webClient.DownloadDataCompleted += WebClientOnDownloadDataCompleted;

            _downloadSpeedEvaluator = new DownloadSpeedEvaluator(_webClient, 1000, speed => SpeedInBytes.Value = speed);

            _webClient.DownloadDataAsync(new Uri(CommonConstants.LatestVersionZipUrl));
        }

        private void CurrentProgressOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _taskbarItemInfo.ProgressValue = CurrentProgress.Value / 100.0;
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            CurrentProgress.Value = e.ProgressPercentage;
        }

        private void WebClientOnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBoxResult dialogResult = MessageBox.Show(
                    string.Format(StringResources.DownloadingError, e.Error.Message),
                    StringResources.ErrorLower,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Asterisk
                );

                if (dialogResult == MessageBoxResult.No)
                    Environment.Exit(0);

                _webClient.DownloadDataAsync(new Uri(CommonConstants.VersionFileUrl));

                return;
            }

            _downloadSpeedEvaluator.Dispose();

            _webClient.DownloadProgressChanged -= WebClientOnDownloadProgressChanged;
            _webClient.DownloadDataCompleted -= WebClientOnDownloadDataCompleted;
            _webClient.Dispose();

            _downloadSpeedEvaluator = null;
            _webClient = null;

            using (var memoryStream = new MemoryStream(e.Result))
            using (var zip = ZipFile.Read(memoryStream))
            {
                zip.ExtractAll(ApplicationDataUtils.PathToRootFolder, ExtractExistingFileAction.OverwriteSilently);
            }

            Process.Start(
                Path.Combine(ApplicationDataUtils.PathToRootFolder, "updater.exe"),
                $"delete_temp \"{Assembly.GetExecutingAssembly().Location}\""
            );

            Environment.Exit(0);
        }
    }
}
