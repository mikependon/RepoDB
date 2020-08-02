using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionInsertForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.Insert<SdsCompleteTable>(table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert<SdsNonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<SdsCompleteTable>(table).Result;

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<SdsNonIdentityCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

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
        public void TestSqLiteConnectionInsertViaTableNameForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
