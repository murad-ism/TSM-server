using MongoDB.Driver;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;

namespace TradingSystemsMonitoring.DataModel.DbContext.Factories
{
    public interface ITradingLogRecordDbFactory
    {
        IMongoClient CreateClient();
        IMongoDatabase GetDbConnection(IMongoClient client);
    }

    public class TradingLogRecordDbFactory : ITradingLogRecordDbFactory
    {
        public TradingLogRecordDbFactory() { }

        public IMongoClient CreateClient()
        {
            return new MongoClient(TradingLogRecordsDbSettings.Url);
        }

        public IMongoDatabase GetDbConnection(IMongoClient client)
        {
            return client.GetDatabase(TradingLogRecordsDbSettings.Database);
        }
    }
}
