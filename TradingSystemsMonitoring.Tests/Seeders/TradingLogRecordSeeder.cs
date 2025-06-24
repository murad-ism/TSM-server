using MongoDB.Bson;
using MongoDB.Driver;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Tests.Seeders
{
    public class TradingLogRecordSeeder
    {
        private readonly IMongoCollection<TradingLogRecord> _logsCollection;
        public TradingLogRecordSeeder(IMongoDatabase database)
        {
            _logsCollection = database.GetCollection<TradingLogRecord>("LogRecs");
        }

        public async Task SeedAsync()
        {
            var existingCount = await _logsCollection.CountDocumentsAsync(FilterDefinition<TradingLogRecord>.Empty);
            if (existingCount > 0)
            {
                return;
            }

            var testData = new List<TradingLogRecord>
            {
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)), Level = "Info", TradingSystem = "SystemA", Security = "AAPL", Message = "Trade executed", Exception = null, Properties = new BsonDocument("OrderId", 12345) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 2, 20, 12, 45, 0, DateTimeKind.Utc)), Level = "Warning", TradingSystem = "SystemB", Security = "MSFT", Message = "High volatility detected", Exception = null, Properties = new BsonDocument("Volatility", 0.8) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 3, 5, 14, 15, 0, DateTimeKind.Utc)), Level = "Error", TradingSystem = "SystemC", Security = "GOOG", Message = "Order failed", Exception = "TimeoutException", Properties = new BsonDocument("RetryCount", 3) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 4, 10, 9, 0, 0, DateTimeKind.Utc)), Level = "Info", TradingSystem = "SystemD", Security = "TSLA", Message = "Order placed", Exception = null, Properties = new BsonDocument("OrderType", "Limit") },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 5, 25, 16, 20, 0, DateTimeKind.Utc)), Level = "Warning", TradingSystem = "SystemA", Security = "AMZN", Message = "Unusual activity detected", Exception = null, Properties = new BsonDocument("ActivityScore", 95) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 6, 30, 18, 10, 0, DateTimeKind.Utc)), Level = "Error", TradingSystem = "SystemB", Security = "NFLX", Message = "Execution failed", Exception = "NetworkException", Properties = new BsonDocument("Latency", 120) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 7, 14, 20, 5, 0, DateTimeKind.Utc)), Level = "Info", TradingSystem = "SystemC", Security = "NVDA", Message = "Order confirmed", Exception = null, Properties = new BsonDocument("ConfirmationId", "XYZ123") },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 8, 8, 8, 30, 0, DateTimeKind.Utc)), Level = "Warning", TradingSystem = "SystemD", Security = "META", Message = "Price spike detected", Exception = null, Properties = new BsonDocument("PriceChange", 5.2) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 9, 19, 22, 50, 0, DateTimeKind.Utc)), Level = "Error", TradingSystem = "SystemA", Security = "IBM", Message = "Invalid order", Exception = "ValidationException", Properties = new BsonDocument("ErrorCode", 400) },
                new TradingLogRecord { Date = BsonDateTime.Create(new DateTime(2025, 10, 31, 11, 15, 0, DateTimeKind.Utc)), Level = "Info", TradingSystem = "SystemB", Security = "ORCL", Message = "Trade settled", Exception = null, Properties = new BsonDocument("SettlementId", 67890) }
            };

            await _logsCollection.InsertManyAsync(testData);
        }
    }
}
