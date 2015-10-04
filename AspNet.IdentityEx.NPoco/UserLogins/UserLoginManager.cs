using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserLogins
{

    /// <summary>
    ///     Exposes userlogin related api which will automatically save changes to the UserLoginStore
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserLoginManager<TUser> : IDisposable where TUser : IdentityUser
    {
        private bool _disposed;

        protected UserLoginStore<TUser> Store
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="store">The UserLoginStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods</param>
		public UserLoginManager(UserLoginStore<TUser> store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(IdentityConstants.UserLogin);
            }

            Store = store;
        }


        /// <summary>
        ///     Returns the claims of a user
        /// </summary>
		public virtual async Task<List<UserLoginInfo>> GetUserLoginsAsync(string userId)
        {
			ThrowIfDisposed();

			return await Store.FindByUserIdAsync(userId);
        }


		/// <summary>
		///     Returns the logins of a user, wrapped in a Property of IdentityUserLoginList
		/// </summary>
		public virtual async Task<IdentityUserLoginList> GetUserLoginListAsync(string userId)
	    {
		    ThrowIfDisposed();

			var userLoginList = await Store.FindByUserIdAsync(userId);

			return new IdentityUserLoginList()
		    {
				List = userLoginList
		    };
	    }


        public virtual async Task<IdentityUser> FindUserByLoginAsync(IdentityUserLogin userLogin)
        {
            ThrowIfDisposed();

            return await Store.FindUserByLoginAsync(userLogin);
        }


        /// <summary>
        ///     Create a uerlogin
        /// </summary>
        /// <param name="login"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> CreateAsync(IdentityUserLogin login)
        {
            ThrowIfDisposed();

			if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

			await Store.CreateAsync(login);

            return IdentityResult.Success;
        }


	    /// <summary>
	    ///     Delete a certain userlogin
	    /// </summary>
		/// <param name="role"></param>
	    /// <param name="user"></param>
	    /// <returns></returns>
		public virtual async Task<IdentityResult> DeleteAsync(IdentityUserLogin login)
        {
            ThrowIfDisposed();

			if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

			await Store.DeleteAsync(login);

            return IdentityResult.Success;
        }


		/// <summary>
		///     Deletes all logins of a user
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

			await Store.DeleteAsync(user.Id);

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