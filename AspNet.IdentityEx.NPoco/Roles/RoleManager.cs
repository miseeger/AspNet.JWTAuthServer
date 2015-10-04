using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Roles
{

    /// <summary>
    ///     Exposes role related api which will automatically save changes to the RoleStore
    /// </summary>
    /// <typeparam name="TClaim"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class RoleManager<TRole> : IDisposable where TRole : IdentityRole
    {
        private bool _disposed;

        protected RoleStore<TRole> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The RoleStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
		public RoleManager(RoleStore<TRole> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            this.Store = store;
        }


        /// <summary>
        ///     Returns an IQueryable of role.
        /// </summary>
        public virtual IEnumerable<IdentityRole> Roles
        {
            get
            {
                return Store.Roles;
            }
        }

        /// <summary>
        ///     Returns all roles, wrapped in a Property of IdentityRolesList
        /// </summary>
        public virtual async Task<IdentityRoleList> GetRoleListAsync()
        {
            ThrowIfDisposed();

            return  await Store.GetRolesAsync() as IdentityRoleList;
        }



        /// <summary>
        ///     Create a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(TRole role)
        {
            this.ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

			await this.Store.CreateAsync(role);

            return IdentityResult.Success;
        }

        
        /// <summary>
        ///     Update an existing role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
		public virtual async Task<IdentityResult> UpdateAsync(TRole role)
        {
            this.ThrowIfDisposed();

			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

			await this.Store.UpdateAsync(role);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Delete a claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
		public virtual async Task<IdentityResult> DeleteAsync(TRole role)
        {
            this.ThrowIfDisposed();

			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

			await this.Store.DeleteAsync(role);

            return IdentityResult.Success;
        }


        /// <summary>
        ///     Returns true if the claim exists
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
		public virtual async Task<bool> RoleExistsAsync(string clientId, string roleName)
        {
			this.ThrowIfDisposed();

			if (roleName == null) throw new ArgumentNullException(IdentityConstants.Role);

            return await this.Store.FindByNameAsync(clientId, roleName) != null;
        }


        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
		public virtual async Task<TRole> FindByIdAsync(string roleId)
        {
            this.ThrowIfDisposed();

			return await this.Store.FindByIdAsync(roleId) as TRole;
        }


        /// <summary>
        ///     Find a claim
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
		public virtual async Task<TRole> FindAsync(string clientId, string roleName)
        {
            this.ThrowIfDisposed();

			return await this.Store.FindByNameAsync(clientId, roleName) as TRole;
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