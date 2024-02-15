using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TradingSystemsMonitoring.Data.Repos;
using TradingSystemsMonitoring.DataModel.DbContext.Infrastructure;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data
{
    public class TradingLogRecordDbService
    {
        public LogRecsService LogsSvc { get; }
        public TradingLogRecordDbService()
        {
            LogsSvc = new LogRecsService();
        }
        
        public class LogRecsService
        {
            private TradingLogRecordRepo _tradingLogRecordRepo;
            public LogRecsService()
            {
                var mongoDatabase = new MongoClient(TradingLogRecordsDbSettings.Url)
                    .GetDatabase(TradingLogRecordsDbSettings.Database);
                _tradingLogRecordRepo = new TradingLogRecordRepo(mongoDatabase);
            }

            public async Task<IEnumerable<TradingLogRecordDTO>> GetLogRecordsByDate(string tradingSystemId, DateTime? date)
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

                var allFilters = new[] {systemFilter, dateStartFilter, dateEndFilter}.Where(x => x != null).ToList();
                var filter = allFilters.Any() ? builder.And(allFilters) : builder.Empty ;
                var recs = await _tradingLogRecordRepo.Where(filter).SortBy(x => x.Date).ToListAsync();
                return recs.Select(TradingLogRecordDTO.MapFromTrade).ToArray();
            }
        }
    }

    public class TradingLogRecordDTO
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Level { get; set; }
        public string TradingSystem { get; set; }
        public string Security { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public object Properties { get; set; }

        public static TradingLogRecordDTO MapFromTrade(TradingLogRecord rec)
        {
            return new TradingLogRecordDTO
            {
                Id = rec.Id.ToString(),
                Security = rec.Security,
                Date = $"{rec.Date.ToUniversalTime()}",
                TradingSystem = rec.TradingSystem,
                Exception = rec.Exception,
                Level = rec.Level,
                Message = rec.Message,
                Properties = rec.Properties?.ToString()
            };
        }
    }
}
