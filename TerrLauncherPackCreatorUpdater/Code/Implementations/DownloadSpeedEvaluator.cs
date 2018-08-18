using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;

namespace TerrLauncherPackCreatorUpdater.Code.Implementations
{
    internal class DownloadSpeedEvaluator : IDisposable
    {
        public delegate void SpeedMeasured(long speedBytesPerSecond);

        private readonly object _updateLock = new object();

        private WebClient _webClient;
        private SpeedMeasured _onSpeedMeasured;
        private Timer _updateTimer;

        private DateTime? _lastUpdateTime;
        private long _lastUpdateDownloadedBytes;

        private long _currentSpeed;

        [SuppressMessage("ReSharper", "JoinNullCheckWithUsage")]
        public DownloadSpeedEvaluator(WebClient webClient, int updatePeriosMs, SpeedMeasured onSpeedMeasured)
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));
            if (updatePeriosMs < 1)
                throw new ArgumentOutOfRangeException(nameof(updatePeriosMs));
            if (onSpeedMeasured == null)
                throw new ArgumentNullException(nameof(onSpeedMeasured));

            _webClient = webClient;
            _onSpeedMeasured = onSpeedMeasured;

            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _updateTimer = new Timer(state => _onSpeedMeasured(_currentSpeed), null, 0, updatePeriosMs);
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
            if (_webClient == null)
                return;

            _webClient.DownloadProgressChanged -= WebClientOnDownloadProgressChanged;
            _updateTimer.Dispose();

            _webClient = null;
            _onSpeedMeasured = null;
            _updateTimer = null;
        }
    }
}
