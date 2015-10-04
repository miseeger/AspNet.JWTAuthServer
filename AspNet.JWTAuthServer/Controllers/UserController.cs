using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.UserLogins;
using AspNet.IdentityEx.NPoco.Users;
using AspNet.JWTAuthServer.Models.Bindings;
using AspNet.JWTAuthServer.Models.Data;
using AspNet.JWTAuthServer.Models.Entities;
using AspNet.JWTAuthServer.Results;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;

namespace AspNet.JWTAuthServer.Controllers
{

    [RoutePrefix("api/users")]
    public class UserController : BaseJwtApiController
    {

        //ClaimsAuthorization: Also works combined with roles:
        //[ClaimsAuthorization(ClaimType="Clientspecific", ClaimValue="Testvalue")]
        //[ClaimsAuthorization(ClaimType="Global", ClaimValue="Test")]

        [Authorize(Roles="Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            var result = JWTUserManager.Users.Select(u => EntityFactory.Create(u)).ToList();
            return Ok(result);
        }


		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("user/{id}", Name = "GetUserById")]
		public async Task<IHttpActionResult> GetUser(string Id)
		{
			var user = await JWTUserManager.FindByIdAsync(Id);

			if (user != null)
			{
				return Ok(EntityFactory.Create(user));
			}

			return NotFound();
		}


		[Authorize(Roles = "Admin")]
		[Route("user/byname/{username}")]
		public async Task<IHttpActionResult> GetUserByName(string username)
		{
			var user = await JWTUserManager.FindByNameAsync(username);

			if (user != null)
			{
				return Ok(EntityFactory.Create(user));
			}

			return NotFound();
		}


		[AllowAnonymous]
		[HttpPost]
		[Route("create")]
		public async Task<IHttpActionResult> CreateUser(CreateUserBinding newUser)
		{

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = new IdentityUser(newUser.ClientId, newUser.UserName)
			{
				Email = newUser.Email,
				FirstName = newUser.FirstName,
				LastName = newUser.LastName,
				Level = 1,
				JoinDate = DateTime.Now.Date,
			};

			var addUserResult = await JWTUserManager.CreateAsync(user, newUser.Password);

			if (!addUserResult.Succeeded)
			{
				return GetErrorResult(addUserResult);
			}

			var code = await JWTUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

			var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

			await JWTUserManager.SendEmailAsync(user.Id,JWTAuthServerConstants.ConfirmMailSubject,
                string.Format(JWTAuthServerConstants.ConfirmMailBody, callbackUrl));

			var locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

			return Created(locationHeader, EntityFactory.Create(user));
		}


		[AllowAnonymous]
		[HttpGet]
		[Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
		public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
			{
				ModelState.AddModelError("", JWTAuthServerConstants.UserIdAndCode);
				return BadRequest(ModelState);
			}

			var result = await JWTUserManager.ConfirmEmailAsync(userId, code);

			if (result.Succeeded)
			{
                // Assigning the default user role when successfully confirming the email
                // so the user will have the default rights to access the API.
                await JWTUserManager.AddToRoleAsync(userId,
					ConfigurationManager.AppSettings["JWTServer.InitialUserRole"]);

				var confirmedUser = JWTUserManager.FindById(userId);
                var client = JWTClientManager.FindById(confirmedUser.ClientId);

                JWTMailer.Send(confirmedUser.Email, JWTAuthServerConstants.ConfirmResponseMailSubject, 
					string.Format(JWTAuthServerConstants.ConfirmResponseMailBody, client.Name));
				
				return Ok();
			}
			else
			{
				return GetErrorResult(result);
			}
		}


