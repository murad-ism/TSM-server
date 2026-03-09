using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Data.Services.DTO;

namespace TradingSystemsMonitoring.Data.Abstractions
{
    /// <summary>
    /// Service for trading system log records.
    /// </summary>
    public interface ITradingLogsExplorerService
    {
        Task<IEnumerable<TradingLogRecordDTO>> GetRecordsByDate(string tradingSystemId, DateTime? date);
    }
}
