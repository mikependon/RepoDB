#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Contains the click house schema reader test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaReaderTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region TableExists

        #region Sync

        /// <summary>
        /// Verifies click house schema reader table exists.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderTableExists()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader table exists with schema name.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader table exists with quoted name.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "`Person`",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader table exists with the table of another schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Invoice",
                    transaction: null);
                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader table exists with missing table.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "MissingTable",
                    transaction: null);
                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader table exists async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderTableExistsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = await reader.TableExistsAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader table exists async with missing table.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = await reader.TableExistsAsync(connection,
                    "MissingTable",
                    transaction: null);
                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #endregion

        #region GetSchemaName

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get schema name.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get schema name of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.AreEqual(Database.SourceSalesName,
                    actual,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get schema name with explicit schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.AreEqual(Database.SourceSalesName,
                    actual,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get schema name of missing table is the default schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    "MissingTable",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get schema name async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = await reader.GetSchemaNameAsync(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.AreEqual(Database.SourceSalesName,
                    actual,
                    StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetColumns

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get columns.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 },
                    actual.Select(c => c.Ordinal).ToArray());
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of string column.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Name");
                // Assert
                Assert.AreEqual("String",
                    actual.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(typeof(string),
                    actual.Field.Type);
                Assert.IsFalse(actual.Field.IsNullable);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of max size columns.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "NoKey",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(0,
                    columns.Single(c => c.Field.Name == "Value").Field.Size);
                Assert.AreEqual(0,
                    columns.Single(c => c.Field.Name == "Payload").Field.Size);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of column with default.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                var age = columns.Single(c => c.Field.Name == "Age");
                var createdDate = columns.Single(c => c.Field.Name == "CreatedDateUtc");
                // Assert
                Assert.AreEqual("0",
                    age.DefaultExpression,
                    StringComparer.Ordinal);
                Assert.AreEqual(ColumnDefaultValue.Literal,
                    age.DefaultValue);
                Assert.IsTrue(age.Field.HasDefaultValue);
                StringAssert.Contains(createdDate.DefaultExpression,
                    "now64(3)",
                    StringComparison.OrdinalIgnoreCase);
                Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp,
                    createdDate.DefaultValue);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
                Assert.AreEqual(ColumnDefaultValue.None,
                    columns.Single(c => c.Field.Name == "Name").DefaultValue);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of computed column.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                var computed = columns.Single(c => c.Field.Name == "NameUpper");
                // Assert
                Assert.IsNotNull(computed.ComputedExpression);
                StringAssert.Contains(computed.ComputedExpression,
                    "upper",
                    StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").ComputedExpression);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of numeric columns.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var salary = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Salary");
                var createdDate = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "CreatedDateUtc");
                // Assert
                Assert.AreEqual("Decimal",
                    salary.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual((byte)18,
                    salary.Field.Precision);
                Assert.AreEqual((byte)2,
                    salary.Field.Scale);
                Assert.AreEqual("DateTime64",
                    createdDate.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual((byte)3,
                    createdDate.Field.Scale);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of composite primary key table.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "OrderLine",
                    transaction: null).ToList();
                // Assert
                Assert.IsTrue(columns.Single(c => c.Field.Name == "OrderId").Field.IsPrimary);
                Assert.IsTrue(columns.Single(c => c.Field.Name == "LineNumber").Field.IsPrimary);
                Assert.IsFalse(columns.Single(c => c.Field.Name == "Quantity").Field.IsPrimary);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get columns of missing table.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "MissingTable",
                    transaction: null);
                // Assert
                Assert.AreEqual(0,
                    actual.Count());
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get columns async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetColumnsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetColumnsAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                Assert.IsNull(actual.Single(c => c.Field.Name == "Id").IdentitySeed);
            }
        }

        #endregion

        #endregion

        #region GetPrimaryKey

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get primary key.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("PRIMARY",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies click house schema reader get primary key of composite key.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("PRIMARY",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies click house schema reader get primary key of table without key.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "NoKey",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get primary key async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = await reader.GetPrimaryKeyAsync(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("PRIMARY",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Columns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetIndexes

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get indexes.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetIndexes(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("IX_Person_Name",
                    actual[0].Name,
                    StringComparer.Ordinal);
                Assert.IsFalse(actual[0].IsUnique);
                CollectionAssert.AreEqual(new[] { "Name", "Id" },
                    actual[0].Columns.ToArray());
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get indexes async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetIndexesAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                CollectionAssert.AreEqual(new[] { "Name", "Id" },
                    actual[0].Columns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        #endregion

        #region Async

        #endregion

        #endregion

        #region GetUniqueConstraints

        #region Sync

        #endregion

        #region Async

        #endregion

        #endregion

        #region GetCheckConstraints

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get check constraints.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetCheckConstraints()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetCheckConstraints(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("CK_Person_Age",
                    actual[0].Name,
                    StringComparer.Ordinal);
                StringAssert.Contains(actual[0].Expression,
                    "Age",
                    StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get check constraints of table without check constraints.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetCheckConstraints(connection,
                    "Country",
                    transaction: null);
                // Assert
                Assert.AreEqual(0,
                    actual.Count());
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get check constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetCheckConstraintsAsync(connection,
                    "Person",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("CK_Person_Age",
                    actual.Name,
                    StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetSchemas

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get schemas.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemas()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetSchemas(connection,
                    transaction: null).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    connection.Database);
                CollectionAssert.DoesNotContain(actual,
                    "system");
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get schemas async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetSchemasAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetSchemasAsync(connection,
                    transaction: null)).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    connection.Database);
                CollectionAssert.DoesNotContain(actual,
                    "system");
            }
        }

        #endregion

        #endregion

        #region GetTables

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get tables.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTables()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    transaction: null).ToList();
                // Assert
                CollectionAssert.Contains(actual,
                    "Person");
                CollectionAssert.Contains(actual,
                    "Country");
                CollectionAssert.Contains(reader.GetTables(connection,
                        Database.SourceSalesName,
                        transaction: null).ToList(),
                    $"{Database.SourceSalesName}.Invoice");
            }
        }

        /// <summary>
        /// Verifies click house schema reader get tables of schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    Database.SourceSalesName,
                    transaction: null).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item", $"{Database.SourceSalesName}.ItemRef" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get tables of missing schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    "MissingSchema",
                    transaction: null);
                // Assert
                Assert.AreEqual(0,
                    actual.Count());
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get tables async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetTablesAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetTablesAsync(connection,
                    Database.SourceSalesName,
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item", $"{Database.SourceSalesName}.ItemRef" },
                    actual);
            }
        }

        #endregion

        #endregion

        #region GetDependencyOrder

        #region Sync

        #endregion

        #region Async

        #endregion

        #region Relationships

        #endregion

        #endregion

        #region GetTableSchema

        #region Sync

        /// <summary>
        /// Verifies click house schema reader get table schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("CLICKHOUSE",
                    actual.Table.Provider,
                    StringComparer.Ordinal);
                Assert.IsNotNull(actual.Table.Database);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7,
                    actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1,
                    actual.Indexes.Count);
                Assert.AreEqual(0,
                    actual.ForeignKeys.Count);
                Assert.AreEqual(0,
                    actual.UniqueConstraints.Count);
                Assert.AreEqual(1,
                    actual.CheckConstraints.Count);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get table schema of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.AreEqual("Invoice",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(Database.SourceSalesName,
                    actual.Table.Schema,
                    StringComparer.Ordinal);
                Assert.AreEqual(2,
                    actual.Columns.Count);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get table schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "NoKey",
                    transaction: null);
                // Assert
                Assert.AreEqual(2,
                    actual.Columns.Count);
                Assert.IsNull(actual.PrimaryKey);
                Assert.AreEqual(0,
                    actual.Indexes.Count);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies click house schema reader get table schema async.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = await reader.GetTableSchemaAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("CLICKHOUSE",
                    actual.Table.Provider,
                    StringComparer.Ordinal);
                Assert.IsNotNull(actual.Table.Database);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7,
                    actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1,
                    actual.Indexes.Count);
                Assert.AreEqual(0,
                    actual.ForeignKeys.Count);
                Assert.AreEqual(1,
                    actual.CheckConstraints.Count);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get table schema async is the same as the sync one.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var expected = reader.GetTableSchema(connection,
                    "Person",
                    transaction: null);
                var actual = await reader.GetTableSchemaAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Helper.AssertSchemaEquality(expected,
                    actual);
            }
        }
        #endregion

        #endregion

        #region Transaction

        #endregion
    }
}
