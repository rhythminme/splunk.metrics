using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    public interface IStatsPublisher
    {
        Task GaugeAsync(string bucket, double value);
        Task GaugeAsync(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        void Gauge(string bucket, double value);
        void Gauge(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        
        IDisposable BeginTiming(string bucket);
        IDisposable BeginTiming(string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        
        Task TimingAsync(string bucket, long durationMilliseconds);
        Task TimingAsync(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func);
        Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        void Timing(string bucket, long durationMilliseconds);
        void Timing(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        T Timing<T>(string bucket, Func<T> func);
        T Timing<T>(string bucket, Func<T> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        
        Task IncrementAsync(string bucket, long count = 1L);
        Task IncrementAsync(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
        void Increment(string bucket, long count = 1L);
        void Increment(string bucket, long count, IEnumerable<KeyValuePair<string,string>> additionalDimensions);
    }
}