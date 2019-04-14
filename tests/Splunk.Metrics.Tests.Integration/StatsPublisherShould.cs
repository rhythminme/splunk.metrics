using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Statsd;
using Xunit;
using Xunit.Abstractions;

namespace Splunk.Metrics.Tests.Integration
{
    public class StatsPublisherShould
    {
        private readonly ITestOutputHelper _testOutputHelper;

        [Fact]
        public async Task SendWellformedStatsDUdpPacket()
        {
            using (var udpListener = new UdpListener(_testOutputHelper))
            {
                var statsPublisher = new StatsPublisher(Options.Create(new StatsConfiguration
                {
                    Prefix = "test-prefix",
                    Host = "localhost",
                    Port = udpListener.Port,
                    EnsureLowercasedMetricNames = true,
                    SupportSplunkExtendedMetrics = true
                }));
                
                await statsPublisher.IncrementAsync("some-feature.event");
                udpListener.GetWrittenBytesAsString().Should().Be($"some-feature.event:1|c|#host:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix");
            }
        }

        public StatsPublisherShould(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
    }
}