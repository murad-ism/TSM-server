using MongoDB.Driver;
using TradingSystemsMonitoring.Data.Base;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Repos
{
    public class TradingLogRecordRepo : MongoReadonlyRepo<TradingLogRecord>
    {
        public override string MongoCollectionName => "LogRecs";
        public TradingLogRecordRepo(IMongoDatabase db) : base(db) { }
    }
}
