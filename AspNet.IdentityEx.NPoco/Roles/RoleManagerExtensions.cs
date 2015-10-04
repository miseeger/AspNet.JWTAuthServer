using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Roles
{

    /// <summary>
    ///     Extension methods for RoleManager
    /// </summary>
    public static class RoleManagerExtensions
    {

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="manager"></param>
		/// <param name="roleId"></param>
        /// <returns></returns>
        public static TRole FindById<TRole>(this RoleManager<TRole> manager,
			string roleId) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

			return AsyncHelper.RunSync<TRole>(() => manager.FindByIdAsync(roleId));
        }


        public static List<IdentityRole> GetRoles<TRole>(this RoleManager<TRole> manager)
            where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            var result = AsyncHelper.RunSync<IdentityRoleList>(() => manager.GetRoleListAsync());

            return result.List;
        }


        /// <summary>
        ///     Find a role
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
		public static TRole Find<TRole>(this RoleManager<TRole> manager,
			string clientId, string roleName) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

			return AsyncHelper.RunSync<TRole>(() => manager.FindAsync(clientId, roleName));
        }


        /// <summary>
        ///     Create a role
        /// </summary>
        /// <param name="manager"></param>
		/// <param name="role"></param>
        /// <returns></returns>
        public static IdentityResult Create<TRole>(this RoleManager<TRole> manager,
			TRole role) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

			return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(role));
        }


        /// <summary>
        ///     Update an existing role
        /// </summary>
        /// <param name="manager"></param>
		/// <param name="role"></param>
        /// <returns></returns>
		public static IdentityResult Update<TRole>(this RoleManager<TRole> manager,
			TRole role) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.UpdateAsync(role));
        }


        /// <summary>
		///     Delete a role
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="role"></param>
        /// <returns></returns>
		public static IdentityResult Delete<TRole>(this RoleManager<TRole> manager,
			TRole role) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
			return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(role));
        }


        /// <summary>
		///     Returns true if the role exists
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientId"></param>
		/// <param name="roleName"></param>
        /// <returns></returns>
		public static bool RoleExists<TRole>(this RoleManager<TRole> manager,
			string clientId, string roleName) where TRole : IdentityRole
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
            return AsyncHelper.RunSync<bool>(() => manager.RoleExistsAsync(clientId, roleName));
        }

    }

}