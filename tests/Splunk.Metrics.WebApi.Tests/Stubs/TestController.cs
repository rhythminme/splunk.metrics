using System.Web.Http;

namespace Splunk.Metrics.WebApi.Tests.Stubs
{
    public class TestController : ApiController
    {
        [Route("metrics/{bucket}")]
        [HttpGet, HttpPost]
        public string MetricsLoggedByMiddleware(int bucket) => $"bucket:{bucket}";
    }
}