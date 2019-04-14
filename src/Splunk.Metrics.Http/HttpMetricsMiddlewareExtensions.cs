using Microsoft.AspNetCore.Builder;

namespace Splunk.Metrics.Http
{
    public static class HttpMetricsMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpMetrics(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpMetricsMiddleware>();
        }
    }
}