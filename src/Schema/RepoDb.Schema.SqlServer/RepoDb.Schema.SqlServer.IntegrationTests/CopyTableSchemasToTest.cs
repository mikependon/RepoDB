#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema;
using RepoDb.Schema.Models;
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
    public class CopyTableSchemasToTest
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

        private static void MapSchemaReaderConnection(SqlConnection source) =>
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

        private static List<string> GetSourceTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new SqlServerSchemaReader(connection);
                return reader.GetTables()
                    .Where(name =>
                    {
                        var schema = reader.GetTableSchema(name);
                        return schema.Table.Schema == "dbo" && schema.ForeignKeys.All(foreignKey => foreignKey.ReferencedTable.Schema == "dbo");
                    })
                    .ToList();
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

        #region CopyTableSchemasTo

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemasTo()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                var run = source.CopyTableSchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.Target.Table.Table.Name == "Person");
                Assert.AreEqual("RepoDb_Schema_Source", person.Source.Database, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", person.Target.Database, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), person.Source.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), person.Target.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, person.Action);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
                StringAssert.Contains(person.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                StringAssert.Contains(person.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(run.EndTime >= run.StartTime);
                StringAssert.Contains(results.Single(r => r.Target.Table.Table.Name == "Country").Script, "CREATE TABLE [dbo].[Country]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var reported = new List<(string Table, List<string> TargetTables)>();

                // Act
                source.CopyTableSchemaTo(
                    new[] { "Person", "Country", "NoKey" },
                    target,
                    createdCallback: r => reported.Add((r.Target.Table.Table.Name, GetTargetTables())));

                // Assert
                Assert.AreEqual(3, reported.Count);
                Assert.AreEqual("Person", reported.Last().Table, StringComparer.Ordinal);
                Assert.AreEqual(3, reported.Last().TargetTables.Count);
                CollectionAssert.DoesNotContain(reported.First().TargetTables, "dbo.Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                Assert.AreEqual(tables.Count, results.Select(r => Helper.FormatName(r.Source.Table.Table.Schema, r.Target.Table.Table.Name)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackResultOfEachTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.Target.Table.Table.Name).ToArray());
                var person = results[1];
                Assert.AreEqual("dbo", person.Source.Table.Table.Schema, StringComparer.Ordinal);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(7, person.ColumnCount);
                Assert.AreEqual(1, person.IndexCount);
                Assert.AreEqual(1, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual(0, person.UniqueConstraintCount);
                StringAssert.StartsWith(person.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                Assert.AreEqual(1, results[0].UniqueConstraintCount);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfDependencyChainInAnyOrder()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "GrandChild", "Parent", "Child" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, results.Select(t => t.Target.Table.Table.Name).ToArray());
                AssertTargetMatchesSource("Parent", "Child", "GrandChild");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfDiamond()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "DiamondD", "DiamondC", "DiamondB", "DiamondA" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual("DiamondA", results[0].Target.Table.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", results[3].Target.Table.Table.Name, StringComparer.Ordinal);
                AssertTargetMatchesSource("DiamondA", "DiamondB", "DiamondC", "DiamondD");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesThatReferenceEachOther()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("CycleA", "CycleB");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfCycleOfThreeTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "RingZ", "RingX", "RingY" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "LoopLeaf", "LoopB", "LoopA", "LoopRoot" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, results.Select(t => t.Target.Table.Table.Name).ToArray());
                AssertTargetMatchesSource("LoopRoot", "LoopA", "LoopB", "LoopLeaf");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithMultipleIndexesAndKeys()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Product", "Shipment", "OrderLine", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Product", "Shipment", "OrderLine", "Country");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesInDifferentSchemas()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "dbo.Ledger", "Sales.InvoiceLine", "Sales.Invoice" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual("Invoice", results[0].Target.Table.Table.Name, StringComparer.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "Sales", "Sales", "dbo" }, results.Select(r => r.Source.Table.Table.Schema).ToArray());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "dbo"));
                CollectionAssert.AreEquivalent(new[] { "dbo.Invoice", "dbo.InvoiceLine", "dbo.Ledger" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToOfTablesWithTheSameNameInDifferentSchemas()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "dbo.ItemRef", "dbo.Item", "Sales.Item" }, target));
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "[dbo].[Odd.Child]", "[dbo].[Odd.Name]", "[Order Details]", "[dbo].[Weird]]Name]" }, target);

                // Assert
                AssertTargetMatchesSource("[dbo].[Odd.Name]", "[dbo].[Odd.Child]", "[Order Details]", "[dbo].[Weird]]Name]");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabaseInTheReverseOrder()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                source.CopyTableSchemaTo(tables.AsEnumerable().Reverse().ToList(), target);

                // Assert
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabaseCountsTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                var schemas = Helper.GetSourceSchemas(tables);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(schemas.Sum(s => s.Columns.Count), results.Sum(t => t.ColumnCount));
                Assert.AreEqual(schemas.Sum(s => s.Indexes.Count), results.Sum(t => t.IndexCount));
                Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count), results.Sum(t => t.ForeignKeyCount));
                Assert.AreEqual(schemas.Sum(s => s.UniqueConstraints.Count), results.Sum(t => t.UniqueConstraintCount));
                Assert.AreEqual(schemas.Sum(s => s.CheckConstraints.Count), results.Sum(t => t.CheckConstraintCount));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTheTableThatIsGivenMoreThanOnce()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Country", "dbo.Country", "[dbo].[Country]" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                AssertTargetMatchesSource("Country");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithRequestedExistsBehavior()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "NoKey" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, results.Single().Action);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTransaction()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(new[] { "Country", "Person", "CycleA", "CycleB" }, target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTransactionThatIsCommitted()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExistLeavesOutTheForeignKey()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Shipment" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results[0].Warnings.Count > 0);
                Assert.AreEqual(0, results[0].Errors.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" }, target);

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "NoKey" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyTableSchemaTo(new[] { "MissingTable" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsBlank()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "Country", " " }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTableNamesAreNull()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyTableSchemaTo((string)null, target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsync()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.Target.Table.Table.Name).ToArray());
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncOfTablesThatReferenceEachOther()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "RingZ", "RingX", "RingY", "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ", "CycleA", "CycleB");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallbackResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Align, results[1].Action);
                Assert.AreEqual("RepoDb_Schema_Source", results[1].Source.Database, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", results[1].Target.Database, StringComparer.Ordinal);
                Assert.AreEqual(7, results[1].ColumnCount);
                StringAssert.Contains(results[1].Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync(new[] { "NoKey" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopyTableSchemaToAsync(new[] { "Country" }, target));
            }
        }

        #endregion

        #region ErrorCallback

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<ExecutionError>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<ExecutionError>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType<SqlException>(errors[0].Exception);
                Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
                Assert.AreEqual("dbo", errors[0].SchemaName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "ALTER TABLE [dbo].[Person] ADD CONSTRAINT [FK_Person_Country]", StringComparison.Ordinal);
                Assert.AreEqual(2, errors[0].StatementIndex);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithErrorCallbackKeepsTheObjectsThatWereCreated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var created = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(
                    new[] { "Person" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: _ => { });

                // Assert
                Assert.AreEqual(1, created.Count);
                Assert.AreEqual(1, created[0].Errors.Count);
                Assert.IsInstanceOfType<SqlException>(created[0].Errors[0].Exception);
                Assert.AreEqual(CopyTableSchemaOutcome.Failed, created[0].Outcome);
                var person = Helper.GetTargetSchema("Person");
                Assert.AreEqual(7, person.Columns.Count);
                Assert.AreEqual(1, person.Indexes.Count);
                Assert.AreEqual(0, person.ForeignKeys.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);
                var created = new List<CopyTableSchemaResultItem>();
                var errors = new List<ExecutionError>();

                // Act
                source.CopyTableSchemaTo(
                    new[] { "NoKey", "Parent", "Child" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("NoKey", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "CREATE TABLE [dbo].[NoKey]", StringComparison.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "NoKey", "Parent", "Child" }, created.Select(r => r.Target.Table.Table.Name).ToArray());
                Assert.AreSame(errors[0], created.Single(r => r.Target.Table.Table.Name == "NoKey").Errors.Single());
                Assert.AreEqual(CopyTableSchemaOutcome.Failed, created.Single(r => r.Target.Table.Table.Name == "NoKey").Outcome);
                Assert.AreEqual(0, created.Where(r => r.Target.Table.Table.Name != "NoKey").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("NoKey", "Parent", "Child");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                source.CopyTableSchemaTo(tables, target);
                var errors = new List<ExecutionError>();
                var created = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(
                    tables,
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is SqlException));
                Assert.AreEqual(tables.Count, created.Count);
                Assert.IsTrue(created.All(r => r.Errors.Count > 0 && r.Outcome == CopyTableSchemaOutcome.Failed));
                Assert.AreEqual(errors.Count, created.Sum(r => r.Errors.Count));
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);
                var created = new List<string>();

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopyTableSchemaTo(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        createdCallback: r => created.Add(r.Target.Table.Table.Name),
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);
                ExecutionError raised = null;

                // Act
                var exception = Assert.Throws<SqlException>(() =>
                    source.CopyTableSchemaTo(new[] { "NoKey" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopyTableSchemaTo(new[] { "NoKey" }, target, errorCallback: e => throw new NotSupportedException()));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithErrorCallbackInTransactionThatTheServerRollsBack()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(
                        new[] { "NoKey", "Parent" },
                        target,
                        errorCallback: _ => { },
                        transaction: transaction);
                    Assert.Throws<InvalidOperationException>(() => transaction.Commit());
                }

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var errors = 0;

                // Act/Assert
                Assert.Throws<MissingMappingException>(() =>
                    source.CopyTableSchemaTo(new[] { "Country" }, target, errorCallback: e => errors++));
                Assert.AreEqual(0, errors);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<ExecutionError>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType<SqlException>(errors[0].Exception);
                Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "ALTER TABLE [dbo].[Person]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" }, target);
                var created = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(
                    new[] { "NoKey", "Parent", "Child" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: _ => { });

                // Assert
                CollectionAssert.AreEquivalent(new[] { "NoKey", "Parent", "Child" }, created.Select(r => r.Target.Table.Table.Name).ToArray());
                Assert.AreEqual(1, created.Single(r => r.Target.Table.Table.Name == "NoKey").Errors.Count);
                Assert.AreEqual(0, created.Where(r => r.Target.Table.Table.Name != "NoKey").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("NoKey", "Parent", "Child");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    source.CopyTableSchemaToAsync(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" }, target);
                ExecutionError raised = null;

                // Act
                var exception = await Assert.ThrowsAsync<SqlException>(() =>
                    source.CopyTableSchemaToAsync(new[] { "NoKey" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                await source.CopyTableSchemaToAsync(tables, target);

                // Act
                await source.CopyTableSchemaToAsync(tables, target, errorCallback: _ => { });

                // Assert
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
