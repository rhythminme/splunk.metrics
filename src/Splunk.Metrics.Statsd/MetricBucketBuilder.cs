using System;
using System.Collections.Specialized;
using System.Linq;

namespace Splunk.Metrics.Statsd
{
    internal class MetricBucketBuilder
    {
        private readonly IEnvironment _environment;
        private readonly string _prefix;
        private readonly bool _ensureLowercasedMetricNames;
        private readonly bool _supportDimensions;
        private readonly NameValueCollection _defaultDimensions;

        public MetricBucketBuilder(
            IEnvironment environment,
            string prefix = null, 
            bool ensureLowercasedMetricNames = true,
            bool supportDimensions = true)
        {
            _environment = environment;
            _prefix = prefix;
            _ensureLowercasedMetricNames = ensureLowercasedMetricNames;
            _supportDimensions = supportDimensions;
            _defaultDimensions = GenerateDefaultDimensions();
        }
        
        public string Build(string metricType, string name, string value)
        {
            var extendedMetric = GenerateMetricBucketName(name)
                                 + ":" + value
                                 + "|" + metricType
                                 + GenerateDimensionsForExtendedMetrics();
            return _ensureLowercasedMetricNames ? extendedMetric.ToLowerInvariant() : extendedMetric;
        }

        private string GenerateDimensionsForExtendedMetrics() => 
            _defaultDimensions != null && _supportDimensions  ? "|#" + GenerateDimensions(_defaultDimensions) : string.Empty;

        private string GenerateMetricBucketName(string name) => 
            string.IsNullOrEmpty(_prefix) || _supportDimensions  ? name : _prefix + "." + name;

        private static string GenerateDimensions(NameValueCollection dimensions) => 
            string.Join(",", dimensions.AllKeys.Select(dimension => $"{dimension}:{dimensions[dimension]}"));
        
        private NameValueCollection GenerateDefaultDimensions() =>
            !_supportDimensions
                ? null
                : new NameValueCollection
                {
                    {"host", _environment.GetMachineName().ToLowerInvariant()},
                    {"namespace", _prefix.ToLowerInvariant()}
                };
    }
}