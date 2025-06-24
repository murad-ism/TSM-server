using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Tests.Seeders
{
    public class AccountClosedTradeSeeder
    {
        public static void Seed(TradingDataDbContext tradingDataDbContext)
        {
            var aapl = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "AAPL" };
            var googl = new Security { Exchange = "NASDAQ", Class = "Stock", Ticker = "GOOGL" };
            var msft = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "MSFT" };
            var tsla = new Security { Exchange = "NASDAQ", Class = "Stock", Ticker = "TSLA" };
            var amzn = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "AMZN" };
            var fb = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "FB" };
            var nflx = new Security { Exchange = "NASDAQ", Class = "Stock", Ticker = "NFLX" };
            var nvda = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "NVDA" };
            var pypl = new Security { Exchange = "NASDAQ", Class = "Stock", Ticker = "PYPL" };
            var intc = new Security { Exchange = "NYSE", Class = "Stock", Ticker = "INTC" };

            tradingDataDbContext.Set<Security>().AddRange(
                aapl, googl, msft, tsla, amzn, fb, nflx, nvda, pypl, intc
            );
            tradingDataDbContext.SaveChanges();

            tradingDataDbContext.Set<AccountClosedTrade>().AddRange(
                new AccountClosedTrade { AccountId = "ACC001", SystemId = "SYS001", Security = aapl, OpeningDate = new DateTime(2025, 1, 10), ClosingDate = new DateTime(2025, 1, 15), OpeningPrice = 100.5, ClosingPrice = 110.3, Quantity = 50, Operation = 1, FutCode = "FUT001", ResultInTicks = 100, ResultInPercent = 5, ResultInCash = 500 },
                new AccountClosedTrade { AccountId = "ACC002", SystemId = "SYS002", Security = googl, OpeningDate = new DateTime(2025, 2, 5), ClosingDate = new DateTime(2025, 2, 10), OpeningPrice = 200.8, ClosingPrice = 190.2, Quantity = 30, Operation = 2, FutCode = "FUT002", ResultInTicks = -50, ResultInPercent = -3, ResultInCash = -300 },
                new AccountClosedTrade { AccountId = "ACC003", SystemId = "SYS003", Security = msft, OpeningDate = new DateTime(2025, 3, 12), ClosingDate = new DateTime(2025, 3, 18), OpeningPrice = 150.3, ClosingPrice = 160.7, Quantity = 40, Operation = 1, FutCode = "FUT003", ResultInTicks = 80, ResultInPercent = 6, ResultInCash = 640 },
                new AccountClosedTrade { AccountId = "ACC004", SystemId = "SYS001", Security = tsla, OpeningDate = new DateTime(2025, 4, 20), ClosingDate = new DateTime(2025, 4, 25), OpeningPrice = 300.6, ClosingPrice = 290.1, Quantity = 60, Operation = 2, FutCode = "FUT004", ResultInTicks = -70, ResultInPercent = -4, ResultInCash = -420 },
                new AccountClosedTrade { AccountId = "ACC005", SystemId = "SYS002", Security = amzn, OpeningDate = new DateTime(2025, 5, 15), ClosingDate = new DateTime(2025, 5, 20), OpeningPrice = 250.9, ClosingPrice = 260.4, Quantity = 70, Operation = 1, FutCode = "FUT005", ResultInTicks = 60, ResultInPercent = 4, ResultInCash = 700 },
                new AccountClosedTrade { AccountId = "ACC001", SystemId = "SYS003", Security = fb, OpeningDate = new DateTime(2025, 6, 8), ClosingDate = new DateTime(2025, 6, 14), OpeningPrice = 180.2, ClosingPrice = 170.5, Quantity = 45, Operation = 2, FutCode = "FUT006", ResultInTicks = -40, ResultInPercent = -2, ResultInCash = -360 },
                new AccountClosedTrade { AccountId = "ACC002", SystemId = "SYS007", Security = nflx, OpeningDate = new DateTime(2025, 7, 22), ClosingDate = new DateTime(2025, 7, 28), OpeningPrice = 220.4, ClosingPrice = 230.6, Quantity = 55, Operation = 1, FutCode = "FUT007", ResultInTicks = 90, ResultInPercent = 5, ResultInCash = 500 },
                new AccountClosedTrade { AccountId = "ACC003", SystemId = "SYS008", Security = nvda, OpeningDate = new DateTime(2025, 8, 10), ClosingDate = new DateTime(2025, 8, 15), OpeningPrice = 270.7, ClosingPrice = 260.9, Quantity = 35, Operation = 2, FutCode = "FUT008", ResultInTicks = -60, ResultInPercent = -3, ResultInCash = -400 },
                new AccountClosedTrade { AccountId = "ACC004", SystemId = "SYS009", Security = pypl, OpeningDate = new DateTime(2025, 9, 5), ClosingDate = new DateTime(2025, 9, 12), OpeningPrice = 310.1, ClosingPrice = 320.3, Quantity = 80, Operation = 1, FutCode = "FUT009", ResultInTicks = 100, ResultInPercent = 5, ResultInCash = 800 },
                new AccountClosedTrade { AccountId = "ACC005", SystemId = "SYS001", Security = intc, OpeningDate = new DateTime(2025, 10, 15), ClosingDate = new DateTime(2025, 10, 20), OpeningPrice = 130.3, ClosingPrice = 120.5, Quantity = 25, Operation = 2, FutCode = "FUT010", ResultInTicks = -50, ResultInPercent = -4, ResultInCash = -300 }
            );                                                     
            tradingDataDbContext.SaveChanges();                    
        }
    }
}
