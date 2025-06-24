using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.RestAPI.Services.Identity
{
    public class TsmUsersService
    {
        private readonly UserManager<TsmUser> _userManager;
        private readonly SignInManager<TsmUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public TsmUsersService(UserManager<TsmUser> userManager,
            SignInManager<TsmUser> signInManager, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<TsmUserToken> Login(TsmUserLoginData request, CancellationToken cancellationToken)
        {
            var user = !string.IsNullOrEmpty(request.Username) ?
                await _userManager.FindByNameAsync(request.Username) :
                await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return new TsmUserToken
                {
                    Token = _jwtGenerator.CreateToken(user, userRoles.ToArray()),
                    UserName = user.UserName
                };
            }

            return null;
        }
        public async Task<OperationResult> AddUser(TsmUserRegisterData request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new OperationResult(false, new[] { "Пользователь c таким логином уже сущесвует!" });
            }

            var tsmUser = new TsmUser
            {
                UserName = request.UserName,
                Email = request.Email
            };
            var userResult = await _userManager.CreateAsync(tsmUser, request.Password);
            var roleResult = await _userManager.AddToRoleAsync(tsmUser, TsmRoleNames.User);

            if (userResult.Succeeded && roleResult.Succeeded)
            {
                return new OperationResult(true, null);
            }
            else
            {
                var errors = Enumerable.ToArray(userResult.Errors.Concat(roleResult.Errors))
                    .Select(x => $"{x.Code}.{x.Description}").ToArray();

                return new OperationResult(false, errors);
            }
        }

        public async Task<OperationResult> DeleteUser(string userName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new OperationResult(false, new[] { "Пользователя c таким логином не сущесвует!" });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return new OperationResult(true, null);
            }
            else
            {
                return new OperationResult(false, Enumerable.ToArray(result.Errors)
                    .Select(x => $"{x.Code}.{x.Description}").ToArray());
            }
        }
        
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }

    public class TsmUserLoginData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TsmUserToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class TsmUserRegisterData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class OperationResult
    {
        public bool IsSucceded { get; }
        public string[] Errors { get; }

        public OperationResult(bool isSucceded, string[] errors)
        {
            IsSucceded = isSucceded;
            Errors = errors;
        }
    }
}
