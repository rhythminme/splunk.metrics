using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Splunk.Metrics.FakeStatsServer;
using Splunk.Metrics.Statsd;
using Splunk.Metrics.WebApi.Tests.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Splunk.Metrics.WebApi.Tests
{
    public class HttpMiddlewareShould : IDisposable
    {
        private readonly UdpListener _udpListener;
        private readonly TestApiServer _testApiServer;

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        public async Task EmitTwoMetricsPerMethod(string method)
        {
            using (var testApiClient = _testApiServer.Start())
            {
                var request = new HttpRequestMessage(new HttpMethod(method), "/metrics/1");

                await testApiClient.SendAsync(request);

                _udpListener.GetWrittenBytesAsString().Should().HaveCount(2);
            }
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        public async Task EmitTimingMetricsForMvcRoutesRequested(string method)
        {
            const string controllerName = "test";
            const string actionName = "metricsloggedbymiddleware";

            using (var testApiClient = _testApiServer.Start())
            {
                var expectedRouteBucket = $@"http\.{controllerName}-{actionName}-{method}\.msecs:([0-9]+)\|ms\|#instance:{Environment.MachineName},namespace:unit\.tests".ToLowerInvariant();

                var request = new HttpRequestMessage(new HttpMethod(method), "/metrics/1");

                await testApiClient.SendAsync(request);
                _udpListener.GetWrittenBytesAsString().First().Should().MatchRegex(expectedRouteBucket);
            }
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        public async Task EmitCountMetricsForMvcRoutesRequested(string method)
        {
            const string controllerName = "test";
            const string actionName = "metricsloggedbymiddleware";

            using (var testApiClient = _testApiServer.Start())
            {
                var request = new HttpRequestMessage(new HttpMethod(method), "/metrics/1");

                var response = await testApiClient.SendAsync(request);
                var expectedStatusBucket = $"http.{controllerName}-{actionName}-{method}.{(int)response.StatusCode}:1|c|#instance:{Environment.MachineName},namespace:unit.tests"
                    .ToLowerInvariant();

                _udpListener.GetWrittenBytesAsString().Last().Should().Be(expectedStatusBucket);
            }
        }

        [Fact]
        public async Task EmitTimingMetricsForRequestsThatDoNotContainRouteData()
        {
            using (var testApiClient = _testApiServer.Start())
            {
                var expectedRouteBucket = $@"http\.no-route-data\.msecs:([0-9]+)\|ms\|#instance:{Environment.MachineName},namespace:unit\.tests".ToLowerInvariant();
                var request = new HttpRequestMessage(HttpMethod.Get, "/non-existent-page");

                await testApiClient.SendAsync(request);
                _udpListener.GetWrittenBytesAsString().First().Should().MatchRegex(expectedRouteBucket);
            }
        }

        [Fact]
        public async Task EmitCountMetricsForRequestsThatDoNotContainRouteData()
        {
            using (var testApiClient = _testApiServer.Start())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/non-existent-page");

                var response = await testApiClient.SendAsync(request);
                var expectedStatusBucket = $"http.no-route-data.{(int)response.StatusCode}:1|c|#instance:{Environment.MachineName},namespace:unit.tests"
                    .ToLowerInvariant();

                _udpListener.GetWrittenBytesAsString().Last().Should().Be(expectedStatusBucket);
            }
        }

        public HttpMiddlewareShould(ITestOutputHelper testOutputHelper)
        {
            _udpListener = new UdpListener(testOutputHelper, 2);
            _testApiServer = new TestApiServer(new StatsConfiguration
            {
                Prefix = "Unit.Tests",
                Host = "localhost",
                Port = _udpListener.Port,
                EnsureLowercasedMetricNames = true,
                SupportSplunkExtendedMetrics = true
            }, testOutputHelper);
        }

        public void Dispose()
        {
            _testApiServer?.Dispose();
            _udpListener?.Dispose();
        }
    }
}
