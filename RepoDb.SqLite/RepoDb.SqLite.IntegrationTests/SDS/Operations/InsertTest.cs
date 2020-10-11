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
        public void TestSQLiteConnectionInsertForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.Insert<SdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
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
        public void TestSQLiteConnectionInsertForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert<SdsNonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

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
        public void TestSQLiteConnectionInsertAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<SdsCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
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
        public void TestSQLiteConnectionInsertAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<SdsNonIdentityCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

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
        public void TestSQLiteConnectionInsertViaTableNameForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

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
        public void TestSQLiteConnectionInsertViaTableNameAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<SdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

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
