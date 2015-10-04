using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;

namespace AspNet.IdentityEx.NPoco.Clients
{

	public class ClientUserManager<TClient, TUser>: IDisposable 
        where TClient : IdentityClient
        where TUser : IdentityUser
    {

        private ClientStore<TClient> _clientStore;
		private UserManager<TUser> _userManager;
		private bool _disposed;

		private ClientUserManager()
		{
		}

		public ClientUserManager(ClientStore<TClient> clientStore, UserManager<TUser> userManager)
		{
			_clientStore = clientStore;
			_userManager = userManager;
		}


		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}


		public async Task<TUser> FindUserAsync(string clientId, string userName, string password)
		{
			ThrowIfDisposed();

			var result = default(IdentityUser);
			var userList = await(_clientStore.GetUsersAsync(clientId));
			var user = userList.FirstOrDefault(u => u.UserName == userName) as TUser;

			if (user != null)
			{
				result = 
					await (_userManager.CheckPasswordAsync(user, password)) 
						? user
						: default(TUser);
			}

			return (TUser) result;
		}


		public void Dispose()
		{
			_userManager.Dispose();
			_clientStore.Dispose();
			_disposed = true;
			GC.SuppressFinalize(this);
		}

	}

}