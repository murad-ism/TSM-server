using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.RestAPI.Services.Identity;
using TradingSystemsMonitoring.Tests.Seeders;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.DataModel.DbContext.Factories;
using TradingSystemsMonitoring.Tests.Helpers;

namespace TradingSystemsMonitoring.Tests
{
    [TestFixture]
    public class TsmUserManagerTests
    {
        private UserManager<TsmUser> _tsmUserManager;
        private RoleManager<TsmRole> _tsmRoleManager;
        private SignInManager<TsmUser> _tsmSignInManager;
        private IJwtGenerator _tsmJwtGenerator;
        private TsmUsersDbContext _tsmUsersDbContext;

        [OneTimeSetUp]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfig();
            TsmUsersDbSettings.ReadConfiguration(configuration);
            var services = new ServiceCollection();
            services.AddScoped(_ => new TsmUsersDbContextFactory().CreateDbContext());
            services
                .AddIdentityCore<TsmUser>()
                .AddRoles<TsmRole>()
                .AddEntityFrameworkStores<TsmUsersDbContext>()
                .AddSignInManager<SignInManager<TsmUser>>();
            services.AddScoped<IJwtGenerator>(x => new JwtGenerator(configuration));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Identity:TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                        };
                    });

            var serviceProvider = services.BuildServiceProvider();
            _tsmUserManager = serviceProvider.GetRequiredService<UserManager<TsmUser>>();
            _tsmRoleManager = serviceProvider.GetRequiredService<RoleManager<TsmRole>>();
            _tsmSignInManager = serviceProvider.GetRequiredService<SignInManager<TsmUser>>();
            _tsmJwtGenerator = serviceProvider.GetRequiredService<IJwtGenerator>();

            _tsmUsersDbContext = serviceProvider.GetRequiredService<TsmUsersDbContext>();
            _tsmUsersDbContext.Database.EnsureDeleted();
            _tsmUsersDbContext.Database.EnsureCreated();
            new TsmUsersSeeder().AddUsers(_tsmUserManager, _tsmRoleManager).GetAwaiter().GetResult();
        }

        [Test]
        public void TsmUserManager_GetAllUsers_ReturnsUsers()
        {
            var userCount = _tsmUserManager.Users.Count();
            Assert.IsTrue(userCount > 0);
        }

        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase(null, "test")]
        [TestCase("user1", null)]
        public void TsmUserManager_UserLogin_ThrowsArgNullEx(string? userName, string? userPwd)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new TsmUsersService(_tsmUserManager, _tsmSignInManager, _tsmJwtGenerator)
                    .Login(new TsmUserLoginData
                    {
                        Username = userName,
                        Password = userPwd
                    }, CancellationToken.None)
                    .GetAwaiter().GetResult();
            });
        }

        [TestCase("user", null, ExpectedResult = false)]
        [TestCase("user", "", ExpectedResult = false)]
        [TestCase("user", "1", ExpectedResult = false)]
        [TestCase("user", "123", ExpectedResult = false)]
        [TestCase("user", "Test_pwd_1", ExpectedResult = false)]
        [TestCase("user1", "1", ExpectedResult = false)]
        [TestCase("user1", "123", ExpectedResult = false)]
        [TestCase("user1", "Test_pwd_1", ExpectedResult = true)]
        [TestCase("user2", "Test_pwd_1", ExpectedResult = true)]
        [TestCase("admin", "Test_pwd_1", ExpectedResult = true)]
        public bool TsmUserManager_UserLogin_ReturnsValidResult(string? userName, string? userPwd)
        {
            var userToken = new TsmUsersService(_tsmUserManager, _tsmSignInManager, _tsmJwtGenerator)
                .Login(new TsmUserLoginData
                {
                    Username = userName,
                    Password = userPwd
                }, CancellationToken.None)
                .GetAwaiter().GetResult();
            return userToken != null;
        }

        [Test]
        public void TsmUserManager_Add1User_ReturnsPlus1Users()
        {
            var userCountBeforeAdd = _tsmUserManager.Users.Count();
            var tsmUserLoginHandler = new TsmUsersService(_tsmUserManager, _tsmSignInManager, _tsmJwtGenerator);
            var result = tsmUserLoginHandler.AddUser(new TsmUserRegisterData
            {
                UserName = "user7",
                Password = "Test_pwd_1"
            }, CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Errors, null);
            Assert.AreEqual(result.IsSucceeded, true);

            var userCountAfterAdd = _tsmUserManager.Users.Count();
            Assert.AreEqual(userCountBeforeAdd + 1, userCountAfterAdd);
        }

        [Test]
        public void TsmUserManager_Delete1User_ReturnsMinus1Users()
        {
            var userCountBeforeDelete = _tsmUserManager.Users.Count();
            var tsmUserLoginHandler = new TsmUsersService(_tsmUserManager, _tsmSignInManager, _tsmJwtGenerator);
            var user = _tsmUserManager.Users.FirstOrDefault(x => x.UserName == "user5");
            var result = tsmUserLoginHandler.DeleteUser("user5", CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Errors, null);
            Assert.AreEqual(result.IsSucceeded, true);

            var userCountAfterDelete = _tsmUserManager.Users.Count();
            Assert.AreEqual(userCountBeforeDelete - 1, userCountAfterDelete);

            var deletedUser = _tsmUserManager.Users.FirstOrDefault(x => x.UserName == "user5");
            Assert.AreEqual(deletedUser, null);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _tsmUserManager.Dispose();
            _tsmRoleManager.Dispose();
            _tsmUsersDbContext.Dispose();
        }
    }
}
