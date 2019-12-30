using System.Net.Http;
using System.Web.Http;

namespace Splunk.Metrics.WebApi.Tests.Stubs
{
    public class TestController : ApiController
    {
        [Route("metrics/{bucket}")]
        [HttpGet, HttpPost]
        public string MetricsRecordedByMiddleware(int bucket) => $"bucket:{bucket}";

        [Route("dimensions/{value}")]
        [HttpGet, HttpPost]
        public string DimensionsRecordedByMiddleware(string value)
        {
            Request.GetOwinContext().SetDimensionForHttpMetrics("dimension", value);
            return $"bucket:{value}";
        }
    }
}