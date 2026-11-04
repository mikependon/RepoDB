using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests.Readers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class TursoSchemaReaderCoverageTest
    {
        #region Private Variables

        private static readonly KeyValuePair<string, object>[] _columnValues =
            new[]
            {
                new KeyValuePair<string, object>("ColumnName",
                    "Id"),
                new KeyValuePair<string, object>("DeclaredType",
                    DBNull.Value),
                new KeyValuePair<string, object>("DefaultDefinition",
                    DBNull.Value),
                new KeyValuePair<string, object>("Definition",
                    DBNull.Value),
                new KeyValuePair<string, object>("DeleteAction",
                    DBNull.Value),
                new KeyValuePair<string, object>("Hidden",
                    0),
                new KeyValuePair<string, object>("Id",
                    0),
                new KeyValuePair<string, object>("IndexName",
                    DBNull.Value),
                new KeyValuePair<string, object>("IsDescending",
                    0),
                new KeyValuePair<string, object>("IsKey",
                    0),
                new KeyValuePair<string, object>("IsNotNull",
                    0),
                new KeyValuePair<string, object>("IsPartial",
                    0),
                new KeyValuePair<string, object>("IsUnique",
                    0),
                new KeyValuePair<string, object>("Name",
                    "Id"),
                new KeyValuePair<string, object>("Origin",
                    DBNull.Value),
                new KeyValuePair<string, object>("ParentTable",
                    DBNull.Value),
                new KeyValuePair<string, object>("PrimaryKeyPosition",
                    0),
                new KeyValuePair<string, object>("ReferencedColumn",
                    DBNull.Value),
                new KeyValuePair<string, object>("ReferencedTable",
                    DBNull.Value),
                new KeyValuePair<string, object>("SchemaName",
                    DBNull.Value),
                new KeyValuePair<string, object>("TableName",
                    DBNull.Value),
                new KeyValuePair<string, object>("UpdateAction",
                    DBNull.Value)
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads an empty catalog without inventing objects.
        /// </summary>
        [TestMethod]
        public void GetColumnsReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetColumns("Items").ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetColumnsAsyncReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetColumnsAsync("Items"))
                .ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog without inventing objects.
        /// </summary>
        [TestMethod]
        public void GetIndexesReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetIndexes("Items").ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetIndexesAsyncReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetIndexesAsync("Items"))
                .ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog without inventing objects.
        /// </summary>
        [TestMethod]
        public void GetForeignKeysReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetForeignKeys("Items").ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetForeignKeysAsyncReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetForeignKeysAsync("Items"))
                .ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog without inventing objects.
        /// </summary>
        [TestMethod]
        public void GetUniqueConstraintsReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetUniqueConstraints("Items").ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetUniqueConstraintsAsyncReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetUniqueConstraintsAsync("Items"))
                .ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog without inventing objects.
        /// </summary>
        [TestMethod]
        public void GetCheckConstraintsReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetCheckConstraints("Items").ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Reads an empty catalog asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetCheckConstraintsAsyncReturnsEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetCheckConstraintsAsync("Items"))
                .ToArray();

            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Preserves an absent primary key.
        /// </summary>
        [TestMethod]
        public void GetPrimaryKeyReturnsNullForEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetPrimaryKey("Items");

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Preserves an absent primary key asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetPrimaryKeyAsyncReturnsNullForEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = await reader.GetPrimaryKeyAsync("Items");

            // Assert
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Reports that a missing table does not exist.
        /// </summary>
        [TestMethod]
        public void TableExistsReturnsFalseForEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.TableExists("Items");

            // Assert
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Reports a missing table asynchronously.
        /// </summary>
        [TestMethod]
        public async Task TableExistsAsyncReturnsFalseForEmptyCatalog()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = await reader.TableExistsAsync("Items");

            // Assert
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Keeps the requested table identity for an empty catalog.
        /// </summary>
        [TestMethod]
        public void GetTableSchemaPreservesTableName()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetTableSchema("Items");

            // Assert
            Assert.AreEqual("Items",
                actual.Table.Name,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Keeps the requested table identity asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetTableSchemaAsyncPreservesTableName()
        {
            // Setup
            using var connection = new CatalogConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = await reader.GetTableSchemaAsync("Items");

            // Assert
            Assert.AreEqual("Items",
                actual.Table.Name,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Maps a real catalog record to a named column.
        /// </summary>
        [TestMethod]
        public void GetColumnsMapsCatalogRecord()
        {
            // Setup
            using var connection = CreateColumnConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = reader.GetColumns("Items").Single();

            // Assert
            Assert.AreEqual("Id",
                actual.Field.Name,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Maps a catalog record asynchronously.
        /// </summary>
        [TestMethod]
        public async Task GetColumnsAsyncMapsCatalogRecord()
        {
            // Setup
            using var connection = CreateColumnConnection();
            var reader = new TursoSchemaReader(connection);

            // Act
            var actual = (await reader.GetColumnsAsync("Items")).Single();

            // Assert
            Assert.AreEqual("Id",
                actual.Field.Name,
                StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the table and column catalog projections.
        /// </summary>
        /// <returns>The configured catalog connection.</returns>
        private static CatalogConnection CreateColumnConnection()
        {
            var connection = new CatalogConnection { Data = CreateColumnCatalog() };
            connection.Query = sql => sql.Contains(" AS DeclaredType",
                    StringComparison.Ordinal) ? connection.Data :
                sql.Contains(" AS TableName",
                    StringComparison.Ordinal) ? TableDefinitionCatalog() : new DataTable();
            return connection;
        }

        /// <summary>
        /// Creates the SQLite master projection for a table.
        /// </summary>
        /// <returns>The table definition record.</returns>
        private static DataTable TableDefinitionCatalog()
        {
            var table = new DataTable();
            table.Columns.Add("TableName",
                typeof(string));
            table.Columns.Add("Definition",
                typeof(string));
            table.Rows.Add("Items",
                "CREATE TABLE Items (Id INTEGER)");
            return table;
        }

        /// <summary>
        /// Creates a catalog column with nullable optional metadata.
        /// </summary>
        /// <returns>The catalog record table.</returns>
        private static DataTable CreateColumnCatalog()
        {
            var table = new DataTable();
            var fields = _columnValues;
            foreach (var field in fields)
                table.Columns.Add(field.Key,
                    typeof(object));
            table.Rows.Add(fields.Select(field => field.Value).ToArray());
            return table;
        }

        #endregion
    }
}
