using MongoDB.Driver;
using TradingSystemsMonitoring.Data.Base;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Repos
{
    /// <summary>
    /// Репозиторий для работы с лог записями торговой системы.
    /// </summary>
    public class TradingLogRecordRepo : MongoReadonlyRepo<TradingLogRecord>
    {
        public override string MongoCollectionName => "LogRecs";
        public TradingLogRecordRepo(IMongoDatabase db) : base(db) { }
    }
}
