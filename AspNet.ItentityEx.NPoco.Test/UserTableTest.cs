using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class UserTableTest
    {

        private static Database _database;
        private static UserTable<IdentityUser> _userTable;
	    private static ClientTable _clientTable;
		private static string TestClientId;

	    private const string client = "TestClient";

		private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";

        private const string passwordHash = "ThisIsThePasswordHash...";
        private const string securityStamp = "ThisIsTheSecurityStamp...";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnectionName);
            _userTable = new UserTable<IdentityUser>(_database);
			_clientTable = new ClientTable(_database);
			_database.Execute("DELETE FROM User");

	        _clientTable.Insert(new IdentityClient(client));
	        TestClientId = _clientTable.GetClientByName(client).Id;
        }


        private static int CreateUser(string name, string email)
        {
			return _userTable.Insert(
                    new IdentityUser(TestClientId, name)
                    {
                        Email = email,
                    });
        }


        [TestMethod]
        public void It_creates_and_inserts_new_user()
        {
            var result = 0;
            IdentityUser newUser;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateUser(name1, email1);
                newUser = _userTable.GetUserByEmail(email1).FirstOrDefault();
                transaction.Dispose();
            }

            Assert.IsNotNull(newUser.Id);
            Assert.IsTrue(newUser.Id.StartsWith("U"));
            Assert.AreEqual(email1, newUser.Email);
			Assert.AreEqual(TestClientId, newUser.ClientId);
            Assert.AreEqual(1, result);
        }


        [TestMethod]
        public void It_updates_a_user()
        {
            IdentityUser user;
            IdentityUser updatedUser;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                user = _userTable.GetUserByEmail(email1).FirstOrDefault();
                user.Email = string.Empty;
                _userTable.Update(user);
                updatedUser = _userTable.GetUserByEmail(string.Empty).FirstOrDefault();
            }

            Assert.AreEqual(user.Id, updatedUser.Id);
            Assert.AreEqual(user.UserName, updatedUser.UserName);
            Assert.AreEqual(user.Email, updatedUser.Email);
        }


        [TestMethod]
        public void It_deletes_a_user()
        {
            int userCount;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                var user = _userTable.GetUserByEmail(email1).FirstOrDefault();
                _userTable.Delete(user);
                userCount = _userTable.GetUsers().Count();
            }

            Assert.AreEqual(0, userCount);
        }


        [TestMethod]
        public void It_gets_userlist()
        {
            IQueryable<IdentityUser> userList;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                CreateUser(name2, email2);
                userList = _userTable.GetUsers().AsQueryable();

                transaction.Dispose();
            }

            Assert.AreEqual(2, userList.Count());
            Assert.IsTrue(userList.Select(u => u.Email).Contains(email1));
            Assert.IsTrue(userList.Select(u => u.Email).Contains(email2));
        }


        [TestMethod]
        public void It_gets_username()
        {
            string username;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                var newUser = _userTable.GetUserByEmail(email1).FirstOrDefault();
                username = _userTable.GetUserName(newUser.Id);

                transaction.Dispose();
            }

            Assert.AreEqual(name1, username);
        }


        [TestMethod]
        public void It_gets_userid_by_name()
        {
            string userid;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                userid = _userTable.GetUserId(name1);

                transaction.Dispose();
            }

            Assert.IsTrue(userid.StartsWith("U"));
        }


        [TestMethod]
        public void It_gets_user_by_id()
        {
            IdentityUser user;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                var userId = _userTable.GetUserId(name1);
                user = _userTable.GetUserById(userId);

                transaction.Dispose();
            }

            Assert.AreEqual(name1, user.UserName);
            Assert.AreEqual(email1, user.Email);
        }

        [TestMethod]
        public void It_gets_user_by_name()
        {
            IdentityUser user;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                user = _userTable.GetUserByName(name1).FirstOrDefault();

                transaction.Dispose();
            }

            Assert.AreEqual(name1, user.UserName);
            Assert.AreEqual(email1, user.Email);
        }


        [TestMethod]
        public void It_gets_user_by_enail()
        {
            IdentityUser user;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                user = _userTable.GetUserByEmail(email1).FirstOrDefault();

                transaction.Dispose();
            }

            Assert.AreEqual(name1, user.UserName);
            Assert.AreEqual(email1, user.Email);
        }


        [TestMethod]
        public void It_sets_and_gets_password_hash()
        {
            var newPasswordHash = string.Empty;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                var user = _userTable.GetUserByEmail(email1).FirstOrDefault();
                _userTable.SetPasswordHash(user.Id, passwordHash);
                newPasswordHash = _userTable.GetPasswordHash(user.Id);

                transaction.Dispose();
            }

            Assert.AreEqual(passwordHash, newPasswordHash);
        }


        [TestMethod]
        public void It_gets_the_security_stamp()
        {
            string mySecurityStamp;

            using (var transaction = _database.GetTransaction())
            {
                CreateUser(name1, email1);
                var user = _userTable.GetUserByEmail(email1).FirstOrDefault();
                user.SecurityStamp = securityStamp;
                _userTable.Update(user);
                mySecurityStamp = _userTable.GetSecurityStamp(user.Id);

                transaction.Dispose();
            }

            Assert.AreEqual(securityStamp, mySecurityStamp);
        }

    }

}


