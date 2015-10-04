using System;
using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.UserClaims;
using AspNet.IdentityEx.NPoco.UserRoles;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Users
{

    /// <summary>
    ///     Extension methods for UserManagerEx
    /// </summary>
    public static class UserManagerExExtensions
    {

        /// <summary>
        ///     Create a userclaim
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="claim"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IdentityResult AddClaim<TClaim, TUser>(this UserManagerEx<TUser> manager,
            TUser user, TClaim claim)
            where TClaim : IdentityClaim
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.AddClaimAsync(user, claim));
        }


        /// <summary>
        /// Gets the claims of a user as list of IdentityClaim
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<IdentityClaim> GetClaimsEx<TUser>(this UserManagerEx<TUser> manager,
            TUser user)
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            var result = AsyncHelper.RunSync<IdentityUserClaimList>(() => manager.GetClaimsAsyncEx(user));
            return result.List;
        }


        /// <summary>
        /// Removes an IdentitClaim from user
        /// </summary>
        /// <typeparam name="TClaim"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static IdentityResult RemoveClaim<TClaim, TUser>(this UserManagerEx<TUser> manager,
            TUser user, TClaim claim)
            where TClaim : IdentityClaim
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.RemoveClaimAsync(user, claim));
        }


        /// <summary>
        /// Adds a user to an IdentityRole
        /// </summary>
        /// <typeparam name="TRole"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IdentityResult AddToRole<TRole, TUser>(this UserManagerEx<TUser> manager,
            TUser user, TRole role)
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.AddToRoleAsync(user, role));
        }


        /// <summary>
        /// Gets all roles a user is in (as List of IdentityRole)
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<IdentityRole> GetRoles<TUser>(this UserManagerEx<TUser> manager,
            TUser user)
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            var result = AsyncHelper.RunSync<IdentityUserRoleList>(() => manager.GetRolesAsync(user.Id));
            return result.List;
        }


        /// <summary>
        /// Removes a User from a role
        /// </summary>
        /// <typeparam name="TRole"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IdentityResult RemoveFromRole<TRole, TUser>(this UserManagerEx<TUser> manager,
            TUser user, TRole role)
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.RemoveFromRoleAsync(user.Id, role.Id));
        }

    }

}