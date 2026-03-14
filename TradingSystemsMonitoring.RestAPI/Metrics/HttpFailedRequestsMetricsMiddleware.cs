using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TradingSystemsMonitoring.RestAPI.Metrics
{
    /// <summary>
    /// Middleware that records failed HTTP responses (4xx, 5xx) to Prometheus.
    /// </summary>
    public class HttpFailedRequestsMetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpFailedRequestsMetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var statusCode = context.Response.StatusCode;
            if (statusCode >= 400)
            {
                var method = context.Request.Method;
                var route = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? "unknown";
                TsmMetrics.HttpRequestsFailedTotal.WithLabels(method, route, statusCode.ToString()).Inc();
            }
        }
    }
}
