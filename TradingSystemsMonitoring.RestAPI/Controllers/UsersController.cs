using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.RestAPI.Abstractions.Identity;
using TradingSystemsMonitoring.RestAPI.Services.Identity;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ITsmUsersService _tsmUsersService;

        public UsersController(ILogger<UsersController> logger, ITsmUsersService tsmUsersService)
        {
            _logger = logger;
            _tsmUsersService = tsmUsersService;
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
            var result = await _tsmUsersService.Login(loginData, token);
            if (result == null)
                return Unauthorized();
            return Ok(result);
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
                await _tsmUsersService.Logout();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Logout failed");
                return StatusCode(StatusCodes.Status500InternalServerError);
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
            return await _tsmUsersService.AddUser(query, token);
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
            return await _tsmUsersService.DeleteUser(userName, token);
        }
    }
}
