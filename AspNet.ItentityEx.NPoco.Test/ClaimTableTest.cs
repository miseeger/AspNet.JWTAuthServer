using System.Collections.Generic;
using System.Linq;
using AspNet.IdentityEx.NPoco;
using AspNet.IdentityEx.NPoco.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.ItentityEx.NPoco.Test
{

    [TestClass]
    public class ClaimTableTest
    {

        private static Database _database;
        private static ClaimTable _claimTable;

        private const string claimType1 = "Account";
        private const string claimValueAccount1 = "R/W";
        private const string claimValueAccount2 = "RO";
        private const string claimType2 = "Contact";
        private const string claimValueContact1 = "R/W";
        private const string claimValueContact2 = "RO";

        private const string clientId = "TestClientId";
        private const string otherClientId = "OtherTestClientId";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnectionName);
            _claimTable = new ClaimTable(_database);
            _database.Execute("DELETE FROM Claim");

        }


        private static int CreateClaim(string claimType, string claimValue)
        {
            return _claimTable.Insert(new IdentityClaim(claimType, claimValue));
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_claims()
        {
            var result = 0;
            List<IdentityClaim> newClaims;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateClaim(claimType1, claimValueAccount1);
                result = CreateClaim(claimType1, claimValueAccount2);
                newClaims = _claimTable.GetClaims()
                                .Where(c => c.Type == claimType1)
                                .ToList();
                transaction.Dispose();
            }

            Assert.IsNotNull(newClaims);
            Assert.AreEqual(2, newClaims.Count());
            Assert.IsTrue(newClaims.FirstOrDefault().Id.StartsWith("CL"));
            Assert.IsNull(newClaims.FirstOrDefault().ClientId);
            Assert.AreEqual(claimType1, newClaims.FirstOrDefault().Type);
            Assert.AreEqual(claimValueAccount1, newClaims.FirstOrDefault().Value);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_claims()
        {
            var result = 0;
            List<IdentityClaim> newClaims;

            using (var transaction = _database.GetTransaction())
            {
                result = _claimTable.Insert(new IdentityClaim(clientId, claimType1, claimValueAccount1));
                result = _claimTable.Insert(new IdentityClaim(clientId, claimType1, claimValueAccount2));
                newClaims = _claimTable.GetClaims()
                                .Where(c => c.Type == claimType1)
                                .ToList();
                transaction.Dispose();
            }

            Assert.IsNotNull(newClaims);
            Assert.AreEqual(2, newClaims.Count());
            Assert.IsTrue(newClaims.FirstOrDefault().Id.StartsWith("CL"));
            Assert.AreEqual(clientId, newClaims.FirstOrDefault().ClientId);
            Assert.AreEqual(claimType1, newClaims.FirstOrDefault().Type);
            Assert.AreEqual(claimValueAccount1, newClaims.FirstOrDefault().Value);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_claim_with_existing_global_claim()
        {
            var result = 0;
            IdentityClaim existingClaim, resultingClaim;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateClaim(claimType1, claimValueAccount1);
                existingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                result = _claimTable.Insert(new IdentityClaim(clientId, claimType1, claimValueAccount1));
                resultingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingClaim.Id, resultingClaim.Id);
            Assert.IsTrue(resultingClaim.Id.StartsWith("CL"));
            Assert.AreEqual(claimType1, resultingClaim.Type);
            Assert.AreEqual(claimValueAccount1, resultingClaim.Value);
            Assert.IsNull(resultingClaim.ClientId);
            Assert.AreEqual(3, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_client_claim_with_existing_client_claim()
        {
            var result = 0;
            IdentityClaim existingClaim, resultingClaim;

            using (var transaction = _database.GetTransaction())
            {
                result = _claimTable.Insert(new IdentityClaim(otherClientId, claimType1, claimValueAccount1)); ;
                existingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                result = _claimTable.Insert(new IdentityClaim(clientId, claimType1, claimValueAccount1));
                resultingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingClaim.Id, resultingClaim.Id);
            Assert.IsTrue(resultingClaim.Id.StartsWith("CL"));
            Assert.AreEqual(claimType1, resultingClaim.Type);
            Assert.AreEqual(claimValueAccount1, resultingClaim.Value);
            Assert.IsNull(resultingClaim.ClientId);
            Assert.AreEqual(2, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_claim_with_existing_global_claim()
        {
            var result = 0;
            IdentityClaim existingClaim, resultingClaim;

            using (var transaction = _database.GetTransaction())
            {
                result = CreateClaim(claimType1, claimValueAccount1);
                existingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                result = _claimTable.Insert(new IdentityClaim(null, claimType1, claimValueAccount1));
                resultingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingClaim.Id, resultingClaim.Id);
            Assert.IsTrue(resultingClaim.Id.StartsWith("CL"));
            Assert.AreEqual(claimType1, resultingClaim.Type);
            Assert.AreEqual(claimValueAccount1, resultingClaim.Value);
            Assert.IsNull(resultingClaim.ClientId);
            Assert.AreEqual(3, result);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_global_claim_with_existing_client_claim()
        {
            var result = 0;
            IdentityClaim existingClaim, resultingClaim;

            using (var transaction = _database.GetTransaction())
            {
                result = _claimTable.Insert(new IdentityClaim(otherClientId, claimType1, claimValueAccount1)); ;
                existingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                result = _claimTable.Insert(new IdentityClaim(null, claimType1, claimValueAccount1));
                resultingClaim = _claimTable.GetClaim(claimType1, claimValueAccount1);
                transaction.Dispose();
            }

            Assert.AreEqual(existingClaim.Id, resultingClaim.Id);
            Assert.IsTrue(resultingClaim.Id.StartsWith("CL"));
            Assert.AreEqual(claimType1, resultingClaim.Type);
            Assert.AreEqual(claimValueAccount1, resultingClaim.Value);
            Assert.IsNull(resultingClaim.ClientId);
            Assert.AreEqual(2, result);
        }


        [TestMethod]
        public void It_updates_a_Claim()
        {
            IdentityClaim claim;
            IdentityClaim updatedClaim;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType2, claimValueContact1);
                claim = _claimTable.GetClaim(claimType2, claimValueContact1);
                claim.Value = claimValueContact2;
                _claimTable.Update(claim);
                updatedClaim = _claimTable.GetClaim(claimType2, claimValueContact2);
            }

            Assert.AreEqual(claim.Id, updatedClaim.Id);
            Assert.AreEqual(claim.Value, updatedClaim.Value);
        }


        [TestMethod]
        public void It_deletes_a_claim()
        {
            int claimCount;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType2, claimValueContact1);
                var claim = _claimTable.GetClaim(claimType2, claimValueContact1);
                _claimTable.Delete(claim.Id);
                claimCount = _claimTable.GetClaims().Count();
            }

            Assert.AreEqual(0, claimCount);
        }


        [TestMethod]
        public void It_gets_claimlist()
        {
            IQueryable<IdentityClaim> claimList;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType1, claimValueAccount1);
                CreateClaim(claimType1, claimValueAccount2);
                CreateClaim(claimType2, claimValueContact1);
                CreateClaim(claimType2, claimValueContact2);
                claimList = _claimTable.GetClaims().AsQueryable();

                transaction.Dispose();
            }

            Assert.AreEqual(4, claimList.Count());
            Assert.IsTrue(claimList.Select(c => c.Type).Contains(claimType1));
            Assert.IsTrue(claimList.Select(c => c.Type).Contains(claimType2));
        }


        [TestMethod]
        public void It_gets_claimid_by_claimtype_and_value()
        {
            string claimid;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType2, claimValueContact2);
                claimid = _claimTable.GetClaimId(claimType2, claimValueContact2);

                transaction.Dispose();
            }

            Assert.IsTrue(claimid.StartsWith("CL"));
        }


        [TestMethod]
        public void It_gets_claim_by_id()
        {
            IdentityClaim claim;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType2, claimValueContact1);
                var claimId = _claimTable.GetClaimId(claimType2, claimValueContact1);
                claim = _claimTable.GetClaimById(claimId);

                transaction.Dispose();
            }

            Assert.AreEqual(claimType2, claim.Type);
            Assert.AreEqual(claimValueContact1, claim.Value);
        }


        [TestMethod]
        public void It_gets_claim_by_claimtype_and_value()
        {
            IdentityClaim claim;

            using (var transaction = _database.GetTransaction())
            {
                CreateClaim(claimType2, claimValueContact1);
                claim = _claimTable.GetClaim(claimType2, claimValueContact1);

                transaction.Dispose();
            }

            Assert.AreEqual(claimType2, claim.Type);
            Assert.AreEqual(claimValueContact1, claim.Value);
        }

    }

}





