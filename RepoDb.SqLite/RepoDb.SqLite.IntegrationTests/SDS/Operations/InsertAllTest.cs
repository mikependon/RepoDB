using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class InsertAllTest
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
        public void TestSqLiteConnectionInsertAllForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #endregion
    }
}
