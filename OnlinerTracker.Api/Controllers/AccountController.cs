using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using OnlinerTracker.Api.Results;
using OnlinerTracker.Data.SecurityModels;
using OnlinerTracker.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        #region Fields and Properties

        private readonly SecurityRepository _repo;

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        #endregion

        #region Constructor

        public AccountController()
        {
            _repo = new SecurityRepository();
        }

        #endregion

        #region Web API methods

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            var redirectUri = string.Empty;

            if (error != null)
                return BadRequest(Uri.EscapeDataString(error));

            if (!User.Identity.IsAuthenticated)
                return new ChallengeResult(provider, this);

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
                return BadRequest(redirectUriValidationResult);

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
                return InternalServerError();

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await _repo.FindUserAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            var hasRegistered = user != null;

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&user_id={5}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered,
                                            externalLogin.UserName,
                                            externalLogin.UserId);

            return Redirect(redirectUri);
        }

        //GET api/account/ObtainLocalAccessToken
        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken, string userId)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
                return BadRequest("Provider or external access token is not sent");

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken, userId);
            if (!verifiedAccessToken)
                return BadRequest("Invalid Provider or External Access Token");

            var user = await _repo.FindUserAsync(new UserLoginInfo(provider, userId));

            var hasRegistered = user != null;
            if (!hasRegistered)
                return BadRequest("External user is not registered");

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);
            return Ok(accessTokenResponse);
        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [HttpPost]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken, model.UserId);
            if (!verifiedAccessToken)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            var user = await _repo.FindUserAsync(new UserLoginInfo(model.Provider, model.UserId));

            var hasRegistered = user != null;

            if (hasRegistered)
                return BadRequest("External user is already registered");

            user = new IdentityUser() { UserName = model.UserName };
            var result = await _repo.CreateUserAsync(user);
            if (!result.Succeeded)
                return GetErrorResult(result);

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, model.UserId)
            };

            result = await _repo.AddUserLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
                return GetErrorResult(result);

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName);
            return Ok(accessTokenResponse);
        }
       
        #endregion

        #region Additional methods
        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetValueFromQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetValueFromQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }

        private string GetValueFromQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }


        private async Task<bool> VerifyExternalAccessToken(string provider, string accessToken, string userId)
        {
            var parsedToken = new ParsedExternalAccessToken();
            switch (provider)
            {
                case "Google":
                    var verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
                    var client = new HttpClient();
                    var uri = new Uri(verifyTokenEndPoint);
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                        
                        parsedToken.app_id = jObj["audience"];

                        if (!string.Equals(Startup.GoogleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                    break;
                case "Twitter":
                    //without verification, need fix!
                    parsedToken.user_id = userId;
                    break;
                case "Vkontakte":
                    //without verification, need fix!
                    parsedToken.user_id = userId;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private JObject GenerateLocalAccessTokenResponse(string userName)
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
            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
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
        
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return InternalServerError();

            if (result.Succeeded)
                return null;

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
                return BadRequest();

            return BadRequest(ModelState);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _repo.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}