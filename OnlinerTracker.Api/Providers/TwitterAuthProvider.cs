using Microsoft.Owin.Security.Twitter;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlinerTracker.Api.Providers
{
    public class TwitterAuthProvider : TwitterAuthenticationProvider
    {
        public override Task Authenticated(TwitterAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            context.Identity.AddClaim(new Claim("UserId", context.UserId));
            return Task.FromResult<object>(null);
        }
    }
}