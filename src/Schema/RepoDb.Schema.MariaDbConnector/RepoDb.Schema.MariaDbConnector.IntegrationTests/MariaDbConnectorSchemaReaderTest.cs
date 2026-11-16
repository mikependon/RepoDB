#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.MariaDbConnector.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.MariaDbConnector.IntegrationTests
{
    /// <summary>
    /// Contains the maria db connector schema reader test behavior.
    /// </summary>
    [TestClass]
    public class MariaDbConnectorSchemaReaderTest
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
        /// Verifies maria db connector schema reader table exists.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderTableExists()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader table exists with schema name.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader table exists with quoted name.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "`Person`",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader table exists with the table of another schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Invoice",
                    transaction: null);
                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader table exists with missing table.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader table exists async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderTableExistsAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = await reader.TableExistsAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader table exists async with missing table.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get schema name.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetSchemaName()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get schema name of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get schema name with explicit schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get schema name of missing table is the default schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get schema name async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get columns.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumns()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get columns of identity column.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfIdentityColumn()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Id");
                // Assert
                Assert.IsTrue(actual.Field.IsIdentity);
                Assert.IsTrue(actual.Field.IsPrimary);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("bigint",
                    actual.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(typeof(long),
                    actual.Field.Type);
                Assert.IsNull(actual.IdentitySeed);
                Assert.IsNull(actual.IdentityIncrement);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of non identity column.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Age");
                // Assert
                Assert.IsFalse(actual.Field.IsIdentity);
                Assert.IsFalse(actual.Field.IsPrimary);
                Assert.IsTrue(actual.Field.IsNullable);
                Assert.IsNull(actual.IdentitySeed);
                Assert.IsNull(actual.IdentityIncrement);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of string column.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Name");
                // Assert
                Assert.AreEqual("varchar",
                    actual.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(typeof(string),
                    actual.Field.Type);
                Assert.AreEqual(128,
                    actual.Field.Size);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("utf8mb4_bin",
                    actual.Collation,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of max size columns.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get columns of column with comment.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfColumnWithComment()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual("The name of the person.",
                    columns.Single(c => c.Field.Name == "Name").Comment,
                    StringComparer.Ordinal);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Age").Comment);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of column with default.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
                    "current_timestamp",
                    StringComparison.OrdinalIgnoreCase);
                Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp,
                    createdDate.DefaultValue);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
                Assert.AreEqual(ColumnDefaultValue.None,
                    columns.Single(c => c.Field.Name == "Name").DefaultValue);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of computed column.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                var computed = columns.Single(c => c.Field.Name == "NameUpper");
                // Assert
                Assert.IsNotNull(computed.ComputedExpression);
                StringAssert.Contains(computed.ComputedExpression,
                    "ucase",
                    StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").ComputedExpression);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of numeric columns.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var salary = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "Salary");
                var createdDate = reader.GetColumns(connection,
                    "Person",
                    transaction: null).Single(c => c.Field.Name == "CreatedDateUtc");
                // Assert
                Assert.AreEqual("decimal",
                    salary.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual((byte)18,
                    salary.Field.Precision);
                Assert.AreEqual((byte)2,
                    salary.Field.Scale);
                Assert.AreEqual("datetime",
                    createdDate.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual((byte)3,
                    createdDate.Field.Scale);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get columns of composite primary key table.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get columns of missing table.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get columns async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetColumnsAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get primary key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetPrimaryKey()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get primary key of composite key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get primary key of table without key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get primary key async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get indexes.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexes()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
                CollectionAssert.AreEqual(new[] { "Name", "Age" },
                    actual[0].Columns.ToArray());
                CollectionAssert.AreEqual(new[] { "Name" },
                    actual[0].DescendingColumns.ToArray());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get indexes does not include the primary key and the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexesDoesNotIncludeThePrimaryKeyAndTheUniqueConstraints()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetIndexes(connection,
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
        /// Verifies maria db connector schema reader get indexes async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetIndexesAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                CollectionAssert.AreEqual(new[] { "Name" },
                    actual[0].DescendingColumns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get foreign keys.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetForeignKeys()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("FK_Person_Country",
                    actual[0].Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" },
                    actual[0].Columns.ToArray());
                Assert.AreEqual(new TableInfo("Country",
                        null),
                    actual[0].ReferencedTable);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual[0].ReferencedColumns.ToArray());
                Assert.AreEqual(ForeignKeyRule.Cascade,
                    actual[0].UpdateRule);
                Assert.AreEqual(ForeignKeyRule.SetNull,
                    actual[0].DeleteRule);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get foreign keys default rules.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetForeignKeysDefaultRules()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Child",
                    transaction: null).Single();
                // Assert
                Assert.AreEqual("FK_Child_Parent",
                    actual.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(ForeignKeyRule.Restrict,
                    actual.UpdateRule);
                Assert.AreEqual(ForeignKeyRule.Restrict,
                    actual.DeleteRule);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get foreign keys of table without foreign keys.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
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
        /// Verifies maria db connector schema reader get foreign keys async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetForeignKeysAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetForeignKeysAsync(connection,
                    "Person",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("FK_Person_Country",
                    actual.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(new TableInfo("Country",
                        null),
                    actual.ReferencedTable);
            }
        }

        #endregion

        #endregion

        #region GetUniqueConstraints

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get unique constraints.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetUniqueConstraints()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetUniqueConstraints(connection,
                    "Country",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("UQ_Country_Name",
                    actual[0].Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Name" },
                    actual[0].Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get unique constraints of table without unique constraints.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetUniqueConstraints(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual(0,
                    actual.Count());
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies maria db connector schema reader get unique constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetUniqueConstraintsAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetUniqueConstraintsAsync(connection,
                    "Country",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("UQ_Country_Name",
                    actual.Name,
                    StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetCheckConstraints

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get check constraints.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetCheckConstraints()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get check constraints of table without check constraints.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get check constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get schemas.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetSchemas()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetSchemas(connection,
                    transaction: null).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    connection.Database);
                CollectionAssert.DoesNotContain(actual,
                    "mysql");
                CollectionAssert.DoesNotContain(actual,
                    "information_schema");
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies maria db connector schema reader get schemas async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetSchemasAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetSchemasAsync(connection,
                    transaction: null)).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    connection.Database);
                CollectionAssert.DoesNotContain(actual,
                    "mysql");
                CollectionAssert.DoesNotContain(actual,
                    "information_schema");
            }
        }

        #endregion

        #endregion

        #region GetTables

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get tables.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTables()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get tables of schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    Database.SourceSalesName,
                    transaction: null).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get tables of missing schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get tables async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetTablesAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetTablesAsync(connection,
                    Database.SourceSalesName,
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item" },
                    actual);
            }
        }

        #endregion

        #endregion

        #region GetDependencyOrder

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrder()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "GrandChild", "Parent", "Child" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order of person and country.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderOfPersonAndCountry()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "Person", "Country" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order ignores the tables that are not in the list.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderIgnoresTheTablesThatAreNotInTheList()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "GrandChild" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "GrandChild" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order with circular references.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderWithCircularReferences()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "CycleA", "CycleB" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "CycleA", "CycleB" },
                    actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetDependencyOrderAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(await reader.GetDependencyOrderAsync(connection,
                        new[] { "GrandChild", "Parent", "Child" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" },
                    actual);
            }
        }

        #endregion

        #region Relationships

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsOfDependencyChain()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "GrandChild", "Parent", "Child" },
                    transaction: null).ToList();
                var parent = actual[0];
                var child = actual[1];
                var grandChild = actual[2];
                // Assert
                Assert.AreEqual(0,
                    parent.Parents.Count);
                CollectionAssert.AreEqual(new[] { "Child" },
                    parent.Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" },
                    child.Parents.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" },
                    child.Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Child" },
                    grandChild.Parents.Select(r => r.Schema.Table.Name).ToArray());
                Assert.AreEqual(0,
                    grandChild.Children.Count);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships share the same instances.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsShareTheSameInstances()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "Child", "Parent" },
                    transaction: null).ToList();
                // Assert
                Assert.AreSame(actual[0],
                    actual[1].Parents.Single());
                Assert.AreSame(actual[1],
                    actual[0].Children.Single());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships have the schema of the table.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsHaveTheSchemaOfTheTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "Person", "Country" },
                    transaction: null).ToList();
                var person = actual.Single(r => r.Schema.Table.Name == "Person");
                // Assert
                Assert.AreEqual(7,
                    person.Schema.Columns.Count);
                Assert.AreEqual(1,
                    person.Schema.ForeignKeys.Count);
                Assert.IsNotNull(person.Schema.PrimaryKey);
                CollectionAssert.AreEqual(new[] { "Country" },
                    person.Parents.Select(r => r.Schema.Table.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships of table with multiple parents.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsOfTableWithMultipleParents()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "Shipment", "OrderLine", "Country" },
                    transaction: null).ToList();
                var shipment = actual.Single(r => r.Schema.Table.Name == "Shipment");
                // Assert
                Assert.AreEqual("Shipment",
                    actual[2].Schema.Table.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "OrderLine", "Country" },
                    shipment.Parents.Select(r => r.Schema.Table.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships ignore the self reference.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsIgnoreTheSelfReference()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "Employee" },
                    transaction: null).Single();
                // Assert
                Assert.AreEqual(0,
                    actual.Parents.Count);
                Assert.AreEqual(0,
                    actual.Children.Count);
                Assert.AreEqual(1,
                    actual.Schema.ForeignKeys.Count);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order relationships of circular references.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderRelationshipsOfCircularReferences()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetDependencyOrder(connection,
                    new[] { "CycleA", "CycleB" },
                    transaction: null).ToList();
                // Assert
                Assert.AreSame(actual[1],
                    actual[0].Parents.Single());
                Assert.AreSame(actual[1],
                    actual[0].Children.Single());
                Assert.AreSame(actual[0],
                    actual[1].Parents.Single());
                Assert.AreSame(actual[0],
                    actual[1].Children.Single());
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order breaks the cycle at the table that was given first.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderBreaksTheCycleAtTheTableThatWasGivenFirst()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "Ledger", "CycleA", "CycleB", $"{Database.SourceSalesName}.Invoice" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "CycleA", "CycleB", $"{Database.SourceSalesName}.Invoice", "Ledger" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order ignores the table that is given more than once.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetDependencyOrderIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "Parent", "Parent", "`Parent`" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "Parent" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get dependency order async relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetDependencyOrderAsyncRelationshipsOfDependencyChain()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetDependencyOrderAsync(connection,
                    new[] { "GrandChild", "Parent", "Child" },
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { "Child" },
                    actual[0].Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" },
                    actual[1].Parents.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" },
                    actual[1].Children.Select(r => r.Schema.Table.Name).ToArray());
                Assert.AreEqual(0,
                    actual[2].Children.Count);
            }
        }

        #endregion

        #endregion

        #region GetTableSchema

        #region Sync

        /// <summary>
        /// Verifies maria db connector schema reader get table schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTableSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("MARIADB",
                    actual.Table.Provider,
                    StringComparer.Ordinal);
                Assert.IsNotNull(actual.Table.Database);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7,
                    actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1,
                    actual.Indexes.Count);
                Assert.AreEqual(1,
                    actual.ForeignKeys.Count);
                Assert.AreEqual(0,
                    actual.UniqueConstraints.Count);
                Assert.AreEqual(1,
                    actual.CheckConstraints.Count);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get table schema of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get table schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get table schema async.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = await reader.GetTableSchemaAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("MARIADB",
                    actual.Table.Provider,
                    StringComparer.Ordinal);
                Assert.IsNotNull(actual.Table.Database);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7,
                    actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1,
                    actual.Indexes.Count);
                Assert.AreEqual(1,
                    actual.ForeignKeys.Count);
                Assert.AreEqual(1,
                    actual.CheckConstraints.Count);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get table schema async is the same as the sync one.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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

        /// <summary>
        /// Verifies maria db connector schema reader with transaction.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderWithTransaction()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "Person",
                    transaction: transaction);
                // Assert
                Assert.AreEqual(7,
                    actual.Columns.Count);
            }
        }
        #endregion
    }
}
