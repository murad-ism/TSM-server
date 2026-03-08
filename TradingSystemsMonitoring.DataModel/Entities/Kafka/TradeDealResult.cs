using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.DataModel.Entities.Kafka
{
    public class TradeDealResult
    {
        public string Id { get; set; }

        public string Account { get; set; }
        public string System { get; set; }

        public Security Security { get; set; }

        public DateTime OpenDateTime { get; set; }
        public DateTime? CloseDateTime { get; set; }

        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }

        public TradeOperation Operation { get; set; }
        public long Quantity { get; set; }

        public double TotalInTicks { get; set; }
        public double TotalInCash { get; set; }
        public double TotalInPercent { get; set; }
    }


    /// <summary>
    /// Направление сделки
    /// </summary>
    public enum TradeOperation
    {
        /// <summary>
        /// Направление неопределено
        /// </summary>
        None = 0,

        /// <summary>
        /// Сделка на покупку
        /// </summary>
        Buy = 1 << 0,

        /// <summary>
        /// Сделка на продажу
        /// </summary>
        Sell = 1 << 1
    }
}
