using System;
using System.Linq;
using System.Threading.Tasks;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Claims
{

    /// <summary>
    /// Class that implements the claim store iterfaces
    /// </summary>
    public class ClaimStore<TClaim>
        where TClaim : IdentityClaim
    {
        private ClaimTable _claimTable;
		public Database NPocoDb { get; private set; }

        public ClaimStore()
        {
            new ClaimStore<TClaim>(new Database(IdentityConstants.ConnectionName));
        }

        public ClaimStore(Database database)
        {
			NPocoDb = database;
            _claimTable = new ClaimTable(database);
        }


        public IQueryable<TClaim> Claims
        {
            get { return _claimTable.GetClaims() as IQueryable<TClaim>; }

        }


        public Task CreateAsync(TClaim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            _claimTable.Insert(claim);

            return Task.FromResult<object>(null);
        }


        public Task DeleteAsync(TClaim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _claimTable.Delete(claim.Id);

            return Task.FromResult<Object>(null);
        }


        public Task<TClaim> FindByIdAsync(string claimId)
        {
            var result = _claimTable.GetClaimById(claimId) as TClaim;

            return Task.FromResult<TClaim>(result);
        }


        public Task<TClaim> FindAsync(string claimType, string claimValue)
        {
            var result = _claimTable.GetClaim(claimType, claimValue) as TClaim;

            return Task.FromResult<TClaim>(result);
        }


		public Task<TClaim> FindAsync(string clientId, string claimType, string claimValue)
		{
			TClaim result = _claimTable.GetClaim(clientId, claimType, claimValue) as TClaim;

			return Task.FromResult<TClaim>(result);
		}


        public Task UpdateAsync(TClaim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            _claimTable.Update(claim);

            return Task.FromResult<Object>(null);
        }


        public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

	        _claimTable = null;
        }

    }

}
