using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace OnlinerTracker.Data.SecurityModels
{
    public class ExternalLoginData
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string UserName { get; set; }

        public string ExternalAccessToken { get; set; }

        public string UserId { get; set; }

        public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
                return null;

            var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
                return null;

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                return null;

            return new ExternalLoginData
            {
                LoginProvider = providerKeyClaim.Issuer,
                ProviderKey = providerKeyClaim.Value,
                UserName = identity.FindFirstValue(ClaimTypes.Name),
                ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                UserId = identity.FindFirstValue("UserId"),
            };
        }
    }
}
