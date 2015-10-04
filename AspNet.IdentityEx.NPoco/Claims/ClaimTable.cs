using System.Collections.Generic;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Claims
{

    /// <summary>
    /// Class that represents the Claim table in the database,
    /// containing given global or client specific claims.
    /// </summary>
    public class ClaimTable 
    {
        private Database _database;

        public ClaimTable(Database database)
        {
            _database = database;
        }


        public int Delete(string claimId)
        {
            return 
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   Claim \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    claimId);
        }


        /// <summary>
        /// Inserts a new claim.
        /// </summary>
        /// <param name="claim"></param>
        /// <returns>
        ///     0 - Not inserted (error)
        ///     1 - Successfully inserted
        ///     2 - Gloabl claim created
        ///     3 - No action needed
        /// </returns>
        public int Insert(IdentityClaim claim)
        {
			var existingClaim = GetClaim(claim.Type, claim.Value);

            if (existingClaim != null)
            {
                // Claim already exists but not for this client: Make existing claim global 
                // if it not already is!
                if (existingClaim.ClientId != null && existingClaim.ClientId != claim.ClientId)
                {
                    existingClaim.ClientId = null;
                    return _database.Update("Claim","Id", existingClaim) != 0 ? 2 : 0;
                }

                return 3;
            }
            else
            {
                return _database.Insert("Claim", "", claim) != null ? 1 : 0;
            }
        }


        public IEnumerable<IdentityClaim> GetClaims()
        {
			return
                _database.Fetch<IdentityClaim>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Claim"
                );
        }


		public string GetClaimId(string claimType, string claimValue)
		{
			return
				_database.ExecuteScalar<string>(
					"SELECT \r\n" +
					"   Id \r\n" +
					"FROM \r\n" +
					"   Claim \r\n" +
					"WHERE \r\n" +
					"   ClientId IS NULL \r\n" +
					"   AND Type = @0 \r\n" +
					"   AND Value = @1",
					claimType,
					claimValue
				);
		}

        public string GetClaimId(string clientId, string claimType, string claimValue)
        {
            return 
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Id \r\n" +
                    "FROM \r\n" +
                    "   Claim \r\n" +
                    "WHERE \r\n" +
					"   ClientId = @0 \r\n" +
					"   AND Type = @1 \r\n" +
                    "   AND Value = @2",
                    clientId, 
					claimType, 
					claimValue
                );
        }


        public IdentityClaim GetClaimById(string claimId)
        {
            return
                _database.FirstOrDefault<IdentityClaim>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Claim \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    claimId
                );
        }


        public IdentityClaim GetClaim(string claimType, string claimValue)
        {
            return
                _database.FirstOrDefault<IdentityClaim>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Claim \r\n" +
                    "WHERE \r\n" +
					"   Type = @0 \r\n" +
					"   AND Value = @1",
                    claimType, 
					claimValue
                );
        }


        public IdentityClaim GetClaim(string clientId, string claimType, string claimValue)
        {
            return
                _database.FirstOrDefault<IdentityClaim>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Claim \r\n" +
                    "WHERE \r\n" +
                    "   Type = @0 \r\n" +
                    "   AND Value = @1 \r\n" +
                    "   AND ClientId = @2",
                    claimType, 
					claimValue, 
					clientId
                );
        }


        public int Update(IdentityClaim claim)
        {
            return
                _database.Update("Claim", "Id", claim);

        }

    }

}
