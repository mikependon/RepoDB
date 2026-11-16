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
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Contains the copy table schemas to test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToTest
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
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs map schema reader connection.
        /// </summary>
        /// <param name="source">The source.</param>
        private static void MapSchemaReaderConnection(ClickHouseConnection source) => SchemaReaderMapper.Add<ClickHouseConnection>(new ClickHouseSchemaReader(),
            true);

        /// <summary>
        /// Performs get source tables.
        /// </summary>
        private static List<string> GetSourceTables()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                var reader = new ClickHouseSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Where(name =>
                {
                    var schema = reader.GetTableSchema(connection,
                            name,
                            transaction: null);
                    return schema.Table.Schema == null && schema.ForeignKeys.All(foreignKey => foreignKey.ReferencedTable.Schema == null);
                }).ToList();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                return new ClickHouseSchemaReader().GetTables(connection,
                    schemaName,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs assert target matches source.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
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

        /// <summary>
        /// Verifies copy table schemas to.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasTo()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target);
                // Assert
                AssertTargetMatchesSource("Country",
                    "Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with the referencing table before the referenced table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country" },
                    target);
                // Assert
                AssertTargetMatchesSource("Country",
                    "Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to callback result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallbackResult()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                var run = source.CopyTableSchemaTo(new[] { "Person", "Country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(2,
                    results.Count);
                var person = results.Single(r => r.Target.Table.Table.Name == "Person");
                Assert.AreEqual(Database.SourceName,
                    person.Source.Database,
                    StringComparer.Ordinal);
                Assert.AreEqual(Database.TargetName,
                    person.Target.Database,
                    StringComparer.Ordinal);
                Assert.AreEqual(nameof(ClickHouseConnection),
                    person.Source.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(nameof(ClickHouseConnection),
                    person.Target.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(TableExistsBehavior.Skip,
                    person.Action);
                Assert.AreEqual(TableOutcome.Created,
                    person.Outcome);
                StringAssert.Contains(person.Script,
                    "CREATE TABLE `Person`",
                    StringComparison.Ordinal);
                Assert.IsTrue(run.EndTime >= run.StartTime);
                StringAssert.Contains(results.Single(r => r.Target.Table.Table.Name == "Country").Script,
                    "CREATE TABLE `Country`",
                    StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback for every table of the whole database.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables,
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(tables.Count,
                    results.Count);
                Assert.AreEqual(tables.Count,
                    results.Select(r => Helper.FormatName(r.Source.Table.Table.Schema,
                            r.Target.Table.Table.Name)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            }
        }

        /// <summary>
        /// Verifies copy table schemas to callback result of each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallbackResultOfEachTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Person", "Country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" },
                    results.Select(t => t.Target.Table.Table.Name).ToArray());
                var person = results[1];
                Assert.IsNull(person.Source.Table.Table.Schema);
                Assert.AreEqual(TableOutcome.Created,
                    person.Outcome);
                Assert.AreEqual(7,
                    person.ColumnCount);
                Assert.AreEqual(1,
                    person.IndexCount);
                Assert.AreEqual(0,
                    person.ForeignKeyCount);
                Assert.AreEqual(1,
                    person.CheckConstraintCount);
                Assert.AreEqual(0,
                    person.UniqueConstraintCount);
                StringAssert.StartsWith(person.Script,
                    "CREATE TABLE `Person`",
                    StringComparison.Ordinal);
                Assert.AreEqual(0,
                    results[0].UniqueConstraintCount);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of tables with multiple indexes and keys.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithMultipleIndexesAndKeys()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "Product", "OrderLine", "Country" },
                    target);
                // Assert
                AssertTargetMatchesSource("Product",
                    "OrderLine",
                    "Country");
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToOfTablesWithTheSameNameInDifferentSchemas()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "ItemRef", "Item", $"{Database.SourceSalesName}.Item" },
                        target,
                        Database.TargetSalesName));
                Assert.AreEqual(0,
                    GetTargetTables().Count);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of tables with names that need quoting.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "`Odd.Child`", "`Odd.Name`", "`Order Details`", "`Weird]Name`" },
                    target);
                // Assert
                AssertTargetMatchesSource("`Odd.Name`",
                    "`Odd.Child`",
                    "`Order Details`",
                    "`Weird]Name`");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of the whole database.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabase()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables,
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(tables.Count,
                    results.Count);
                CollectionAssert.AreEquivalent(tables,
                    GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of the whole database in the reverse order.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabaseInTheReverseOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                // Act
                source.CopyTableSchemaTo(tables.AsEnumerable().Reverse().ToList(),
                    target);
                // Assert
                CollectionAssert.AreEquivalent(tables,
                    GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of the whole database counts the objects.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTheWholeDatabaseCountsTheObjects()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                var schemas = Helper.GetSourceSchemas(tables);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(tables,
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(schemas.Sum(s => s.Columns.Count),
                    results.Sum(t => t.ColumnCount));
                Assert.AreEqual(schemas.Sum(s => s.Indexes.Count),
                    results.Sum(t => t.IndexCount));
                Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count),
                    results.Sum(t => t.ForeignKeyCount));
                Assert.AreEqual(schemas.Sum(s => s.UniqueConstraints.Count),
                    results.Sum(t => t.UniqueConstraintCount));
                Assert.AreEqual(schemas.Sum(s => s.CheckConstraints.Count),
                    results.Sum(t => t.CheckConstraintCount));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of the table that is given more than once.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTheTableThatIsGivenMoreThanOnce()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Country", "Country", "`Country`" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(1,
                    results.Count);
                AssertTargetMatchesSource("Country");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with requested exists behavior.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithRequestedExistsBehavior()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "NoKey" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableExistsBehavior.Drop,
                    results.Single().Action);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to without tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithoutTables()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(Enumerable.Empty<string>(),
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(0,
                    results.Count);
                Assert.AreEqual(0,
                    GetTargetTables().Count);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to if the referenced table is not part of them and does not exist leaves out the foreign key.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExistLeavesOutTheForeignKey()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                source.CopyTableSchemaTo(new[] { "Shipment" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(1,
                    results.Count);
                Assert.IsTrue(results[0].Warnings.Count > 0);
                Assert.AreEqual(0,
                    results[0].Errors.Count);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to of table that references atable that already exists in the target.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" },
                    target);
                // Act
                source.CopyTableSchemaTo(new[] { "Person" },
                    target);
                // Assert
                AssertTargetMatchesSource("Country",
                    "Person");
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if atable already exists.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableAlreadyExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" },
                    target);
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "NoKey" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if atable does not exist in the source.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<ClickHouseServerException>(() => source.CopyTableSchemaTo(new[] { "MissingTable" },
                        target));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if there is no mapped schema reader.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if atable name is blank.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsBlank()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "Country", " " },
                        target));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTableNamesAreNull()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyTableSchemaTo((string)null,
                        target));
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies copy table schemas to async.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsync()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" },
                    results.Select(t => t.Target.Table.Table.Name).ToArray());
                AssertTargetMatchesSource("Country",
                    "Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async of the whole database.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(tables,
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(tables.Count,
                    results.Count);
                CollectionAssert.AreEquivalent(tables,
                    GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async callback result.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallbackResult()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(2,
                    results.Count);
                Assert.AreEqual(TableExistsBehavior.Align,
                    results[1].Action);
                Assert.AreEqual(Database.SourceName,
                    results[1].Source.Database,
                    StringComparer.Ordinal);
                Assert.AreEqual(Database.TargetName,
                    results[1].Target.Database,
                    StringComparer.Ordinal);
                Assert.AreEqual(7,
                    results[1].ColumnCount);
                StringAssert.Contains(results[1].Script,
                    "CREATE TABLE `Person`",
                    StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async without tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTables()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var results = new List<CopyTableSchemaResultItem>();
                await source.CopyTableSchemaToAsync(Enumerable.Empty<string>(),
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(0,
                    results.Count);
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if atable already exists.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" },
                    target);
                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync(new[] { "NoKey" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if there is no mapped schema reader.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopyTableSchemaToAsync(new[] { "Country" },
                        target));
            }
        }

        #endregion

        #region ErrorCallback

        #region Sync

        /// <summary>
        /// Verifies copy table schemas to does not call the error callback if nothing fails.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<ExecutionError>();
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    errorCallback: errors.Add);
                // Assert
                Assert.AreEqual(0,
                    errors.Count);
                AssertTargetMatchesSource("Country",
                    "Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with error callback can copy the whole database twice.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                source.CopyTableSchemaTo(tables,
                    target);
                var errors = new List<ExecutionError>();
                var created = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(tables,
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);
                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is ClickHouseServerException));
                Assert.AreEqual(tables.Count,
                    created.Count);
                Assert.IsTrue(created.All(r => r.Errors.Count > 0 && r.Outcome == TableOutcome.Failed));
                Assert.AreEqual(errors.Count,
                    created.Sum(r => r.Errors.Count));
                CollectionAssert.AreEquivalent(tables,
                    GetTargetTables());
                foreach (var table in tables)
                {
                    AssertTargetMatchesSource(table);
                }
            }
        }

        /// <summary>
        /// Verifies copy table schemas to stops if the error callback throws.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" },
                    target);
                var created = new List<string>();
                // Act/Assert
                Assert.Throws<NotSupportedException>(() => source.CopyTableSchemaTo(new[] { "NoKey", "Parent", "Child" },
                        target,
                        createdCallback: r => created.Add(r.Target.Table.Table.Name),
                        errorCallback: _ => throw new NotSupportedException()));
                // Assert
                Assert.AreEqual(0,
                    created.Count);
                CollectionAssert.AreEqual(new[] { "NoKey" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the error callback throws the captured exception.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" },
                    target);
                ExecutionError raised = null;
                // Act
                var exception = Assert.Throws<ClickHouseServerException>(() => source.CopyTableSchemaTo(new[] { "NoKey" },
                        target,
                        errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));
                // Assert
                Assert.AreSame(raised.Exception,
                    exception);
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the error callback throws.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" },
                    target);
                // Act/Assert
                Assert.Throws<NotSupportedException>(() => source.CopyTableSchemaTo(new[] { "NoKey" },
                        target,
                        errorCallback: e => throw new NotSupportedException()));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to with error callback if there is no mapped schema reader.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var errors = 0;
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target,
                        errorCallback: e => errors++));
                Assert.AreEqual(0,
                    errors);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies copy table schemas to async stops if the error callback throws.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" },
                    target);
                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() => source.CopyTableSchemaToAsync(new[] { "NoKey", "Parent", "Child" },
                        target,
                        errorCallback: _ => throw new NotSupportedException()));
                // Assert
                CollectionAssert.AreEqual(new[] { "NoKey" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if the error callback throws the captured exception.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" },
                    target);
                ExecutionError raised = null;
                // Act
                var exception = await Assert.ThrowsAsync<ClickHouseServerException>(() => source.CopyTableSchemaToAsync(new[] { "NoKey" },
                        target,
                        errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));
                // Assert
                Assert.AreSame(raised.Exception,
                    exception);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with error callback can copy the whole database twice.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                await source.CopyTableSchemaToAsync(tables,
                    target);
                // Act
                await source.CopyTableSchemaToAsync(tables,
                    target,
                    errorCallback: _ =>
                {
                });
                // Assert
                CollectionAssert.AreEquivalent(tables,
                    GetTargetTables());
            }
        }
        #endregion

        #endregion

        #endregion
    }
}
