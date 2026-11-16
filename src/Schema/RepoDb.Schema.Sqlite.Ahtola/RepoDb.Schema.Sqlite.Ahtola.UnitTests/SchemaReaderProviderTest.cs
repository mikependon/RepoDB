using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests
{
    /// <summary>
    /// Verifies that registered readers use each operation's connection.
    /// </summary>
    [TestClass]
    public class SchemaReaderProviderTest
    {
        #region Public Methods

        /// <summary>
        /// Uses the database of the current operation.
        /// </summary>
        [TestMethod]
        public void ReadsTheCurrentDatabase()
        {
            using var first = new CatalogConnection
            {
                ConnectionString = "First"
            };
            using var second = new CatalogConnection
            {
                ConnectionString = "Second"
            };
            var reader = new AhtolaSchemaReader();
            reader.GetTableSchema(first,
                "test.Items");
            var actual = reader.GetTableSchema(second,
                "test.Items");
            Assert.AreEqual("Second",
                actual.Table.Database);
        }

        /// <summary>
        /// Rejects a missing connection for GetTableSchema.
        /// </summary>
        [TestMethod]
        public void GetTableSchemaRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetTableSchema(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for TableExists.
        /// </summary>
        [TestMethod]
        public void TableExistsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().TableExists(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemaName.
        /// </summary>
        [TestMethod]
        public void GetSchemaNameRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetSchemaName(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetColumns.
        /// </summary>
        [TestMethod]
        public void GetColumnsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetColumns(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetPrimaryKey.
        /// </summary>
        [TestMethod]
        public void GetPrimaryKeyRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetPrimaryKey(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetIndexes.
        /// </summary>
        [TestMethod]
        public void GetIndexesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetIndexes(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetForeignKeys.
        /// </summary>
        [TestMethod]
        public void GetForeignKeysRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetForeignKeys(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetUniqueConstraints.
        /// </summary>
        [TestMethod]
        public void GetUniqueConstraintsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetUniqueConstraints(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetCheckConstraints.
        /// </summary>
        [TestMethod]
        public void GetCheckConstraintsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetCheckConstraints(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemas.
        /// </summary>
        [TestMethod]
        public void GetSchemasRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetSchemas(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetTables.
        /// </summary>
        [TestMethod]
        public void GetTablesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetDependencyOrder.
        /// </summary>
        [TestMethod]
        public void GetDependencyOrderRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetDependencyOrder(null,
                    new[] { "Items" }));
        }

        /// <summary>
        /// Rejects a missing connection for GetRelatedTables.
        /// </summary>
        [TestMethod]
        public void GetRelatedTablesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetRelatedTables(null,
                    new[] { "Items" },
                    RepoDb.Schema.Enumerations.RelatedTableBehavior.TableOnly));
        }

        /// <summary>
        /// Rejects a missing connection for GetTableSchemaAsync.
        /// </summary>
        [TestMethod]
        public async Task GetTableSchemaAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetTableSchemaAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for TableExistsAsync.
        /// </summary>
        [TestMethod]
        public async Task TableExistsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().TableExistsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemaNameAsync.
        /// </summary>
        [TestMethod]
        public async Task GetSchemaNameAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetSchemaNameAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetColumnsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetColumnsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetColumnsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetPrimaryKeyAsync.
        /// </summary>
        [TestMethod]
        public async Task GetPrimaryKeyAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetPrimaryKeyAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetIndexesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetIndexesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetIndexesAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetForeignKeysAsync.
        /// </summary>
        [TestMethod]
        public async Task GetForeignKeysAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetForeignKeysAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetUniqueConstraintsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetUniqueConstraintsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetUniqueConstraintsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetCheckConstraintsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetCheckConstraintsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetCheckConstraintsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemasAsync.
        /// </summary>
        [TestMethod]
        public async Task GetSchemasAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetSchemasAsync(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetTablesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetTablesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetTablesAsync(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetDependencyOrderAsync.
        /// </summary>
        [TestMethod]
        public async Task GetDependencyOrderAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetDependencyOrderAsync(null,
                    new[] { "Items" }));
        }

        /// <summary>
        /// Rejects a missing connection for GetRelatedTablesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetRelatedTablesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetRelatedTablesAsync(null,
                    new[] { "Items" },
                    RepoDb.Schema.Enumerations.RelatedTableBehavior.TableOnly));
        }

        /// <summary>
        /// Uses the database across asynchronous operations.
        /// </summary>
        [TestMethod]
        public async Task ReadsTheCurrentDatabaseAsync()
        {
            using var first = new CatalogConnection
            {
                ConnectionString = "First"
            };
            using var second = new CatalogConnection
            {
                ConnectionString = "Second"
            };
            var reader = new AhtolaSchemaReader();
            await reader.GetTableSchemaAsync(first,
                "test.Items");
            var actual = await reader.GetTableSchemaAsync(second,
                "test.Items");
            Assert.AreEqual("Second",
                actual.Table.Database);
        }

        /// <summary>
        /// Uses the provider of the current connection setting.
        /// </summary>
        [TestMethod]
        public void ReadsTheMappedProvider()
        {
            using var connection = new CatalogConnection();
            DbSettingMapper.Add<CatalogConnection>(new CatalogDbSetting("FAKE"),
                true);
            var actual = new AhtolaSchemaReader().GetTableSchema(connection,
                "test.Items");
            Assert.AreEqual("FAKE",
                actual.Table.Provider);
        }

        /// <summary>
        /// Preserves a missing provider value.
        /// </summary>
        [TestMethod]
        public void ReadsANullProvider()
        {
            using var connection = new CatalogConnection();
            var actual = new AhtolaSchemaReader().GetTableSchema(connection,
                "test.Items");
            Assert.IsNull(actual.Table.Provider);
        }

        /// <summary>
        /// Forwards the operation transaction to catalog commands.
        /// </summary>
        [TestMethod]
        public void ForwardsTheTransaction()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            new AhtolaSchemaReader().GetTableSchema(connection,
                "test.Items",
                transaction);
            Assert.AreSame(transaction,
                connection.LastTransaction);
        }

        /// <summary>
        /// Forwards the transaction across asynchronous catalog commands.
        /// </summary>
        [TestMethod]
        public async Task ForwardsTheTransactionAsync()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            await new AhtolaSchemaReader().GetTableSchemaAsync(connection,
                "test.Items",
                transaction);
            Assert.AreSame(transaction,
                connection.LastTransaction);
        }

        /// <summary>
        /// Does not retain a transaction for later operations.
        /// </summary>
        [TestMethod]
        public void DoesNotRetainTheTransaction()
        {
            using var connection = new CatalogConnection();
            using var transaction = connection.BeginTransaction();
            var reader = new AhtolaSchemaReader();
            reader.GetTableSchema(connection,
                "test.Items",
                transaction);
            reader.GetTableSchema(connection,
                "test.Items");
            Assert.IsNull(connection.LastTransaction);
        }

        /// <summary>
        /// Supports overlapping reads on independent connections.
        /// </summary>
        [TestMethod]
        public async Task ReadsIndependentConnectionsConcurrently()
        {
            using var first = new CatalogConnection
            {
                ConnectionString = "First"
            };
            using var second = new CatalogConnection
            {
                ConnectionString = "Second"
            };
            var reader = new AhtolaSchemaReader();
            var actual = await Task.WhenAll(Task.Run(() => reader.GetTableSchema(first,
                        "test.Items")),
                Task.Run(() => reader.GetTableSchema(second,
                        "test.Items")));
            CollectionAssert.AreEqual(new[] { "First", "Second" },
                new[] { actual[0].Table.Database, actual[1].Table.Database });
        }

        /// <summary>
        /// Rejects a missing operation connection.
        /// </summary>
        [TestMethod]
        public void RejectsANullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetTableSchema(null,
                    "test.Items"));
        }

        /// <summary>
        /// Rejects a missing asynchronous operation connection.
        /// </summary>
        [TestMethod]
        public async Task RejectsANullConnectionAsync()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new AhtolaSchemaReader().GetTableSchemaAsync(null,
                    "test.Items"));
        }

        /// <summary>
        /// Observes cancellation before reading the catalog.
        /// </summary>
        [TestMethod]
        public async Task ObservesCancellationAsync()
        {
            using var connection = new CatalogConnection();
            await Assert.ThrowsAsync<OperationCanceledException>(() => new AhtolaSchemaReader().GetTableSchemaAsync(connection,
                    "test.Items",
                    cancellationToken: new CancellationToken(true)));
        }
        #endregion
    }
}
