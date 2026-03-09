using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TradingSystemsMonitoring.Data.Abstractions;
using TradingSystemsMonitoring.Data.Repos;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.DbContext.Factories;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Services
{
    /// <summary>
    /// Сервис для работы с лог записями торговой системы.
    /// </summary>
    public class TradingLogsExplorerService : ITradingLogsExplorerService
    {
        private ITradingLogRecordDbFactory _tradingLogRecordDbFactory;
        public TradingLogsExplorerService(ITradingLogRecordDbFactory tradingLogRecordDbFactory)
        {
            _tradingLogRecordDbFactory = tradingLogRecordDbFactory;
        }

        /// <summary>
        /// Получить лог записи.
        /// </summary>
        /// <param name="tradingSystemId">Идентификатор торговой системы.</param>
        /// <param name="date">Дата (день) для поиска лог записей.</param>
        /// <returns>Лог записи типа <see cref="TradingLogRecordDTO"/>.</returns>
        public async Task<IEnumerable<TradingLogRecordDTO>> GetRecordsByDate(string tradingSystemId, DateTime? date)
        {
            var builder = Builders<TradingLogRecord>.Filter;
            FilterDefinition<TradingLogRecord> dateStartFilter = null;
            FilterDefinition<TradingLogRecord> dateEndFilter = null;
            FilterDefinition<TradingLogRecord> systemFilter = null;

            if (!string.IsNullOrEmpty(tradingSystemId))
            {
                systemFilter = builder.Where(x => x.TradingSystem == tradingSystemId);
            }

            if (date != null)
            {
                var startDate = date.Value.Date;
                var endDate = date.Value.Date.AddDays(1);
                dateStartFilter = builder.Where(x => x.Date >= startDate);
                dateEndFilter = builder.Where(x => x.Date < endDate);
            }

            var allFilters = new[] { systemFilter, dateStartFilter, dateEndFilter }
                .Where(x => x != null).ToList();
            var filter = allFilters.Any() ? builder.And(allFilters) : builder.Empty;

            using (var mongoClient = _tradingLogRecordDbFactory.CreateClient())
            {
                var db = _tradingLogRecordDbFactory.GetDbConnection(mongoClient);
                var tradingLogRecordRepo = new TradingLogRecordRepo(db);
                var recs = await tradingLogRecordRepo
                    .Where(filter).SortBy(x => x.Date).ToListAsync();
                return recs.Select(TradingLogRecordDTO.MapFromTrade).ToArray();
            }
        }
    }


}
