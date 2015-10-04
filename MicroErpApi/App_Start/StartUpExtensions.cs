using System.Configuration;
using System.Web.Http;
using MicroErpApi.Providers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace MicroErpApi
{

    /// <summary>
    /// Extensions zum vereinfachten Aufruf der Middleware-Komponenten
    /// </summary>
    public static class StartUpExtensions
    {
		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }


        /// <summary>
        /// Needed for external Authentication (Google, Facebook, etc.)
        /// </summary>
        /// <param name="app"></param>
        public static void UseOAuthBearerAuthentication(this IAppBuilder app)
	    {
			OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
		    app.UseOAuthBearerAuthentication(OAuthBearerOptions);
	    }


        /// <summary>
        /// This piece of code is to configure the consuption of JWT auth tokens issued from
        /// the JWT identity server. Api controllers with an [Authorize] attribute will be 
        /// validated with JWT.
        /// 
        /// The following values are taken from the App.config file ...
        /// JWTIssuer: The URL of the authentication server
        /// AudienceClientId: Id of the client, registered in the CLIENT-Table of the auth database
        /// AudienceClientSecret: Secret key of the client, mentioned above.
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UseOAuthJWTAuthentication(this IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["MicroErpApi.JWTIssuer"];
			var audienceId = ConfigurationManager.AppSettings["MicroErpApi.AudienceClientId"];
			var clientSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["MicroErpApi.AudienceClientSecret"]);

            // Api controllers with an "[Authorize]" attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, clientSecret)
                    }
                });
        }


        /// <summary>
        /// Configure the Google authentication mechanism. This piece of code is only used in the
        /// WebApi that'll be accessed by users which are authenticated by Google login.
        /// </summary>
        /// <param name="app"></param>
        public static void UseGoogleAuthentication(this IAppBuilder app)
        {
            app.UseGoogleAuthentication(
                new GoogleOAuth2AuthenticationOptions()
                {
                    ClientId = ConfigurationManager.AppSettings["MicroErpApi.Google.ClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["MicroErpApi.Google.ClientSecret"],
                    Provider = new GoogleAuthProvider()

                }
            );
        }


        /// <summary>
        /// Configure the Facebook authentication mechanism. This piece of code is only used in the
        /// WebApi that'll be accessed by users which are authenticated by Facebook login.
        /// </summary>
        /// <param name="app"></param>
        public static void UseFacebookAuthentication(this IAppBuilder app)
        {
            // Standard: redirectin_url e.g.: http://localhost:9999/signin-facebook
            app.UseFacebookAuthentication(
                new FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings["MicroErpApi.Facebook.AppId"],
                    AppSecret = ConfigurationManager.AppSettings["MicroErpApi.Facebook.AppSecret"],
                    Provider = new FacebookAuthProvider()
                }
            );
        }


        /// <summary>
        /// WebApi Middleware registrieren
        /// </summary>
        /// <param name="app">AppBuilder</param>
        /// <param name="config">Konfiguration für die Api</param>
        public static void UseWebApi(this IAppBuilder app, ref HttpConfiguration config)
        {
            WebApiConfig.Register(ref app, ref config);
        }

    }

}