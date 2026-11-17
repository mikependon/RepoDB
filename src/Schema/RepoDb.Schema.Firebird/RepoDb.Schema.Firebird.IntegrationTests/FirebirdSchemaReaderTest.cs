#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Firebird.IntegrationTests
{
    /// <summary>
    /// Contains the firebird schema reader test behavior.
    /// </summary>
    [TestClass]
    public class FirebirdSchemaReaderTest
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
        /// Verifies firebird schema reader table exists.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderTableExists()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader table exists with quoted name.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.TableExists(connection,
                    "\"Person\"",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader table exists with missing table.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader table exists async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderTableExistsAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = await reader.TableExistsAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader table exists async with missing table.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get schema name.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetSchemaName()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetSchemaName(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get schema name of missing table is the default schema.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get schema name async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = await reader.GetSchemaNameAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #endregion

        #region GetColumns

        #region Sync

        /// <summary>
        /// Verifies firebird schema reader get columns.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumns()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of identity column.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfIdentityColumn()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of non identity column.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of string column.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
                Assert.IsNull(actual.Collation);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get columns of max size columns.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns have no comment.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsHaveNoComment()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of column with default.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
                    "CURRENT_TIMESTAMP",
                    StringComparison.OrdinalIgnoreCase);
                Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp,
                    createdDate.DefaultValue);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
                Assert.AreEqual(ColumnDefaultValue.None,
                    columns.Single(c => c.Field.Name == "Name").DefaultValue);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get columns of computed column.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of numeric columns.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of composite primary key table.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns of missing table.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get columns async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetColumnsAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get primary key.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetPrimaryKey()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("PK_Person",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get primary key of composite key.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("PK_OrderLine",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get primary key of table without key.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get primary key async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = await reader.GetPrimaryKeyAsync(connection,
                    "OrderLine",
                    transaction: null);
                // Assert
                Assert.AreEqual("PK_OrderLine",
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
        /// Verifies firebird schema reader get indexes.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetIndexes()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
                CollectionAssert.AreEqual(new[] { "Name", "Age" },
                    actual[0].DescendingColumns.ToArray());
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get indexes does not include the primary key and the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetIndexesDoesNotIncludeThePrimaryKeyAndTheUniqueConstraints()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get indexes async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetIndexesAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Person",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(1,
                    actual.Count);
                CollectionAssert.AreEqual(new[] { "Name", "Age" },
                    actual[0].DescendingColumns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        /// <summary>
        /// Verifies firebird schema reader get foreign keys.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetForeignKeys()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual[0].UpdateRule);
                Assert.AreEqual(ForeignKeyRule.SetNull,
                    actual[0].DeleteRule);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get foreign keys default rules.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetForeignKeysDefaultRules()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Child",
                    transaction: null).Single();
                // Assert
                Assert.AreEqual("FK_Child_Parent",
                    actual.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual.UpdateRule);
                Assert.AreEqual(ForeignKeyRule.NoAction,
                    actual.DeleteRule);
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get foreign keys of table without foreign keys.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get foreign keys async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetForeignKeysAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get unique constraints.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetUniqueConstraints()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get unique constraints of table without unique constraints.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get unique constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetUniqueConstraintsAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get check constraints.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetCheckConstraints()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get check constraints of table without check constraints.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get check constraints async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get schemas.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetSchemas()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetSchemas(connection,
                    transaction: null).Select(schema => schema.Name).ToList();
                // Assert
                Assert.AreEqual(0,
                    actual.Count);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies firebird schema reader get schemas async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetSchemasAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = (await reader.GetSchemasAsync(connection,
                    transaction: null)).Select(schema => schema.Name).ToList();
                // Assert
                Assert.AreEqual(0,
                    actual.Count);
            }
        }

        #endregion

        #endregion

        #region GetTables

        #region Sync

        /// <summary>
        /// Verifies firebird schema reader get tables.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetTables()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    transaction: null).ToList();
                // Assert
                CollectionAssert.Contains(actual,
                    "Person");
                CollectionAssert.Contains(actual,
                    "Country");
            }
        }

        /// <summary>
        /// Verifies throw exception on firebird schema reader get tables if the schema is given.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetTablesIfTheSchemaIsGiven()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act/Assert
                Assert.Throws<NotSupportedException>(() => reader.GetTables(connection,
                        "MissingSchema",
                        transaction: null));
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies firebird schema reader get tables async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetTablesAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = (await reader.GetTablesAsync(connection,
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.Contains(actual,
                    "Person");
                CollectionAssert.Contains(actual,
                    "Country");
            }
        }

        #endregion

        #endregion

        #region GetDependencyOrder

        #region Sync

        /// <summary>
        /// Verifies firebird schema reader get dependency order.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrder()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order of person and country.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderOfPersonAndCountry()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order ignores the tables that are not in the list.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderIgnoresTheTablesThatAreNotInTheList()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order with circular references.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderWithCircularReferences()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetDependencyOrderAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsOfDependencyChain()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships share the same instances.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsShareTheSameInstances()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships have the schema of the table.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsHaveTheSchemaOfTheTable()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships of table with multiple parents.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsOfTableWithMultipleParents()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships ignore the self reference.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsIgnoreTheSelfReference()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order relationships of circular references.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderRelationshipsOfCircularReferences()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order breaks the cycle at the table that was given first.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderBreaksTheCycleAtTheTableThatWasGivenFirst()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(connection,
                        new[] { "Ledger", "CycleA", "CycleB", "Item" },
                        transaction: null));
                // Assert
                Assert.AreEqual(4,
                    actual.Length);
                Assert.IsTrue(Array.IndexOf(actual,
                        "CycleA") < Array.IndexOf(actual,
                        "CycleB"));
            }
        }

        /// <summary>
        /// Verifies firebird schema reader get dependency order ignores the table that is given more than once.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetDependencyOrderIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get dependency order async relationships of dependency chain.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetDependencyOrderAsyncRelationshipsOfDependencyChain()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get table schema.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetTableSchema()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("FIREBIRD",
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
        /// Verifies firebird schema reader get table schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader get table schema async.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
                // Act
                var actual = await reader.GetTableSchemaAsync(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.AreEqual("Person",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("FIREBIRD",
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
        /// Verifies firebird schema reader get table schema async is the same as the sync one.
        /// </summary>
        [TestMethod]
        public async Task TestFirebirdSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
        /// Verifies firebird schema reader with transaction.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderWithTransaction()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                // Setup
                var reader = new FirebirdSchemaReader();
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
