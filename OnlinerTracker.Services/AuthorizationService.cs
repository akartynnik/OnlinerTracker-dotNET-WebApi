using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using OnlinerTracker.Core;
using OnlinerTracker.Data.SecurityModels;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace OnlinerTracker.Services
{
    public class AuthorizationService: IAuthorizationService
    {
        private readonly SecurityRepository _repo;
        public AuthorizationService()
        {
            _repo = new SecurityRepository();
        }
        public JObject GenerateLocalAccessTokenResponse(string userName, OAuthBearerAuthenticationOptions oAuthBearerOptions)
        {
            var tokenExpiration = TimeSpan.FromHours(10);
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };
            var ticket = new AuthenticationTicket(identity, props);
            var accessToken = oAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            var tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );
            return tokenResponse;
        }

        public bool ValidateRedirectUri(ref string redirectUriOutput, ref string error, HttpRequestMessage request, Client client)
        {
            Uri redirectUri;
            var redirectUriString = request.GetValueFromQueryString("redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                error = "redirect_uri is required";
                return false;
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                error = "redirect_uri is invalid";
                return false;
            }


            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority),
                    StringComparison.OrdinalIgnoreCase))
            {
                error = string.Format("The given URL is not allowed by Client_id '{0}' configuration.", client.Id);
                return false;
            }

            redirectUriOutput = redirectUri.AbsoluteUri;
            return true;
        }


        public bool ValidateClient(HttpRequestMessage request, ref string error, ref Client client)
        {
            var clientId = request.GetValueFromQueryString("client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                error = "client_Id is required";
                return false;
            }

            var findedClient = _repo.FindClient(clientId);

            if (findedClient == null)
            {
                error = string.Format("Client_id '{0}' is not registered in the system.", findedClient.Id);
                return false;
            }
            client = findedClient;
            return true;
        }
    }
}
