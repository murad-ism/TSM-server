using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TradingSystemsMonitoring.RestAPI.Hubs;

namespace TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver
{
    public class TrackingDataHandler : BackgroundService
    {
        private ITrackingDataReceiver _trackingDataReceiver;
        private IHubContext<TradingDataMonitoringHub> _tradingDataMonitoringHub;
        private ILogger<TrackingDataReceiver> _logger;

        public TrackingDataHandler(ILogger<TrackingDataReceiver> logger,
        ITrackingDataReceiver trackingDataReceiver,
            IHubContext<TradingDataMonitoringHub> tradingDataMonitoringHub)
        {
            _trackingDataReceiver = trackingDataReceiver;
            _tradingDataMonitoringHub = tradingDataMonitoringHub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _trackingDataReceiver.OnDataReceived += _trackingDataReceiver_DataReceived;
                await _trackingDataReceiver.BeginReceiveData(stoppingToken);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private void _trackingDataReceiver_DataReceived(string obj)
        {
            _tradingDataMonitoringHub.NotifyAllClients(obj);
        }
    }
}
