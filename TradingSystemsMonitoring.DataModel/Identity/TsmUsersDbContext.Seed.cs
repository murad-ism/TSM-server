using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TradingSystemsMonitoring.DataModel.Identity
{
    public class DataSeed
    {
        public static async Task SeedDataAsync(TsmUsersDbContext context, UserManager<TsmUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<TsmUser>
                {
                    new TsmUser
                    {
                        UserName = "TestUserFirst",
                        Email = "testuserfirst@test.com"
                    },

                    new TsmUser
                    {
                        UserName = "TestUserSecond",
                        Email = "testusersecond@test.com"
                    }
                };
                
                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "qazwsX123@");
                }
            }
        }
    }
}
