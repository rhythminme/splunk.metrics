using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Splunk.Metrics.Abstractions;
namespace Splunk.Metrics.Http
{
    public class HttpMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStatsPublisher _statsPublisher;

        public HttpMetricsMiddleware(RequestDelegate next, IStatsPublisher statsPublisher)
        {
            this._next = next;
            this._statsPublisher = statsPublisher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var bucketPrefix = CreateBucketPrefix(context);
            var dimensions = ExtractDimensions(context);

            await Task.WhenAll(
                _statsPublisher.TimingAsync($"{bucketPrefix}.msecs", stopwatch.ElapsedMilliseconds, dimensions), 
                _statsPublisher.IncrementAsync($"{bucketPrefix}.{context.Response.StatusCode}", 1, dimensions));
        }

        private static string CreateBucketPrefix(HttpContext context)
        {
            var routeData = context.GetRouteData();
            return routeData != null && routeData.Values.Count > 0
                ? $"http.{routeData.Values["controller"]}-{routeData.Values["action"]}-{context.Request.Method}"
                : "http.no-route-data";
        }

        private static KeyValuePair<string, string>[] ExtractDimensions(HttpContext context)
        {
            return context.Items.Keys
                .Where(key => key.ToString().StartsWith(HttpMetrics.OwinContextSplunkMetricsDimensionPrefix))
                .Select(key => new KeyValuePair<string, string>(
                    key.ToString().Substring(HttpMetrics.OwinContextSplunkMetricsDimensionPrefix.Length + 1),
                    context.Items[key].ToString()))
                .ToArray();
        }
    }
}

    
