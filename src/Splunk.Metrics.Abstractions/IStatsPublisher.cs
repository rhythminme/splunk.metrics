using System;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    public interface IStatsPublisher
    {
        Task GaugeAsync(string bucket, double value);
        void Gauge(string bucket, double value);
        
        IDisposable BeginTiming(string bucket);
        
        Task TimingAsync(string feature, long durationMilliseconds);
        Task<T> TimingAsync<T>(string feature, Func<Task<T>> func);
        void Timing(string feature, long durationMilliseconds);
        T Timing<T>(string feature, Func<T> func);
        
        Task IncrementAsync(string bucket, long count = 1L);
        void Increment(string bucket, int count = 1);
    }
}