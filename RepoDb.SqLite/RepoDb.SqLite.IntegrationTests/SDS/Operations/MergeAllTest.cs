using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class MergeAllTest
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
        public void TestSQLiteConnectionMergeAllForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsCompleteTable>(tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsExpandoObjects(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(string))
                };
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsNonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion
    }
}
