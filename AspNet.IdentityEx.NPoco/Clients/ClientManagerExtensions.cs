using System;
using AspNet.IdentityEx.NPoco.Helpers;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Clients
{

    /// <summary>
    ///     Extension methods for ClientManager
    /// </summary>
    public static class ClientManagerExtensions
    {

        /// <summary>
        ///     Find a client by id
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static TClient FindById<TClient>(this ClientManager<TClient> manager, 
            string clientId) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<TClient>(() => manager.FindByIdAsync(clientId));
        }


        /// <summary>
        ///     Find a client
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public static TClient FindByName<TClient>(this ClientManager<TClient> manager, 
            string clientName) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<TClient>(() => manager.FindByNameAsync(clientName));
        }


        /// <summary>
        ///     Create a client
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IdentityResult Create<TClient>(this ClientManager<TClient> manager, 
            TClient client) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.CreateAsync(client));
        }


        /// <summary>
        ///     Update an existing client
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IdentityResult Update<TClient>(this ClientManager<TClient> manager, 
            TClient client) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<IdentityResult>(() => manager.UpdateAsync(client));
        }


        /// <summary>
        ///     Delete a client
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IdentityResult Delete<TClient>(this ClientManager<TClient> manager, 
            TClient client) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
            return AsyncHelper.RunSync<IdentityResult>(() => manager.DeleteAsync(client));
        }


        /// <summary>
        ///     Returns true if the client exists
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public static bool ClientExists<TClient>(this ClientManager<TClient> manager, 
            string clientName) where TClient : IdentityClient
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }
            return AsyncHelper.RunSync<bool>(() => manager.ClientExistsAsync(clientName));
        }

    }

}