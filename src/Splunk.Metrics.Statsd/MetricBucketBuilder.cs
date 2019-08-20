using System.Collections.Generic;
using System.Linq;

namespace Splunk.Metrics.Statsd
{
    internal class MetricBucketBuilder
    {
        private const string DimensionsPrefix = "|#";
        private readonly IEnvironment _environment;
        private readonly string _prefix;
        private readonly bool _ensureLowercasedMetricNames;
        private readonly bool _supportDimensions;
        private readonly string _defaultDimensionPrefix;

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
            _defaultDimensionPrefix = $"{DimensionsPrefix}{GenerateDimensions(GenerateDefaultDimensions(additionalDimensions))}";
        }

        public string Build(string metricType, string name, string value)
        {
            var extendedMetric = GenerateMetricBucketName(name)
                                 + ":" + value
                                 + "|" + metricType
                                 + GenerateDimensionsForExtendedMetrics();
            return _ensureLowercasedMetricNames ? extendedMetric.ToLowerInvariant() : extendedMetric;
        }
        
        public string Build(string metricType, string name, string value, IEnumerable<KeyValuePair<string, string>> additionalDimensions)
        {
            var extendedMetric = GenerateMetricBucketName(name)
                                 + ":" + value
                                 + "|" + metricType
                                 + GenerateDimensionsForExtendedMetrics(additionalDimensions);
            return _ensureLowercasedMetricNames ? extendedMetric.ToLowerInvariant() : extendedMetric;
        }

        private string GenerateDimensionsForExtendedMetrics()
        {
            if (!_supportDimensions)
                return string.Empty;
            
            return _defaultDimensionPrefix;
        }
        
        private string GenerateDimensionsForExtendedMetrics(IEnumerable<KeyValuePair<string, string>> additionalDimensions)
        {
            if (!_supportDimensions)
                return string.Empty;

            var additionalDimensionText = GenerateDimensions(additionalDimensions);

            return !string.IsNullOrEmpty(additionalDimensionText)
                ? $"{_defaultDimensionPrefix},{additionalDimensionText}"
                : _defaultDimensionPrefix;
        }

        private string GenerateMetricBucketName(string name) =>
            string.IsNullOrEmpty(_prefix) || _supportDimensions ? name : _prefix + "." + name;

        private static string GenerateDimensions(IEnumerable<KeyValuePair<string, string>> dimensions) =>
            dimensions == null
                ? string.Empty
                : string.Join(",", dimensions.Select(dimension => $"{dimension.Key}:{dimension.Value}"));

        private IEnumerable<KeyValuePair<string, string>> GenerateDefaultDimensions(IEnumerable<KeyValuePair<string, string>> additionalDimensions) =>
            _supportDimensions
                ? additionalDimensions == null
                    ? DefaultDimensions()
                    : DefaultDimensions().Concat(additionalDimensions)
                : null;

        private Dictionary<string, string> DefaultDimensions()
        {
            return new Dictionary<string, string>
            {
                {"instance", _environment.GetMachineName()},
                {"namespace", _prefix}
            };
        }
    }
}