using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Splunk.Metrics.Abstractions;

namespace Splunk.Metrics.Http
{
    public class HttpMetricsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IStatsPublisher statsPublisher;

        public HttpMetricsMiddleware(RequestDelegate next, IStatsPublisher statsPublisher)
        {
            this.next = next;
            this.statsPublisher = statsPublisher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await next(context);

            stopwatch.Stop();

            var routeData = context.GetRouteData();
            var bucketPrefix = routeData != null
                ? $"http.{routeData.Values["controller"]}-{routeData.Values["action"]}-{context.Request.Method}"
                : $"http.no-route-data";

            await Task.WhenAll(
                statsPublisher.TimingAsync($"{bucketPrefix}.msecs", stopwatch.ElapsedMilliseconds), 
                statsPublisher.IncrementAsync($"{bucketPrefix}.{context.Response.StatusCode}"));
        }
    }
}

    
