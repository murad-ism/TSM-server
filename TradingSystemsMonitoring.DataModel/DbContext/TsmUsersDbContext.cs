using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.DataModel.Mappings.Identity;

namespace TradingSystemsMonitoring.DataModel.DbContext
{
    public class TsmUsersDbContext : IdentityDbContext<TsmUser, TsmRole, string>
    {
        public TsmUsersDbContext(DbContextOptions<TsmUsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TsmUserConfiguration());
            builder.ApplyConfiguration(new IdentityRoleConfiguration());
            builder.ApplyConfiguration(new IdentityUserRoleConfiguration());
            builder.ApplyConfiguration(new IdentityRoleClaimConfiguration());
            builder.ApplyConfiguration(new IdentityUserClaimConfiguration());
            builder.ApplyConfiguration(new IdentityUserTokenConfiguration());
            builder.ApplyConfiguration(new IdentityUserLoginConfiguration());
        }
    }
}
