using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.DbContext.Infrastructure;

namespace TradingSystemsMonitoring.DataModel.DbContext
{
    public class TradingDataDbContextFactory : IDbContextFactory<TradingDataDbContext>
    {
        public TradingDataDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TradingDataDbContext>();
            var opt = optionsBuilder
                .UseSqlServer(TradingDataDbSettings.ConnectionString)
                .Options;

            return new TradingDataDbContext(opt);
        }
    }
}
