namespace Splunk.Metrics.Statsd
{
    internal interface IEnvironment
    {
        string GetMachineName();
    }
}