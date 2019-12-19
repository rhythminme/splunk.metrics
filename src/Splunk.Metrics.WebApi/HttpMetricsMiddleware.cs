using System.Diagnostics;
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
            
            var bucketPrefix = "http.no-route-data";
            if (context.Environment.ContainsKey(HttpMetrics.OwinContextSplunkMetricsActionKey) &&
                context.Environment.ContainsKey(HttpMetrics.OwinContextSplunkMetricsControllerKey))
            {
                var controller = context.Environment[HttpMetrics.OwinContextSplunkMetricsControllerKey];
                var action = context.Environment[HttpMetrics.OwinContextSplunkMetricsActionKey];
                bucketPrefix = $"http.{controller}-{action}-{context.Request.Method}";
            }

            await Task.WhenAll(
                _statsPublisher.TimingAsync($"{bucketPrefix}.msecs", stopwatch.ElapsedMilliseconds), 
                _statsPublisher.IncrementAsync($"{bucketPrefix}.{context.Response.StatusCode}"));
        }
    }
}
