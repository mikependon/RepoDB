using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10);

                // Act
                var result = connection.InsertAll<MdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll<MdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<MdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync<MdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsExpandoObjectForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllViaTableNameAsyncAsExpandoObjectForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(table)), table);
                });
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<MdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<MdsNonIdentityCompleteTable>();

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
