using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using NPoco;

namespace AspNet.JWTAuthServer.Infrastructure
{
    public class JWTServerUserManager : UserManagerEx<IdentityUser>
    {
        public JWTServerUserManager(UserStore<IdentityUser> store)
            : base(store)
        {
        }

        public static JWTServerUserManager Create(
            IdentityFactoryOptions<JWTServerUserManager> options, IOwinContext context)
        {
            var database = new Database(IdentityConstants.ConnectionName);
            var jwtUserManager = new JWTServerUserManager(new UserStore<IdentityUser>(database));

            // Configure validation logic for usernames
            jwtUserManager.UserValidator = new UserValidator<IdentityUser>(jwtUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            jwtUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
            };

			// Put your mailer implementation of choice, here:
			jwtUserManager.EmailService = new Services.SimpleSMTPMailService();

			options.DataProtectionProvider = new DpapiDataProtectionProvider(JWTAuthServerConstants.AppName);

			jwtUserManager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(
				options.DataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
					TokenLifespan = TimeSpan.FromHours(Convert.ToInt16(
						ConfigurationManager.AppSettings["JWTServer.ConfirmationEmailTokenLifespan"]))
                };

            return jwtUserManager;
        }
    }
}