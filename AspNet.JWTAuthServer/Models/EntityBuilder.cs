using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;
using AspNet.JWTAuthServer.Infrastructure;
using AspNet.JWTAuthServer.Models.Entities;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.Users;

namespace AspNet.JWTAuthServer.Models
{

    public class EntityBuilder
    {

        private UrlHelper _UrlHelper;
        private JWTServerUserManager _UserManager;

        public EntityBuilder(HttpRequestMessage request, JWTServerUserManager userManager)
        {
            _UrlHelper = new UrlHelper(request);
            _UserManager = userManager;
        }

        public UserEntity Create(IdentityUser user)
        {
			return new UserEntity
            {
                Url = _UrlHelper.Link("GetUserById", new { id = user.Id }),
                Id = user.Id,
				ClientId = user.ClientId,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Roles = _UserManager.GetRolesAsync(user.Id).Result.List,
                Claims = _UserManager.GetClaimsAsyncEx(user).Result.List,
                FullName = string.Format("{0} {1}", user.FirstName, user.LastName),
				FullNameLF = string.Format("{0}, {1}", user.LastName, user.FirstName),
                Level = user.Level,
                JoinDate = user.JoinDate
            };

        }

        public RoleEntity Create(IdentityRole role)
        {

            return new RoleEntity
            {
                Url = _UrlHelper.Link("GetRoleById", new { id = role.Id }),
                Id = role.Id,
                Name = role.Name
            };

        }

    }

}