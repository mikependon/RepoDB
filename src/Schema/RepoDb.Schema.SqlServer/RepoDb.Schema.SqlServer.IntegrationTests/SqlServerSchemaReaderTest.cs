#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaReaderTest
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
        public void TestSqlServerSchemaReaderTableExists()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderTableExistsWithSchemaName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.TableExists("Sales.Invoice");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderTableExistsWithQuotedName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.TableExists("[dbo].[Person]");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.TableExists("dbo.Invoice");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderTableExistsWithMissingTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.TableExists("MissingTable");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderTableExistsAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = await reader.TableExistsAsync("Person");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public async Task TestSqlServerSchemaReaderTableExistsAsyncWithMissingTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetSchemaName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("Person");

                // Assert
                Assert.AreEqual("dbo", actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetSchemaNameOfTableInAnotherSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("Invoice");

                // Assert
                Assert.AreEqual("Sales", actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetSchemaNameWithExplicitSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("Sales.Invoice");

                // Assert
                Assert.AreEqual("Sales", actual, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetSchemaName("MissingTable");

                // Assert
                Assert.AreEqual("dbo", actual, StringComparer.Ordinal);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetSchemaNameAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = await reader.GetSchemaNameAsync("Invoice");

                // Assert
                Assert.AreEqual("Sales", actual, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetColumns

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetColumnsOfIdentityColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Id");

                // Assert
                Assert.IsTrue(actual.Field.IsIdentity);
                Assert.IsTrue(actual.Field.IsPrimary);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("bigint", actual.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(typeof(long), actual.Field.Type);
                Assert.AreEqual(10L, actual.IdentitySeed);
                Assert.AreEqual(5L, actual.IdentityIncrement);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetColumnsOfStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("Person").Single(c => c.Field.Name == "Name");

                // Assert
                Assert.AreEqual("nvarchar", actual.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(typeof(string), actual.Field.Type);
                Assert.AreEqual(128, actual.Field.Size);
                Assert.IsFalse(actual.Field.IsNullable);
                Assert.AreEqual("Latin1_General_CI_AS", actual.Collation, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfMaxSizeColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("NoKey").ToList();

                // Assert
                Assert.AreEqual(-1, columns.Single(c => c.Field.Name == "Value").Field.Size);
                Assert.AreEqual(-1, columns.Single(c => c.Field.Name == "Payload").Field.Size);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfColumnWithComment()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();

                // Assert
                Assert.AreEqual("The name of the person.", columns.Single(c => c.Field.Name == "Name").Comment, StringComparer.Ordinal);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Age").Comment);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("Person").ToList();
                var age = columns.Single(c => c.Field.Name == "Age");
                var createdDate = columns.Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("((0))", age.DefaultExpression, StringComparer.Ordinal);
                Assert.IsTrue(age.Field.HasDefaultValue);
                StringAssert.Contains(createdDate.DefaultExpression, "sysutcdatetime", StringComparison.OrdinalIgnoreCase);
                Assert.IsNull(columns.Single(c => c.Field.Name == "Name").DefaultExpression);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfComputedColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetColumnsOfNumericColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var salary = reader.GetColumns("Person").Single(c => c.Field.Name == "Salary");
                var createdDate = reader.GetColumns("Person").Single(c => c.Field.Name == "CreatedDateUtc");

                // Assert
                Assert.AreEqual("decimal", salary.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)18, salary.Field.Precision);
                Assert.AreEqual((byte)2, salary.Field.Scale);
                Assert.AreEqual("datetime2", createdDate.Field.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual((byte)3, createdDate.Field.Scale);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var columns = reader.GetColumns("OrderLine").ToList();

                // Assert
                Assert.IsTrue(columns.Single(c => c.Field.Name == "OrderId").Field.IsPrimary);
                Assert.IsTrue(columns.Single(c => c.Field.Name == "LineNumber").Field.IsPrimary);
                Assert.IsFalse(columns.Single(c => c.Field.Name == "Quantity").Field.IsPrimary);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetColumnsOfMissingTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetColumns("MissingTable");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetColumnsAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetColumnsAsync("Person")).ToList();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "Id", "Name", "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    Helper.GetNames(actual));
                Assert.AreEqual(10L, actual.Single(c => c.Field.Name == "Id").IdentitySeed);
            }
        }

        #endregion

        #endregion

        #region GetPrimaryKey

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual("PK_Person", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfCompositeKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("OrderLine");

                // Assert
                Assert.AreEqual("PK_OrderLine", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfTableWithoutKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("NoKey");

                // Assert
                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetPrimaryKeyAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = await reader.GetPrimaryKeyAsync("OrderLine");

                // Assert
                Assert.AreEqual("PK_OrderLine", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetIndexes

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexes()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("IX_Person_Name", actual[0].Name, StringComparer.Ordinal);
                Assert.IsFalse(actual[0].IsUnique);
                CollectionAssert.AreEqual(new[] { "Name" }, actual[0].Columns.ToArray());
                CollectionAssert.AreEqual(new[] { "Age" }, actual[0].IncludedColumns.ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesDoesNotIncludeThePrimaryKeyAndTheUniqueConstraints()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetIndexesAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Person")).ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                CollectionAssert.AreEqual(new[] { "Age" }, actual[0].IncludedColumns.ToArray());
            }
        }

        #endregion

        #endregion

        #region GetForeignKeys

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("FK_Person_Country", actual[0].Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" }, actual[0].Columns.ToArray());
                Assert.AreEqual("dbo.Country", actual[0].ReferencedTable, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual[0].ReferencedColumns.ToArray());
                Assert.AreEqual(ForeignKeyRule.Cascade, actual[0].UpdateRule);
                Assert.AreEqual(ForeignKeyRule.SetNull, actual[0].DeleteRule);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysDefaultRules()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Child").Single();

                // Assert
                Assert.AreEqual("FK_Child_Parent", actual.Name, StringComparer.Ordinal);
                Assert.AreEqual(ForeignKeyRule.NoAction, actual.UpdateRule);
                Assert.AreEqual(ForeignKeyRule.NoAction, actual.DeleteRule);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetForeignKeysAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetForeignKeysAsync("Person")).Single();

                // Assert
                Assert.AreEqual("FK_Person_Country", actual.Name, StringComparer.Ordinal);
                Assert.AreEqual("dbo.Country", actual.ReferencedTable, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetUniqueConstraints

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetUniqueConstraints()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetUniqueConstraints("Country").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("UQ_Country_Name", actual[0].Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Name" }, actual[0].Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetUniqueConstraints("Person");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetUniqueConstraintsAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetCheckConstraints()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Person").ToList();

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("CK_Person_Age", actual[0].Name, StringComparer.Ordinal);
                StringAssert.Contains(actual[0].Expression, "Age", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetCheckConstraintsOfTableWithoutCheckConstraints()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetCheckConstraints("Country");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetCheckConstraintsAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderGetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTables().ToList();

                // Assert
                CollectionAssert.Contains(actual, "dbo.Person");
                CollectionAssert.Contains(actual, "dbo.Country");
                CollectionAssert.Contains(actual, "Sales.Invoice");
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTablesOfSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTables("Sales").ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { "Sales.Invoice", "Sales.InvoiceLine", "Sales.Item" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTablesOfMissingSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTables("MissingSchema");

                // Assert
                Assert.AreEqual(0, actual.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetTablesAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetTablesAsync("Sales")).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { "Sales.Invoice", "Sales.InvoiceLine", "Sales.Item" }, actual);
            }
        }

        #endregion

        #endregion

        #region GetDependencyOrder

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrder()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderOfPersonAndCountry()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "dbo.Person", "Country" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.Country", "dbo.Person" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderIgnoresTheTablesThatAreNotInTheList()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "GrandChild" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.GrandChild" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderWithCircularReferences()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "CycleA", "CycleB" }));

                // Assert (the cycle is broken at the table that was given first)
                CollectionAssert.AreEqual(new[] { "dbo.CycleA", "dbo.CycleB" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetDependencyOrderAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(await reader.GetDependencyOrderAsync(new[] { "GrandChild", "Parent", "Child" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, actual);
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsOfDependencyChain()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }).ToList();
                var parent = actual[0];
                var child = actual[1];
                var grandChild = actual[2];

                // Assert
                Assert.AreEqual(0, parent.Parents.Count);
                CollectionAssert.AreEqual(new[] { "Child" }, parent.Children.Select(r => r.Table.TableName).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" }, child.Parents.Select(r => r.Table.TableName).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" }, child.Children.Select(r => r.Table.TableName).ToArray());
                CollectionAssert.AreEqual(new[] { "Child" }, grandChild.Parents.Select(r => r.Table.TableName).ToArray());
                Assert.AreEqual(0, grandChild.Children.Count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsShareTheSameInstances()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Child", "Parent" }).ToList();

                // Assert
                Assert.AreSame(actual[0], actual[1].Parents.Single());
                Assert.AreSame(actual[1], actual[0].Children.Single());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsHaveTheSchemaOfTheTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Person", "Country" }).ToList();
                var person = actual.Single(r => r.Table.TableName == "Person");

                // Assert
                Assert.AreEqual(7, person.Table.Columns.Count);
                Assert.AreEqual(1, person.Table.ForeignKeys.Count);
                Assert.IsNotNull(person.Table.PrimaryKey);
                CollectionAssert.AreEqual(new[] { "Country" }, person.Parents.Select(r => r.Table.TableName).ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsOfTableWithMultipleParents()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Shipment", "OrderLine", "Country" }).ToList();
                var shipment = actual.Single(r => r.Table.TableName == "Shipment");

                // Assert
                Assert.AreEqual("Shipment", actual[2].Table.TableName, StringComparer.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "OrderLine", "Country" }, shipment.Parents.Select(r => r.Table.TableName).ToArray());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsIgnoreTheSelfReference()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "Employee" }).Single();

                // Assert
                Assert.AreEqual(0, actual.Parents.Count);
                Assert.AreEqual(0, actual.Children.Count);
                Assert.AreEqual(1, actual.Table.ForeignKeys.Count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderRelationshipsOfCircularReferences()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetDependencyOrder(new[] { "CycleA", "CycleB" }).ToList();

                // Assert (each one is the parent and the child of the other one)
                Assert.AreSame(actual[1], actual[0].Parents.Single());
                Assert.AreSame(actual[1], actual[0].Children.Single());
                Assert.AreSame(actual[0], actual[1].Parents.Single());
                Assert.AreSame(actual[0], actual[1].Children.Single());
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderBreaksTheCycleAtTheTableThatWasGivenFirst()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act (the tables are kept in the order that they were given, as long as the tables that they reference come first)
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "Ledger", "CycleA", "CycleB", "Sales.Invoice" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.CycleA", "dbo.CycleB", "Sales.Invoice", "dbo.Ledger" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetDependencyOrderIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = Helper.GetTableNames(reader.GetDependencyOrder(new[] { "Parent", "dbo.Parent", "[dbo].[Parent]" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.Parent" }, actual);
            }
        }

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetDependencyOrderAsyncRelationshipsOfDependencyChain()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetDependencyOrderAsync(new[] { "GrandChild", "Parent", "Child" })).ToList();

                // Assert
                CollectionAssert.AreEqual(new[] { "Child" }, actual[0].Children.Select(r => r.Table.TableName).ToArray());
                CollectionAssert.AreEqual(new[] { "Parent" }, actual[1].Parents.Select(r => r.Table.TableName).ToArray());
                CollectionAssert.AreEqual(new[] { "GrandChild" }, actual[1].Children.Select(r => r.Table.TableName).ToArray());
                Assert.AreEqual(0, actual[2].Children.Count);
            }
        }

        #endregion

        #endregion

        #region GetTableSchema

        #region Sync

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTableSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("Person");

                // Assert
                Assert.AreEqual("Person", actual.TableName, StringComparer.Ordinal);
                Assert.AreEqual("dbo", actual.SchemaName, StringComparer.Ordinal);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(1, actual.ForeignKeys.Count);
                Assert.AreEqual(0, actual.UniqueConstraints.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTableSchemaOfTableInAnotherSchema()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("Invoice");

                // Assert
                Assert.AreEqual("Invoice", actual.TableName, StringComparer.Ordinal);
                Assert.AreEqual("Sales", actual.SchemaName, StringComparer.Ordinal);
                Assert.AreEqual(2, actual.Columns.Count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public async Task TestSqlServerSchemaReaderGetTableSchemaAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = await reader.GetTableSchemaAsync("Person");

                // Assert
                Assert.AreEqual("Person", actual.TableName, StringComparer.Ordinal);
                Assert.AreEqual("dbo", actual.SchemaName, StringComparer.Ordinal);
                Assert.AreEqual(7, actual.Columns.Count);
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.AreEqual(1, actual.ForeignKeys.Count);
                Assert.AreEqual(1, actual.CheckConstraints.Count);
            }
        }

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetTableSchemaAsyncIsTheSameAsTheSyncOne()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaReaderWithTransaction()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection, transaction);

                // Act
                var actual = reader.GetTableSchema("Person");

                // Assert
                Assert.AreEqual(7, actual.Columns.Count);
            }
        }

        #endregion
    }
}
