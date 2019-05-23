using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class NullStatsPublisher : IStatsPublisher
    {
        public Task GaugeAsync(string bucket, double value) => Task.CompletedTask;

        public void Gauge(string bucket, double value)
        {
        }

        public IDisposable BeginTiming(string bucket) => new NullTimingScope();

        public Task TimingAsync(string bucket, long durationMilliseconds) => Task.CompletedTask;

        public Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func) => Task.FromResult(default(T));

        public void Timing(string bucket, long durationMilliseconds)
        {
        }

        public T Timing<T>(string bucket, Func<T> func) => default(T);

        public Task IncrementAsync(string bucket, long count = 1) => Task.CompletedTask;

        public void Increment(string bucket, long count = 1)
        {
        }
    }
}