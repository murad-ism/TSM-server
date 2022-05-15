using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TradeDataAccessor.Shared.Services.Callback;

namespace TradingSystemsMonitoring.RestAPI.Hubs
{
    public class TradingDataMonitoringHub : Hub
    {
        public TradingDataMonitoringHub(){}
        public async Task SubscribeTradingData()
        {
            await Clients.Caller.SendAsync("SubscribedOnTradingData", "ok");
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }

    public static class ITradingDataMonitoringHubContextExt
    {
        public static void NotifyAllClients(this
            IHubContext<TradingDataMonitoringHub> hub, string msg)
        {
            hub.Clients.All.SendAsync("NewTrackingData", msg);
        }
    }
}
