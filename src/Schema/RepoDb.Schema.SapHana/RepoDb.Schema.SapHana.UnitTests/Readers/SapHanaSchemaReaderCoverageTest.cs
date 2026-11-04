using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.UnitTestHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Schema.SapHana.UnitTests.Readers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class SapHanaSchemaReaderCoverageTest
    {
        #region Private Variables

        private static readonly KeyValuePair<string, object>[] _columnValues =
            new[]
            {
                new KeyValuePair<string, object>("ChildSchema",
                    DBNull.Value),
                new KeyValuePair<string, object>("ChildTable",
                    DBNull.Value),
                new KeyValuePair<string, object>("ColumnName",
                    "Id"),
                new KeyValuePair<string, object>("ComputedDefinition",
                    DBNull.Value),
                new KeyValuePair<string, object>("ConstraintName",
                    DBNull.Value),
                new KeyValuePair<string, object>("DefaultDefinition",
                    DBNull.Value),
                new KeyValuePair<string, object>("Definition",
                    DBNull.Value),
                new KeyValuePair<string, object>("DeleteAction",
                    DBNull.Value),
                new KeyValuePair<string, object>("FilterDefinition",
                    DBNull.Value),
                new KeyValuePair<string, object>("ForeignKeyName",
                    DBNull.Value),
                new KeyValuePair<string, object>("IdentityOptions",
                    DBNull.Value),
                new KeyValuePair<string, object>("IndexName",
                    DBNull.Value),
                new KeyValuePair<string, object>("IsClustered",
                    0),
                new KeyValuePair<string, object>("IsDescending",
                    0),
                new KeyValuePair<string, object>("IsIdentity",
                    0),
                new KeyValuePair<string, object>("IsIncluded",
                    0),
                new KeyValuePair<string, object>("IsNullable",
                    true),
                new KeyValuePair<string, object>("IsPrimary",
                    0),
                new KeyValuePair<string, object>("IsUnique",
                    0),
                new KeyValuePair<string, object>("MaxLength",
                    10),
                new KeyValuePair<string, object>("Name",
                    "Id"),
                new KeyValuePair<string, object>("Ordinal",
                    0),
                new KeyValuePair<string, object>("ParentSchema",
                    DBNull.Value),
                new KeyValuePair<string, object>("ParentTable",
                    DBNull.Value),
                new KeyValuePair<string, object>("PrecisionValue",
                    0),
                new KeyValuePair<string, object>("ReferencedColumn",
                    DBNull.Value),
                new KeyValuePair<string, object>("ReferencedSchema",
                    DBNull.Value),
                new KeyValuePair<string, object>("ReferencedTable",
                    DBNull.Value),
                new KeyValuePair<string, object>("ScaleValue",
                    0),
                new KeyValuePair<string, object>("SchemaName",
                    DBNull.Value),
                new KeyValuePair<string, object>("TableName",
                    DBNull.Value),
                new KeyValuePair<string, object>("TypeName",
                    "INTEGER"),
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetColumns("test.Items").ToArray();

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetColumnsAsync("test.Items"))
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetIndexes("test.Items").ToArray();

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetIndexesAsync("test.Items"))
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetForeignKeys("test.Items").ToArray();

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetForeignKeysAsync("test.Items"))
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetUniqueConstraints("test.Items").ToArray();

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetUniqueConstraintsAsync("test.Items"))
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetCheckConstraints("test.Items").ToArray();

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetCheckConstraintsAsync("test.Items"))
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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetPrimaryKey("test.Items");

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = await reader.GetPrimaryKeyAsync("test.Items");

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.TableExists("test.Items");

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = await reader.TableExistsAsync("test.Items");

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetTableSchema("test.Items");

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
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = await reader.GetTableSchemaAsync("test.Items");

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
            using var connection = new CatalogConnection { Data = CreateColumnCatalog() };
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = reader.GetColumns("test.Items").Single();

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
            using var connection = new CatalogConnection { Data = CreateColumnCatalog() };
            var reader = new SapHanaSchemaReader(connection);

            // Act
            var actual = (await reader.GetColumnsAsync("test.Items")).Single();

            // Assert
            Assert.AreEqual("Id",
                actual.Field.Name,
                StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

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
