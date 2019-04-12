using System;
using System.Collections.Specialized;
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
                GenerateDefaultDimensions());
        }

        public Task GaugeAsync(string bucket, double value)
        {
            throw new NotImplementedException();
        }

        public void Gauge(string bucket, double value)
        {
            throw new NotImplementedException();
        }

        public Task TimingAsync(string feature, long durationMilliseconds)
        {
            throw new NotImplementedException();
        }

        public Task<T> TimingAsync<T>(string feature, Func<Task<T>> func)
        {
            throw new NotImplementedException();
        }

        public void Timing(string feature, long durationMilliseconds)
        {
            throw new NotImplementedException();
        }

        public T Timing<T>(string feature, Func<T> func)
        {
            throw new NotImplementedException();
        }

        public Task IncrementAsync(string bucket, long count = 1)
        {
            throw new NotImplementedException();
        }

        public void Increment(string bucket, int count = 1)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginTiming(string bucket) => new TimingScope(this, bucket);
        
        private async Task SendMetricAsync(string metricType, string name, string prefix, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            await _channel.SendAsync(_metricBucketBuilder.Build(metricType, name, value));
        }
        
        private void SendMetric(string metricType, string name, string prefix, string value)
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