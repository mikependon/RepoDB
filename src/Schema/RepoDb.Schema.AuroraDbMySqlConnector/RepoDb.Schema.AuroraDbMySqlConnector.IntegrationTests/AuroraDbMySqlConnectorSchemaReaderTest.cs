#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaReaderTest
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

        #region TableExists

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderTableExists()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.TableExists($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.TableExists("`Person`");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Invoice");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.TableExists("MissingTable");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderTableExistsAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = await reader.TableExistsAsync("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = await reader.TableExistsAsync("MissingTable");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #endregion

        #region GetSchemaName

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetSchemaName()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("Person");

                // Assert
                Assert.IsNull(actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual(Database.SourceSalesName, actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual(Database.SourceSalesName, actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("MissingTable");

                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = await reader.GetSchemaNameAsync($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual(Database.SourceSalesName, actual, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetColumns

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumns()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").ToList();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 }, actual.Select(c => c.Ordinal).ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfIdentityColumn()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Id");

                // Assert
                Assert.IsTrue(actual.Field.IsIdentity);
                Assert.IsTrue(actual.Field.IsPrimary);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("bigint", actual.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(typeof(long), actual.Field.Type);
                Assert.IsNull(actual.IdentitySeed);
                Assert.IsNull(actual.IdentityIncrement);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Age");

                // Assert
                Assert.IsFalse(actual.Field.IsIdentity);
                Assert.IsFalse(actual.Field.IsPrimary);
                Assert.IsTrue(actual.Field.IsNullable);
                Assert.IsNull(actual.IdentitySeed);
                Assert.IsNull(actual.IdentityIncrement);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Name");

                // Assert
                Assert.AreEqual("varchar", actual.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(typeof(string), actual.Field.Type);
                Assert.AreEqual(128, actual.Field.Size);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("utf8mb4_bin", actual.Collation, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("NoKey").ToList();

                // Assert
                Assert.AreEqual(0, columns.Single(c => c.Field.Name == "Value").Field.Size);
                Assert.AreEqual(0, columns.Single(c => c.Field.Name == "Payload").Field.Size);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfColumnWithComment()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();

                // Assert
                Assert.AreEqual("The name of the person.", columns.Single(c => c.Field.Name == "Name").Comment, StringComparer.Ordinal);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Age").Comment);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();
                var age = columns.Single(c => c.Field.Name == "Age");
                var createdDate = columns.Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("0", age.DefaultExpression, StringComparer.Ordinal);
                Assert.IsTrue(age.Field.HasDefaultValue);
                StringAssert.Contains(createdDate.DefaultExpression, "current_timestamp", StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();
                var computed = columns.Single(c => c.Field.Name == "NameUpper");

                // Assert
                Assert.IsNotNull(computed.ComputedExpression);
                StringAssert.Contains(computed.ComputedExpression, "upper", StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").ComputedExpression);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var salary = reader.GetColumns("Person").Single(c => c.Field.Name == "Salary");
                var createdDate = reader.GetColumns("Person").Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("decimal", salary.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)18, salary.Field.Precision);
                Assert.AreEqual((byte)2, salary.Field.Scale);
                Assert.AreEqual("datetime", createdDate.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)3, createdDate.Field.Scale);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("OrderLine").ToList();

                // Assert
                Assert.IsTrue(columns.Single(c => c.Field.Name == "OrderId").Field.IsPrimary);
                Assert.IsTrue(columns.Single(c => c.Field.Name == "LineNumber").Field.IsPrimary);
                Assert.IsFalse(columns.Single(c => c.Field.Name == "Quantity").Field.IsPrimary);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("MissingTable");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetColumnsAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetColumnsAsync("Person")).ToList();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                Assert.IsNull(actual.Single(c => c.Field.Name == "Id").IdentitySeed);
            }
        }

        #endregion

        #endregion

        #region GetPrimaryKey

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKey()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("OrderLine");

                // Assert
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("NoKey");

                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = await reader.GetPrimaryKeyAsync("OrderLine");

                // Assert
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetIndexes

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexes()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("IX_Person_Name", actual[0].Name, StringComparer.Ordinal);
                Assert.IsFalse(actual[0].IsUnique);
                CollectionAssert.AreEqual(new[] { "Name", "Age" }, actual[0].Columns.ToArray());
                CollectionAssert.AreEqual(new[] { "Name" }, actual[0].DescendingColumns.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexesDoesNotIncludeThePrimaryKeyAndTheUniqueConstraints()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetIndexesAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Person")).ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                CollectionAssert.AreEqual(new[] { "Name" }, actual[0].DescendingColumns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetForeignKeys()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("FK_Person_Country", actual[0].Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" }, actual[0].Columns.ToArray());
                Assert.AreEqual(new TableInfo("Country", null), actual[0].ReferencedTable);
                CollectionAssert.AreEqual(new[] { "Id" }, actual[0].ReferencedColumns.ToArray());
                Assert.AreEqual(CopySchemaForeignKeyRule.Cascade, actual[0].UpdateRule);
                Assert.AreEqual(CopySchemaForeignKeyRule.SetNull, actual[0].DeleteRule);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetForeignKeysDefaultRules()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Child").Single();

                // Assert
                Assert.AreEqual("FK_Child_Parent", actual.Name, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
                Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.DeleteRule);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetForeignKeysAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetForeignKeysAsync("Person")).Single();

                // Assert
                Assert.AreEqual("FK_Person_Country", actual.Name, StringComparer.Ordinal);
                Assert.AreEqual(new TableInfo("Country", null), actual.ReferencedTable);
            }
        }

        #endregion

        #endregion

        #region GetUniqueConstraints

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetUniqueConstraints()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetUniqueConstraints("Country").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("UQ_Country_Name", actual[0].Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Name" }, actual[0].Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetUniqueConstraints("Person");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetUniqueConstraintsAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetUniqueConstraintsAsync("Country")).Single();

                // Assert
                Assert.AreEqual("UQ_Country_Name", actual.Name, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetCheckConstraints

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetCheckConstraints()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("CK_Person_Age", actual[0].Name, StringComparer.Ordinal);
                StringAssert.Contains(actual[0].Expression, "Age", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetCheckConstraintsAsync("Person")).Single();

                // Assert
                Assert.AreEqual("CK_Person_Age", actual.Name, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetTables

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTables()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTables().ToList();

                // Assert
                CollectionAssert.Contains(actual, "Person");
                CollectionAssert.Contains(actual, "Country");
                CollectionAssert.Contains(reader.GetTables(Database.SourceSalesName).ToList(), $"{Database.SourceSalesName}.Invoice");
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTables(Database.SourceSalesName).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item" }, actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTables("MissingSchema");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetTablesAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetTablesAsync(Database.SourceSalesName)).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item" }, actual);
            }
        }

        #endregion

        #endregion

        #region GetDependencyOrder

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrder()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderOfPersonAndCountry()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "Person", "Country" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderIgnoresTheTablesThatAreNotInTheList()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "GrandChild" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "GrandChild" }, actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderWithCircularReferences()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "CycleA", "CycleB" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "CycleA", "CycleB" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(await reader.GetDependencyOrderAsync(new[] { "GrandChild", "Parent", "Child" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsOfDependencyChain()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }).ToList();
                var parent = actual[0];
                var child = actual[1];
                var grandChild = actual[2];

                // Assert
                Assert.AreEqual(0, parent.Parents.Count);
                CollectionAssert.AreEqual(new[] { "Child" }, parent.Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" }, child.Parents.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" }, child.Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Child" }, grandChild.Parents.Select(r => r.Schema.Table.Name).ToArray());
                Assert.AreEqual(0, grandChild.Children.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsShareTheSameInstances()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Child", "Parent" }).ToList();

                // Assert
                Assert.AreSame(actual[0], actual[1].Parents.Single());
                Assert.AreSame(actual[1], actual[0].Children.Single());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsHaveTheSchemaOfTheTable()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Person", "Country" }).ToList();
                var person = actual.Single(r => r.Schema.Table.Name == "Person");

                // Assert
                Assert.AreEqual(7, person.Schema.Columns.Count);
                Assert.AreEqual(1, person.Schema.ForeignKeys.Count);
                Assert.IsNotNull(person.Schema.PrimaryKey);
                CollectionAssert.AreEqual(new[] { "Country" }, person.Parents.Select(r => r.Schema.Table.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsOfTableWithMultipleParents()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Shipment", "OrderLine", "Country" }).ToList();
                var shipment = actual.Single(r => r.Schema.Table.Name == "Shipment");

                // Assert
                Assert.AreEqual("Shipment", actual[2].Schema.Table.Name, StringComparer.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "OrderLine", "Country" }, shipment.Parents.Select(r => r.Schema.Table.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsIgnoreTheSelfReference()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Employee" }).Single();

                // Assert
                Assert.AreEqual(0, actual.Parents.Count);
                Assert.AreEqual(0, actual.Children.Count);
                Assert.AreEqual(1, actual.Schema.ForeignKeys.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderRelationshipsOfCircularReferences()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "CycleA", "CycleB" }).ToList();

                // Assert
                Assert.AreSame(actual[1], actual[0].Parents.Single());
                Assert.AreSame(actual[1], actual[0].Children.Single());
                Assert.AreSame(actual[0], actual[1].Parents.Single());
                Assert.AreSame(actual[0], actual[1].Children.Single());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderBreaksTheCycleAtTheTableThatWasGivenFirst()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "Ledger", "CycleA", "CycleB", $"{Database.SourceSalesName}.Invoice" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "CycleA", "CycleB", $"{Database.SourceSalesName}.Invoice", "Ledger" }, actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "Parent", "Parent", "`Parent`" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent" }, actual);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderAsyncRelationshipsOfDependencyChain()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetDependencyOrderAsync(new[] { "GrandChild", "Parent", "Child" })).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { "Child" }, actual[0].Children.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" }, actual[1].Parents.Select(r => r.Schema.Table.Name).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" }, actual[1].Children.Select(r => r.Schema.Table.Name).ToArray());
                Assert.AreEqual(0, actual[2].Children.Count);
            }
        }

        #endregion

        #endregion

        #region GetTableSchema

        #region Sync

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTableSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("Person");

                // Assert
                Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(1, actual.ForeignKeys.Count);
                Assert.AreEqual(0, actual.UniqueConstraints.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual("Invoice", actual.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual(Database.SourceSalesName, actual.Table.Schema, StringComparer.Ordinal);
                Assert.AreEqual(2, actual.Columns.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("NoKey");

                // Assert
                Assert.AreEqual(2, actual.Columns.Count);
                Assert.IsNull(actual.PrimaryKey);
                Assert.AreEqual(0, actual.Indexes.Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = await reader.GetTableSchemaAsync("Person");

                // Assert
                Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(1, actual.ForeignKeys.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var expected = reader.GetTableSchema("Person");
                var actual = await reader.GetTableSchemaAsync("Person");

                // Assert
                Helper.AssertSchemaEquality(expected, actual);
            }
        }

        #endregion

        #endregion

        #region Transaction

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderWithTransaction()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection, transaction);

                // Act
                var actual = reader.GetTableSchema("Person");

                // Assert
                Assert.AreEqual(7, actual.Columns.Count);
            }
        }

        #endregion
    }
}
