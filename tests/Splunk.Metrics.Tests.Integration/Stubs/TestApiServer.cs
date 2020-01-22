using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Abstractions;
using Splunk.Metrics.Statsd;

namespace Splunk.Metrics.Tests.Integration.Stubs
{
    public class TestApiServer : IDisposable
    {
        private readonly int _port;
        private IHost _host;
        
        public TestApiServer(int port) => _port = port;

        public async Task<HttpClient> Start()
        {
            var configureWebHost = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseTestServer()
                        .UseStartup<Startup>()
                        .ConfigureTestServices(s =>
                        {
                            s.AddTransient(sp => Options.Create(new StatsConfiguration
                            {
                                Prefix = "Integration.Tests",
                                Port = _port
                            }));
                            s.AddTransient<IStatsPublisher, StatsPublisher>();
                        });
                });
            _host = await configureWebHost.StartAsync();
            return _host.GetTestClient();
        }

        public void Dispose() => _host?.Dispose();
    }
}