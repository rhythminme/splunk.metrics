using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Splunk.Metrics.FakeStatsServer;
using Splunk.Metrics.Statsd;
using Xunit;
using Xunit.Abstractions;

namespace Splunk.Metrics.Tests.Integration
{
    public class TimingStatsPublisherShould : IDisposable
    {
        private readonly UdpListener _udpListener;
        private readonly StatsPublisher _statsPublisher;

        [Fact]
        public async Task SendWelformedStatsDUdpPacket()
        {
            using (_statsPublisher.BeginTiming("some-feature.event"))
            {
                await Task.Delay(100);
            }
            _udpListener.GetWrittenBytesAsString().First().Should()
                .MatchRegex($@"some-feature\.event\.msecs:([0-9]+)\|ms\|#instance:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix".ToLowerInvariant());
        }

        public TimingStatsPublisherShould(ITestOutputHelper testOutputHelper)
        {
            _udpListener = new UdpListener(testOutputHelper);
            _statsPublisher = new StatsPublisher(Options.Create(new StatsConfiguration
            {
                Prefix = "test-prefix",
                Host = "localhost",
                Port = _udpListener.Port,
                EnsureLowercasedMetricNames = true,
                SupportSplunkExtendedMetrics = true
            }));
        }

        public void Dispose() => _udpListener?.Dispose();
    }
}