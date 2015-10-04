using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Clients
{

    /// <summary>
    ///     Exposes client related api which will automatically save changes to the ClientStore
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public class ClientManager<TClient> : IDisposable where TClient : IdentityClient
    {
        private bool _disposed;

        protected ClientStore<IdentityClient> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The ClientStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
        public ClientManager(ClientStore<IdentityClient> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            this.Store = store;
        }


        /// <summary>
        ///     Returns an IQueryable of clients.
        /// </summary>
        public virtual IEnumerable<IdentityClient> Clients
        {
            get
            {
                return Store.Clients;
            }
        }


        /// <summary>
        ///     Create a client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(TClient client)
        {
            this.ThrowIfDisposed();

            if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.Client);
            }

            await this.Store.CreateAsync(client);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Update an existing client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> UpdateAsync(TClient client)
        {
            this.ThrowIfDisposed();

            if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.Client);
            }

            await this.Store.UpdateAsync(client);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Delete a client
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> DeleteAsync(TClient client)
        {
            this.ThrowIfDisposed();

            if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.Client);
            }

            await this.Store.DeleteAsync(client);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Returns true if the claim exists
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public virtual async Task<bool> ClientExistsAsync(string clientName)
        {
            this.ThrowIfDisposed();

            return await this.Store.FindByNameAsync(clientName) != null;
        }


        /// <summary>
        ///     Find a claim by id
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public virtual async Task<TClient> FindByIdAsync(string clientId)
        {
            this.ThrowIfDisposed();

            return await this.Store.FindByIdAsync(clientId) as TClient;
        }


        /// <summary>
        ///     Find a claim
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public virtual async Task<TClient> FindByNameAsync(string clientName)
        {
            this.ThrowIfDisposed();

            return await this.Store.FindByNameAsync(clientName) as TClient;
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