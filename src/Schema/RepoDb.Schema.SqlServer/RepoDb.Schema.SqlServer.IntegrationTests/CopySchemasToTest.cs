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
        public void TestCopySchemasToCallbackResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.TableName == "Person");
                Assert.AreEqual("RepoDb_Schema_Source", person.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", person.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), person.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), person.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaExistsBehavior.SkipOnExists, person.Action);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
                StringAssert.Contains(person.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                StringAssert.Contains(person.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(person.EndTime >= person.StartTime);
                StringAssert.Contains(results.Single(r => r.TableName == "Country").Script, "CREATE TABLE [dbo].[Country]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var reported = new List<(string Table, List<string> TargetTables)>();

                // Act (the tables that exist in the target when each schema is reported)
                source.CopySchemasTo(
                    new[] { "Person", "Country", "NoKey" },
                    target,
                    createdCallback: r => reported.Add((r.TableName, GetTargetTables())));

                // Assert (the Country and the NoKey do not wait for the foreign key of the Person)
                Assert.AreEqual(3, reported.Count);
                Assert.AreEqual("Person", reported.Last().Table, StringComparer.Ordinal);
                Assert.AreEqual(3, reported.Last().TargetTables.Count);
                CollectionAssert.DoesNotContain(reported.First().TargetTables, "dbo.Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(tables, target, createdCallback: results.Add);

                // Assert (every table is reported once)
                Assert.AreEqual(tables.Count, results.Count);
                Assert.AreEqual(tables.Count, results.Select(r => Helper.FormatName(r.SourceSchema, r.TableName)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallbackResultOfEachTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert (in the order that the tables were created)
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
                var person = results[1];
                Assert.AreEqual("dbo", person.SourceSchema, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
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
        public void TestCopySchemasToOfDependencyChainInAnyOrder()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "GrandChild", "Parent", "Child" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, results.Select(t => t.TableName).ToArray());
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "DiamondD", "DiamondC", "DiamondB", "DiamondA" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual("DiamondA", results[0].TableName, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", results[3].TableName, StringComparer.Ordinal);
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "LoopLeaf", "LoopB", "LoopA", "LoopRoot" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, results.Select(t => t.TableName).ToArray());
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "dbo.Ledger", "Sales.InvoiceLine", "Sales.Invoice" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual("Invoice", results[0].TableName, StringComparer.Ordinal);
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "dbo.ItemRef", "dbo.Item", "Sales.Item" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(3, results.Count);
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(tables, target, createdCallback: results.Add);

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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(schemas.Sum(s => s.Columns.Count), results.Sum(t => t.ColumnCount));
                Assert.AreEqual(schemas.Sum(s => s.Indexes.Count), results.Sum(t => t.IndexCount));
                Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count), results.Sum(t => t.ForeignKeyCount));
                Assert.AreEqual(schemas.Sum(s => s.UniqueConstraints.Count), results.Sum(t => t.UniqueConstraintCount));
                Assert.AreEqual(schemas.Sum(s => s.CheckConstraints.Count), results.Sum(t => t.CheckConstraintCount));
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "Country", "dbo.Country", "[dbo].[Country]" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(new[] { "NoKey" }, target, CopySchemaExistsBehavior.DropOnExists, results.Add);

                // Assert
                Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, results.Single().Action);
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
                var results = new List<CopySchemaResult>();
                source.CopySchemasTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
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
                var results = new List<CopySchemaResult>();
                await source.CopySchemasToAsync(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
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
                var results = new List<CopySchemaResult>();
                await source.CopySchemasToAsync(tables, target, createdCallback: results.Add);

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
        public async Task TestCopySchemasToAsyncCallbackResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);

                // Act
                var results = new List<CopySchemaResult>();
                await source.CopySchemasToAsync(new[] { "Person", "Country" }, target, CopySchemaExistsBehavior.AlignOnExists, results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(CopySchemaExistsBehavior.AlignOnExists, results[1].Action);
                Assert.AreEqual("RepoDb_Schema_Source", results[1].SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", results[1].DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(7, results[1].ColumnCount);
                StringAssert.Contains(results[1].Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
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
                var results = new List<CopySchemaResult>();
                await source.CopySchemasToAsync(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
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

        #region ErrorCallback

        #region Sync

        [TestMethod]
        public void TestCopySchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemasTo(new[] { "Country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup (the Person references the Country, which does not exist in the target)
                MapReader(source);
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemasTo(new[] { "Person" }, target, errorCallback: errors.Add);

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
        public void TestCopySchemasToWithErrorCallbackKeepsTheObjectsThatWereCreated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var created = new List<CopySchemaResult>();

                // Act (the foreign key of the Person fails, but its table and its index are created)
                source.CopySchemasTo(
                    new[] { "Person" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: _ => { });

                // Assert (the Person is reported with the error of its foreign key)
                Assert.AreEqual(1, created.Count);
                Assert.AreEqual(1, created[0].Errors.Count);
                Assert.IsInstanceOfType<SqlException>(created[0].Errors[0].Exception);
                Assert.AreEqual(CopySchemaOutcome.Failed, created[0].Outcome);
                var person = Helper.GetTargetSchema("Person");
                Assert.AreEqual(7, person.Columns.Count);
                Assert.AreEqual(1, person.Indexes.Count);
                Assert.AreEqual(0, person.ForeignKeys.Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackContinuesWithTheOtherTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup (the NoKey table already exists in the target)
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);
                var created = new List<CopySchemaResult>();
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemasTo(
                    new[] { "NoKey", "Parent", "Child" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert (the error is in the result of the NoKey only)
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("NoKey", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "CREATE TABLE [dbo].[NoKey]", StringComparison.Ordinal);
                CollectionAssert.AreEquivalent(new[] { "NoKey", "Parent", "Child" }, created.Select(r => r.TableName).ToArray());
                Assert.AreSame(errors[0], created.Single(r => r.TableName == "NoKey").Errors.Single());
                Assert.AreEqual(CopySchemaOutcome.Failed, created.Single(r => r.TableName == "NoKey").Outcome);
                Assert.AreEqual(0, created.Where(r => r.TableName != "NoKey").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("NoKey", "Parent", "Child");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();
                source.CopySchemasTo(tables, target);
                var errors = new List<CopySchemaError>();
                var created = new List<CopySchemaResult>();

                // Act (everything already exists, so every statement fails)
                source.CopySchemasTo(
                    tables,
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert (every table is reported with its errors, and every error is in the result of a table)
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is SqlException));
                Assert.AreEqual(tables.Count, created.Count);
                Assert.IsTrue(created.All(r => r.Errors.Count > 0 && r.Outcome == CopySchemaOutcome.Failed));
                Assert.AreEqual(errors.Count, created.Sum(r => r.Errors.Count));
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup (the NoKey table already exists in the target, and it is the first one that is created)
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);
                var created = new List<string>();

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopySchemasTo(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        createdCallback: r => created.Add(r.TableName),
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert (nothing else is created)
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);
                CopySchemaError raised = null;

                // Act
                var exception = Assert.Throws<SqlException>(() =>
                    source.CopySchemasTo(new[] { "NoKey" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert (the exception that was captured is the one that is thrown)
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopySchemasTo(new[] { "NoKey" }, target, errorCallback: e => throw new NotSupportedException()));
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackInTransactionThatTheServerRollsBack()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapReader(source);
                source.CopySchemasTo(new[] { "NoKey" }, target);

                // Act (SQL Server rolls back the transaction itself when the creation of the existing table fails, so skipping the error does not save it)
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemasTo(
                        new[] { "NoKey", "Parent" },
                        target,
                        errorCallback: _ => { },
                        transaction: transaction);
                    Assert.Throws<InvalidOperationException>(() => transaction.Commit());
                }

                // Assert (nothing of the transaction is kept)
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup (the errors of reading the schemas are not reported to the error callback)
                var errors = 0;

                // Act/Assert
                Assert.Throws<MissingMappingException>(() =>
                    source.CopySchemasTo(new[] { "Country" }, target, errorCallback: e => errors++));
                Assert.AreEqual(0, errors);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var errors = new List<CopySchemaError>();

                // Act
                await source.CopySchemasToAsync(new[] { "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType<SqlException>(errors[0].Exception);
                Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "ALTER TABLE [dbo].[Person]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithErrorCallbackContinuesWithTheOtherTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                await source.CopySchemasToAsync(new[] { "NoKey" }, target);
                var created = new List<CopySchemaResult>();

                // Act
                await source.CopySchemasToAsync(
                    new[] { "NoKey", "Parent", "Child" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: _ => { });

                // Assert
                CollectionAssert.AreEquivalent(new[] { "NoKey", "Parent", "Child" }, created.Select(r => r.TableName).ToArray());
                Assert.AreEqual(1, created.Single(r => r.TableName == "NoKey").Errors.Count);
                Assert.AreEqual(0, created.Where(r => r.TableName != "NoKey").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("NoKey", "Parent", "Child");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                await source.CopySchemasToAsync(new[] { "NoKey" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    source.CopySchemasToAsync(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                await source.CopySchemasToAsync(new[] { "NoKey" }, target);
                CopySchemaError raised = null;

                // Act
                var exception = await Assert.ThrowsAsync<SqlException>(() =>
                    source.CopySchemasToAsync(new[] { "NoKey" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapReader(source);
                var tables = GetSourceTables();
                await source.CopySchemasToAsync(tables, target);

                // Act
                await source.CopySchemasToAsync(tables, target, errorCallback: _ => { });

                // Assert
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
