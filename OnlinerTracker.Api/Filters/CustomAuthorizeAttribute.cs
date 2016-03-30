using OnlinerTracker.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OnlinerTracker.Api.Filters
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext,
                        CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            IPrincipal principal = actionContext.RequestContext.Principal;
            var newUser = new CustomPrincipal(principal.Identity);
            HttpContext.Current.User = newUser;
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                ||actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                return continuation();
            
            if (string.IsNullOrEmpty(actionContext.Request.Headers.Authorization?.Parameter) || principal == null)
                return Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized));

                var identity = (ClaimsIdentity) principal.Identity;
                var firstOrDefault = identity.Claims.FirstOrDefault(u => u.Type == "userId");
                if (firstOrDefault != null)
                    newUser.Id = Guid.Parse(firstOrDefault.Value);
                HttpContext.Current.User = newUser;
                return continuation();
            
        }

        public bool AllowMultiple => false;
    }
}