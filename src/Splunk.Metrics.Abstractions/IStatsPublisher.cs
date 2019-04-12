using System;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    public interface IStatsPublisher
    {
        Task GaugeAsync(string bucket, double value);
        void Gauge(string bucket, double value);
        
        IDisposable BeginTiming(string bucket);
        
        Task TimingAsync(string bucket, long durationMilliseconds);
        Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func);
        void Timing(string bucket, long durationMilliseconds);
        T Timing<T>(string bucket, Func<T> func);
        
        Task IncrementAsync(string bucket, long count = 1L);
        void Increment(string bucket, long count = 1L);
    }
}