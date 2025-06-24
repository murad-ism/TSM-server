using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.RestAPI.Services.Identity;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly TsmUsersDbContext _dbContext;
        private readonly UserManager<TsmUser> _userManager;
        private readonly SignInManager<TsmUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public UsersController(ILogger<UsersController> logger, TsmUsersDbContext dbContext,
            UserManager<TsmUser> userManager, SignInManager<TsmUser> signInManager, IJwtGenerator jwtGenerator)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        /// <summary>
        /// Login by user.
        /// </summary>
        /// <param name="loginData"><see cref="TsmUserLoginData">Login params.</see></param>
        /// <returns>User token of type <see cref="TsmUserToken"/>.</returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TsmUserToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TsmUserToken>> LoginAsync(TsmUserLoginData loginData, CancellationToken token)
        {
            return await new TsmUsersService(_userManager, _signInManager, _jwtGenerator).Login(loginData, token);
        }
        
        /// <summary>
        /// Logout by user.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Logout")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LogoutAsync()
        {
            try
            {
                await new TsmUsersService(_userManager, _signInManager, _jwtGenerator).Logout();
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(HttpStatusCode.InternalServerError);
            }
        }
        
        /// <summary>
        /// Add user (admin role required).
        /// </summary>
        [HttpPost("Add")]
        [Authorize(Roles = TsmRoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationResult>> AddUserAsync(TsmUserRegisterData query, CancellationToken token)
        {
            return await new TsmUsersService(_userManager, _signInManager, _jwtGenerator).AddUser(query, token);
        }

        /// <summary>
        /// Delete user (admin role required).
        /// </summary>
        [HttpPost("Delete")]
        [Authorize(Roles = TsmRoleNames.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationResult>> DeleteUserAsync(string userName, CancellationToken token)
        {
            return await new TsmUsersService(_userManager, _signInManager, _jwtGenerator).DeleteUser(userName, token);
        }
    }
}
