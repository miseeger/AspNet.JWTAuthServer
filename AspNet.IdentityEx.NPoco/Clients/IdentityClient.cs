using System;
using System.Security.Cryptography;
using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace AspNet.IdentityEx.NPoco.Clients
{
	public class IdentityClient

	{
		public string Id { get; set; }
		public string Base64Secret { get; set; }
		public string Name { get; set; }
        public string AllowedOrigin { get; set; }
        public Boolean IsActive { get; set; }

        public IdentityClient()
		{
		}

		public IdentityClient(string name)
		{
			Id = RandomIdHelper.GetBase62("C", 11);
			SetSecret();
			Name = name;
		    AllowedOrigin = "*";
		    IsActive = true;
		}


		public void SetSecret()
		{	
			var key = new byte[32];
			RNGCryptoServiceProvider.Create().GetBytes(key);
			Base64Secret = TextEncodings.Base64Url.Encode(key);
		}

	}

}