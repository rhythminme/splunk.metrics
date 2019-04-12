using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Statsd;
using Xunit;

namespace Splunk.Metrics.Tests.Integration
{
    public class StatsPublisherShould
    {
        [Fact]
        public async Task SendStatsDUdpPacket()
        {
            using (var udpListener = new UdpListener())
            {
                var statsPublisher = new StatsPublisher(Options.Create(new StatsConfiguration
                {
                    Prefix = "test-prefix",
                    Host = "localhost",
                    Port = udpListener.Port,
                }));
                
                await statsPublisher.IncrementAsync("some-feature.event");
                await Task.Delay(2000);
                udpListener.GetWrittenBytesAsString().Should().Be($"some-feature.event:1|c|#host:{Environment.MachineName.ToLowerInvariant()},namespace:test-prefix");
            }
        }
    }
}