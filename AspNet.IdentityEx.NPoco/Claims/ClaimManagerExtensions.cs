using System;
using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Claims
{

    /// <summary>
    ///     Extension methods for ClaimManager
    /// </summary>
    public static class ClaimManagerExtensions
    {

        /// <summary>
        ///     Find a claim by id
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public static TClaim FindById<TClaim>(this ClaimManager<TClaim> manager, 
            string claimId) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<TClaim>(() => manager.FindByIdAsync(claimId));
        }


        /// <summary>
        ///     Find a claim
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientId"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        public static TClaim Find<TClaim>(this ClaimManager<TClaim> manager, 
            string clientId, string claimType, string claimValue) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<TClaim>(() => manager.FindAsync(clientId, claimType, claimValue));
        }


        /// <summary>
        ///     Create a claim
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static IdentityResult Create<TClaim>(this ClaimManager<TClaim> manager, 
            TClaim claim) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(claim));
        }


        /// <summary>
        ///     Update an existing claim
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static IdentityResult Update<TClaim>(this ClaimManager<TClaim> manager, 
            TClaim claim) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.UpdateAsync(claim));
        }


        /// <summary>
        ///     Delete a claim
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static IdentityResult Delete<TClaim>(this ClaimManager<TClaim> manager, 
            TClaim claim) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
            return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(claim));
        }


        /// <summary>
        ///     Returns true if the claim exists
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientId"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        public static bool ClaimExists<TClaim>(this ClaimManager<TClaim> manager, 
            string clientId, string claimType, string claimValue) where TClaim : IdentityClaim
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
            return AsyncHelper.RunSync<bool>(() => manager.ClaimExistsAsync(clientId, claimType, claimValue));
        }

    }

}