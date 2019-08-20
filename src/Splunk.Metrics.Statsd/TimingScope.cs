using System;
using System.Collections.Generic;
using System.Diagnostics;
using Splunk.Metrics.Abstractions;

namespace Splunk.Metrics.Statsd
{
    public class TimingScope : IDisposable
    {
        private readonly IStatsPublisher statsd;
        private readonly string bucket;
        private readonly IEnumerable<KeyValuePair<string, string>> additionalDimensions;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public TimingScope(IStatsPublisher statsd, string bucket, IEnumerable<KeyValuePair<string,string>> additionalDimensions = null)
        {
            this.statsd = statsd;
            this.bucket = bucket;
            this.additionalDimensions = additionalDimensions;
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            statsd.Timing($"{bucket}.msecs".ToLowerInvariant(), (long)stopwatch.Elapsed.TotalMilliseconds, additionalDimensions);
        }
    }
}