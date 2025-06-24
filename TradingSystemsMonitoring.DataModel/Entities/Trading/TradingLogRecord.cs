using MongoDB.Bson;

namespace TradingSystemsMonitoring.DataModel.Entities.Trading
{
    /// <summary>
    /// Лог-запись торгового робота.
    /// </summary>
    public class TradingLogRecord
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// Дата и время записи.
        /// </summary>
        public BsonDateTime Date { get; set; }
        /// <summary>
        /// Уровень логирования.
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// Код торговой системы.
        /// </summary>
        public string TradingSystem { get; set; }
        /// <summary>
        /// Торговый инструмент.
        /// </summary>
        public string Security { get; set; }
        /// <summary>
        /// Текст лог-записи.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Исключение, если есть.
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// Доп. свойства.
        /// </summary>
        public BsonDocument Properties { get; set; }
    }
}
