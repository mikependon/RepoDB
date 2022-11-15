using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
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
        public void TestSqLiteConnectionMergeForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge<MdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge<MdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateMdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge<MdsCompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Act
                var result = await connection.MergeAsync<MdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<MdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateMdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = await connection.MergeAsync<MdsCompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateMdsCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(1, connection).First();
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.IsTrue((long)result > 0);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMergeViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(1, connection).First();
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var table = Helper.CreateMdsCompleteTablesAsDynamics(1).First();

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(1, connection).First();
                Helper.UpdateMdsCompleteTableProperties(table);
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(1, connection).First();
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionMergeAsyncViaTableNameAsExpandoObjectForIdentityForNonEmptyTable()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                Database.CreateMdsCompleteTables(1, connection).First();
                var table = Helper.CreateMdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
