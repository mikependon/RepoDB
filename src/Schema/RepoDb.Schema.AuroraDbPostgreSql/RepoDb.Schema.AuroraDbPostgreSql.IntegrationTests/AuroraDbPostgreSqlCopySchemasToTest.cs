#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlCopySchemasToTest
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

        private static void MapSchemaReaderConnection(AuroraDbConnection source) =>
            SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(source), true);

        private static void AssertTargetMatchesSource(params string[] tableNames)
        {
            foreach (var tableName in tableNames)
            {
                Helper.AssertTargetMatchesSource(tableName);
            }
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasTo()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Person", "country" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToCallbackResult()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.TableName == "Person");
                Assert.AreEqual("public", person.SourceSchema);
                Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(CopySchemaExistsBehavior.Skip, person.Action);
                Assert.AreEqual(10, person.ColumnCount);
                Assert.AreEqual(2, person.IndexCount);
                Assert.AreEqual(1, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual("repodb_schema_source", person.SourceDatabase);
                Assert.AreEqual("repodb_schema_target", person.DestinationDatabase);
                Assert.AreEqual(nameof(AuroraDbConnection), person.SourceDatabaseType);
                StringAssert.StartsWith(person.Script, "CREATE TABLE ", StringComparison.Ordinal);
                Assert.IsTrue(person.EndTime >= person.StartTime);
                Assert.AreEqual(0, person.Errors.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var existing = new List<bool>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, createdCallback: result =>
                    existing.Add(Helper.TargetTableExists(result.TableName) && Helper.GetTargetSchema(result.TableName).Indexes.Count == result.IndexCount));

                // Assert
                Assert.AreEqual(2, existing.Count);
                Assert.IsTrue(existing.All(x => x));
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                Assert.IsTrue(results.All(r => r.Outcome == CopySchemaOutcome.Created && r.Errors.Count == 0));
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfDependencyChainAndDiamond()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "chain_c", "d_leaf", "chain_a", "d_right", "chain_b", "d_left", "d_root" };

                // Act
                source.CopySchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfCyclesAndSelfReferences()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "ring_2", "node_b", "loop_tail", "ring_1", "employee", "loop_a", "node_a", "ring_3", "loop_b", "loop_base" };

                // Act
                source.CopySchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlCopySchemasToOfTablesInDifferentSchemasAndWithTheSameName()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopySchemaTo(new[] { "item_ref", "sales.item", "public.item", "country", "Person", "sales.order_line" }, target));
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "public.\"odd name\"", "public.\"odd.name\"", "public.\"odd\"\"name\"" };

                // Act
                source.CopySchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfTheWholeDatabase()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                source.CopySchemaTo(tables, target);

                // Assert
                CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfTheWholeDatabaseInTheReverseOrder()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                source.CopySchemaTo(Enumerable.Reverse(tables), target);

                // Assert
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfTheTableThatIsGivenMoreThanOnce()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "public.country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                AssertTargetMatchesSource("country");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithRequestedExistsBehavior()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaExistsBehavior.Drop, results.Single().Action);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithoutTables()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country" }, target);

                // Act
                source.CopySchemaTo(new[] { "Person" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        #endregion

        #region Transactions

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithTransactionThatIsRolledBack()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(new[] { "country", "Person" }, target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithTransactionThatIsCommitted()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(new[] { "country", "Person" }, target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithErrorCallbackInTransactionThatTheServerAborts()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country" }, target);
                var errors = new List<CopySchemaError>();

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo(new[] { "country", "Person" }, target, errorCallback: errors.Add, transaction: transaction);

                    // Assert
                    Assert.IsTrue(errors.Count > 1);
                    Assert.IsTrue(errors.All(e => e.Exception is AuroraDbException));
                    Assert.AreEqual("42P07", ((AuroraDbException)errors[0].Exception).SqlState);
                    Assert.AreEqual("25P02", ((AuroraDbException)errors[1].Exception).SqlState);
                    transaction.Rollback();
                }
            }
        }

        #endregion

        #region Exceptions

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExist()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<AuroraDbException>(() => source.CopySchemaTo(new[] { "Person" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country" }, target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopySchemaTo(new[] { "country", "Missing" }, target));
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopySchemaTo(new[] { "country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsBlankOrTheTableNamesAreNull()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopySchemaTo(new[] { "country", " " }, target));
                Assert.Throws<ArgumentNullException>(() => source.CopySchemaTo((IEnumerable<string>)null, target));
            }
        }

        #endregion

        #region Error callback

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country" }, target);
                var errors = new List<CopySchemaError>();

                // Act
                source.CopySchemaTo(new[] { "country" }, target, errorCallback: errors.Add);

                // Assert
                var error = errors.Single();
                Assert.AreEqual("country", error.TableName);
                Assert.AreEqual("public", error.SchemaName);
                Assert.AreEqual(0, error.StatementIndex);
                StringAssert.StartsWith(error.Statement, "CREATE TABLE \"public\".\"country\"", StringComparison.Ordinal);
                Assert.IsInstanceOfType<AuroraDbException>(error.Exception);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithErrorCallbackKeepsTheObjectsThatWereCreated()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Person" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                var person = Helper.GetTargetSchema("Person");
                Assert.AreEqual(10, person.Columns.Count);
                Assert.AreEqual(2, person.Indexes.Count);
                Assert.AreEqual(0, person.ForeignKeys.Count);
                Assert.AreEqual(1, created.Single().Errors.Count);
                Assert.AreEqual(CopySchemaOutcome.Failed, created.Single().Outcome);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "no_key" }, target);
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "no_key", "chain_a", "chain_b" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                Assert.AreEqual(3, created.Count);
                Assert.AreEqual(1, created.Single(r => r.TableName == "no_key").Errors.Count);
                Assert.AreEqual(0, created.Where(r => r.TableName != "no_key").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("no_key", "chain_a", "chain_b");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();
                source.CopySchemaTo(tables, target);
                var errors = new List<CopySchemaError>();
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(tables, target, createdCallback: created.Add, errorCallback: errors.Add);

                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is AuroraDbException));
                Assert.AreEqual(tables.Count, created.Count);
                Assert.IsTrue(created.All(r => r.Errors.Count > 0 && r.Outcome == CopySchemaOutcome.Failed));
                Assert.AreEqual(errors.Count, created.Sum(r => r.Errors.Count));
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "no_key" }, target);
                var created = new List<string>();

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopySchemaTo(new[] { "no_key", "chain_a" }, target, createdCallback: r => created.Add(r.TableName), errorCallback: _ => throw new NotSupportedException()));

                // Assert
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "public.no_key" }, Helper.GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "no_key" }, target);
                CopySchemaError raised = null;

                // Act
                var exception = Assert.Throws<AuroraDbException>(() =>
                    source.CopySchemaTo(new[] { "no_key" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                var errors = new List<CopySchemaError>();
                Assert.Throws<MissingMappingException>(() => source.CopySchemaTo(new[] { "country" }, target, errorCallback: errors.Add));
                Assert.AreEqual(0, errors.Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsync()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopySchemaToAsync(new[] { "Person", "country" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncOfTablesThatReferenceEachOther()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopySchemaToAsync(new[] { "node_b", "node_a", "ring_3", "ring_1", "ring_2" }, target);

                // Assert
                AssertTargetMatchesSource("node_a", "node_b", "ring_1", "ring_2", "ring_3");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                await source.CopySchemaToAsync(tables, target);

                // Assert
                CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncCallbackResult()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(10, results.Single(r => r.TableName == "Person").ColumnCount);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncWithoutTables()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopySchemaToAsync(Enumerable.Empty<string>(), target);

                // Assert
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "country" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopySchemaToAsync(new[] { "country" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopySchemaToAsync(new[] { "country" }, target));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "country" }, target);
                var errors = new List<CopySchemaError>();

                // Act
                await source.CopySchemaToAsync(new[] { "country" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual("country", errors.Single().TableName);
                Assert.IsInstanceOfType<AuroraDbException>(errors.Single().Exception);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "no_key" }, target);
                var created = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "no_key", "chain_a", "chain_b" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                Assert.AreEqual(1, created.Single(r => r.TableName == "no_key").Errors.Count);
                AssertTargetMatchesSource("no_key", "chain_a", "chain_b");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync(new[] { "no_key" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    source.CopySchemaToAsync(new[] { "no_key", "chain_a" }, target, errorCallback: _ => throw new NotSupportedException()));
                CollectionAssert.AreEqual(new[] { "public.no_key" }, Helper.GetTargetTables());
            }
        }

        #endregion
    }
}
