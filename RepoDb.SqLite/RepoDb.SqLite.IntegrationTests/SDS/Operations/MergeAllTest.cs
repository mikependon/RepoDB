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
        public void TestSqLiteConnectionMergeAllForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<SdsNonIdentityCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertPropertiesEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<SdsNonIdentityCompleteTable>(tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

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
        public void TestSqLiteConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertMembersEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    entities);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    entities,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    entities);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    entities,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertMembersEquality(tables.Last(), queryResult.Last());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    entities).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    entities,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Setup
                tables.ForEach(table => Helper.UpdateSdsNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    entities).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsNonIdentityCompleteTables(10, connection).AsList();
                var entities = tables.Select(table => new
                {
                    Id = table.Id,
                    ColumnInt = int.MaxValue
                }).AsList();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<SdsNonIdentityCompleteTable>(),
                    entities,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<SdsNonIdentityCompleteTable>();

                // Assert
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        #endregion

        #endregion
    }
}