		[Authorize]
		[HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBinding changedPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			var requestingUserId = User.Identity.GetUserId();

            var result = await JWTUserManager.ChangePasswordAsync(
				User.Identity.GetUserId(), changedPassword.OldPassword, changedPassword.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

			var changingUser = JWTUserManager.FindById(requestingUserId);
			var client = JWTClientManager.FindById(changingUser.ClientId);

			JWTMailer.Send(changingUser.Email, JWTAuthServerConstants.PasswordChangeMailSubject,
				string.Format(JWTAuthServerConstants.PasswordChangeMailBody, client.Name));

			return Ok();
        }


		[Authorize(Roles = "Admin")]
		[HttpDelete]
		[Route("delete/{id}")]
		public async Task<IHttpActionResult> DeleteUser(string id)
		{
			var appUser = await JWTUserManager.FindByIdAsync(id);

			if (appUser != null)
			{
				var result = await JWTUserManager.DeleteAsync(appUser);

				if (!result.Succeeded)
				{
					return GetErrorResult(result);
				}

				return Ok();
			}

			return NotFound();
		}


        [Authorize(Roles = "Admin")]
        [Route("user/{id}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser(
            [FromUri] string id, [FromBody] string[] rolesToAssign)
        {

            var user = await JWTUserManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoleSet =
                JWTRoleManager.Roles
                    .Where(r => r.ClientId == user.ClientId || r.ClientId == null)
                    .ToList();

            var currentUserRolesList = await JWTUserManager.GetRolesAsync(user.Id);
            var currentUserRoles = currentUserRolesList.List;
            
            var nonExistingRoles = rolesToAssign.Except(currentRoleSet.Select(r => r.Name)).ToArray();

            if (nonExistingRoles.Any())
            {
                ModelState.AddModelError("",
                    string.Format(JWTAuthServerConstants.RolesDontExist, string.Join(",", nonExistingRoles)));
                return BadRequest(ModelState);
            }

            var removeResult =
                await JWTUserManager.RemoveFromRolesAsync(user.Id, currentUserRoles.Select(r => r.Name).ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", JWTAuthServerConstants.RoleRemoveFailed);
                return BadRequest(ModelState);
            }

            var addResult = await JWTUserManager.AddToRolesAsync(user.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", JWTAuthServerConstants.RoleAddFailed);
                return BadRequest(ModelState);
            }

            return Ok();

        }


        // ----- External Authentication --------------------------------------

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            var redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user =
                await
                    JWTUserLoginManager.FindUserByLoginAsync(new IdentityUserLogin()
                        {
                            LoginProvider = externalLogin.LoginProvider,
                            ProviderKey = externalLogin.ProviderKey
                        }
                    );

            var hasRegistered = user != null;

            redirectUri = string.Format(
                "{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&email={5}"
                , redirectUri
                , externalLogin.ExternalAccessToken
                , externalLogin.LoginProvider
                , hasRegistered.ToString()
                , externalLogin.UserName.Trim().Replace(" ","")
				, externalLogin.Email
            );

            return Redirect(redirectUri);
        }


        [AllowAnonymous]
        [Route("RegisterExternal")]
		[HttpPost]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            var user =
                await JWTUserLoginManager.FindUserByLoginAsync(new IdentityUserLogin()
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = verifiedAccessToken.user_id
                    }
                );

            var hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = new IdentityUser(model.ClientId, model.UserName)
                {
					Email = model.EMail
                };

            var result = await JWTUserManager.CreateAsync(user);

			if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

	        var userRole = await JWTRoleManager.FindByNameAsync(IdentityConstants.User);

			if (userRole != null)
			{
				await JWTUserManager.AddToRoleAsync(user, userRole);
			}

			result = await JWTUserLoginManager.CreateAsync(new IdentityUserLogin()
                {
                    UserId = user.Id
                    , ClientId = user.ClientId
                    , LoginProvider = model.Provider
                    , ProviderKey = verifiedAccessToken.user_id
                }
            );

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

            return Ok(accessTokenResponse);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            var user =
                await JWTUserLoginManager.FindUserByLoginAsync(new IdentityUserLogin()
                {
                    LoginProvider = provider,
                    ProviderKey = verifiedAccessToken.user_id
                }
                );

            var hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

            return Ok(accessTokenResponse);
        }


        // ----- External Auth Logic ------------------------------------------

        private string ValidateClientAndRedirectUri(ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            var validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = JWTClientManager.FindById(clientId);

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


        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get the FB-appToken from here: 
                //    https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: 
                //    http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                var appToken = ConfigurationManager.AppSettings["JWTServerClient.Facebook.AppToken"];
                verifyTokenEndPoint = string.Format(
                    "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", 
                    accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format(
                    "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(ConfigurationManager.AppSettings["JWTServerClient.Facebook.AppId"], 
                        parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else if (provider == "Google")
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(ConfigurationManager.AppSettings["JWTServerClient.Google.ClientId"],
                        parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }

            }

            return parsedToken;
        }

															 
        private JObject GenerateLocalAccessTokenResponse(IdentityUser user)
        {
            var tokenExpiration = TimeSpan.FromHours(1);

            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
			identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

			//Get all assigned claims and add them
	        var userClaims = JWTUserManager.GetClaims(user.Id);
	        foreach (var userClaim in userClaims)
	        {
		        identity.AddClaim(new Claim(userClaim.Type, userClaim.Value));
	        }

			//Get all assigned roles and add them as claims
			var userRoles = JWTUserManager.GetRoles(user.Id);
	        foreach (var userRole in userRoles)
	        {
		        identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
	        }

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

			var accessToken = StartUpExtensions.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            var tokenResponse = new JObject(
                new JProperty("userName", user.UserName)
                , new JProperty("access_token", accessToken)
                , new JProperty("token_type", "bearer")
                , new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString())
                , new JProperty(".issued", ticket.Properties.IssuedUtc.ToString())
                , new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );

            return tokenResponse;
        }

    }

}