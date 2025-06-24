using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TradingSystemsMonitoring.RestAPI.Services.Handlers
{
    public class TsmExceptionHandler
    {
        private ILogger<TsmExceptionHandler> _logger;
        private IWebHostEnvironment _env;
        public TsmExceptionHandler(ILogger<TsmExceptionHandler> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task HandleException(HttpContext context)
        {
            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";
            var includeExDetails = !_env.IsProduction();
            
            var detailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var details = detailsFactory.CreateProblemDetails(context, 
                (int)HttpStatusCode.InternalServerError, 
                title:null,
                type: null,
                detail: includeExDetails ? ex?.ToString() : null, 
                instance: context.Request.GetDisplayUrl());

            _logger.LogError(ex, "Exception has been throwed");
            var stream = context.Response.Body;

            await JsonSerializer.SerializeAsync(stream, details);
        }
    }
}
