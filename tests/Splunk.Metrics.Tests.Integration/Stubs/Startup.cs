using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Splunk.Metrics.Http;

namespace Splunk.Metrics.Tests.Integration.Stubs
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpMetrics();
            app.UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}