using Microsoft.AspNetCore.Mvc;

namespace Splunk.Metrics.Tests.Integration.Stubs
{
    public class TestController : Controller
    {
        [HttpGet]
        [HttpPost]
        [Route("/metrics")]
        public IActionResult MetricsLoggedByMiddleware(string id) => Ok();
    }
}