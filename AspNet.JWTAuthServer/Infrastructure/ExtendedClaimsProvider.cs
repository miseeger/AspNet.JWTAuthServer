using System.Collections.Generic;
using System.Security.Claims;
using AspNet.IdentityEx.NPoco.Users;

namespace AspNet.JWTAuthServer.Infrastructure
{

	public static class ExtendedClaimsProvider
	{

		public static IEnumerable<Claim> GetClaims(IdentityUser user)
		{

			var claims = new List<Claim>();

			//Add some extended logic for implicit claim assignment:
			//======================================================
			//
			//var daysInWork = (DateTime.Now.Date - user.JoinDate).TotalDays;
			//
			//if (daysInWork > 90)
			//{
			//	claims.Add(CreateClaim("FTE", "1"));
			//
			//}
			//else
			//{
			//	claims.Add(CreateClaim("FTE", "0"));
			//}

			return claims;
		}


		public static Claim CreateClaim(string type, string value)
		{
			return new Claim(type, value, ClaimValueTypes.String);
		}

	}

}
