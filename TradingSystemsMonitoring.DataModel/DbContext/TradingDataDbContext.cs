using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.DataModel.Mappings.Trading;

namespace TradingSystemsMonitoring.DataModel.DbContext
{
    public class TradingDataDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Security> Securities { get; set; }
        public DbSet<AccountClosedTrade> AccountClosedTrades { get; set; }
        public TradingDataDbContext(DbContextOptions<TradingDataDbContext> options) : base(options)
        {
        }
        public TradingDataDbContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new SecurityMap());
            modelBuilder.ApplyConfiguration(new AccountClosedTradeMap());
        }
    }
}
