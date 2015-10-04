using System.Collections.Generic;
using AspNet.IdentityEx.NPoco.Users;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Clients
{

    /// <summary>
    /// Class that represents the Client table in the database
    /// </summary>
    public class ClientTable
    {
        private readonly Database _database;

        public ClientTable(Database database)
        {
            _database = database;
        }


		public IEnumerable<IdentityClient> GetClients()
		{
			return
				_database.Fetch<IdentityClient>(
					"SELECT \r\n" +
					"   * \r\n" +
					"FROM \r\n" +
					"   Client"
				);
		}


		public IdentityClient GetClientById(string clientId)
        {
            return
				_database.FirstOrDefault<IdentityClient>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
					"   Client \r\n" +
					"WHERE \r\n" +
					"   Id = @0",
					clientId
                );
        }


		public IdentityClient GetClientByName(string clientName)
		{
			return
				_database.FirstOrDefault<IdentityClient>(
					"SELECT \r\n" +
					"   * \r\n" +
					"FROM \r\n" +
					"   Client \r\n" +
					"WHERE \r\n" +
					"   Name = @0",
					clientName
				);
		}


		public IEnumerable<IdentityUser> GetClientUsers(string clientId)
		{
			var result = 
			
				_database.Fetch<IdentityUser>(
					"SELECT \r\n" +
					"   * \r\n" +
					"FROM \r\n" +
					"   User \r\n" +
					"WHERE \r\n" +
					"   ClientId = @0",
					clientId
				);

			return result;
		}


		public int Insert(IdentityClient client)
        {
            return _database.Insert("Client", "", client) != null ? 1 : 0;
        }


        private int Delete(string clientId)
        {
			_database.Execute("PRAGMA foreign_keys = ON");

			return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   Client \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    clientId
                );
        }


		public int Delete(IdentityClient client)
        {
            return Delete(client.Id);
        }


		public int Update(IdentityClient client)
        {
            return
                _database.Update("Client", "Id", client);
        }


	    public int ResetSecret(IdentityClient client)
	    {
		    client.SetSecret();

			return
			    Update(client);
	    }

    }

}
