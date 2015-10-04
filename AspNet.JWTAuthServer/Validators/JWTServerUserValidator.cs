using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.JWTAuthServer.Validators
{
    public class MyCustomUserValidator : UserValidator<IdentityUser>
    {

        List<string> _allowedEmailDomains = new List<string>
            { "outlook.com", "hotmail.com", "gmail.com", "yahoo.com" , "gmx.de"};

        public MyCustomUserValidator(UserManager<IdentityUser> appUserManager)
            : base(appUserManager)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(IdentityUser user)
        {
            var result = await base.ValidateAsync(user);

            var emailDomain = user.Email.Split('@')[1];

            if (!_allowedEmailDomains.Contains(emailDomain.ToLower()))
            {
                var errors = result.Errors.ToList();

                errors.Add(String.Format("Email domain '{0}' is not allowed", emailDomain));

                result = new IdentityResult(errors);
            }

            return result;
        }

    }

}