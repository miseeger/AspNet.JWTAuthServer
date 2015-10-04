using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Roles;

namespace AspNet.JWTAuthServer.Models.Entities
{

    public class UserEntity
    {

        public string Url { get; set; }
        public string Id { get; set; }
		public string ClientId { get; set; }
        public string UserName { get; set; }
		public string FullName { get; set; }
		public string FullNameLF { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
		public int Level { get; set; }
		public DateTime? JoinDate { get; set; }
		public IList<IdentityRole> Roles { get; set; }
        public IList<IdentityClaim> Claims { get; set; }

    }

}