using Microsoft.AspNetCore.Http;

namespace Splunk.Metrics.Http
{
    public static class HttpContextMetricsExtensions
    {
        public static void SetDimensionForHttpMetrics(this HttpContext httpContext, string name, string value) =>
            httpContext.Items[$"{HttpMetrics.OwinContextSplunkMetricsDimensionPrefix}-{name}"] = value;
    }
}