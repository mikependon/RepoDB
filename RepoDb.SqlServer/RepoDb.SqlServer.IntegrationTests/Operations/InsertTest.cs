using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
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
        public void TestSqlConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public void TestSqlConnectionInsertViaTableNameAsExpandoObjectForNonIdentityTable()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncViaTableNameForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentityTable()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
