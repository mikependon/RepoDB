#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests
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

        private static void MapSchemaReaderConnection(SqliteConnection source) =>
            SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

        private static List<string> GetSourceTables()
        {
            using (var connection = Database.CreateSource())
            {
                var reader = new TursoSchemaReader(connection);
                return reader.GetTables()
                    .Where(name =>
                    {
                        var schema = reader.GetTableSchema(name);
                        return schema.Table.Schema == null && schema.ForeignKeys.All(foreignKey => foreignKey.ReferencedTable.Schema == null);
                    })
                    .ToList();
            }
        }

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader(connection).GetTables(schemaName).ToList();
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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Person", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallbackResult()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.TableName == "Person");
                Assert.AreEqual("main", person.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("main", person.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqliteConnection), person.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqliteConnection), person.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaExistsBehavior.Skip, person.Action);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
                StringAssert.Contains(person.Script, "CREATE TABLE [Person]", StringComparison.Ordinal);
                StringAssert.Contains(person.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(person.EndTime >= person.StartTime);
                StringAssert.Contains(results.Single(r => r.TableName == "Country").Script, "CREATE TABLE [Country]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var reported = new List<(string Table, List<string> TargetTables)>();

                // Act
                source.CopySchemaTo(
                    new[] { "Person", "Country", "NoKey" },
                    target,
                    createdCallback: r => reported.Add((r.TableName, GetTargetTables())));

                // Assert
                Assert.AreEqual(3, reported.Count);
                Assert.AreEqual("Person", reported.Last().Table, StringComparer.Ordinal);
                Assert.AreEqual(3, reported.Last().TargetTables.Count);
                CollectionAssert.DoesNotContain(reported.First().TargetTables, "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                Assert.AreEqual(tables.Count, results.Select(r => Helper.FormatName(r.SourceSchema, r.TableName)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallbackResultOfEachTable()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
                var person = results[1];
                Assert.IsNull(person.SourceSchema);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(6, person.ColumnCount);
                Assert.AreEqual(1, person.IndexCount);
                Assert.AreEqual(1, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual(0, person.UniqueConstraintCount);
                StringAssert.StartsWith(person.Script, "CREATE TABLE [Person]", StringComparison.Ordinal);
                Assert.AreEqual(1, results[0].UniqueConstraintCount);
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfDependencyChainInAnyOrder()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "GrandChild", "Parent", "Child" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, results.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("Parent", "Child", "GrandChild");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfDiamond()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "DiamondD", "DiamondC", "DiamondB", "DiamondA" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual("DiamondA", results[0].TableName, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", results[3].TableName, StringComparer.Ordinal);
                AssertTargetMatchesSource("DiamondA", "DiamondB", "DiamondC", "DiamondD");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesThatReferenceEachOther()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("CycleA", "CycleB");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfCycleOfThreeTables()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "RingZ", "RingX", "RingY" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "LoopLeaf", "LoopB", "LoopA", "LoopRoot" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, results.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("LoopRoot", "LoopA", "LoopB", "LoopLeaf");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTablesWithMultipleIndexesAndKeys()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Product", "Shipment", "OrderLine", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Product", "Shipment", "OrderLine", "Country");
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTheWholeDatabase()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(tables, target, createdCallback: results.Add);

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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                source.CopySchemaTo(tables.AsEnumerable().Reverse().ToList(), target);

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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                var schemas = Helper.GetSourceSchemas(tables);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(tables, target, createdCallback: results.Add);

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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "Country", "Country", "[Country]" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                AssertTargetMatchesSource("Country");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithRequestedExistsBehavior()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(new[] { "NoKey" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaExistsBehavior.Drop, results.Single().Action);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithoutTables()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                source.CopySchemaTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTransactionThatIsRolledBack()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(new[] { "Country", "Person", "CycleA", "CycleB" }, target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTransactionThatIsCommitted()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(new[] { "Country", "Person" }, target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCreatesTheTableEvenIfTheReferencedTableIsNotPartOfThemAndDoesNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Shipment" }, target);

                // Assert
                CollectionAssert.AreEqual(new[] { "Shipment" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestCopySchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "Country" }, target);

                // Act
                source.CopySchemaTo(new[] { "Person" }, target);

                // Assert
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableAlreadyExists()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopySchemaTo(new[] { "NoKey" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<SqliteException>(() => source.CopySchemaTo(new[] { "MissingTable" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopySchemaTo(new[] { "Country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsBlank()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopySchemaTo(new[] { "Country", " " }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTableNamesAreNull()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopySchemaTo((string)null, target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsync()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                await source.CopySchemaToAsync(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncOfTablesThatReferenceEachOther()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopySchemaToAsync(new[] { "RingZ", "RingX", "RingY", "CycleA", "CycleB" }, target);

                // Assert
                AssertTargetMatchesSource("RingX", "RingY", "RingZ", "CycleA", "CycleB");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();

                // Act
                var results = new List<CopySchemaResult>();
                await source.CopySchemaToAsync(tables, target, createdCallback: results.Add);

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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                await source.CopySchemaToAsync(new[] { "Person", "Country" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(CopySchemaExistsBehavior.Align, results[1].Action);
                Assert.AreEqual("main", results[1].SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("main", results[1].DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(6, results[1].ColumnCount);
                StringAssert.Contains(results[1].Script, "CREATE TABLE [Person]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTables()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopySchemaResult>();
                await source.CopySchemaToAsync(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "NoKey" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopySchemaToAsync(new[] { "NoKey" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopySchemaToAsync(new[] { "Country" }, target));
            }
        }

        #endregion

        #region ErrorCallback

        #region Sync

        [TestMethod]
        public void TestCopySchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
                AssertTargetMatchesSource("Country", "Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.BlockIndexName("IX_Person_Name");
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemaTo(new[] { "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType<SqliteException>(errors[0].Exception);
                Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
                Assert.IsNull(errors[0].SchemaName);
                StringAssert.StartsWith(errors[0].Statement, "CREATE INDEX [IX_Person_Name] ON [Person]", StringComparison.Ordinal);
                Assert.AreEqual(1, errors[0].StatementIndex);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackKeepsTheObjectsThatWereCreated()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.BlockIndexName("IX_Person_Name");
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(
                    new[] { "Person" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: _ => { });

                // Assert
                Assert.AreEqual(1, created.Count);
                Assert.AreEqual(1, created[0].Errors.Count);
                Assert.IsInstanceOfType<SqliteException>(created[0].Errors[0].Exception);
                Assert.AreEqual(CopySchemaOutcome.Failed, created[0].Outcome);
                var person = Helper.GetTargetSchema("Person");
                Assert.AreEqual(6, person.Columns.Count);
                Assert.AreEqual(0, person.Indexes.Count);
                Assert.AreEqual(1, person.ForeignKeys.Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);
                var created = new List<CopySchemaResult>();
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemaTo(
                    new[] { "NoKey", "Parent", "Child" },
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("NoKey", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "CREATE TABLE [NoKey]", StringComparison.Ordinal);
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
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                source.CopySchemaTo(tables, target);
                var errors = new List<CopySchemaError>();
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(
                    tables,
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is SqliteException));
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
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);
                var created = new List<string>();

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopySchemaTo(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        createdCallback: r => created.Add(r.TableName),
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);
                CopySchemaError raised = null;

                // Act
                var exception = Assert.Throws<SqliteException>(() =>
                    source.CopySchemaTo(new[] { "NoKey" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopySchemaTo(new[] { "NoKey" }, target, errorCallback: e => throw new NotSupportedException()));
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithErrorCallbackInTransactionThatTheServerCommits()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "NoKey" }, target);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(
                        new[] { "NoKey", "Parent" },
                        target,
                        errorCallback: _ => { },
                        transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                CollectionAssert.AreEquivalent(new[] { "NoKey", "Parent" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                var errors = 0;

                // Act/Assert
                Assert.Throws<MissingMappingException>(() =>
                    source.CopySchemaTo(new[] { "Country" }, target, errorCallback: e => errors++));
                Assert.AreEqual(0, errors);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.BlockIndexName("IX_Person_Name");
                var errors = new List<CopySchemaError>();

                // Act
                await source.CopySchemaToAsync(new[] { "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType<SqliteException>(errors[0].Exception);
                Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
                StringAssert.StartsWith(errors[0].Statement, "CREATE INDEX [IX_Person_Name]", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "NoKey" }, target);
                var created = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(
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
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "NoKey" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    source.CopySchemaToAsync(
                        new[] { "NoKey", "Parent", "Child" },
                        target,
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                CollectionAssert.AreEqual(new[] { "NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "NoKey" }, target);
                CopySchemaError raised = null;

                // Act
                var exception = await Assert.ThrowsAsync<SqliteException>(() =>
                    source.CopySchemaToAsync(new[] { "NoKey" }, target, errorCallback: e =>
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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                await source.CopySchemaToAsync(tables, target);

                // Act
                await source.CopySchemaToAsync(tables, target, errorCallback: _ => { });

                // Assert
                CollectionAssert.AreEquivalent(tables, GetTargetTables());
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
