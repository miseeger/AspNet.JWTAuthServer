using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.UserClaims
{

    /// <summary>
    /// Class that implements the user claim store iterfaces
    /// </summary>
    public class UserClaimStore<TClaim, TUser> where TUser : IdentityUser where TClaim : IdentityClaim 
    {
        private UserClaimTable _userClaimTable;
		public Database NPocoDb { get; private set; }

        public UserClaimStore()
        {
			new UserClaimStore<TClaim, TUser>(new Database(IdentityConstants.ConnectionName));
        }

		public UserClaimStore(Database database)
        {
			NPocoDb = database;
            _userClaimTable = new UserClaimTable(database);
        }


	    public Task<ClaimsIdentity> FindByUserAsync(string userId)
	    {
		    var result = _userClaimTable.FindByUserId(userId);
		    return Task.FromResult(result);
	    }


		public Task<List<TClaim>> GetClaimsAsync(string userId)
		{
			var result = _userClaimTable.GetClaims(userId) as List<TClaim>;

			return Task.FromResult(result);
		}


        public Task<int> CreateAsync(TClaim claim, TUser user)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

	        var result = _userClaimTable.Insert(claim.Id, user.Id);

            return Task.FromResult(result);
        }


		public Task DeleteAsync(TClaim claim, TUser user)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

            _userClaimTable.Delete(claim, user);

            return Task.FromResult<Object>(null);
        }


		public Task DeleteAsync(TUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			_userClaimTable.Delete(user.Id);

			return Task.FromResult<Object>(null);
		}


	    public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

		    _userClaimTable = null;
        }

    }

}
