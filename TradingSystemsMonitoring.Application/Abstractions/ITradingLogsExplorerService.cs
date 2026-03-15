using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Application.DTO;

namespace TradingSystemsMonitoring.Application.Abstractions
{
    /// <summary>
    /// Service for trading system log records.
    /// </summary>
    public interface ITradingLogsExplorerService
    {
        Task<IEnumerable<TradingLogRecordDTO>> GetRecordsByDate(string tradingSystemId, DateTime? date);
    }
}
