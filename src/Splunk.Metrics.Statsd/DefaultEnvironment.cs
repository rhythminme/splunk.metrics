using System;

namespace Splunk.Metrics.Statsd
{
    public class DefaultEnvironment : IEnvironment
    {
        public string GetMachineName() => Environment.MachineName;
    }
}