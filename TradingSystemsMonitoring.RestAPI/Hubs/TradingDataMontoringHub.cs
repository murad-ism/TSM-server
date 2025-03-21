using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

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
