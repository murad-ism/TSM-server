using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.DataModel.Identity;
using TradingSystemsMonitoring.RestAPI.Services;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly TsmUsersDbContext _dbContext;

        private readonly UserManager<TsmUser> _userManager;
        private readonly SignInManager<TsmUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public UsersController(ILogger<WeatherForecastController> logger, TsmUsersDbContext dbContext,
            UserManager<TsmUser> userManager, SignInManager<TsmUser> signInManager, IJwtGenerator jwtGenerator)
        {
            
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }
        
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<TsmUserToken>> LoginAsync(TsmUserLoginData query, CancellationToken token)
        {
            return await new TsmUserLoginHandler(_userManager, _signInManager, _jwtGenerator).Login(query, token);
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> LogoutAsync()
        {
            try
            {
                await new TsmUserLoginHandler(_userManager, _signInManager, _jwtGenerator).Logout();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost("Add")]
        [Authorize(Roles = TsmRoleNames.Admin)]
        public async Task<ActionResult<OperationResult>> AddUserAsync(TsmUserRegisterData query, CancellationToken token)
        {
            return await new TsmUserLoginHandler(_userManager, _signInManager, _jwtGenerator).AddUser(query, token);
        }

        [HttpPost("Delete")]
        [Authorize(Roles = TsmRoleNames.Admin)]
        public async Task<ActionResult<OperationResult>> DeleteUserAsync(TsmUserRegisterData query, CancellationToken token)
        {
            return await new TsmUserLoginHandler(_userManager, _signInManager, _jwtGenerator).DeleteUser(query, token);
        }

    }
}
