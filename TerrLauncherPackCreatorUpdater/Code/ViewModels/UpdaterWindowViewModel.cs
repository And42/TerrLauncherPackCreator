using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Shell;
using CommonLibrary;
using CommonLibrary.CommonUtils;
using Ionic.Zip;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreatorUpdater.Code.Implementations;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;

namespace TerrLauncherPackCreatorUpdater.Code.ViewModels
{
    public class UpdaterWindowViewModel : BindableBase
    {
        public int CurrentProgress
        {
            get => _currentProgress;
            set => SetProperty(ref _currentProgress, value);
        }
        public long SpeedInBytes
        {
            get => _speedInBytes;
            set => SetProperty(ref _speedInBytes, value);
        }

        private readonly TaskbarItemInfo _taskbarItemInfo;

        private readonly DownloadSpeedEvaluator _downloadSpeedEvaluator;
        private readonly WebClient _webClient;
        private bool _started;

        #region backing fields
        private int _currentProgress;
        private long _speedInBytes;
        #endregion

        public UpdaterWindowViewModel(TaskbarItemInfo taskbarItemInfo)
        {
            _taskbarItemInfo = taskbarItemInfo ?? throw new ArgumentNullException(nameof(taskbarItemInfo));
            _taskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _webClient.DownloadDataCompleted += WebClientOnDownloadDataCompleted;
            
            _downloadSpeedEvaluator = new DownloadSpeedEvaluator(
                webClient: _webClient,
                updatePeriodMs: 1000,
                onSpeedMeasured: speed => SpeedInBytes = speed
            );
            
            PropertyChanged += OnPropertyChanged;
        }

        public void StartLoading()
        {
            if (_started)
                return;

            _started = true;

            _webClient.DownloadDataAsync(new Uri(CommonConstants.LatestVersionZipUrl));
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
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

            using (var memoryStream = new MemoryStream(e.Result))
            using (var zip = ZipFile.Read(memoryStream))
            {
                zip.ExtractAll(ApplicationDataUtils.PathToRootFolder, ExtractExistingFileAction.OverwriteSilently);
            }

            Process.Start(
                Path.Combine(ApplicationDataUtils.PathToRootFolder, "updater.exe"),
                $"delete_temp \"{Process.GetCurrentProcess().MainModule!.FileName}\""
            );

            Environment.Exit(0);
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentProgress):
                    _taskbarItemInfo.ProgressValue = CurrentProgress / 100.0;
                    break;
            }
        }
    }
}
