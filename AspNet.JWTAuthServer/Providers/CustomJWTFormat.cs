using System;
using System.IdentityModel.Tokens;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using NPoco;
using Thinktecture.IdentityModel.Tokens;

namespace AspNet.JWTAuthServer.Providers
{

	public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
	{

		private readonly string _issuer = string.Empty;
		private readonly ClientTable _clientTable;

        /// <summary>
        /// Creates the CustomJwtFormat(-ter)
        /// </summary>
        /// <param name="issuer">The Identity API/Server</param>
        public CustomJwtFormat(string issuer)
		{
			_issuer = issuer;
			_clientTable = new ClientTable(new Database(IdentityConstants.ConnectionName));
		}


        /// <summary>
        /// Protecting the data by creating a JWT, containing all claims of the Identity.
        /// The Audience Secret (automatically and randomly generated on client creation) 
        /// is retrieved from the client entry saved in the Identity database. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Protect(AuthenticationTicket data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(IdentityConstants.Data);
			}

			var clientId = 
				data.Properties.Dictionary.ContainsKey(IdentityConstants.ClientPropertyKey) 
					? data.Properties.Dictionary[IdentityConstants.ClientPropertyKey] 
					: null;

			if (string.IsNullOrWhiteSpace(clientId)) 
				throw new InvalidOperationException("AuthenticationTicket.Properties does not include clientid");

			var client = _clientTable.GetClientById(clientId);

			var symmetricKeyAsBase64 = client.Base64Secret;

			var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

			var signingKey = new HmacSigningCredentials(keyByteArray);

			var issued = data.Properties.IssuedUtc;

			var expires = data.Properties.ExpiresUtc;

			var token = new JwtSecurityToken(_issuer, clientId, data.Identity.Claims, 
				issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

			var handler = new JwtSecurityTokenHandler();

			var jwt = handler.WriteToken(token);

			return jwt;
		}


        public AuthenticationTicket Unprotect(string protectedText)
		{
			throw new NotImplementedException();
		}

    }

}