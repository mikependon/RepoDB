using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    [TestClass]
    public class InsertTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestVerticaConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert<NonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync<NonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionInsertViaTableNameAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
