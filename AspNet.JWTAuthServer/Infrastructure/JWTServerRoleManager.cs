using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Roles;
using IdentityRole = AspNet.IdentityEx.NPoco.Roles.IdentityRole;

namespace AspNet.JWTAuthServer.Infrastructure

{
    public class JWTServerRoleManager : Microsoft.AspNet.Identity.RoleManager<IdentityRole>
    {
        public JWTServerRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static JWTServerRoleManager Create(
            IdentityFactoryOptions<JWTServerRoleManager> options, IOwinContext context)
        {
            var appRoleManager = new JWTServerRoleManager(
                new RoleStore<IdentityRole>(context.Get<JWTServerDatabase>()));

            return appRoleManager;
        }
    }
}