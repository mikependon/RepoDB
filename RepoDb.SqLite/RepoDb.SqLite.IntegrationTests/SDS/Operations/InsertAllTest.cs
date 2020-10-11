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
        public void TestSQLiteConnectionInsertAllForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
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
        public void TestSQLiteConnectionInsertAllForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
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
        public void TestSQLiteConnectionInsertAllAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameAsyncForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameAsyncAsExpandoObjectForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllViaTableNameAsyncForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
        public void TestSQLiteConnectionInsertAllAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

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
