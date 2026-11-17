#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.DuckDb.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.DuckDb.IntegrationTests
{
    /// <summary>
    /// Contains the duck db schema reader test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaReaderTest
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
        /// Verifies duck db schema reader table exists.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderTableExists()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader table exists with schema name.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    $"{Database.SourceSalesName}.Invoice",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader table exists with quoted name.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "\"Person\"",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader table exists with the table of another schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Invoice",
                    transaction: null);
                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader table exists with missing table.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader table exists async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderTableExistsAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = await reader.TableExistsAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader table exists async with missing table.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get schema name.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetSchemaName()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get schema name of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get schema name with explicit schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get schema name of missing table is the default schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get schema name async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumns()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns of identity column.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfIdentityColumn()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
                Assert.AreEqual(10L,
                    actual.IdentitySeed);
                Assert.AreEqual(5L,
                    actual.IdentityIncrement);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get columns of non identity column.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns of string column.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
                Assert.AreEqual(0,
                    actual.Field.Size);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.IsNull(actual.Collation);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get columns of max size columns.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns have no comment.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsHaveNoComment()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var columns = reader.GetColumns(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").Comment);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Age").Comment);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get columns of column with default.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns of computed column.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns of numeric columns.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
                Assert.AreEqual("timestamp",
                    createdDate.Field.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual((byte)0,
                    createdDate.Field.Scale);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get columns of composite primary key table.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns of missing table.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get columns async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetColumnsAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetColumnsAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                Assert.AreEqual(10L,
                    actual.Single(c => c.Field.Name == "Id").IdentitySeed);
            }
        }

        #endregion

        #endregion

        #region GetPrimaryKey

        #region Sync

        /// <summary>
        /// Verifies duck db schema reader get primary key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetPrimaryKey()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("Person_id_pkey",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get primary key of composite key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("OrderLine_orderid_linenumber_pkey",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get primary key of table without key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get primary key async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = await reader.GetPrimaryKeyAsync(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("OrderLine_orderid_linenumber_pkey",
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
        /// Verifies duck db schema reader get indexes.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexes()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get indexes does not include the primary key and the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexesDoesNotIncludeThePrimaryKeyAndTheUniqueConstraints()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get indexes async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetIndexesAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        /// <summary>
        /// Verifies duck db schema reader get foreign keys.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("Person_countryid_id_fkey",
                    actual[0].Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" },
                    actual[0].Columns.ToArray());
                Assert.AreEqual(new TableInfo("Country",
                        null),
                    actual[0].ReferencedTable);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual[0].ReferencedColumns.ToArray());
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual[0].UpdateRule);
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual[0].DeleteRule);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys default rules.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysDefaultRules()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Child",
                    transaction: null).Single();
                // Assert
                Assert.AreEqual("Child_parentid_id_fkey",
                    actual.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual.UpdateRule);
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual.DeleteRule);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys of table without foreign keys.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get foreign keys async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetForeignKeysAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetForeignKeysAsync(connection,
                    "Person",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("Person_countryid_id_fkey",
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
        /// Verifies duck db schema reader get unique constraints.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetUniqueConstraints()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetUniqueConstraints(connection,
                    "Country",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("Country_name_key",
                    actual[0].Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Name" },
                    actual[0].Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get unique constraints of table without unique constraints.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get unique constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetUniqueConstraintsAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetUniqueConstraintsAsync(connection,
                    "Country",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("Country_name_key",
                    actual.Name,
                    StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetCheckConstraints

        #region Sync

        /// <summary>
        /// Verifies duck db schema reader get check constraints.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetCheckConstraints()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetCheckConstraints(connection,
                    "Person",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                Assert.AreEqual("Person_age_check",
                    actual[0].Name,
                    StringComparer.Ordinal);
                StringAssert.Contains(actual[0].Expression,
                    "Age",
                    StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get check constraints of table without check constraints.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get check constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetCheckConstraintsAsync(connection,
                    "Person",
                    transaction: null)).Single();
                // Assert
                Assert.AreEqual("Person_age_check",
                    actual.Name,
                    StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetSchemas

        #region Sync

        /// <summary>
        /// Verifies duck db schema reader get schemas.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetSchemas()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetSchemas(connection,
                    transaction: null).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    "main");
                CollectionAssert.Contains(actual,
                    Database.SourceSalesName);
                CollectionAssert.DoesNotContain(actual,
                    "information_schema");
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies duck db schema reader get schemas async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetSchemasAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetSchemasAsync(connection,
                    transaction: null)).Select(schema => schema.Name).ToList();
                // Assert
                Assert.IsTrue(actual.Count > 0);
                CollectionAssert.Contains(actual,
                    "main");
                CollectionAssert.Contains(actual,
                    Database.SourceSalesName);
                CollectionAssert.DoesNotContain(actual,
                    "information_schema");
            }
        }

        #endregion

        #endregion

        #region GetTables

        #region Sync

        /// <summary>
        /// Verifies duck db schema reader get tables.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTables()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get tables of schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get tables of missing schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get tables async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetTablesAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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

        /// <summary>
        /// Verifies duck db schema reader get dependency order.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrder()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order of person and country.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderOfPersonAndCountry()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order ignores the tables that are not in the list.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderIgnoresTheTablesThatAreNotInTheList()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "GrandChild" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "GrandChild" },
                    actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies duck db schema reader get dependency order async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetDependencyOrderAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderRelationshipsOfDependencyChain()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order relationships share the same instances.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderRelationshipsShareTheSameInstances()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order relationships have the schema of the table.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderRelationshipsHaveTheSchemaOfTheTable()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order relationships of table with multiple parents.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderRelationshipsOfTableWithMultipleParents()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order relationships ignore the self reference.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderRelationshipsIgnoreTheSelfReference()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get dependency order ignores the table that is given more than once.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetDependencyOrderIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "Parent", "Parent", "\"Parent\"" },
                        transaction: null));
                // Assert
                CollectionAssert.AreEqual(new[] { "Parent" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get dependency order async relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetDependencyOrderAsyncRelationshipsOfDependencyChain()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get table schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTableSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("DUCKDB",
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
        /// Verifies duck db schema reader get table schema of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get table schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader get table schema async.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = await reader.GetTableSchemaAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("DUCKDB",
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
        /// Verifies duck db schema reader get table schema async is the same as the sync one.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
        /// Verifies duck db schema reader with transaction.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderWithTransaction()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                // Setup
                var reader = new DuckDbSchemaReader();
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
