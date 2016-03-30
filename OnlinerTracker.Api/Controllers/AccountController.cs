using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlinerTracker.Api.Results;
using OnlinerTracker.Data.SecurityModels;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [AutofacControllerConfiguration]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiControllerBase
    {
        #region Fields and Properties

        private readonly SecurityRepository _repo;
        private IAuthorizationService _authService;

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        #endregion

        #region Constructor

        public AccountController(IAuthorizationService authService)
        {
            _repo = new SecurityRepository();
            _authService = authService;
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
            var outError = string.Empty;
            var outClient = new Client();

            if (error != null)
                return BadRequest(Uri.EscapeDataString(error));

            if (!User.Identity.IsAuthenticated)
                return new ChallengeResult(provider, this);

            if (!_authService.ValidateClient(Request, ref outError, ref outClient))
            {
                return BadRequest(outError);
            }

            if (!_authService.ValidateRedirectUri(ref redirectUri, ref outError, Request, outClient))
            {
                return BadRequest(outError);
            }

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

            var user = await _repo.FindUserAsync(new UserLoginInfo(provider, userId));

            var hasRegistered = user != null;
            if (!hasRegistered)
                return BadRequest("External user is not registered");

            //generate access token response
            var accessTokenResponse = _authService.GenerateLocalAccessTokenResponse(user.UserName, user.Id, Startup.OAuthBearerOptions);
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

            var user = await _repo.FindUserAsync(new UserLoginInfo(model.Provider, model.UserId));

            var hasRegistered = user != null;

            if (hasRegistered)
                return BadRequest("External user is already registered");

            user = new ApplicationUser() { UserName = model.UserName, Id = Guid.NewGuid().ToString()};
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
            var accessTokenResponse = _authService.GenerateLocalAccessTokenResponse(user.UserName, user.Id, Startup.OAuthBearerOptions);
            return Ok(accessTokenResponse);
        }
       
        #endregion

        #region Additional methods

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