using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Statsd;
using Xunit;
using Xunit.Abstractions;

namespace Splunk.Metrics.Tests.Integration
{
    public class StatsPublisherShould : IDisposable
    {
        private readonly UdpListener _udpListener;
        private readonly StatsPublisher _statsPublisher;
        [Fact]
        public async Task SendASingleStatsdUdpPacket()
        {
            await _statsPublisher.IncrementAsync("some-feature.event");
            _udpListener.GetWrittenBytesAsString().Should().HaveCount(1);
        }
        
        [Fact]
        public async Task SendWellformedStatsDUdpPacket()
        {
            await _statsPublisher.IncrementAsync("some-feature.event");
            _udpListener.GetWrittenBytesAsString().Should()
                .Contain($"some-feature.event:1|c|#instance:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix");
        }

        public StatsPublisherShould(ITestOutputHelper testOutputHelper)
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