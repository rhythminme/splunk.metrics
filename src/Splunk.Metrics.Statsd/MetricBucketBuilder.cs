using System.Collections.Generic;
using System.Linq;

namespace Splunk.Metrics.Statsd
{
    internal class MetricBucketBuilder
    {
        private readonly IEnvironment _environment;
        private readonly string _prefix;
        private readonly bool _ensureLowercasedMetricNames;
        private readonly bool _supportDimensions;
        private readonly IEnumerable<KeyValuePair<string, string>> _defaultDimensions;

        public MetricBucketBuilder(
            IEnvironment environment,
            string prefix = null,
            bool ensureLowercasedMetricNames = true,
            bool supportDimensions = true,
            IEnumerable<KeyValuePair<string, string>> additionalDimensions = null)
        {
            _environment = environment;
            _prefix = prefix;
            _ensureLowercasedMetricNames = ensureLowercasedMetricNames;
            _supportDimensions = supportDimensions;
            _defaultDimensions = additionalDimensions == null 
                ? GenerateDefaultDimensions()
                : GenerateDefaultDimensions().Concat(additionalDimensions);
        }

        public string Build(string metricType, string name, string value,
            IEnumerable<KeyValuePair<string, string>> additionalDimensions = null)
        {
            var extendedMetric = GenerateMetricBucketName(name)
                                 + ":" + value
                                 + "|" + metricType
                                 + GenerateDimensionsForExtendedMetrics(additionalDimensions);
            return _ensureLowercasedMetricNames ? extendedMetric.ToLowerInvariant() : extendedMetric;
        }

        private string GenerateDimensionsForExtendedMetrics(
            IEnumerable<KeyValuePair<string, string>> additionalDimensions = null)
        {
            if ((_defaultDimensions == null && additionalDimensions == null) || !_supportDimensions)
                return string.Empty;

            return _defaultDimensions != null && additionalDimensions != null
                ? "|#" + GenerateDimensions(_defaultDimensions) + "," + GenerateDimensions(additionalDimensions)
                : "|#" + GenerateDimensions(_defaultDimensions) + GenerateDimensions(additionalDimensions);
        }

        private string GenerateMetricBucketName(string name) =>
            string.IsNullOrEmpty(_prefix) || _supportDimensions ? name : _prefix + "." + name;

        private static string GenerateDimensions(IEnumerable<KeyValuePair<string, string>> dimensions) =>
            dimensions == null
                ? string.Empty
                : string.Join(",", dimensions.Select(dimension => $"{dimension.Key}:{dimension.Value}"));

        private IEnumerable<KeyValuePair<string, string>> GenerateDefaultDimensions() =>
            !_supportDimensions
                ? null
                : new Dictionary<string, string>
                    {
                        {"instance", _environment.GetMachineName()},
                        {"namespace", _prefix}
                    };
    }
}