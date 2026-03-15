using MongoDB.Driver;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.Infrastructure.Base;

namespace TradingSystemsMonitoring.Infrastructure.Repos
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


