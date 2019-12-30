using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Splunk.Metrics.Statsd;

namespace Splunk.Metrics.WebApi
{
    public class HttpMetricsMiddleware : OwinMiddleware
    {
        private readonly StatsPublisher _statsPublisher;

        public HttpMetricsMiddleware(OwinMiddleware next,
            StatsConfiguration options) : base(next)
        {
            _statsPublisher = new StatsPublisher(Options.Create(options));
        }

        public override async Task Invoke(IOwinContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            await Next.Invoke(context);

            stopwatch.Stop();
            
            var bucketPrefix = ExtractBucketPrefix(context);

            var dimensions = ExtractDimensions(context);

            await Task.WhenAll(
                _statsPublisher.TimingAsync($"{bucketPrefix}.msecs", stopwatch.ElapsedMilliseconds, dimensions), 
                _statsPublisher.IncrementAsync($"{bucketPrefix}.{context.Response.StatusCode}", 1, dimensions));
        }

        private static string ExtractBucketPrefix(IOwinContext context)
        {
            var bucketPrefix = "http.no-route-data";

            if (!context.Environment.ContainsKey(HttpMetrics.OwinContextSplunkMetricsActionKey) ||
                !context.Environment.ContainsKey(HttpMetrics.OwinContextSplunkMetricsControllerKey))
                return bucketPrefix;

            var controller = context.Environment[HttpMetrics.OwinContextSplunkMetricsControllerKey];
            var action = context.Environment[HttpMetrics.OwinContextSplunkMetricsActionKey];
            return $"http.{controller}-{action}-{context.Request.Method}";

        }

        private static KeyValuePair<string, string>[] ExtractDimensions(IOwinContext context)
        {
            return context.Environment.Keys
                .Where(key => key.StartsWith(HttpMetrics.OwinContextSplunkMetricsDimensionPrefix))
                .Select(key => new KeyValuePair<string, string>(
                    key.Substring(HttpMetrics.OwinContextSplunkMetricsDimensionPrefix.Length + 1), 
                    context.Environment[key].ToString()))
                .ToArray();
        }
    }
}
