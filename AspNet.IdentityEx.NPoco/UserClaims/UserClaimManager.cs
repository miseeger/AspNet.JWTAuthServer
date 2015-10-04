using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserClaims
{

    /// <summary>
    ///     Exposes userclaim related api which will automatically save changes to the UserClaimStore
    /// </summary>
    /// <typeparam name="TClaim"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public class UserClaimManager<TClaim, TUser> : IDisposable where TClaim : IdentityClaim where TUser : IdentityUser
    {
        private bool _disposed;

        protected UserClaimStore<TClaim, TUser> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The ClaimStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
        public UserClaimManager(UserClaimStore<TClaim, TUser> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.UserClaim);
            }

            Store = store;
        }


        /// <summary>
        ///     Returns the claims of a user
        /// </summary>
        public virtual async Task<List<TClaim>> GetUserClaimsAsync(string userId)
        {
			ThrowIfDisposed();

			return await Store.GetClaimsAsync(userId) as List<TClaim>;
        }


		/// <summary>
		///     Returns the claims of a user, wrapped in a Property of IdentityUserClaimList
		/// </summary>
	    public virtual async Task<IdentityUserClaimList> GetUserClaimsListAsync(string userId)
	    {
		    ThrowIfDisposed();

		    var userClaimList = await Store.GetClaimsAsync(userId) as List<IdentityClaim>;

		    return new IdentityUserClaimList()
		    {
			    List = userClaimList
		    };
	    }


	    /// <summary>
	    ///     Create a userclaim
	    /// </summary>
	    /// <param name="claim"></param>
	    /// <param name="user"></param>
	    /// <returns></returns>
	    public virtual async Task<IdentityResult> CreateAsync(TClaim claim, TUser user)
        {
            ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

            await Store.CreateAsync(claim, user);

            return IdentityResult.Success;
        }


	    /// <summary>
	    ///     Delete a certain userclaim
	    /// </summary>
	    /// <param name="claim"></param>
	    /// <param name="user"></param>
	    /// <returns></returns>
	    public virtual async Task<IdentityResult> DeleteAsync(TClaim claim, TUser user)
        {
            ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

            await Store.DeleteAsync(claim, user);

            return IdentityResult.Success;
        }


		/// <summary>
		///     Deletes all claims of a user
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> DeleteAsync(TUser user)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			await Store.DeleteAsync(user);

			return IdentityResult.Success;
		}


        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }


        /// <summary>
        ///     When disposing, actually dipose the store
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
            }
            _disposed = true;
        }


        /// <summary>
        ///     Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}