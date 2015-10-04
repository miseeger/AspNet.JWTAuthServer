using System.Linq;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco;
using Microsoft.AspNet.Identity;

namespace AspNet.JWTAuthServer.Validators
{
    public class MyCustomPasswordValidator : PasswordValidator
    {

        public override async Task<IdentityResult> ValidateAsync(string password)
        {

            var result = await base.ValidateAsync(password);

            if (password.Contains("abcdef") || password.Contains("123456"))
            {
                var errors = result.Errors.ToList();
                errors.Add(JWTAuthServerConstants.NoSequence);
                result = new IdentityResult(errors);
            }

            return result;
        }

    }
}