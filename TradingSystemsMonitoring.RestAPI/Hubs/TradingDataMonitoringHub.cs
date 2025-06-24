using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TradingSystemsMonitoring.RestAPI.Hubs
{
    /// <summary>
    /// Хаб для получения данных торговой системы на стороне UI клиента.
    /// </summary>
    public class TradingDataMonitoringHub : Hub
    {
        public TradingDataMonitoringHub(){}
        public async Task SubscribeOnData()
        {
            await Task.CompletedTask;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
