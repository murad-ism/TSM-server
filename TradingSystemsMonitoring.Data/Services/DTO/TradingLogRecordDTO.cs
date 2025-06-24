using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Services.DTO
{
    /// <summary>
    /// Лог запись торговой системы (робота).
    /// </summary>
    public class TradingLogRecordDTO
    {
        public string Id { get; set; }

        /// <summary>
        /// Дата и время записи.
        /// </summary>
        public string Date { get; set; }
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
