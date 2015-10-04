using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.JWTAuthServer.Infrastructure;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace AspNet.JWTAuthServer.Providers
{
	public class CustomOAuthProvider : OAuthAuthorizationServerProvider
	{
	    private string ClientId { get; set; }


        /// <summary>
        /// Checks if the requesting client is correctly registered at the identity site and 
        /// thus is trusted. The Client-Id is first retrieved from the HTTP auth header and 
        /// if not contained then from the HTTP request body. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var clientId = string.Empty;
            var clientSecret = string.Empty;

			if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
			{
				context.TryGetFormCredentials(out clientId, out clientSecret);
			}

			if (context.ClientId == null)
			{
				context.SetError("invalid_clientId", "client_Id is not set");
				return Task.FromResult<object>(null);
			}

            var clientManager = context.OwinContext.GetUserManager<JWTServerClientManager>();
            var client = clientManager.FindById(context.ClientId);

            if (client == null)
			{
				context.SetError("invalid_clientId", string.Format("Invalid client_id '{0}'", context.ClientId));
				return Task.FromResult<object>(null);
			}

			context.Validated();
            ClientId = clientId;
			return Task.FromResult<object>(null);
		}


        /// <summary>
        /// Responsible for receiving the username and password from the request and validate them 
        /// against the Identity system. If the credentials are valid and the email is confirmed the
        /// identity for the logged in user is built, containing all the roles and claims for the 
        /// authenticated user.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{

			const string allowedOrigin = "*";

			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<JWTServerUserManager>();
			var clientUserManager = context.OwinContext.GetUserManager<JWTServerClientUserManager>();

            var user = await clientUserManager.FindUserAsync(ClientId, context.UserName, context.Password);
			

			if (user == null)
			{
				context.SetError("invalid_grant", JWTAuthServerConstants.UserNamePasswordError);
				return;
			}

			if (!user.EmailConfirmed)
			{
				context.SetError("invalid_grant", JWTAuthServerConstants.NoEmailConfirmation);
				return;
			}

			var authProps = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         IdentityConstants.ClientPropertyKey, context.ClientId ?? string.Empty
                    }
                });


            // fetching user information from database and creating a ClaimsIdentity
            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

			// extended funcionality ...
			//oAuthIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
			//oAuthIdentity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(oAuthIdentity));

			var ticket = new AuthenticationTicket(oAuthIdentity, authProps);

            // transferring the produced Authentication ticket to an OAuth 2.0 bearer access token
            context.Validated(ticket);

		}
	}
}