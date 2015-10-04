using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Owin;

namespace MicroErpApi
{

    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // activate CORS
            var cors = new EnableCorsAttribute(
                origins: ConfigurationManager.AppSettings["MicroErpApi.AllowedOrigins"], 
				headers: "*", methods: "*");
            config.EnableCors(cors);

            //Use a cookie to temporarily store information about a user logging in with 
            //a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // For JWT authentication on the resource API
            app.UseOAuthJWTAuthentication();

            // For external auth
            //app.UseOAuthBearerAuthentication();

            // For Google authentication ...
            //app.UseGoogleAuthentication();

            // For Facebook authentication ...
            //app.UseFacebookAuthentication();

            // OWIN Pipeline (Web API)
            app.UseWebApi(ref config);
            app.UseWelcomePage();
        }

    }

}