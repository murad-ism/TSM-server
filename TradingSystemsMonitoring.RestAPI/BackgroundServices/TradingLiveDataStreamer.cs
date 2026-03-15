using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Application.Abstractions;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Metrics;

namespace TradingSystemsMonitoring.RestAPI.BackgroundServices
{
    /// <summary>
    /// Стример торговых данных торговой системы на UI клиент.
    /// </summary>
    public class TradingLiveDataStreamer : BackgroundService
    {
        private ITradingDataSubscriber _tradingDataSubscriber;
        private IHubContext<TradingDataMonitoringHub> _tradingDataMonitoringHub;
        private ILogger<TradingLiveDataStreamer> _logger;

        public TradingLiveDataStreamer(ILogger<TradingLiveDataStreamer> logger,
            ITradingDataSubscriber tradingDataSubscriber,
            IHubContext<TradingDataMonitoringHub> tradingDataMonitoringHub)
        {
            _tradingDataSubscriber = tradingDataSubscriber;
            _tradingDataMonitoringHub = tradingDataMonitoringHub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _tradingDataSubscriber.OnDataReceived += TradingDataSubscriberDataReceived;
            await _tradingDataSubscriber.Subscribe(token);
            await Task.Delay(Timeout.Infinite, token);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _tradingDataSubscriber.OnDataReceived -= TradingDataSubscriberDataReceived;
            return base.StopAsync(cancellationToken);
        }

        public void TradingDataSubscriberDataReceived(string obj)
        {
            TsmMetrics.SignalrMessagesSentTotal.Inc();
            _ = _tradingDataMonitoringHub.NotifyAllClients(obj)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        TsmMetrics.SignalrMessagesSendErrorsTotal.Inc();
                        _logger.LogError(t.Exception, "Failed to push trading data to SignalR clients");
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
