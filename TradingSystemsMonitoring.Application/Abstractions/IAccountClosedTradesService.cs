using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Application.DTO;

namespace TradingSystemsMonitoring.Application.Abstractions
{
    /// <summary>
    /// Service for completed trades of the trading system.
    /// </summary>
    public interface IAccountClosedTradesService
    {
        Task<IEnumerable<AccountClosedTradeDTO>> SearchByParams(
            string systemId, long? securityId, DateTime? from, DateTime? to,
            int? pageIndex, int? pageSize);

        Task<int> CountByParams(
            string systemId, long? securityId, DateTime? from, DateTime? to);

        Task<string[]> GetAccountIds();
        Task<string[]> GetSystemIds();
        Task<List<AccountClosedTradeDTO>> GetCurrentTradesAsync();
    }
}
