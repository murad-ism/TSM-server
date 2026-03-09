using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.DataModel.Mappings.Identity
{
    public class TsmUserConfiguration : IEntityTypeConfiguration<TsmUser>
    {
        public void Configure(EntityTypeBuilder<TsmUser> entity)
        {
            entity.ToTable("Users");
        }
    }
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<TsmRole>
    {
        public void Configure(EntityTypeBuilder<TsmRole> entity)
        {
            entity.ToTable("Roles");
        }
    }

    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> entity)
        {
            entity.ToTable("UserRoles");
            entity.HasKey(key => new { key.UserId, key.RoleId });
        }
    }

    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> entity)
        {
            entity.ToTable("UserClaims");
        }
    }

    public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> entity)
        {
            entity.ToTable("RoleClaims");
        }
    }

    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> entity)
        {
            entity.ToTable("UserLogins");
            entity.HasKey(key => new { key.LoginProvider, key.ProviderKey });
        }
    }

    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> entity)
        {
            entity.ToTable("UserTokens");
            entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });
        }
    }
}
