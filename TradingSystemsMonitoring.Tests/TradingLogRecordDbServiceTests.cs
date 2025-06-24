using TradingSystemsMonitoring.Data;
using TradingSystemsMonitoring.Data.Services;
using TradingSystemsMonitoring.DataModel.DbContext.Factories;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.Tests.Helpers;
using TradingSystemsMonitoring.Tests.Seeders;

namespace TradingSystemsMonitoring.Tests
{
    public class TradingLogRecordDbServiceTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfig(); 
            TradingLogRecordsDbSettings.ReadConfiguration(configuration);
            var fac = new TradingLogRecordDbFactory();
            using (var client = fac.CreateClient())
            {
                client.DropDatabase(TradingLogRecordsDbSettings.Database);
                var db = fac.GetDbConnection(client);
                new TradingLogRecordSeeder(db).SeedAsync().GetAwaiter().GetResult();
            }
        }

        [Test]
        public void TradingLogsExplorer_GetRecordsForSystemA_Returns3Rows()
        {
            var fac = new TradingLogRecordDbFactory();
            var logsSvc = new TradingLogsExplorerService(fac);
            var logs = logsSvc.GetRecordsByDate("SystemA", null).GetAwaiter().GetResult();

            Assert.IsNotNull(logs);
            Assert.IsNotEmpty(logs);
            Assert.AreEqual(3, logs.Count());
        }
    }
}