using Splunk.Metrics.Statsd;

namespace Splunk.Metrics.Tests
{
    public class TestEnvironment : IEnvironment
    {
        public string GetMachineName() => "Test-Machine";
    }
}