using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.UserClaims;
using AspNet.IdentityEx.NPoco.UserRoles;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Users
{
	public class UserManagerEx<TUser> : UserManager<IdentityUser> where TUser : IdentityUser
	{
		private bool _disposed;

		public UserStore<IdentityUser> StoreEx { get; set; }

		public UserManagerEx(UserStore<IdentityUser> store)
			: base(store)
		{
			StoreEx = store;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddClaimAsync(IdentityUser user, IdentityClaim claim)
		{
			this.ThrowIfDisposed();
			await StoreEx.AddClaimAsync(user, claim);

			return new IdentityResult(new string[] { });
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IdentityUserClaimList> GetClaimsAsyncEx(TUser user)
		{
			this.ThrowIfDisposed();
			var result = new IdentityUserClaimList
				{
					List = await StoreEx.GetClaimsAsyncEx(user) as List<IdentityClaim> 
				};

			return result;
		}


		public virtual async Task<IdentityResult> RemoveClaimAsync(TUser user, IdentityClaim claim)
		{
			this.ThrowIfDisposed();
			await StoreEx.RemoveClaimAsync(user, claim);

			return new IdentityResult(new string[] { });
		}


		public virtual async Task<IdentityResult> AddToRoleAsync(TUser user, IdentityRole role)
		{
			this.ThrowIfDisposed();
			await StoreEx.AddToRoleAsync(user, role);

			return new IdentityResult(new string[] { });
		}


		public new virtual async Task<IdentityUserRoleList> GetRolesAsync(string userId)
		{
			this.ThrowIfDisposed();
			var result = new IdentityUserRoleList
			{
				List = await StoreEx.GetRolesAsync(userId) as List<IdentityRole>
			};

			return result;
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
		protected new virtual void Dispose(bool disposing)
		{
			if (disposing && !this._disposed)
			{
				this.Store.Dispose();
				this.StoreEx.Dispose();
				this._disposed = true;
			}
		}



	}

}