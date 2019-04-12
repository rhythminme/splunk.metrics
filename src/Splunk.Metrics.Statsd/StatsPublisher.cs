using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Abstractions;
using Splunk.Metrics.Statsd.Channels;

namespace Splunk.Metrics.Statsd
{
    public class StatsPublisher : IStatsPublisher
    {
        private readonly IOptions<StatsConfiguration> _statsConfiguration;
        private readonly IChannel _channel;
        private readonly MetricBucketBuilder _metricBucketBuilder;
        
        public StatsPublisher(IOptions<StatsConfiguration> statsConfiguration)
        {
            _statsConfiguration = statsConfiguration;
            _channel = new UdpChannel(_statsConfiguration.Value.Host, _statsConfiguration.Value.Port);
            _metricBucketBuilder = new MetricBucketBuilder(
                _statsConfiguration.Value.Prefix, 
                _statsConfiguration.Value.EnsureLowercasedMetricNames, 
                _statsConfiguration.Value.SupportSplunkExtendedMetrics,
                GenerateDefaultDimensions());
        }

        public Task GaugeAsync(string bucket, double value) => 
            SendMetricAsync(MetricTypes.Gauge, bucket, value);

        public void Gauge(string bucket, double value) => 
            SendMetric(MetricTypes.Gauge, bucket, value);

        public Task TimingAsync(string bucket, long durationMilliseconds) => 
            SendMetricAsync(MetricTypes.Tinming, bucket, durationMilliseconds);

        public Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func)
        {
            throw new NotImplementedException();
        }

        public void Timing(string bucket, long durationMilliseconds) => 
            SendMetric(MetricTypes.Tinming, bucket, durationMilliseconds);

        public T Timing<T>(string feature, Func<T> func)
        {
            throw new NotImplementedException();
        }

        public Task IncrementAsync(string bucket, long count = 1) => 
            SendMetricAsync(MetricTypes.Count, bucket, count);

        public void Increment(string bucket, long count = 1) => 
            SendMetric(MetricTypes.Count, bucket, count);

        public IDisposable BeginTiming(string bucket) => 
            new TimingScope(this, bucket);

        private async Task SendMetricAsync(string metricType, string bucket, long value)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            await SendMetricAsync(metricType, bucket, value.ToString(CultureInfo.InvariantCulture));
        }
        
        private async Task SendMetricAsync(string metricType, string bucket, double value)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            await SendMetricAsync(metricType, bucket, value.ToString(CultureInfo.InvariantCulture));
        }

        private void SendMetric(string metricType, string bucket, long value)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            SendMetric(metricType, bucket, value.ToString(CultureInfo.InvariantCulture));
        }
        
        private void SendMetric(string metricType, string bucket, double value)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            SendMetric(metricType, bucket, value.ToString(CultureInfo.InvariantCulture));
        }

        private async Task SendMetricAsync(string metricType, string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            await _channel.SendAsync(_metricBucketBuilder.Build(metricType, name, value));
        }
        
        private void SendMetric(string metricType, string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            _channel.Send(_metricBucketBuilder.Build(metricType, name, value));
        }

        private NameValueCollection GenerateDefaultDimensions() =>
            !_statsConfiguration.Value.SupportSplunkExtendedMetrics
                ? null
                : new NameValueCollection
                {
                    {"host", Environment.MachineName.ToLowerInvariant()},
                    {"namespace", _statsConfiguration.Value.Prefix.ToLowerInvariant()}
                };
    }
}