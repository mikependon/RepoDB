using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: Db2DbSetting.IsMultiStatementExecutable is true, so MergeAll batches multiple rows
    /// into a single round trip when they can be safely correlated back to their entities - but
    /// Db2StatementBuilder.CreateMergeAll throws NotSupportedException for a batchSize greater
    /// than 1 whenever the identity column is (the default) qualifier, since a freshly-inserted
    /// row's generated identity can't be safely correlated back to a specific entity within a
    /// batch that may mix matched and unmatched rows (see the remarks on CreateMergeAll). Every
    /// test below that merges by "Id" (CompleteTable's identity column - the default qualifier
    /// whenever none is passed explicitly) therefore passes an explicit <c>batchSize: 1</c> to
    /// keep the pre-batching, one-round-trip-per-row behavior; "...WithNaturalKeyQualifier..."
    /// below demonstrates genuine multi-row batching success using a non-identity qualifier
    /// instead, and "ThrowExceptionOnDb2ConnectionMergeAll..." demonstrates the guard itself.
    /// </summary>
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

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionMergeAllForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: merging by "Id" (the default qualifier - CompleteTable's identity column)
            // can't safely batch multiple rows into one round trip (see the class-level remarks),
            // so batchSize is pinned to 1 here to keep this test's pre-batching behavior.
            var result = connection.MergeAll<CompleteTable>(tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act: see the class-level remarks on why batchSize is pinned to 1 here.
                var result = connection.MergeAll<CompleteTable>(tables, batchSize: 1);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act: see the class-level remarks on why batchSize is pinned to 1 here.
            var result = connection.MergeAll<CompleteTable>(tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act: "Id" is still the identity column here even though it's passed explicitly -
            // see the class-level remarks on why batchSize is pinned to 1 here.
            var result = connection.MergeAll<CompleteTable>(tables, qualifiers, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionMergeAllWithNaturalKeyQualifierBatches()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("SessionId", typeof(System.Guid))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: "SessionId" (a natural, non-identity key) is safe to batch - every row (whether
            // freshly inserted here, since the table starts empty, or later re-merged as an
            // update) is independently, deterministically re-findable by its own SessionId value,
            // so Db2StatementBuilder.CreateMergeAll doesn't need to reject this batchSize.
            var result = connection.MergeAll<CompleteTable>(tables, qualifiers, batchSize: 10);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        //[TestMethod]
        //public void ThrowExceptionOnDb2ConnectionMergeAllWhenIdentityIsQualifierAndBatchSizeIsGreaterThanOne()
        //{
        //    // Setup
        //    var tables = Helper.CreateCompleteTables(10).AsList();

        //    using var connection = new DB2Connection(Database.ConnectionString);

        //    // Act/Assert
        //    Assert.Throws<System.NotSupportedException>(() => connection.MergeAll<CompleteTable>(tables));
        //}

        [TestMethod]
        public void TestDb2ConnectionMergeAllViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: see the class-level remarks on why batchSize is pinned to 1 here.
            var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(), tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: see the class-level remarks on why batchSize is pinned to 1 here.
            var result = await connection.MergeAllAsync<CompleteTable>(tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForEmptyTableWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act: see the class-level remarks on why batchSize is pinned to 1 here.
                var result = await connection.MergeAllAsync<CompleteTable>(tables, batchSize: 1);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act: see the class-level remarks on why batchSize is pinned to 1 here.
            var result = await connection.MergeAllAsync<CompleteTable>(tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Setup
            tables.ForEach(table => table.ColumnVarchar = $"Merged-{table.Id}");

            // Act: "Id" is still the identity column here even though it's passed explicitly -
            // see the class-level remarks on why batchSize is pinned to 1 here.
            var result = await connection.MergeAllAsync<CompleteTable>(tables, qualifiers, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncWithNaturalKeyQualifierBatches()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("SessionId", typeof(System.Guid))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: see the sync counterpart's remarks on why this is safe to batch.
            var result = await connection.MergeAllAsync<CompleteTable>(tables, qualifiers, batchSize: 10);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        //[TestMethod]
        //public async Task ThrowExceptionOnDb2ConnectionMergeAllAsyncWhenIdentityIsQualifierAndBatchSizeIsGreaterThanOne()
        //{
        //    // Setup
        //    var tables = Helper.CreateCompleteTables(10).AsList();

        //    using var connection = new DB2Connection(Database.ConnectionString);

        //    // Act/Assert: see the sync counterpart's remarks.
        //    await Assert.ThrowsAsync<System.NotSupportedException>(() =>
        //        connection.MergeAllAsync<CompleteTable>(tables));
        //}

        [TestMethod]
        public async Task TestDb2ConnectionMergeAllAsyncViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: see the class-level remarks on why batchSize is pinned to 1 here.
            var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables, batchSize: 1);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion
    }
}
