using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using CommonLibrary;
using CommonLibrary.CommonUtils;
using Ionic.Zip;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreatorUpdater.Code.Implementations;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;
using Exception = System.Exception;

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

        public async Task StartLoading()
        {
            if (_started)
                return;

            _started = true;

            string latestUrl = string.Empty;
            try
            {
                latestUrl = await UpdateUtils.GetLatestVersionUrlAsync();
            }
            catch (Exception ex)
            {
                CrashUtils.HandleException(ex);

                MessageBox.Show(
                    StringResources.VersionUrlError,
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Environment.Exit(3);
            }

            _webClient = new WebClient();

            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _webClient.DownloadDataCompleted += WebClientOnDownloadDataCompleted;

            _downloadSpeedEvaluator = new DownloadSpeedEvaluator(_webClient, 1000, speed => SpeedInBytes.Value = speed);

            _webClient.DownloadDataAsync(new Uri(latestUrl));
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

                _webClient.DownloadDataAsync(new Uri(CommonConstants.UpdateUrl));

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
