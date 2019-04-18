using FluentAssertions;
using Splunk.Metrics.Statsd;
using Xunit;

namespace Splunk.Metrics.Tests
{
    public class MetricBucketBuilderWithDefaultsShould
    {
        [Theory]
        [InlineData("metric-prefix", "metric-name", MetricTypes.Count, "1", "metric-name:1|c|#instance:test-machine,namespace:metric-prefix")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Count, "1", "metric-name:1|c|#instance:test-machine,namespace:metric-prefix")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Gauge, "1", "metric-name:1|g|#instance:test-machine,namespace:metric-prefix")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Timing, "1", "metric-name:1|ms|#instance:test-machine,namespace:metric-prefix")]
        public void ReturnAValidMetricName(string prefix, string metricName, string type, string value, string expectedBucketName) =>
            new MetricBucketBuilder(new TestEnvironment(), prefix)
                .Build(type, metricName, value)
                .Should().Be(expectedBucketName);
    }

    public class MetricBucketBuilderWithNonForcedCasingShould
    {
        [Theory]
        [InlineData("metric-prefix", "metric-name", MetricTypes.Count, "1", "metric-name:1|c|#instance:Test-Machine,namespace:metric-prefix")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Count, "1", "METRIC-NAME:1|c|#instance:Test-Machine,namespace:METRIC-PREFIX")]
        public void ReturnAValidMetricName(string prefix, string metricName, string type, string value, string expectedBucketName) =>
            new MetricBucketBuilder(new TestEnvironment(), prefix, false)
                .Build(type, metricName, value)
                .Should().Be(expectedBucketName);
    }
    
    public class MetricBucketBuilderWithNoSupportForDimensionsShould
    {
        [Theory]
        [InlineData("metric-prefix", "metric-name", MetricTypes.Count, "1", "metric-prefix.metric-name:1|c")]
        [InlineData("METRIC-PREFIX", "METRIC-NAME", MetricTypes.Count, "1", "metric-prefix.metric-name:1|c")]
        public void ReturnAValidMetricName(string prefix, string metricName, string type, string value, string expectedBucketName) =>
            new MetricBucketBuilder(new TestEnvironment(), prefix, true, false)
                .Build(type, metricName, value)
                .Should().Be(expectedBucketName);
    }
}