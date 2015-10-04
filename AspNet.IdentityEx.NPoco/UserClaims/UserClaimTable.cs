using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserClaims
{

    /// <summary>
    /// Class that represents the UserClaim table in the database
    /// </summary>
    public class UserClaimTable
    {
        private Database _database;

		private const string queryClaimsOfUser = 
			"SELECT \r\n" +
            "   c.Id \r\n" +
			"   ,c.ClientId \r\n" +
			"   ,c.Type \r\n" +
			"   ,c.Value \r\n" +
            "FROM \r\n" +
            "   UserClaim uc \r\n" +
			"   JOIN Claim c ON (c.Id = uc.ClaimId) \r\n" +
            "WHERE \r\n" +
            "   uc.UserId = @0";

        public UserClaimTable(Database database)
        {
            _database = database;
        }


        public ClaimsIdentity FindByUserId(string userId)
        {
            var userClaims = _database.Fetch<IdentityClaim>(queryClaimsOfUser, userId);

            var claims = new ClaimsIdentity();

            foreach (var claim in userClaims
				.Select(userClaim => new Claim(userClaim.Type, userClaim.Value)))
            {
	            claims.AddClaim(claim);
            }

            return claims;
        }


		public List<IdentityClaim> GetClaims(string userId)
	    {
			return _database.Fetch<IdentityClaim>(queryClaimsOfUser,userId);		    
	    }


        public int Delete(string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserClaim \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                );
        }


		public int Delete(IdentityClaim claim, IdentityUser user )
		{
			return
				_database.Execute(
					"DELETE FROM \r\n" +
					"   UserClaim \r\n" +
					"WHERE \r\n" +
					"   UserId = @0 \r\n" +
					"   AND ClaimId = @1",
					user.Id,
					claim.Id
				);
		}


		public int Insert(string claimId, string userId)
		{
			return
				_database.Insert("UserClaim", "",
					new IdentityUserClaim()
					{
						ClaimId = claimId,
						UserId = userId
					}
				) != null
					? 1
					: 0;
		}
		

		public int Insert(IdentityClaim userClaim, string userId)
		{
			return
				Insert(userClaim.Id, userId);
		}

    }

}
