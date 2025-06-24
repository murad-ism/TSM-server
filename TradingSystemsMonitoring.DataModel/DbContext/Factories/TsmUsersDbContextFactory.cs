using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;

namespace TradingSystemsMonitoring.DataModel.DbContext.Factories
{
    public class TsmUsersDbContextFactory : IDbContextFactory<TsmUsersDbContext>
    {
        public TsmUsersDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TsmUsersDbContext>();
            var opt = optionsBuilder
                .UseNpgsql(TsmUsersDbSettings.ConnectionString)
                .Options;
            return new TsmUsersDbContext(opt);
        }
    }
}
