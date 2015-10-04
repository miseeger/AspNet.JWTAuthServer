using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Owin;

namespace AspNet.JWTAuthServer
{

    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // activate CORS
            var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            config.EnableCors(cors);

            // Registering the Toolbox given to the OWINContext, containing all
            // Services needed.
            app.RegisterContextServices();

            //Use a cookie to temporarily store information about a user logging in with 
            //a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // For the token generation
            app.UseOAuthTokenGeneration();

            // For the resource API (here: the (planned) usermanagement)
            app.UseOAuthJWTAuthentication();

            //For externl Auth
            //app.UseOAuthBearerAuthentication();

            // Gootle Auth (genereation and usage)
            //app.UseGoogleAuthentication();

            // ... aaand also Facebook (genereation and usage) ...
            //app.UseFacebookAuthentication();

            // OWIN Pipeline (Web API)
            app.UseWebApi(ref config);
            app.UseWelcomePage();
        }

    }

}