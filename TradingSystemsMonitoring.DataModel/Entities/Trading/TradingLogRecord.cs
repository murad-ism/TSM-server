using MongoDB.Bson;

namespace TradingSystemsMonitoring.DataModel.Entities.Trading
{
    public class TradingLogRecord
    {
        public ObjectId Id { get; set; }
        public BsonDateTime Date { get; set; }
        public string Level { get; set; }
        public string TradingSystem { get; set; }
        public string Security { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public BsonDocument Properties { get; set; }
    }
}
