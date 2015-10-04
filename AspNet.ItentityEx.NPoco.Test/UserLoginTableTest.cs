using System.Collections.Generic;
using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.UserLogins;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class UserLoginTableTest
    {

        private static Database _database;
        private static RoleTable _roleTable;
        private static UserTable<IdentityUser> _userTable;
        private static UserLoginTable _userLoginTable;
		private static ClientTable _clientTable;
		private static string TestClientId;

		private const string client = "TestClient";

        private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";


        private const string provider1 = "Facebook";
        private const string key1 = "abcdefghijkl";
        private const string provider2 = "Twitter";
        private const string key2 = "mnopqrstuvwx";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnectionName);
            _roleTable = new RoleTable(_database);
            _userTable = new UserTable<IdentityUser>(_database);
            _userLoginTable = new UserLoginTable(_database);
			_clientTable = new ClientTable(_database);
			
			_clientTable.Insert(new IdentityClient(client));
			TestClientId = _clientTable.GetClientByName(client).Id;

            _userTable.Insert(new IdentityUser(TestClientId, name1) { Email = email1 });
            _userTable.Insert(new IdentityUser(TestClientId, name2) { Email = email2 });
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database.Execute("DELETE FROM User");
        }


        private static int CreateUserLogin(string provider, string key, IdentityUser user)
        {
            return 
                _userLoginTable.Insert(
                    new IdentityUserLogin()
                    {
                        ClientId = user.ClientId
                        , UserId = user.Id
                        , LoginProvider = provider
                        , ProviderKey = key
                    });
        }


        [TestMethod]
        public void It_creates_and_inserts_new_userlogin()
        {
            UserLoginInfo userLogin;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserLogin(provider1, key1, user);
                userLogin = _userLoginTable.FindByUserId(user.Id).FirstOrDefault();
                transaction.Dispose();
            }

            Assert.AreEqual(provider1, userLogin.LoginProvider);
        }


        [TestMethod]
        public void It_finds_logins_by_userid()
        {
            List<UserLoginInfo> userLogins;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserLogin(provider1, key1, user);
                CreateUserLogin(provider2, key2, user);
                userLogins = _userLoginTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.IsTrue(userLogins.Select(ul => ul.LoginProvider).Contains(provider1));
            Assert.IsTrue(userLogins.Select(ul => ul.ProviderKey).Contains(key1));
            Assert.IsTrue(userLogins.Select(ul => ul.LoginProvider).Contains(provider2));
            Assert.IsTrue(userLogins.Select(ul => ul.ProviderKey).Contains(key2));
        }


        [TestMethod]
        public void It_deletes_one_login_of_a_user()
        {
            List<UserLoginInfo> userLogins;
            int result;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserLogin(provider1, key1, user);
                CreateUserLogin(provider2, key2, user);
                result = _userLoginTable.Delete(new IdentityUserLogin()
                    {
                        ClientId = user.ClientId
                        , UserId = user.Id
                        , LoginProvider = provider2
                        , ProviderKey = key2

                    });
                userLogins = _userLoginTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.IsTrue(userLogins.Count == 1);
            Assert.IsTrue(userLogins.Select(ul => ul.LoginProvider).Contains(provider1));
            Assert.IsTrue(userLogins.Select(ul => ul.ProviderKey).Contains(key1));

        }

    }

}


