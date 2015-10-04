using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Claims
{

    /// <summary>
    ///     Exposes claim related api which will automatically save changes to the ClaimStore
    /// </summary>
    /// <typeparam name="TClaim"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ClaimManager<TClaim> : IDisposable where TClaim : IdentityClaim
    {
        private bool _disposed;

        protected ClaimStore<IdentityClaim> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The ClaimStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
        public ClaimManager(ClaimStore<IdentityClaim> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            this.Store = store;
        }


        /// <summary>
        ///     Returns an IQueryable of claims.
        /// </summary>
        public virtual IQueryable<IdentityClaim> Claims
        {
            get
            {
                return Store.Claims;
            }
        }


        /// <summary>
        ///     Create a claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(TClaim claim)
        {
            this.ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            await this.Store.CreateAsync(claim);

            return IdentityResult.Success;
        }

        
        /// <summary>
        ///     Update an existing claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> UpdateAsync(TClaim claim)
        {
            this.ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            await this.Store.UpdateAsync(claim);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Delete a claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> DeleteAsync(TClaim claim)
        {
            this.ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            await this.Store.DeleteAsync(claim);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Returns true if the claim exists
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        public virtual async Task<bool> ClaimExistsAsync(string clientId, string claimType, string claimValue)
        {
            if (claimType == null) throw new ArgumentNullException(IdentityConstants.Claim);
            this.ThrowIfDisposed();

            return await this.Store.FindAsync(clientId, claimType, claimValue) != null;
        }


        /// <summary>
        ///     Find a claim by id
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public virtual async Task<TClaim> FindByIdAsync(string claimId)
        {
            this.ThrowIfDisposed();

            return await this.Store.FindByIdAsync(claimId) as TClaim;
        }


        /// <summary>
        ///     Find a claim
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        public virtual async Task<TClaim> FindAsync(string clientId, string claimType, string claimValue)
        {
            this.ThrowIfDisposed();

            return await this.Store.FindAsync(clientId, claimType, claimValue) as TClaim;
        }


        private void ThrowIfDisposed()
        {
            if (this._disposed)
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
            if (disposing && !this._disposed)
            {
                this.Store.Dispose();
            }
            this._disposed = true;
        }


        /// <summary>
        ///     Dispose this object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}