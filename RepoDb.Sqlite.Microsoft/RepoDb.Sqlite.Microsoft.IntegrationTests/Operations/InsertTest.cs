using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act
                var result = connection.Insert<MdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert<MdsNonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<MdsCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync<MdsNonIdentityCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqliteConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqliteConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTables(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsNonIdentityCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.InsertAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(table.Id.ToString(), result?.ToString(), true);

                // Act
                var queryResult = connection.Query<MdsNonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
