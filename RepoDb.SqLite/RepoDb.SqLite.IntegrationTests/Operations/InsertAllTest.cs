using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestInsertAllForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.InsertAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

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
        public void TestInsertAllAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<CompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

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
        public void TestInsertAllViaTableNameForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllViaTableNameForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

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
        public void TestInsertAllViaTableNameAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

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
