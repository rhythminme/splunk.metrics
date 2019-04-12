namespace Splunk.Metrics.Statsd
{
    public class StatsConfiguration
    {
        public string Host { get; set; } = "::1";
        public int Port { get; set; } = 8125;
        public bool SupportSplunkExtendedMetrics { get; set; } = true;
        public string Prefix { get; set; }
        public bool EnsureLowercasedMetricNames { get; set; } = true;
    }
}