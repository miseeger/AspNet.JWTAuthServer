using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Clients
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity client store
    /// </summary>
    public class ClientStore<TClient> 
		where TClient : IdentityClient
    {
        private ClientTable _clientTable;
	    private UserTable<IdentityUser> _userTable;
		public Database NPocoDb { get; private set; }

        public ClientStore()
        {
			new ClientStore<TClient>(new Database(IdentityConstants.ConnectionName));
        }

		public ClientStore(Database database)
        {
			NPocoDb = database;
			_clientTable = new ClientTable(database);
			_userTable = new UserTable<IdentityUser>(database);
        }


		public IEnumerable<TClient> Clients
        {
			get { return _clientTable.GetClients() as IEnumerable<TClient>; }

        }


		public Task CreateAsync(TClient client)
        {
			if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.Client);
            }

			_clientTable.Insert(client);

            return Task.FromResult<object>(null);
        }


		public Task DeleteAsync(TClient client)
        {
			if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

			_clientTable.Delete(client);
			var clientUsers = _clientTable.GetClientUsers(client.Id).ToList();

			foreach (var clientUser in clientUsers)
			{
				_userTable.Delete(clientUser);
			}

            return Task.FromResult<Object>(null);
        }


		public Task<TClient> FindByIdAsync(string clientId)
        {
			var result = _clientTable.GetClientById(clientId) as TClient;

			return Task.FromResult<TClient>(result);
        }


		public Task<TClient> FindByNameAsync(string clientName)
        {
			var result = _clientTable.GetClientByName(clientName) as TClient;

			return Task.FromResult<TClient>(result);
        }


		public Task<IEnumerable<IdentityUser>> GetUsersAsync(string clientId)
		{
			var result = _clientTable.GetClientUsers(clientId);

			return Task.FromResult(result);
		}


		public Task UpdateAsync(TClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

			_clientTable.Update(client);

            return Task.FromResult<Object>(null);
        }


		public Task ResetSecretAsync(TClient client)
		{
			if (client == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			client.SetSecret();
			_clientTable.Update(client);

			return Task.FromResult<Object>(null);
		}


        public void Dispose()
        {
			if (NPocoDb != null)
            {
				NPocoDb.Dispose();
				NPocoDb = null;
            }

	        _clientTable = null;
	        _userTable = null;

        }

    }

}
