using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly TsmUsersDbContext _dbContext;

        private readonly UserManager<TsmUser> _userManager;
        private readonly SignInManager<TsmUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TsmUsersDbContext dbContext,
            UserManager<TsmUser> userManager, SignInManager<TsmUser> signInManager, IJwtGenerator jwtGenerator)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TsmUserToken>> LoginAsync(TsmUserLoginData query, CancellationToken token)
        {
            return await new TsmUserLoginHandler(_userManager, _signInManager, _jwtGenerator).Handle(query, token);
        }
        
        [HttpGet("get")]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            var claims = HttpContext.User.Claims;
            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
