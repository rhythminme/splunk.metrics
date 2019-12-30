using Microsoft.AspNetCore.Mvc;
using Splunk.Metrics.Http;

namespace Splunk.Metrics.Tests.Integration.Stubs
{
    public class TestController : Controller
    {
        [HttpGet, HttpPost]
        [Route("/metrics")]
        public IActionResult MetricsRecordedByMiddleware(string id) => Ok();

        [Route("dimensions/{value}")]
        [HttpGet, HttpPost]
        public IActionResult DimensionsRecordedByMiddleware(string value)
        {
            Request.HttpContext.SetDimensionForHttpMetrics("dimension", value);
            return Ok($"bucket:{value}");
        }
    }
}