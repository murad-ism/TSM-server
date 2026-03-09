using System;
using System.Collections.Generic;
using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.DataModel.Entities.Trading
{
    /// <summary>
    /// Торговый инструмент.
    /// </summary>
    public class Security : Entity<long>
    {
        public override long Id { get; set; }

        /// <summary>
        /// Код биржи.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Класс инструмента.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Тикер инструмента.
        /// </summary>
        public string Ticker { get; set; }

        public virtual ICollection<AccountClosedTrade> AccountClosedTrades { get; set; }

        public Security()
        {
            AccountClosedTrades = new List<AccountClosedTrade>();
        }
    }

    public class SecurityComparer : IEqualityComparer<Security>
    {
        public bool Equals(Security x, Security y)
        {
            if (x == null && y == null)
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            return x.Exchange == y.Exchange && x.Class == y.Class && x.Ticker == y.Ticker;
        }

        public int GetHashCode(Security obj)
        {
            if (obj == null)
                return 0;
            return HashCode.Combine(obj.Ticker, obj.Class, obj.Exchange);
        }
    }
}
