using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Abstractions;
using Splunk.Metrics.Statsd.Channels;

namespace Splunk.Metrics.Statsd
{
    public class StatsPublisher : IStatsPublisher
    {
        private readonly IChannel _channel;
        private readonly MetricBucketBuilder _metricBucketBuilder;

        public StatsPublisher(IOptions<StatsConfiguration> statsConfiguration,
            IEnumerable<KeyValuePair<string, string>> additionalDimensions = null)
        {
            _channel = new UdpChannel(statsConfiguration.Value.Host, statsConfiguration.Value.Port);
            _metricBucketBuilder = new MetricBucketBuilder(
                new DefaultEnvironment(),
                statsConfiguration.Value.Prefix,
                statsConfiguration.Value.EnsureLowercasedMetricNames,
                statsConfiguration.Value.SupportSplunkExtendedMetrics,
                additionalDimensions);
        }

        public Task GaugeAsync(string bucket, double value) => 
            SendMetricAsync(MetricTypes.Gauge, bucket, value);

        public Task GaugeAsync(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetricAsync(MetricTypes.Gauge, bucket, value, additionalDimensions);
        
        public void Gauge(string bucket, double value) => 
            SendMetric(MetricTypes.Gauge, bucket, value);

        public void Gauge(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetric(MetricTypes.Gauge, bucket, value, additionalDimensions);
        
        public Task TimingAsync(string bucket, long durationMilliseconds) => 
            SendMetricAsync(MetricTypes.Timing, bucket, durationMilliseconds);

        public Task TimingAsync(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetricAsync(MetricTypes.Timing, bucket, durationMilliseconds, additionalDimensions);
        
        public void Timing(string bucket, long durationMilliseconds) => 
            SendMetric(MetricTypes.Timing, bucket, durationMilliseconds);
        
        public void Timing(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetric(MetricTypes.Timing, bucket, durationMilliseconds, additionalDimensions);

        public async Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func)
        {
            using (BeginTiming(bucket)) return await func();
        }
        
        public async Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            using (BeginTiming(bucket, additionalDimensions)) return await func();
        }

        public T Timing<T>(string bucket, Func<T> func)
        {
            using (BeginTiming(bucket)) return func();
        }
        
        public T Timing<T>(string bucket, Func<T> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            using (BeginTiming(bucket, additionalDimensions)) return func();
        }

        public Task IncrementAsync(string bucket, long count = 1) => 
            SendMetricAsync(MetricTypes.Count, bucket, count);
        
        public Task IncrementAsync(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetricAsync(MetricTypes.Count, bucket, count, additionalDimensions);

        public void Increment(string bucket, long count = 1) => 
            SendMetric(MetricTypes.Count, bucket, count);

        public void Increment(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            SendMetric(MetricTypes.Count, bucket, count, additionalDimensions);
        
        public IDisposable BeginTiming(string bucket) => 
            new TimingScope(this, bucket);
        
        public IDisposable BeginTiming(string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => 
            new TimingScope(this, bucket, additionalDimensions);

        private async Task SendMetricAsync(string metricType, string bucket, long value, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            await SendMetricAsync(metricType, bucket, value.ToString(CultureInfo.InvariantCulture), additionalDimensions);
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
        
        private async Task SendMetricAsync(string metricType, string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            await SendMetricAsync(metricType, bucket, value.ToString(CultureInfo.InvariantCulture), additionalDimensions);
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
        
        private void SendMetric(string metricType, string bucket, long value, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            SendMetric(metricType, bucket, value.ToString(CultureInfo.InvariantCulture), additionalDimensions);
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
        
        private void SendMetric(string metricType, string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", bucket, value);
                return;
            }

            SendMetric(metricType, bucket, value.ToString(CultureInfo.InvariantCulture), additionalDimensions);
        }

        private async Task SendMetricAsync(string metricType, string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            await _channel.SendAsync(_metricBucketBuilder.Build(metricType, name, value));
        }
        
        private async Task SendMetricAsync(string metricType, string name, string value, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            await _channel.SendAsync(_metricBucketBuilder.Build(metricType, name, value, additionalDimensions.ToArray()));
        }
        
        private void SendMetric(string metricType, string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            _channel.Send(_metricBucketBuilder.Build(metricType, name, value));
        }
        
        private void SendMetric(string metricType, string name, string value, IEnumerable<KeyValuePair<string,string>> additionalDimensions)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            _channel.Send(_metricBucketBuilder.Build(metricType, name, value, additionalDimensions.ToArray()));
        }
    }
}