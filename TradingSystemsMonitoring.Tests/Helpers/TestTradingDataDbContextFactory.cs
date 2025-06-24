using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.Tests.Seeders;

namespace TradingSystemsMonitoring.Tests.Helpers
{
    internal class TestTradingDataDbContextFactory : IDbContextFactory<TradingDataDbContext>
    {
        public TradingDataDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TradingDataDbContext>();
            var opt = optionsBuilder
                .UseSqlServer(TradingDataDbSettings.ConnectionString)
                .UseSeeding((dbContext, _) => AccountClosedTradeSeeder.Seed(dbContext))
                .Options;
            
            var dbContext = new TradingDataDbContext(opt);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return dbContext;
        }
    }
}
