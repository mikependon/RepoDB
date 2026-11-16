using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;

namespace RepoDb.Schema.Vertica.UnitTests
{
    /// <summary>
    /// Verifies that registered readers use each operation's connection.
    /// </summary>
    [TestClass]
    public class SchemaReaderProviderTest
    {
        #region Private Methods

        /// <summary>
        /// Supplies the current schema alongside empty catalog results.
        /// </summary>
        /// <param name="database">The simulated database.</param>
        private static CatalogConnection CreateConnection(string database = "Catalog")
        {
            var schema = new DataTable();
            schema.Columns.Add("Schema",
                typeof(string));
            schema.Rows.Add("test");
            var connection = new CatalogConnection
            {
                ConnectionString = database
            };
            connection.Query = sql => sql == VerticaSchemaText.CurrentSchemaSql ? schema : connection.Data;
            return connection;
        }

        /// <summary>
        /// Rejects a missing connection for GetTableSchema.
        /// </summary>
        [TestMethod]
        public void GetTableSchemaRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetTableSchema(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for TableExists.
        /// </summary>
        [TestMethod]
        public void TableExistsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().TableExists(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemaName.
        /// </summary>
        [TestMethod]
        public void GetSchemaNameRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetSchemaName(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetColumns.
        /// </summary>
        [TestMethod]
        public void GetColumnsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetColumns(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetPrimaryKey.
        /// </summary>
        [TestMethod]
        public void GetPrimaryKeyRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetPrimaryKey(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetIndexes.
        /// </summary>
        [TestMethod]
        public void GetIndexesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetIndexes(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetForeignKeys.
        /// </summary>
        [TestMethod]
        public void GetForeignKeysRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetForeignKeys(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetUniqueConstraints.
        /// </summary>
        [TestMethod]
        public void GetUniqueConstraintsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetUniqueConstraints(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetCheckConstraints.
        /// </summary>
        [TestMethod]
        public void GetCheckConstraintsRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetCheckConstraints(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemas.
        /// </summary>
        [TestMethod]
        public void GetSchemasRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetSchemas(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetTables.
        /// </summary>
        [TestMethod]
        public void GetTablesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetDependencyOrder.
        /// </summary>
        [TestMethod]
        public void GetDependencyOrderRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetDependencyOrder(null,
                    new[] { "Items" }));
        }

        /// <summary>
        /// Rejects a missing connection for GetRelatedTables.
        /// </summary>
        [TestMethod]
        public void GetRelatedTablesRejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetRelatedTables(null,
                    new[] { "Items" },
                    RepoDb.Schema.Enumerations.RelatedTableBehavior.TableOnly));
        }

        /// <summary>
        /// Rejects a missing connection for GetTableSchemaAsync.
        /// </summary>
        [TestMethod]
        public async Task GetTableSchemaAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetTableSchemaAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for TableExistsAsync.
        /// </summary>
        [TestMethod]
        public async Task TableExistsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().TableExistsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemaNameAsync.
        /// </summary>
        [TestMethod]
        public async Task GetSchemaNameAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetSchemaNameAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetColumnsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetColumnsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetColumnsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetPrimaryKeyAsync.
        /// </summary>
        [TestMethod]
        public async Task GetPrimaryKeyAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetPrimaryKeyAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetIndexesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetIndexesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetIndexesAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetForeignKeysAsync.
        /// </summary>
        [TestMethod]
        public async Task GetForeignKeysAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetForeignKeysAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetUniqueConstraintsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetUniqueConstraintsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetUniqueConstraintsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetCheckConstraintsAsync.
        /// </summary>
        [TestMethod]
        public async Task GetCheckConstraintsAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetCheckConstraintsAsync(null,
                    "Items"));
        }

        /// <summary>
        /// Rejects a missing connection for GetSchemasAsync.
        /// </summary>
        [TestMethod]
        public async Task GetSchemasAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetSchemasAsync(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetTablesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetTablesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetTablesAsync(null));
        }

        /// <summary>
        /// Rejects a missing connection for GetDependencyOrderAsync.
        /// </summary>
        [TestMethod]
        public async Task GetDependencyOrderAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetDependencyOrderAsync(null,
                    new[] { "Items" }));
        }

        /// <summary>
        /// Rejects a missing connection for GetRelatedTablesAsync.
        /// </summary>
        [TestMethod]
        public async Task GetRelatedTablesAsyncRejectsNullConnection()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetRelatedTablesAsync(null,
                    new[] { "Items" },
                    RepoDb.Schema.Enumerations.RelatedTableBehavior.TableOnly));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Uses the database of the current operation.
        /// </summary>
        [TestMethod]
        public void ReadsTheCurrentDatabase()
        {
            using var first = CreateConnection("First");
            using var second = CreateConnection("Second");
            var reader = new VerticaSchemaReader();
            reader.GetTableSchema(first,
                "test.Items");
            var actual = reader.GetTableSchema(second,
                "test.Items");
            Assert.AreEqual("Second",
                actual.Table.Database);
        }

        /// <summary>
        /// Uses the database across asynchronous operations.
        /// </summary>
        [TestMethod]
        public async Task ReadsTheCurrentDatabaseAsync()
        {
            using var first = CreateConnection("First");
            using var second = CreateConnection("Second");
            var reader = new VerticaSchemaReader();
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
            using var connection = CreateConnection();
            DbSettingMapper.Add<CatalogConnection>(new CatalogDbSetting("FAKE"),
                true);
            var actual = new VerticaSchemaReader().GetTableSchema(connection,
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
            using var connection = CreateConnection();
            var actual = new VerticaSchemaReader().GetTableSchema(connection,
                "test.Items");
            Assert.IsNull(actual.Table.Provider);
        }

        /// <summary>
        /// Forwards the operation transaction to catalog commands.
        /// </summary>
        [TestMethod]
        public void ForwardsTheTransaction()
        {
            using var connection = CreateConnection();
            using var transaction = connection.BeginTransaction();
            new VerticaSchemaReader().GetTableSchema(connection,
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
            using var connection = CreateConnection();
            using var transaction = connection.BeginTransaction();
            await new VerticaSchemaReader().GetTableSchemaAsync(connection,
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
            using var connection = CreateConnection();
            using var transaction = connection.BeginTransaction();
            var reader = new VerticaSchemaReader();
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
            using var first = CreateConnection("First");
            using var second = CreateConnection("Second");
            var reader = new VerticaSchemaReader();
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
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetTableSchema(null,
                    "test.Items"));
        }

        /// <summary>
        /// Rejects a missing asynchronous operation connection.
        /// </summary>
        [TestMethod]
        public async Task RejectsANullConnectionAsync()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new VerticaSchemaReader().GetTableSchemaAsync(null,
                    "test.Items"));
        }

        /// <summary>
        /// Observes cancellation before reading the catalog.
        /// </summary>
        [TestMethod]
        public async Task ObservesCancellationAsync()
        {
            using var connection = CreateConnection();
            await Assert.ThrowsAsync<OperationCanceledException>(() => new VerticaSchemaReader().GetTableSchemaAsync(connection,
                    "test.Items",
                    cancellationToken: new CancellationToken(true)));
        }
        #endregion
    }
}
