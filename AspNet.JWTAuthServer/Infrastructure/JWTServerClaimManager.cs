using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Claims;

namespace AspNet.JWTAuthServer.Infrastructure

{
    public class JWTServerClaimManager : ClaimManager<IdentityClaim>, IDisposable
    {

        public JWTServerClaimManager(ClaimStore<IdentityClaim> claimStore)
            : base(claimStore)
        {
        }


        public static JWTServerClaimManager Create(
            IdentityFactoryOptions<JWTServerClaimManager> options, IOwinContext context)
        {
            var jwtClaimManager = new JWTServerClaimManager(
                new ClaimStore<IdentityClaim>(context.Get<JWTServerDatabase>()));

            return jwtClaimManager;
        }

    }

}