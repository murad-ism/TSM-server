using System.Threading.Tasks;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Application.Abstractions
{
    /// <summary>
    /// Service for trading instruments (securities).
    /// </summary>
    public interface ISecurityService
    {
        Task<Security[]> GetSecurities();
    }
}
