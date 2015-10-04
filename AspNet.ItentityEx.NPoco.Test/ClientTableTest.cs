using System.Collections.Generic;
using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class ClientTableTest
    {

        private static Database _database;
        private static ClientTable _clientTable;
	    private static UserTable<IdentityUser> _userTable;

	    private const string client = "TestClient";
		private const string client1 = "Web API 1";
        private const string client2 = "Web API 2";

		private const string name1 = "John Doe";
		private const string name2 = "Jane Doe";
		private const string email1 = "john.doe@inter.net";
		private const string email2 = "jane.doe@inter.net";


		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			_database = new Database(IdentityConstants.ConnectionName);
			_clientTable = new ClientTable(_database);
			_database.Execute("DELETE FROM User");
			_database.Execute("DELETE FROM Client");
			
			_database = new Database(IdentityConstants.ConnectionName);
			_userTable = new UserTable<IdentityUser>(_database);
			_clientTable = new ClientTable(_database);

			_clientTable.Insert(new IdentityClient(client));
			var TestClientId = _clientTable.GetClientByName(client).Id;

			_userTable.Insert(new IdentityUser(TestClientId, name1) {Email = email1 });
			_userTable.Insert(new IdentityUser(TestClientId, name2) {Email = email2 });

		}


        private static int CreateClient(string client)
        {
			return _clientTable.Insert(new IdentityClient(client));
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client()
        {
            var result = 0;
            IdentityClient newClient;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateClient(client1);
				newClient = _clientTable.GetClientByName(client1);
                transaction.Dispose();
            }

			Assert.IsNotNull(newClient.Id);
			Assert.IsNotNull(newClient.Base64Secret);
			Assert.IsTrue(newClient.Id.StartsWith("C"));
			Assert.AreEqual(client1, newClient.Name);
            Assert.AreEqual(1, result);
        }


		[TestMethod]
		public void It_gets_clients()
		{
			List<IdentityClient> clients;

			using (var transaction = _database.GetTransaction())
			{
				CreateClient(client1);
				CreateClient(client2);
				clients = _clientTable.GetClients().ToList();
				transaction.Dispose();
			}

			Assert.AreEqual(3, clients.Count);
		}


		[TestMethod]
		public void It_gets_client_by_name()
		{
			IdentityClient client;
			
			using (var transaction = _database.GetTransaction())
			{
				CreateClient(client1);
				CreateClient(client2);
				client = _clientTable.GetClientByName(client2);
				transaction.Dispose();
			}

			Assert.AreEqual(client2, client.Name);
		}


		[TestMethod]
		public void It_gets_client_by_id()
		{
			IdentityClient client;
			IdentityClient clientFromId;

			using (var transaction = _database.GetTransaction())
			{
				CreateClient(client1);
				CreateClient(client2);
				client = _clientTable.GetClientByName(client2);
				clientFromId = _clientTable.GetClientById(client.Id);
				transaction.Dispose();
			}

			Assert.AreEqual(client.Id, clientFromId.Id);
		}


		[TestMethod]
		public void It_gets_client_users()
		{
			List<IdentityUser> users;

			using (var transaction = _database.GetTransaction())
			{
				var testClient = _clientTable.GetClientByName(client);
				users = _clientTable.GetClientUsers(testClient.Id).ToList();
				transaction.Dispose();
			}

			Assert.AreEqual(2, users.Count);
		}


		[TestMethod]
		public void It_deletes_a_client()
		{
			var result = 0;
			List<IdentityUser> users;

			// emulates the delete function in ClientStore
			using (var transaction = _database.GetTransaction())
			{
				var clientToDelete = _clientTable.GetClientByName(client);
				result = _clientTable.Delete(clientToDelete);

				var clientUsers = _clientTable.GetClientUsers(clientToDelete.Id).ToList();

				foreach (var clientUser in clientUsers)
				{
					_userTable.Delete(clientUser);
				}

				users = _clientTable.GetClientUsers(clientToDelete.Id).ToList();
				transaction.Dispose();
			}

			Assert.AreEqual(1, result);
			Assert.IsFalse(users.Any());
		}


		[TestMethod]
		public void It_resets_the_secret()
		{
			string oldSecret;
			string newSecret;

			var result = 0;

			using (var transaction = _database.GetTransaction())
			{
				CreateClient(client1);
				CreateClient(client2);
				var clientToReset = _clientTable.GetClientByName(client1);
				oldSecret = clientToReset.Base64Secret;
				result = _clientTable.ResetSecret(clientToReset);
				var resetClient = _clientTable.GetClientByName(client1);
				newSecret = resetClient.Base64Secret;
				transaction.Dispose();
			}

			Assert.AreEqual(1, result);
			Assert.AreNotEqual(oldSecret, newSecret);
		}

    }

}



