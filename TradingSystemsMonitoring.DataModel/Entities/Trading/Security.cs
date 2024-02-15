using System.Collections.Generic;
using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.DataModel.Entities.Trading
{
    public class Security : Entity<long>
    {
        public override long Id { get; set; }
        public string Exchange { get; set; }
        public string Class { get; set; }
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
            return obj.Ticker.GetHashCode() ^ obj.Class.GetHashCode() ^ obj.Exchange.GetHashCode();
        }
    }
}
