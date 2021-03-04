using System;
using System.Net;
using System.Threading;

namespace TerrLauncherPackCreatorUpdater.Code.Implementations
{
    internal class DownloadSpeedEvaluator : IDisposable
    {
        public delegate void SpeedMeasured(long speedBytesPerSecond);

        private readonly object _updateLock = new();

        private Resources? _resources;

        private DateTime? _lastUpdateTime;
        private long _lastUpdateDownloadedBytes;

        private long _currentSpeed;

        public DownloadSpeedEvaluator(
            WebClient webClient,
            int updatePeriodMs,
            SpeedMeasured onSpeedMeasured
        )
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));
            if (updatePeriodMs < 1)
                throw new ArgumentOutOfRangeException(nameof(updatePeriodMs));
            if (onSpeedMeasured == null)
                throw new ArgumentNullException(nameof(onSpeedMeasured));

            webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            var updateTimer = new Timer(_ => _resources?.OnSpeedMeasured(_currentSpeed), null, 0, updatePeriodMs);

            _resources = new Resources(webClient, onSpeedMeasured, updateTimer);
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (_updateLock)
            {
                DateTime now = DateTime.Now;

                if (_lastUpdateTime.HasValue)
                {
                    double diffSeconds = (now - _lastUpdateTime.Value).TotalSeconds;

                    if (diffSeconds < 1)
                        return;

                    _currentSpeed = (long) ((e.BytesReceived - _lastUpdateDownloadedBytes) / diffSeconds);
                }

                _lastUpdateDownloadedBytes = e.BytesReceived;
                _lastUpdateTime = now;
            }
        }

        public void Dispose()
        {
            if (_resources == null)
                return;

            _resources.WebClient.DownloadProgressChanged -= WebClientOnDownloadProgressChanged;
            _resources.UpdateTimer.Dispose();

            _resources = null;
        }

        private class Resources
        {
            public readonly WebClient WebClient;
            public readonly SpeedMeasured OnSpeedMeasured;
            public readonly Timer UpdateTimer;

            public Resources(WebClient webClient, SpeedMeasured onSpeedMeasured, Timer updateTimer)
            {
                WebClient = webClient;
                OnSpeedMeasured = onSpeedMeasured;
                UpdateTimer = updateTimer;
            }
        }
    }
}
