using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using OnlinerTracker.Data.SecurityModels;
using System.Net.Http;

namespace OnlinerTracker.Interfaces
{
    public interface IAuthorizationService
    {
        JObject GenerateLocalAccessTokenResponse(string userName, OAuthBearerAuthenticationOptions oAuthBearerOptions);

        bool ValidateRedirectUri(ref string redirectUriOutput, ref string error, HttpRequestMessage request, Client client);

        bool ValidateClient(HttpRequestMessage request, ref string error, ref Client client);
    }
}
