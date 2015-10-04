using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Users
{

    public class IdentityUser : IUser
    {

		public string Id { get; set; }
		public string ClientId { get; set; }
		public string UserName { get; set; }
	    public string LastName { get; set; }
	    public string FirstName { get; set; }
	    public DateTime? JoinDate { get; set; }
		public int Level { get; set; }
		public virtual string Email { get; set; }
		public virtual bool EmailConfirmed { get; set; }
		public virtual string PasswordHash { get; set; }
		public virtual string SecurityStamp { get; set; }
		public virtual string PhoneNumber { get; set; }
		public virtual bool PhoneNumberConfirmed { get; set; }
		public virtual bool TwoFactorEnabled { get; set; }
		public virtual DateTime? LockoutEndDateUtc { get; set; }
		public virtual bool LockoutEnabled { get; set; }
		public virtual int AccessFailedCount { get; set; }

	    public IdentityUser()
	    {
	    }

        public IdentityUser(string clientId, string userName)
        {
			Id = RandomIdHelper.GetBase62("U", 11);
			UserName = userName;
	        ClientId = clientId;
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager, string authenticationType)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            // Place to add user claims by a custom Logic ... e.g. ...
            // ...
            //userIdentity.AddClaims(ExtendedClaimsProvider.GetClaims(user));
            //userIdentity.AddClaims(RolesFromClaims.CreateRolesBasedOnClaims(userIdentity));

            return userIdentity;
		}

    }

}
