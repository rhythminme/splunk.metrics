using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class NullStatsPublisher : IStatsPublisher
    {
        public Task GaugeAsync(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => Task.CompletedTask;

        public void Gauge(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null)
        {
        }

        public IDisposable BeginTiming(string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => new NullTimingScope();

        public Task TimingAsync(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => Task.CompletedTask;

        public Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => Task.FromResult(default(T));

        public void Timing(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null)
        {
        }

        public T Timing<T>(string bucket, Func<T> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => default(T);

        public Task IncrementAsync(string bucket, long count = 1, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null) => Task.CompletedTask;

        public void Increment(string bucket, long count = 1, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null)
        {
        }
    }
}