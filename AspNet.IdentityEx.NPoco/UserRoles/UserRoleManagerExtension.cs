using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.UserRoles
{

	/// <summary>
	///     Extension methods for UserRoleManager
	/// </summary>
	public static class UserRoleManagerExtension
	{

		/// <summary>
		///     Gets roles of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<IdentityRole> GetUserRoles<TRole, TUser>(this UserRoleManager<TRole, TUser> manager,
			string userId)
			where TRole : IdentityRole
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			var result = AsyncHelper.RunSync<IdentityUserRoleList>(() => manager.GetUserRoleListAsync(userId));

			return result.List;
		}


		/// <summary>
		///     Create a userrole
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="role"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Create<TRole, TUser>(this UserRoleManager<TRole, TUser> manager,
			TRole role, TUser user)
			where TRole : IdentityRole
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(role, user));
		}


		/// <summary>
		///     Deletas a certain userrole
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="role"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TRole, TUser>(this UserRoleManager<TRole, TUser> manager,
			TRole role, TUser user)
			where TRole : IdentityRole
			where TUser : IdentityUser
		{
			if (manager == null)
			{
				throw new ArgumentNullException(IdentityConstants.Manager);
			}

			return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(role, user));
		}


		/// <summary>
		///     Deletes all roles of a user
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IdentityResult Delete<TRole, TUser>(this UserRoleManager<TRole, TUser> manager,
			TUser user)
			where TRole : IdentityRole
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