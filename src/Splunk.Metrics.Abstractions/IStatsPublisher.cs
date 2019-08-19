using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Splunk.Metrics.Abstractions
{
    public interface IStatsPublisher
    {
        Task GaugeAsync(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        void Gauge(string bucket, double value, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        
        IDisposable BeginTiming(string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        
        Task TimingAsync(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        Task<T> TimingAsync<T>(string bucket, Func<Task<T>> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        void Timing(string bucket, long durationMilliseconds, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        T Timing<T>(string bucket, Func<T> func, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        
        Task IncrementAsync(string bucket, long count = 1L, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
        void Increment(string bucket, long count = 1L, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null);
    }
}