using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TradingSystemsMonitoring.DataModel.Identity
{
    public class TsmUsersDbContext : IdentityDbContext<TsmUser>
    {
        public TsmUsersDbContext(DbContextOptions<TsmUsersDbContext> options)  : base(options)
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
