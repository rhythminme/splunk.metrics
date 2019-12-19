using System.Web.Http;
using System.Web.Http.Filters;
using Microsoft.Owin;
using Owin;
using Splunk.Metrics.Statsd;
using Splunk.Metrics.WebApi.Tests.Stubs;
using Xunit.Abstractions;

[assembly: OwinStartup(typeof(StartUp))]
namespace Splunk.Metrics.WebApi.Tests.Stubs
{
    public class StartUp
    {
        private readonly StatsConfiguration _statsConfiguration;
        private readonly ITestOutputHelper _testOutputHelper;

        public StartUp(StatsConfiguration statsConfiguration, 
            ITestOutputHelper testOutputHelper)
        {
            _statsConfiguration = statsConfiguration;
            _testOutputHelper = testOutputHelper;
        }

        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration, _testOutputHelper);
            app.UseHttpMetrics(_statsConfiguration)
               .UseWebApi(httpConfiguration);
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration httpConfiguration, 
            ITestOutputHelper testOutputHelper)
        {
            httpConfiguration.Filters.Add(new HttpMetricsFilter());
            httpConfiguration.Filters.Add(new TestExceptionFiltersAttribute(testOutputHelper));
            httpConfiguration.MapHttpAttributeRoutes();

            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }

    public class TestExceptionFiltersAttribute : ExceptionFilterAttribute
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestExceptionFiltersAttribute(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _testOutputHelper.WriteLine(context.Exception.ToString());
        }
    }
}