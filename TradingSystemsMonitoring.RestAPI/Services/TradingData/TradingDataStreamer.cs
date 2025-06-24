using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver;

namespace TradingSystemsMonitoring.RestAPI.Services.TradingData
{
    /// <summary>
    /// Стример торговых данных торговой системы на UI клиент.
    /// </summary>
    public class TradingDataStreamer : BackgroundService
    {
        private ITradingDataSubscriber _tradingDataSubscriber;
        private IHubContext<TradingDataMonitoringHub> _tradingDataMonitoringHub;
        private ILogger<TradingDataSubscriber> _logger;

        public TradingDataStreamer(ILogger<TradingDataSubscriber> logger,
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
            _tradingDataMonitoringHub.NotifyAllClients(obj);
        }
    }
}
