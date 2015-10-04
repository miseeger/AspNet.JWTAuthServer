using AspNet.IdentityEx.NPoco.Helpers;

namespace AspNet.IdentityEx.NPoco.Claims
{

    public class IdentityClaim
    {

        public string Id { get; set; }
		public string ClientId { get; set; }
		public string Type { get; set; }
        public string Value { get; set; }

        public IdentityClaim()
        {
        }

        /// <summary>
        /// Creates a global claim
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
		public IdentityClaim(string claimType, string claimValue)
        {
            SetId();
            Type = claimType;
            Value = claimValue;
        }

		/// <summary>
		/// Creates a client specific (or global) claim
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="claimtype"></param>
		/// <param name="claimValue"></param>
		public IdentityClaim(string clientId, string claimtype, string claimValue)
        {
            SetId();
            ClientId = clientId;
            Type = claimtype;
            Value = claimValue;
        }


        public void SetId()
        {
            Id = RandomIdHelper.GetBase62("CL", 10);
        }

    }

}