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
    [TestClass]
    public class ClickHouseSchemaReaderTest
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
        public void TestClickHouseSchemaReaderTableExists()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.TableExists($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.TableExists("`Person`");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Invoice");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.TableExists("MissingTable");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderTableExistsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = await reader.TableExistsAsync("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public async Task TestClickHouseSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetSchemaName()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("Person");

                // Assert
                Assert.IsNull(actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual(Database.SourceSalesName, actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual(Database.SourceSalesName, actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("MissingTable");

                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Name");

                // Assert
                Assert.AreEqual("String", actual.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(typeof(string), actual.Field.Type);
                                Assert.IsFalse(actual.Field.IsNullable);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("NoKey").ToList();

                // Assert
                Assert.AreEqual(0, columns.Single(c => c.Field.Name == "Value").Field.Size);
                Assert.AreEqual(0, columns.Single(c => c.Field.Name == "Payload").Field.Size);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();
                var age = columns.Single(c => c.Field.Name == "Age");
                var createdDate = columns.Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("0", age.DefaultExpression, StringComparer.Ordinal);
                Assert.IsTrue(age.Field.HasDefaultValue);
                StringAssert.Contains(createdDate.DefaultExpression, "now64(3)", StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var salary = reader.GetColumns("Person").Single(c => c.Field.Name == "Salary");
                var createdDate = reader.GetColumns("Person").Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("Decimal", salary.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)18, salary.Field.Precision);
                Assert.AreEqual((byte)2, salary.Field.Scale);
                Assert.AreEqual("DateTime64", createdDate.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)3, createdDate.Field.Scale);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("OrderLine").ToList();

                // Assert
                Assert.IsTrue(columns.Single(c => c.Field.Name == "OrderId").Field.IsPrimary);
                Assert.IsTrue(columns.Single(c => c.Field.Name == "LineNumber").Field.IsPrimary);
                Assert.IsFalse(columns.Single(c => c.Field.Name == "Quantity").Field.IsPrimary);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("MissingTable");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetColumnsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetPrimaryKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("OrderLine");

                // Assert
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("NoKey");

                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("IX_Person_Name", actual[0].Name, StringComparer.Ordinal);
                Assert.IsFalse(actual[0].IsUnique);
                CollectionAssert.AreEqual(new[] { "Name", "Id" }, actual[0].Columns.ToArray());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetIndexesAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Person")).ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                CollectionAssert.AreEqual(new[] { "Name", "Id" }, actual[0].Columns.ToArray());
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

        [TestMethod]
        public void TestClickHouseSchemaReaderGetCheckConstraints()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("CK_Person_Age", actual[0].Name, StringComparer.Ordinal);
                StringAssert.Contains(actual[0].Expression, "Age", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public void TestClickHouseSchemaReaderGetTables()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetTables().ToList();

                // Assert
                CollectionAssert.Contains(actual, "Person");
                CollectionAssert.Contains(actual, "Country");
                CollectionAssert.Contains(reader.GetTables(Database.SourceSalesName).ToList(), $"{Database.SourceSalesName}.Invoice");
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetTables(Database.SourceSalesName).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item", $"{Database.SourceSalesName}.ItemRef" }, actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetTables("MissingSchema");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetTablesAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = (await reader.GetTablesAsync(Database.SourceSalesName)).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Item", $"{Database.SourceSalesName}.ItemRef" }, actual);
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

        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("Person");

                // Assert
                Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(0, actual.ForeignKeys.Count);
                Assert.AreEqual(0, actual.UniqueConstraints.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema($"{Database.SourceSalesName}.Invoice");

                // Assert
                Assert.AreEqual("Invoice", actual.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual(Database.SourceSalesName, actual.Table.Schema, StringComparer.Ordinal);
                Assert.AreEqual(2, actual.Columns.Count);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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
        public async Task TestClickHouseSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = await reader.GetTableSchemaAsync("Person");

                // Assert
                Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(0, actual.ForeignKeys.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

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

        #endregion
    }
}
