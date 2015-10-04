using System;
using AspNet.IdentityEx.NPoco.Helpers;
using AspNet.IdentityEx.NPoco.Users;

namespace AspNet.IdentityEx.NPoco.Clients
{

    /// <summary>
    ///     Extension methods for ClientUserManager
    /// </summary>
    public static class ClientUserManagerExtensions
    {

        public static TUser FindUser<TClient,TUser>(this ClientUserManager<TClient, TUser> manager,
            string clientId, string userName, string password)
            where TClient : IdentityClient where TUser : IdentityUser
        {
            if (manager == null)
            {
                throw new ArgumentNullException(IdentityConstants.Manager);
            }

            return AsyncHelper.RunSync<TUser>(() => manager.FindUserAsync(clientId, userName, password));
        }

    }

}