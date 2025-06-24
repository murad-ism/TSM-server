using Microsoft.AspNetCore.SignalR;

namespace TradingSystemsMonitoring.RestAPI.Hubs
{
    public static class TradingDataMonitoringHubExtension
    {
        public static void NotifyAllClients(this
            IHubContext<TradingDataMonitoringHub> hub, string msg)
        {
            hub.Clients.All.SendAsync("NewTrackingData", msg);
        }
    }
}
