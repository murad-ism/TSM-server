using TradingSystemsMonitoring.Data.Services;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.Tests.Helpers;

namespace TradingSystemsMonitoring.Tests
{
    public class AccountClosedTradeTests
    {
        private TestTradingDataDbContextFactory _tradingDataDbContextFac;

        [SetUp]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfig();
            TradingDataDbSettings.ReadConfiguration(configuration);
            _tradingDataDbContextFac = new TestTradingDataDbContextFactory();
        }
        
        [Test]
        public void AccountClosedTradeTests_GetSecurities_Returns10Rows()
        {
            var secs = new SecurityService(_tradingDataDbContextFac).GetSecurities().GetAwaiter().GetResult();
            Assert.IsNotNull(secs);
            Assert.IsNotEmpty(secs);
            Assert.That(secs.Length, Is.EqualTo(10));
        }

        [Test]
        public void AccountClosedTradeTests_GetAccounts_Returns5Rows()
        {
            var accountIds = new AccountClosedTradesService(_tradingDataDbContextFac).GetAccountIds().GetAwaiter().GetResult();
            Assert.IsNotNull(accountIds);
            Assert.IsNotEmpty(accountIds);
            Assert.That(accountIds.Length, Is.EqualTo(5));
        }

        [Test]
        public void AccountClosedTradeTests_SearchTradesBySystemId_Returns3Rows()
        {
            var trades = new AccountClosedTradesService(_tradingDataDbContextFac)
                .SearchByParams("SYS001", null, null, null, null, null).GetAwaiter().GetResult();
            Assert.IsNotNull(trades);
            Assert.IsNotEmpty(trades);
            Assert.That(trades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void AccountClosedTradeTests_CountTradesBySystemId_Returns3Rows()
        {
            var count = new AccountClosedTradesService(_tradingDataDbContextFac)
                .CountByParams("SYS001", null, null, null).GetAwaiter().GetResult();
            Assert.IsNotNull(count);
            Assert.That(count, Is.EqualTo(3));
        }

        [TearDown]
        public void Cleanup()
        {
            _tradingDataDbContextFac = null;
        }
    }
}
