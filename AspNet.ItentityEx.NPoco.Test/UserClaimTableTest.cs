using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.UserClaims;
using AspNet.IdentityEx.NPoco.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class UserClaimTableTest
    {

        private static Database _database;
        private static UserTable<IdentityUser> _userTable;
        private static UserClaimTable _userClaimTable;
        private static ClientTable _clientTable;
        private static ClaimTable _claimTable;

        private const string client = "TestClient";

        private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";

        private const string cType1 = ClaimTypes.Country;
        private const string cValue1 = "NoMansLand";

        private const string cType2 = ClaimTypes.Gender;
        private const string cValue2 = "male";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnectionName);
            _userTable = new UserTable<IdentityUser>(_database);
            _claimTable = new ClaimTable(_database);
            _userClaimTable = new UserClaimTable(_database);

            _clientTable = new ClientTable(_database);

            _clientTable.Insert(new IdentityClient(client));
            var TestClientId = _clientTable.GetClientByName(client).Id;

            _claimTable.Insert(new IdentityClaim(TestClientId, cType1, cValue1));
            _claimTable.Insert(new IdentityClaim(TestClientId, cType2, cValue2));


            _userTable.Insert(new IdentityUser(TestClientId, name1) { Email = email1 });
            _userTable.Insert(new IdentityUser(TestClientId, name2) { Email = email2 });
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database.Execute("DELETE FROM User");
            _database.Execute("DELETE FROM UserClaim");
        }


        private static int CreateUserClaim(IdentityUser user, string type, string value)
        {
            return _userClaimTable.Insert(new IdentityClaim(type, value), user.Id);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_userclaim()
        {
            ClaimsIdentity claims;
            int result;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var claim = _claimTable.GetClaim(cType1, cValue1);
                result = _userClaimTable.Insert(claim, user.Id);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.IsTrue(claims.HasClaim(cType1, cValue1));
        }


        [TestMethod]
        public void It_finds_userclaims_by_userid()
        {
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                var claim1 = _claimTable.GetClaim(cType1, cValue1);
                var claim2 = _claimTable.GetClaim(cType2, cValue2);
                _userClaimTable.Insert(claim1, user.Id);
                _userClaimTable.Insert(claim2, user.Id);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.IsTrue(claims.HasClaim(cType1, cValue1));
            Assert.IsTrue(claims.HasClaim(cType2, cValue2));
        }


        [TestMethod]
        public void It_deletes_all_userclaims_of_user()
        {
            int result;
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserClaim(user, cType1, cValue1);
                result = _userClaimTable.Delete(user.Id);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.IsTrue(!claims.Claims.Any());
        }


        //[TestMethod]
        //public void It_deletes_one_userClaim_of_a_user()
        //{
        //	int result;
        //	ClaimsIdentity claims;

        //	using (var transaction = _database.GetTransaction())
        //	{
        //		var user = _userTable.GetUserByName(name1).FirstOrDefault();
        //		CreateUserClaim(user, cType1, cValue1);
        //		CreateUserClaim(user, cType2, cValue2);
        //		result = _userClaimTable.Delete(user, new IdentityClaim(cType2, cValue2));
        //		claims = _userClaimTable.FindByUserId(user.Id);
        //		transaction.Dispose();
        //	}


        //	Assert.AreEqual(1, result);
        //	Assert.IsTrue(claims.Claims.Count() == 1);
        //	Assert.IsTrue(claims.HasClaim(cType1, cValue1));
        //}

    }

}


