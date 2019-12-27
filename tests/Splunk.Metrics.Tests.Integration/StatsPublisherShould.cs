using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Splunk.Metrics.FakeStatsServer;
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
        public async Task SendWellFormedStatsDUdpPacket()
        {
            await _statsPublisher.IncrementAsync("some-feature.event");
            _udpListener.GetWrittenBytesAsString().Should()
                .Contain($"some-feature.event:1|c|#instance:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix");
        }
        
        [Fact]
        public async Task SendWellFormedStatsDUdpPacketIncludingDimensions()
        {
            await _statsPublisher.IncrementAsync("some-feature.event", 1, new Dictionary<string, string>{{"dimension1","value1"}});
            _udpListener.GetWrittenBytesAsString().Should()
                .Contain($"some-feature.event:1|c|#instance:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix,dimension1:value1");
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