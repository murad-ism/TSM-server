using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using TradingSystemsMonitoring.RestAPI.Hubs;

namespace TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver
{
    public class TrackingDataHandler : BackgroundService
    {
        private ITrackingDataReceiver _trackingDataReceiver;
        private IHubContext<TradingDataMonitoringHub> _tradingDataMonitoringHub;

        public TrackingDataHandler(ITrackingDataReceiver trackingDataReceiver, 
            IHubContext<TradingDataMonitoringHub> tradingDataMonitoringHub)
        {
            _trackingDataReceiver = trackingDataReceiver;
            _tradingDataMonitoringHub = tradingDataMonitoringHub;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _trackingDataReceiver.DataReceived += _trackingDataReceiver_DataReceived;
                _trackingDataReceiver.StartDataReceive();
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
        
        private void _trackingDataReceiver_DataReceived(string obj)
        {
            //_tradingDataMonitoringHub.Clients.All.SendAsync("ReceiveTradingDataUpdate", obj);
            _tradingDataMonitoringHub.NotifyAllClients(obj);
        }
    }
}
