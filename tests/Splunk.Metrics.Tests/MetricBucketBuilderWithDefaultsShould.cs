using FluentAssertions;
using Splunk.Metrics.Statsd;
using Xunit;

namespace Splunk.Metrics.Tests
{
    public class MetricBucketBuilderWithDefaultsShould
    {
        [Theory]
        [InlineData("metric-prefix", "metric-name", MetricTypes.Count, "1", "metric-prefix.metric-name:1|c")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Count, "1", "metric-prefix.metric-name:1|c")]
        public void ReturnAValidMetricName(string prefix, string metricName, string type, string value, string expectedBucketName) =>
            new MetricBucketBuilder(prefix)
                .Build(type, metricName, value)
                .Should().Be(expectedBucketName);
    }
}