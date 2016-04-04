using KatanaContrib.Security.VK;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlinerTracker.Api.Providers
{
    public class VkontakteAuthProvider : VkAuthenticationProvider
    {
        public override Task Authenticated(VkAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            context.Identity.AddClaim(new Claim("UserId", context.Id));
            return Task.FromResult<object>(null);
        }
    }
}