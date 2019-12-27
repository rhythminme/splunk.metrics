using System;
using Owin;
using Splunk.Metrics.Statsd;

namespace Splunk.Metrics.WebApi
{
    public static class HttpMetricsMiddlewareAppBuilderExtensions
    {
        public static IAppBuilder UseHttpMetrics(this IAppBuilder app, StatsConfiguration options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (options == null) throw new ArgumentNullException(nameof(options));

            app.Use<HttpMetricsMiddleware>(options);

            return app;
        }
    }
}