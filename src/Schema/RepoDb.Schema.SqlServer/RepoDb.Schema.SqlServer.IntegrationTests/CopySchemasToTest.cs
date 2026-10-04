#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class CopySchemasToTest
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
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Helpers

        // The reader owns the connection it reads from, so it is mapped for the connection that is used as the source of the copy.
        private static void MapReader(SqlConnection source) =>
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

        private static List<string> GetSourceTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        private static void AssertTargetMatchesSource(params string[] tableNames)
        {
            foreach (var tableName in tableNames)
            {
                Helper.AssertTargetMatchesSource(tableName);
            }
        }

        #endregion

        #region CopySchemasTo

        #region Sync

        [TestMethod]
        public void TestCopySchemasTo()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                source.CopySchemasTo(new[] { "Country", "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act (copying the tables one by one in this order fails, as the table references one that does not exist yet)
                source.CopySchemasTo(new[] { "Person", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "Person", "Country" }, target);

                // Assert
                Assert.AreEqual(2, result.TableCount);
                Assert.AreEqual("RepoDb_Schema_Source", result.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", result.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaExistsBehavior.SkipOnExists, result.Action);
                StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Country]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(result.EndTime >= result.StartTime);
            }
        }

        [TestMethod]
        public void TestCopySchemasToResultOfEachTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "Person", "Country" }, target);

                // Assert (in the order that the tables were created)
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
                var person = result.Tables[1];
                Assert.AreEqual("dbo", person.SourceSchema, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(7, person.ColumnCount);
                Assert.AreEqual(1, person.IndexCount);
                Assert.AreEqual(1, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual(0, person.UniqueConstraintCount);
                StringAssert.StartsWith(person.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                Assert.AreEqual(1, result.Tables[0].UniqueConstraintCount);
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfDependencyChainInAnyOrder()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "GrandChild", "Parent", "Child" }, target);

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, result.Tables.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("Parent", "Child", "GrandChild");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfDiamond()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "DiamondD", "DiamondC", "DiamondB", "DiamondA" }, target);

                // Assert
                Assert.AreEqual("DiamondA", result.Tables[0].TableName, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", result.Tables[3].TableName, StringComparer.Ordinal);
                AssertTargetMatchesSource("DiamondA", "DiamondB", "DiamondC", "DiamondD");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesThatReferenceEachOther()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act (copying the tables one by one fails, as each one references the other)
                source.CopySchemasTo(new[] { "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("CycleA", "CycleB");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfCycleOfThreeTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                source.CopySchemasTo(new[] { "RingZ", "RingX", "RingY" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "LoopLeaf", "LoopB", "LoopA", "LoopRoot" }, target);

                // Assert
                CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, result.Tables.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("LoopRoot", "LoopA", "LoopB", "LoopLeaf");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesWithMultipleIndexesAndKeys()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                source.CopySchemasTo(new[] { "Product", "Shipment", "OrderLine", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Product", "Shipment", "OrderLine", "Country");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesInDifferentSchemas()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "dbo.Ledger", "Sales.InvoiceLine", "Sales.Invoice" }, target);

                // Assert
                Assert.AreEqual("Invoice", result.Tables[0].TableName, StringComparer.Ordinal);
                AssertTargetMatchesSource("Sales.Invoice", "Sales.InvoiceLine", "dbo.Ledger");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesWithTheSameNameInDifferentSchemas()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "dbo.ItemRef", "dbo.Item", "Sales.Item" }, target);

                // Assert
                Assert.AreEqual(3, result.TableCount);
                AssertTargetMatchesSource("dbo.Item", "Sales.Item", "dbo.ItemRef");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                source.CopySchemasTo(new[] { "[dbo].[Odd.Child]", "[dbo].[Odd.Name]", "[Order Details]", "[dbo].[Weird]]Name]" }, target);

                // Assert
                AssertTargetMatchesSource("[dbo].[Odd.Name]", "[dbo].[Odd.Child]", "[Order Details]", "[dbo].[Weird]]Name]");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();

                // Act
                var result = source.CopySchemasTo(tables, target);

                // Assert
                Assert.AreEqual(tables.Count, result.TableCount);
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTheWholeDatabaseInTheReverseOrder()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();

                // Act
                source.CopySchemasTo(tables.AsEnumerable().Reverse().ToList(), target);

                // Assert
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTheWholeDatabaseCountsTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();
                var schemas = Helper.GetSourceSchemas(tables);

                // Act
                var result = source.CopySchemasTo(tables, target);

                // Assert
                Assert.AreEqual(schemas.Sum(s => s.Columns.Count), result.Tables.Sum(t => t.ColumnCount));
                Assert.AreEqual(schemas.Sum(s => s.Indexes.Count), result.Tables.Sum(t => t.IndexCount));
                Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count), result.Tables.Sum(t => t.ForeignKeyCount));
                Assert.AreEqual(schemas.Sum(s => s.UniqueConstraints.Count), result.Tables.Sum(t => t.UniqueConstraintCount));
                Assert.AreEqual(schemas.Sum(s => s.CheckConstraints.Count), result.Tables.Sum(t => t.CheckConstraintCount));
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTheTableThatIsGivenMoreThanOnce()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "Country", "dbo.Country", "[dbo].[Country]" }, target);

                // Assert
                Assert.AreEqual(1, result.TableCount);
                AssertTargetMatchesSource("Country");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithRequestedExistsBehavior()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(new[] { "NoKey" }, target, CopySchemaExistsBehavior.DropOnExists);

                // Assert
                Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, result.Action);
                Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, result.Tables.Single().Action);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithoutTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = source.CopySchemasTo(Enumerable.Empty<string>(), target);

                // Assert
                Assert.AreEqual(0, result.TableCount);
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTransaction()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapReader(source);

                // Act (the copy of all the tables is undone as a whole)
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemasTo(new[] { "Country", "Person", "CycleA", "CycleB" }, target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTransactionThatIsCommitted()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapReader(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemasTo(new[] { "Country", "Person" }, target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act/Assert (the Shipment table references the Country and the OrderLine tables)
                Assert.Throws<SqlException>(() => source.CopySchemasTo(new[] { "Shipment" }, target));
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                source.CopySchemasTo(new[] { "Country" }, target);

                // Act
                source.CopySchemasTo(new[] { "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);

                // Act/Assert (the existence behavior is not enforced yet, so the creation of the existing table fails)
                Assert.Throws<SqlException>(() => source.CopySchemasTo(new[] { "NoKey" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act/Assert (a table without columns cannot be created)
                Assert.Throws<SqlException>(() => source.CopySchemasTo(new[] { "MissingTable" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopySchemasTo(new[] { "Country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsBlank()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopySchemasTo(new[] { "Country", " " }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTableNamesAreNull()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopySchemasTo(null, target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsync()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = await source.CopySchemasToAsync(new[] { "Person", "Country" }, target);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncOfTablesThatReferenceEachOther()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                await source.CopySchemasToAsync(new[] { "RingZ", "RingX", "RingY", "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ", "CycleA", "CycleB");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();

                // Act
                var result = await source.CopySchemasToAsync(tables, target);

                // Assert
                Assert.AreEqual(tables.Count, result.TableCount);
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = await source.CopySchemasToAsync(new[] { "Person", "Country" }, target, CopySchemaExistsBehavior.AlignOnExists);

                // Assert
                Assert.AreEqual(2, result.TableCount);
                Assert.AreEqual(CopySchemaExistsBehavior.AlignOnExists, result.Action);
                Assert.AreEqual("RepoDb_Schema_Source", result.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", result.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(7, result.Tables[1].ColumnCount);
                StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var result = await source.CopySchemasToAsync(Enumerable.Empty<string>(), target);

                // Assert
                Assert.AreEqual(0, result.TableCount);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                await source.CopySchemasToAsync(new[] { "NoKey" }, target);

                // Act/Assert (the existence behavior is not enforced yet, so the creation of the existing table fails)
                await Assert.ThrowsAsync<SqlException>(() => source.CopySchemasToAsync(new[] { "NoKey" }, target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopySchemasToAsync(new[] { "Country" }, target));
            }
        }

        #endregion

        #endregion
    }
}
