using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.DataModel.DbContext.Seed
{
    public class UserDbInitializer
    {
        public static async Task SeedDataAsync(IConfiguration configuration,
            UserManager<TsmUser> userManager, RoleManager<TsmRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userRole = new TsmRole
                {
                    Name = TsmRoleNames.User
                };
                await roleManager.CreateAsync(userRole);

                var adminRole = new TsmRole
                {
                    Name = TsmRoleNames.Admin
                };
                await roleManager.CreateAsync(adminRole);

                var adminUser = new TsmUser
                {
                    UserName = "admin"
                };
                await userManager.CreateAsync(adminUser, configuration["Identity:TokenKey"]);
                await userManager.AddToRoleAsync(adminUser, adminRole.Name);
            }
        }
    }
}
