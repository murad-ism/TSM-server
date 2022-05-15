using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TradingSystemsMonitoring.RestAPI.Services;

namespace TradingSystemsMonitoring.DataModel.Identity
{
    public class TsmUserLoginHandler
    {
        private readonly UserManager<TsmUser> _userManager;
        private readonly SignInManager<TsmUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public TsmUserLoginHandler(UserManager<TsmUser> userManager,
        SignInManager<TsmUser> signInManager, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<TsmUserToken> Login(TsmUserLoginData request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded)
            {
                return new TsmUserToken
                {
                    Token = _jwtGenerator.CreateToken(user),
                    UserName = user.UserName
                };
            }

            return null;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }

    public class TsmUserLoginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TsmUserToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
