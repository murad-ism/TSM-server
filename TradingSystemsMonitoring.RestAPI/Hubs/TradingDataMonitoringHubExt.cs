using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TradingSystemsMonitoring.RestAPI.Hubs
{
    public static class TradingDataMonitoringHubExtension
    {
        public static Task NotifyAllClients(this IHubContext<TradingDataMonitoringHub> hub, string msg)
        {
            return hub.Clients.All.SendAsync("NewTrackingData", msg);
        }
    }
}
