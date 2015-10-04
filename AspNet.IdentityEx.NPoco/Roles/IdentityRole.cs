using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Roles
{

    public class IdentityRole : IRole
    {

        public string Id { get; set; }
		public string ClientId { get; set; }
		public string Name { get; set; }

        public IdentityRole()
        {
        }

        public IdentityRole(string name)
        {
            SetId();
            Name = name;
        }

        public IdentityRole(string clientId, string name)
        {
            SetId();
            Name = name;
            ClientId = clientId;
        }


        public void SetId()
        {
            Id = RandomIdHelper.GetBase62("R", 11);
        }

    }

}
