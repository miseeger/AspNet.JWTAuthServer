using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserClaims
{

	/// <summary>
	///     Extension methods for UserClaimManager
	/// </summary>
	public static class UserClaimManagerExtension
	{

		/// <summary>
		///     Gets claims of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<IdentityClaim> GetUserClaims<TClaim, TUser>(this UserClaimManager<TClaim, TUser> manager,
			string userId)
			where TClaim : IdentityClaim
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			var result = AsyncHelper.RunSync<IdentityUserClaimList>(() => manager.GetUserClaimsListAsync(userId));

			return result.List;
		}


		/// <summary>
		///     Create a userclaim
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="claim"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Create<TClaim, TUser>(this UserClaimManager<TClaim, TUser> manager,
			TClaim claim, TUser user)
			where TClaim : IdentityClaim
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(claim, user));
		}


		/// <summary>
		///     Deletas a certain userclaim
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="claim"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TClaim, TUser>(this UserClaimManager<TClaim, TUser> manager,
			TClaim claim, TUser user)
			where TClaim : IdentityClaim
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(claim, user));
		}


		/// <summary>
		///     Deletas all claims of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TClaim, TUser>(this UserClaimManager<TClaim, TUser> manager,
			TUser user)
			where TClaim : IdentityClaim
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