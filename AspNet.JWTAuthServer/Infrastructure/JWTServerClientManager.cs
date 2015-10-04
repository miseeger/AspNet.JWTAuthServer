using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;

namespace AspNet.JWTAuthServer.Infrastructure

{
    public class JWTServerClientManager : ClientManager<IdentityClient>, IDisposable
    {

        public JWTServerClientManager(ClientStore<IdentityClient> clientStore)
            : base(clientStore)
        {
        }


        public static JWTServerClientManager Create(
            IdentityFactoryOptions<JWTServerClientManager> options, IOwinContext context)
        {
            var jwtServerClientManager = new JWTServerClientManager(
                new ClientStore<IdentityClient>(context.Get<JWTServerDatabase>()));

            return jwtServerClientManager;
        }

    }
}