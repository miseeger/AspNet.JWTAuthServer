using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Roles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class RoleTableTest
    {

        private static Database _database;
        private static RoleTable _roleTable;

        private const string role1 = "Development";
        private const string role2 = "Controlling";

        private const string clientId = "TestClientId";
        private const string otherClientId = "OtherTestClientId";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnectionName);
            _roleTable = new RoleTable(_database);
            _database.Execute("DELETE FROM Role");

        }


        private static int CreateRole(string role)
        {
            return _roleTable.Insert(
                    new IdentityRole(role));
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_role()
        {
            var result = 0;
            IdentityRole newRole;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateRole(role1);
                newRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.IsNotNull(newRole.Id);
            Assert.IsTrue(newRole.Id.StartsWith("R"));
            Assert.IsNull(newRole.ClientId);
            Assert.AreEqual(role1, newRole.Name);
            Assert.AreEqual(1, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_role()
        {
            var result = 0;
            IdentityRole newRole;

            using (var transaction = _database.GetTransaction())
            {
                result = _roleTable.Insert(new IdentityRole(clientId, role1));
                newRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.IsNotNull(newRole.Id);
            Assert.IsTrue(newRole.Id.StartsWith("R"));
            Assert.AreEqual(clientId, newRole.ClientId);
            Assert.AreEqual(role1, newRole.Name);
            Assert.AreEqual(1, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_role_with_existing_global_role()
        {
            var result = 0;
            IdentityRole existingRole, resultingRole;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateRole(role1);
                existingRole = _roleTable.GetRoleByName(role1);
                result = _roleTable.Insert(new IdentityRole(clientId, role1));
                resultingRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingRole.Id, resultingRole.Id);
            Assert.IsTrue(resultingRole.Id.StartsWith("R"));
            Assert.AreEqual(role1, resultingRole.Name);
            Assert.IsNull(resultingRole.ClientId);
            Assert.AreEqual(3, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_role_with_existing_client_role()
        {
            var result = 0;
            IdentityRole existingRole, resultingRole;

            using (var transaction = _database.GetTransaction())
            {
                result = _roleTable.Insert(new IdentityRole(otherClientId, role1));
                existingRole = _roleTable.GetRoleByName(role1);
                result = _roleTable.Insert(new IdentityRole(clientId, role1));
                resultingRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingRole.Id, resultingRole.Id);
            Assert.IsTrue(resultingRole.Id.StartsWith("R"));
            Assert.AreEqual(role1, resultingRole.Name);
            Assert.IsNull(resultingRole.ClientId);
            Assert.AreEqual(2, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_role_with_existing_global_role()
        {
            var result = 0;
            IdentityRole existingRole, resultingRole;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateRole(role1);
                existingRole = _roleTable.GetRoleByName(role1);
                result = _roleTable.Insert(new IdentityRole(null, role1));
                resultingRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingRole.Id, resultingRole.Id);
            Assert.IsTrue(resultingRole.Id.StartsWith("R"));
            Assert.AreEqual(role1, resultingRole.Name);
            Assert.IsNull(resultingRole.ClientId);
            Assert.AreEqual(3, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_role_with_existing_client_role()
        {
            var result = 0;
            IdentityRole existingRole, resultingRole;

            using (var transaction = _database.GetTransaction())
            {
                result = _roleTable.Insert(new IdentityRole(otherClientId, role1));
                existingRole = _roleTable.GetRoleByName(role1);
                result = _roleTable.Insert(new IdentityRole(null, role1));
                resultingRole = _roleTable.GetRoleByName(role1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingRole.Id, resultingRole.Id);
            Assert.IsTrue(resultingRole.Id.StartsWith("R"));
            Assert.AreEqual(role1, resultingRole.Name);
            Assert.IsNull(resultingRole.ClientId);
            Assert.AreEqual(2, result);
        }


        [TestMethod]
        public void It_updates_a_role()
        {
            IdentityRole role;
            IdentityRole updatedRole;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                role = _roleTable.GetRoleByName(role1);
                role.Name = string.Empty;
                _roleTable.Update(role);
                updatedRole = _roleTable.GetRoleByName(string.Empty);
            }

            Assert.AreEqual(role.Id, updatedRole.Id);
            Assert.AreEqual(role.Name, updatedRole.Name);
        }


        [TestMethod]
        public void It_deletes_a_role()
        {
            int roleCount;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                var role = _roleTable.GetRoleByName(role1);
                _roleTable.Delete(role.Id);
                roleCount = _roleTable.GetRoles().Count();
            }

            Assert.AreEqual(0, roleCount);
        }


        [TestMethod]
        public void It_gets_rolelist()
        {
            IQueryable<IdentityRole> roleList;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                CreateRole(role2);
                roleList = _roleTable.GetRoles().AsQueryable();

                transaction.Dispose();
            }

            Assert.AreEqual(2, roleList.Count());
            Assert.IsTrue(roleList.Select(r => r.Name).Contains(role1));
            Assert.IsTrue(roleList.Select(r => r.Name).Contains(role2));
        }


        [TestMethod]
        public void It_gets_rolename()
        {
            string rolename;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                var newRole = _roleTable.GetRoleByName(role1);
                rolename = _roleTable.GetRoleName(newRole.Id);

                transaction.Dispose();
            }

            Assert.AreEqual(role1, rolename);
        }


        [TestMethod]
        public void It_gets_roleid_by_name()
        {
            string roleid;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                roleid = _roleTable.GetRoleId(role1);

                transaction.Dispose();
            }

            Assert.IsTrue(roleid.StartsWith("R"));
        }


        [TestMethod]
        public void It_gets_role_by_id()
        {
            IdentityRole role;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                var roleId = _roleTable.GetRoleId(role1);
                role = _roleTable.GetRoleById(roleId);

                transaction.Dispose();
            }

            Assert.AreEqual(role1, role.Name);
        }


        [TestMethod]
        public void It_gets_role_by_name()
        {
            IdentityRole role;

            using (var transaction = _database.GetTransaction())
            {
                CreateRole(role1);
                role = _roleTable.GetRoleByName(role1);

                transaction.Dispose();
            }

            Assert.AreEqual(role1, role.Name);
        }

    }

}



