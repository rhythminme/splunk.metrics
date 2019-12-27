using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Splunk.Metrics.WebApi
{
    public class HttpMetricsFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            SetActionAndControllerInOwinContextFromActionContext(actionContext);
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private static void SetActionAndControllerInOwinContextFromActionContext(HttpActionContext actionContext)
        {
            var descriptor = actionContext.ActionDescriptor;
            var owinContext = actionContext.Request.GetOwinContext();
            owinContext.Environment.Add(HttpMetrics.OwinContextSplunkMetricsActionKey, descriptor.ActionName);
            owinContext.Environment.Add(HttpMetrics.OwinContextSplunkMetricsControllerKey, descriptor.ControllerDescriptor.ControllerName);
        }
    }
}