using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class MergeTest
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
        public void TestSQLiteConnectionMergeForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge<SdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge<SdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateSdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge<SdsCompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Act
                var result = connection.MergeAsync<SdsCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync<SdsCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateSdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.MergeAsync<SdsCompleteTable>(table,
                    qualifiers: qualifiers).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSQLiteConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateSdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue((long)result > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var table = Helper.CreateSdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.IsTrue((long)result > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
