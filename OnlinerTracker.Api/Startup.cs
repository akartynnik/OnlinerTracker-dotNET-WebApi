using Autofac;
using FluentScheduler;
using KatanaContrib.Security.VK;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Twitter;
using OnlinerTracker.Api.Providers;
using Owin;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

[assembly: OwinStartup(typeof(OnlinerTracker.Api.Startup))]
namespace OnlinerTracker.Api
{
    public class Startup
    {
        #region Properties
        
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static TwitterAuthenticationOptions TwitterAuthOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }
        public static VkAuthenticationOptions VkontakteAuthOptions { get; private set; }

        #endregion

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //SignalR
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true,
                EnableJSONP = true
            };
            app.MapSignalR("/signalr", hubConfiguration);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            IContainer iocContainer;
            AutofacConfig.Register(config, out iocContainer);
            app.UseAutofacMiddleware(iocContainer);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
            
            JobManager.JobFactory = new JobFactory(config);
            JobManager.Initialize(new FluentSchedulerConfig());
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);

            //Bearer authentication init
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);


            #region Google authentication configurations

            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["authConfig:GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["authConfig:GoogleClientSecret"],
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleAuthOptions);

            #endregion

            #region Twitter authentication configurations

            TwitterAuthOptions = new TwitterAuthenticationOptions()
            {
                ConsumerKey = ConfigurationManager.AppSettings["authConfig:TwitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["authConfig:TwitterConsumerSecret"],
                Provider = new TwitterAuthProvider(),
                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[]
                    {
                        "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                        "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                        "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                        "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                        "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A 
                        "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
                    })
            };
            app.UseTwitterAuthentication(TwitterAuthOptions);

            #endregion

            #region Vkontakte authentication configurations

            VkontakteAuthOptions = new VkAuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["authConfig:VkontakteClientId"],
                ClientSecret = ConfigurationManager.AppSettings["authConfig:VkontakteClientSecret"],
                Scope = new List<string>() { "email" },
                Provider = new VkontakteAuthProvider()
            };
            app.UseVkontakteAuthentication(VkontakteAuthOptions);

            #endregion
        }
    }
}