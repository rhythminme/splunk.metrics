using Microsoft.Owin;

namespace Splunk.Metrics.WebApi
{
    public static class OwinContextMetricsExtensions
    {
        public static void SetDimensionForHttpMetrics(this IOwinContext owinContext, string name, string value) => 
            owinContext.Environment[$"{HttpMetrics.OwinContextSplunkMetricsDimensionPrefix}-{name}"] = value;
    }
}