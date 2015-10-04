using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserLogins
{

	/// <summary>
	///     Extension methods for UserLoginManager
	/// </summary>
	public static class UserLoginManagerExtension
	{

		/// <summary>
		///     Gets roles of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<UserLoginInfo> GetUserLogins<TUser>(this UserLoginManager<TUser> manager,
			string userId)
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			var result = AsyncHelper.RunSync<IdentityUserLoginList>(() => manager.GetUserLoginListAsync(userId));

			return result.List;
		}


        public static IdentityUser FindUserByLogin<TUser>(this UserLoginManager<TUser> manager,
            IdentityUserLogin userLogin)
            where TUser : IdentityUser
    	{
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityUser>(() => manager.FindUserByLoginAsync(userLogin));
    	}


	    /// <summary>
        ///     Create a userlogin
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="login"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IdentityResult Create<TUser>(this UserLoginManager<TUser> manager,
			IdentityUserLogin login)
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(login));
		}


		/// <summary>
		///     Deletas a certain userlogin
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="login"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TUser>(this UserLoginManager<TUser> manager,
			IdentityUserLogin login)
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(login));
		}


		/// <summary>
		///     Deletes all roles of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TUser>(this UserLoginManager<TUser> manager,
			TUser user)
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(user));
		}

	}

}