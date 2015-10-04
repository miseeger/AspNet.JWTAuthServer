using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserRoles
{

    /// <summary>
    ///     Exposes userrole related api which will automatically save changes to the UserRoleStore
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public class UserRoleManager<TRole, TUser> : IDisposable where TRole : IdentityRole where TUser : IdentityUser
    {
        private bool _disposed;

        protected UserRoleStore<TRole, TUser> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The RoleStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
		public UserRoleManager(UserRoleStore<TRole, TUser> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.UserRole);
            }

            Store = store;
        }


        /// <summary>
        ///     Returns the claims of a user
        /// </summary>
		public virtual async Task<List<TRole>> GetUserRolesAsync(string userId)
        {
			ThrowIfDisposed();

			return await Store.GetRolesAsync(userId) as List<TRole>;
        }


		/// <summary>
		///     Returns the roles of a user, wrapped in a Property of IdentityRolesList
		/// </summary>
	    public virtual async Task<IdentityUserRoleList> GetUserRoleListAsync(string userId)
	    {
		    ThrowIfDisposed();

		    var userRoleList = await Store.GetRolesAsync(userId) as List<IdentityRole>;

			return new IdentityUserRoleList()
		    {
				List = userRoleList
		    };
	    }


	    /// <summary>
	    ///     Create a userrole
	    /// </summary>
		/// <param name="role"></param>
	    /// <param name="user"></param>
	    /// <returns></returns>
	    public virtual async Task<IdentityResult> CreateAsync(TRole role, TUser user)
        {
            ThrowIfDisposed();

			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			await Store.CreateAsync(role, user);

            return IdentityResult.Success;
        }


	    /// <summary>
	    ///     Delete a certain userrole
	    /// </summary>
		/// <param name="role"></param>
	    /// <param name="user"></param>
	    /// <returns></returns>
		public virtual async Task<IdentityResult> DeleteAsync(TRole role, TUser user)
        {
            ThrowIfDisposed();

			if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			await Store.DeleteAsync(role, user);

            return IdentityResult.Success;
        }


		/// <summary>
		///     Deletes all roles of a user
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