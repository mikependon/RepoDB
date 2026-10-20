#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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

        private static void MapSchemaReaderConnection(ClickHouseConnection source) =>
            SchemaReaderMapper.Add<ClickHouseConnection>(new ClickHouseSchemaReader(source), true);

        private static List<string> GetSourceTables()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                var reader = new ClickHouseSchemaReader(connection);
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
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                return new ClickHouseSchemaReader(connection).GetTables(schemaName).ToList();
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.TableName == "Person");
                Assert.AreEqual(Database.SourceName, person.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual(Database.TargetName, person.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(ClickHouseConnection), person.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(ClickHouseConnection), person.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, person.Action);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
                StringAssert.Contains(person.Script, "CREATE TABLE `Person`", StringComparison.Ordinal);
                Assert.IsTrue(person.EndTime >= person.StartTime);
                StringAssert.Contains(results.Single(r => r.TableName == "Country").Script, "CREATE TABLE `Country`", StringComparison.Ordinal);
            }
        }

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
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                Assert.AreEqual(tables.Count, results.Select(r => Helper.FormatName(r.SourceSchema, r.TableName)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackResultOfEachTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
                var person = results[1];
                Assert.IsNull(person.SourceSchema);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(7, person.ColumnCount);
                Assert.AreEqual(1, person.IndexCount);
                Assert.AreEqual(0, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual(0, person.UniqueConstraintCount);
                StringAssert.StartsWith(person.Script, "CREATE TABLE `Person`", StringComparison.Ordinal);
                Assert.AreEqual(0, results[0].UniqueConstraintCount);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithMultipleIndexesAndKeys()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Product", "OrderLine", "Country" }, target);

                // Assert
                AssertTargetMatchesSource("Product", "OrderLine", "Country");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToOfTablesWithTheSameNameInDifferentSchemas()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "ItemRef", "Item", $"{Database.SourceSalesName}.Item" }, target, Database.TargetSalesName));
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "`Odd.Child`", "`Odd.Name`", "`Order Details`", "`Weird]Name`" }, target);

                // Assert
                AssertTargetMatchesSource("`Odd.Name`", "`Odd.Child`", "`Order Details`", "`Weird]Name`");
            }
        }

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
                var results = new List<CopyTableSchemaResult>();
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = GetSourceTables();
                var schemas = Helper.GetSourceSchemas(tables);

                // Act
                var results = new List<CopyTableSchemaResult>();
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(new[] { "Country", "Country", "`Country`" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                AssertTargetMatchesSource("Country");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithRequestedExistsBehavior()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(new[] { "NoKey" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, results.Single().Action);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTables()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                source.CopyTableSchemaTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ClickHouseServerException>(() => source.CopyTableSchemaTo(new[] { "Shipment" }, target));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ClickHouseServerException>(() => source.CopyTableSchemaTo(new[] { "MissingTable" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsBlank()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
                AssertTargetMatchesSource("Country", "Person");
            }
        }

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
                var results = new List<CopyTableSchemaResult>();
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                await source.CopyTableSchemaToAsync(new[] { "Person", "Country" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Align, results[1].Action);
                Assert.AreEqual(Database.SourceName, results[1].SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual(Database.TargetName, results[1].DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(7, results[1].ColumnCount);
                StringAssert.Contains(results[1].Script, "CREATE TABLE `Person`", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTables()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var results = new List<CopyTableSchemaResult>();
                await source.CopyTableSchemaToAsync(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<CopyTableSchemaError>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
                AssertTargetMatchesSource("Country", "Person");
            }
        }

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
                source.CopyTableSchemaTo(tables, target);
                var errors = new List<CopyTableSchemaError>();
                var created = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(
                    tables,
                    target,
                    createdCallback: created.Add,
                    errorCallback: errors.Add);

                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is ClickHouseServerException));
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

            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
                        createdCallback: r => created.Add(r.TableName),
                        errorCallback: _ => throw new NotSupportedException()));

                // Assert
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "NoKey" }, target);
                CopyTableSchemaError raised = null;

                // Act
                var exception = Assert.Throws<ClickHouseServerException>(() =>
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

            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
        public async Task TestCopyTableSchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
                CollectionAssert.AreEqual(new[] { "NoKey" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "NoKey" }, target);
                CopyTableSchemaError raised = null;

                // Act
                var exception = await Assert.ThrowsAsync<ClickHouseServerException>(() =>
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
