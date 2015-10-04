using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using AspNet.JWTAuthServer.Infrastructure;
using AspNet.JWTAuthServer.Providers;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using NPoco;
using Owin;

namespace AspNet.JWTAuthServer
{

    /// <summary>
    /// Extensions zum vereinfachten Aufruf der Middleware-Komponenten
    /// </summary>
    public static class StartUpExtensions
    {
		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

		/// <summary>
		/// Register the Identity database (context) and UserManager so that thec can be used
		/// as a Singleton per OWIN-Request inside a Controller. These are the "tools" that can 
		/// be retrieved like this:
		/// 
		///    HttpContext.GetOwinContext().Get<JWTServerDatabase>(); 
		///    HttpContext.GetOwinContext().Get<JWTServerUserManager>(); 
		/// 
		/// This is - as I can see - a kind of Servicelocator ... ;-)
		/// 
		/// </summary>
		/// <param name="app"></param>
		public static void RegisterContextServices(this IAppBuilder app)
        {
            app.CreatePerOwinContext(JWTServerLogger.Create);
            app.CreatePerOwinContext(JWTServerSimpleMailer.Create);
            app.CreatePerOwinContext(JWTServerDatabase.Create);
            app.CreatePerOwinContext<JWTServerClientManager>(JWTServerClientManager.Create);
            app.CreatePerOwinContext<JWTServerUserManager>(JWTServerUserManager.Create);
            app.CreatePerOwinContext<JWTServerClientUserManager>(JWTServerClientUserManager.Create);
            app.CreatePerOwinContext<JWTServerRoleManager>(JWTServerRoleManager.Create);
            app.CreatePerOwinContext<JWTServerClaimManager>(JWTServerClaimManager.Create);
            app.CreatePerOwinContext<JWTServerUserLoginManager>(JWTServerUserLoginManager.Create);
        }


        /// <summary>
        /// Middleware to issue a JWT using the Bearer scheme.
        /// </summary>
        /// <param name="app"></param>
        public static void UseOAuthTokenGeneration(this IAppBuilder app)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString(
                    ConfigurationManager.AppSettings["JWTServer.TokenEndpointPath"]),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(
                    ConfigurationManager.AppSettings["JWTServer.JWTIssuer"])
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }


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
        /// </summary>
        /// <param name="app"></param>
        public static void UseOAuthJWTAuthentication(this IAppBuilder app)
        {
            var clientManager = new JWTServerClientManager(
                new ClientStore<IdentityClient>(new Database(IdentityConstants.ConnectionName)));

            var clientIds = clientManager.Clients
                .Where(c => c.IsActive)
                .Select(c => c.Id)
                .ToArray();

            var clientSecret = TextEncodings.Base64Url.Decode(
                ConfigurationManager.AppSettings["JWTServer.ApiClientSecret"]);

            // Api controllers with an "[Authorize]" attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    // AllowedAudiences = new[] { clientId },
                    AllowedAudiences = clientIds,
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(
                            ConfigurationManager.AppSettings["JWTServer.JWTIssuer"], clientSecret)
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
                    // CallbackPath = new PathString("/auth"), ==> redirection_url = http://localhost:9999/auth
                    // w/o CallbackPath ==> redirection_url = http://localhost:9999/signin-google
                    ClientId = ConfigurationManager.AppSettings["JWTServerClient.Google.GlientId"],
                    ClientSecret = ConfigurationManager.AppSettings["JWTServerClient.Google.ClientSecret"],
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
            // Standard: redirectoin_url = http://localhost:9999/signin-facebook

            app.UseFacebookAuthentication(
                new FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings["JWTServerClient.Facebook.AppId"],
                    AppSecret = ConfigurationManager.AppSettings["JWTServerClient.Facebook.AppSecret"],
                    Provider = new FacebookAuthProvider()
                }
            );
        }


        /// <summary>
        /// Register WebApi Middleware
        /// </summary>
        /// <param name="app">AppBuilder</param>
        /// <param name="config">Konfiguration für die Api</param>
        public static void UseWebApi(this IAppBuilder app, ref HttpConfiguration config)
        {
            WebApiConfig.Register(ref app, ref config);
        }

    }

}