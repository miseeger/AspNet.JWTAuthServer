using System.Collections.Generic;
using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.UserRoles;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class UserRoleTableTest
    {

        private static Database _database;
        private static RoleTable _roleTable;
        private static UserTable<IdentityUser> _userTable;
		private static ClientTable _clientTable;
		private static UserRoleTable _userRoleTable;

		private const string client = "TestClient";

		private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";


        private const string role1 = "Development";
        private const string role2 = "Controlling";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
			_database = new Database(IdentityConstants.ConnectionName);
            _roleTable = new RoleTable(_database);
            _userTable = new UserTable<IdentityUser>(_database);
			_clientTable = new ClientTable(_database);
			_userRoleTable = new UserRoleTable(_database);

			_clientTable.Insert(new IdentityClient(client));
			var TestClientId = _clientTable.GetClientByName(client).Id;

            _userTable.Insert(new IdentityUser(TestClientId, name1) { Email = email1 });
            _userTable.Insert(new IdentityUser(TestClientId, name2) { Email = email2 });

            _roleTable.Insert(new IdentityRole(role1));
            _roleTable.Insert(new IdentityRole(role2));
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database.Execute("DELETE FROM User");
            _database.Execute("DELETE FROM Role");
        }


        [TestMethod]
        public void It_creates_and_inserts_new_userrole()
        {
            IdentityRole userRole;
            int result;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var role = _roleTable.GetRoleByName(role1);
                result = _userRoleTable.Insert(user.Id, role.Id);
                userRole = _userRoleTable.GetRoles(user.Id).FirstOrDefault();
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.AreEqual(role1, userRole.Name);
        }


        [TestMethod]
        public void It_gets_all_roles_of_a_user()
        {
            List<IdentityRole> userRoles;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var identityRole1 = _roleTable.GetRoleByName(role1);
                var identityRole2 = _roleTable.GetRoleByName(role2);
                _userRoleTable.Insert(user.Id, identityRole1.Id);
                _userRoleTable.Insert(user.Id, identityRole2.Id);
                userRoles = _userRoleTable.GetRoles(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(2, userRoles.Count());
            Assert.IsTrue(userRoles.Select(r => r.Name).Contains(role1));
            Assert.IsTrue(userRoles.Select(r => r.Name).Contains(role2));
        }


        [TestMethod]
        public void It_deletes_all_roles_of_a_user()
        {
            List<IdentityRole> userRolesBeforeDelete;
            List<IdentityRole> userRolesAfterDelete;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var identityRole1 = _roleTable.GetRoleByName(role1);
                var identityRole2 = _roleTable.GetRoleByName(role2);
                _userRoleTable.Insert(user.Id, identityRole1.Id);
                _userRoleTable.Insert(user.Id, identityRole2.Id);
                userRolesBeforeDelete = _userRoleTable.GetRoles(user.Id);
                _userRoleTable.Delete(user.Id);
                userRolesAfterDelete = _userRoleTable.GetRoles(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(2, userRolesBeforeDelete.Count());
            Assert.AreEqual(0, userRolesAfterDelete.Count());
        }


        [TestMethod]
        public void It_deletes_one_role_of_a_user()
        {
            List<IdentityRole> userRolesBeforeDelete;
            List<IdentityRole> userRolesAfterDelete;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var identityRole1 = _roleTable.GetRoleByName(role1);
                var identityRole2 = _roleTable.GetRoleByName(role2);
                _userRoleTable.Insert(user.Id, identityRole1.Id);
                _userRoleTable.Insert(user.Id, identityRole2.Id);
                userRolesBeforeDelete = _userRoleTable.GetRoles(user.Id);
                _userRoleTable.Delete(identityRole1.Id, user.Id);
                userRolesAfterDelete = _userRoleTable.GetRoles(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(2, userRolesBeforeDelete.Count());
            Assert.AreEqual(1, userRolesAfterDelete.Count());
            Assert.IsTrue(userRolesAfterDelete.Select(r => r.Name).Contains(role2));

        }


        //[TestMethod]
        //public void It_deletes_one_role_of_a_user()
        //{
        //    List<string> userRoles;
        //    int result;

        //    using (var transaction = _database.GetTransaction())
        //    {
        //        var user = _userTable.GetUserByName(name1).FirstOrDefault();
        //        CreateUserRole(user, role1);
        //        CreateUserRole(user, role2);
        //        var roleId = _roleTable.GetRoleId(role2);
        //        result = _userRoleTable.Delete(roleId, user.Id);
        //        userRoles = _userRoleTable.FindByUserId(user.Id);
        //        transaction.Dispose();
        //    }

        //    Assert.AreEqual(1, result);
        //    Assert.IsTrue(userRoles.Count == 1);
        //    Assert.IsTrue(userRoles.Contains(role1));
        //}

    }

}


