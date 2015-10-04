using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.JWTAuthServer.Infrastructure

{
    public class JWTServerClientUserManager : ClientUserManager<IdentityClient, IdentityUser>, IDisposable
    {

        public JWTServerClientUserManager(ClientStore<IdentityClient> clientStore, 
            UserManager<IdentityUser> userManager) : base(clientStore, userManager)
        {
        }


        public static JWTServerClientUserManager Create(
            IdentityFactoryOptions<JWTServerClientUserManager> options, IOwinContext context)
        {
            var jwtServerClientUserManager = new JWTServerClientUserManager(
                new ClientStore<IdentityClient>(context.Get<JWTServerDatabase>()),
                context.Get<JWTServerUserManager>());

            return jwtServerClientUserManager;
        }

    }
}