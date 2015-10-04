using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.UserLogins;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.JWTAuthServer.Infrastructure

{
    public class JWTServerUserLoginManager : UserLoginManager<IdentityUser>, IDisposable
    {

        public JWTServerUserLoginManager(UserLoginStore<IdentityUser> userLoginStore) :
            base(userLoginStore)
        {
        }


        public static JWTServerUserLoginManager Create(
            IdentityFactoryOptions<JWTServerUserLoginManager> options, IOwinContext context)
        {
            var jwtServerUserLoginManager = new JWTServerUserLoginManager(
                new UserLoginStore<IdentityUser>(context.Get<JWTServerDatabase>()));

            return jwtServerUserLoginManager;
        }

    }
}