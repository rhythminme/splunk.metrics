using System;
using System.Net.Http;
using Microsoft.Owin.Testing;
using Splunk.Metrics.Statsd;
using Xunit.Abstractions;

namespace Splunk.Metrics.WebApi.Tests.Stubs
{
    public class TestApiServer : IDisposable
    {
        private readonly StatsConfiguration _statsConfiguration;
        private readonly ITestOutputHelper _testOutputHelper;
        private TestServer _server;

        public TestApiServer(
            StatsConfiguration statsConfiguration, 
            ITestOutputHelper testOutputHelper)
        {
            _statsConfiguration = statsConfiguration;
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose() => _server?.Dispose();

        public HttpClient Start()
        {
            _server = TestServer.Create(app =>
            {
                var startUp = new StartUp(_statsConfiguration, _testOutputHelper);
                startUp.Configuration(app);
            });
            return _server.HttpClient;
        }
    }
}
