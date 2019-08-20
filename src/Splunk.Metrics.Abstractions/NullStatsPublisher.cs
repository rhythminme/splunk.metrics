using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class NullStatsPublisher : IStatsPublisher
    {
        public Task GaugeAsync(string bucket, double value) => Task.CompletedTask;
        public Task GaugeAsync(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => Task.CompletedTask;

        public void Gauge(string bucket, double value) { }
        public void Gauge(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions) { }

        public IDisposable BeginTiming(string bucket) => new NullTimingScope();
        public IDisposable BeginTiming(string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => new NullTimingScope();

        public Task TimingAsync(string bucket, long durationMilliseconds) => Task.CompletedTask;
        public Task TimingAsync(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => Task.CompletedTask;

        public Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func) => Task.FromResult(default(T));
        public Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => Task.FromResult(default(T));

        public void Timing(string bucket, long durationMilliseconds) { }
        public void Timing(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions) { }

        public T Timing<T>(string bucket, Func<T> func) => default(T);
        public T Timing<T>(string bucket, Func<T> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => default(T);

        public Task IncrementAsync(string bucket, long count = 1) => Task.CompletedTask;
        public Task IncrementAsync(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions) => Task.CompletedTask;

        public void Increment(string bucket, long count = 1) { }
        public void Increment(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions) { }
    }
}