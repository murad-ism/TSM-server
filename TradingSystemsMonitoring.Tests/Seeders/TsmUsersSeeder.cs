using Microsoft.AspNetCore.Identity;
using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.Tests.Seeders
{
    internal class TsmUsersSeeder
    {
        internal async Task AddUsers(UserManager<TsmUser> userManager, RoleManager<TsmRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var adminRole = new TsmRole { Name = TsmRoleNames.Admin };
                var userRole = new TsmRole { Name = TsmRoleNames.User };
                await roleManager.CreateAsync(adminRole);
                await roleManager.CreateAsync(userRole);
            }

            if (!userManager.Users.Any())
            {
                var adminUser = new TsmUser { UserName = "admin" };
                var user1 = new TsmUser { UserName = "user1" };
                var user2 = new TsmUser { UserName = "user2" };
                var user3 = new TsmUser { UserName = "user3" };
                var user4 = new TsmUser { UserName = "user4" };
                var user5 = new TsmUser { UserName = "user5" };
                
                await userManager.CreateAsync(adminUser, "Test_pwd_1");
                await userManager.CreateAsync(user1, "Test_pwd_1");
                await userManager.CreateAsync(user2, "Test_pwd_1");
                await userManager.CreateAsync(user3, "Test_pwd_1");
                await userManager.CreateAsync(user4, "Test_pwd_1");
                await userManager.CreateAsync(user5, "Test_pwd_1");

                await userManager.AddToRoleAsync(adminUser, TsmRoleNames.Admin);
                await userManager.AddToRoleAsync(user1, TsmRoleNames.User);
                await userManager.AddToRoleAsync(user2, TsmRoleNames.User);
                await userManager.AddToRoleAsync(user3, TsmRoleNames.User);
                await userManager.AddToRoleAsync(user4, TsmRoleNames.User);
                await userManager.AddToRoleAsync(user5, TsmRoleNames.User);
            }
        }
    }
}
