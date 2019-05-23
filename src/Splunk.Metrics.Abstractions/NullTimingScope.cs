using System;

namespace Splunk.Metrics.Abstractions
{
    public class NullTimingScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}