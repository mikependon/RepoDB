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
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlCopyTableSchemasToTest
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

        private static void MapSchemaReaderConnection(NpgsqlConnection source) =>
            SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

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
        public void TestPostgreSqlCopyTableSchemasTo()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithTheReferencingTableBeforeTheReferencedTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "country" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallbackResult()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                var person = results.Single(r => r.TableName == "Person");
                Assert.AreEqual("public", person.SourceSchema);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, person.Action);
                Assert.AreEqual(10, person.ColumnCount);
                Assert.AreEqual(2, person.IndexCount);
                Assert.AreEqual(1, person.ForeignKeyCount);
                Assert.AreEqual(1, person.CheckConstraintCount);
                Assert.AreEqual("repodb_schema_source", person.SourceDatabase);
                Assert.AreEqual("repodb_schema_target", person.DestinationDatabase);
                Assert.AreEqual(nameof(NpgsqlConnection), person.SourceDatabaseType);
                StringAssert.StartsWith(person.Script, "CREATE TABLE ", StringComparison.Ordinal);
                Assert.IsTrue(person.EndTime >= person.StartTime);
                Assert.AreEqual(0, person.Errors.Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var existing = new List<bool>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, createdCallback: result =>
                    existing.Add(Helper.TargetTableExists(result.TableName) && Helper.GetTargetSchema(result.TableName).Indexes.Count == result.IndexCount));

                // Assert
                Assert.AreEqual(2, existing.Count);
                Assert.IsTrue(existing.All(x => x));
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallsTheCallbackForEveryTableOfTheWholeDatabase()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(tables, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(tables.Count, results.Count);
                Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Created && r.Errors.Count == 0));
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfDependencyChainAndDiamond()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "chain_c", "d_leaf", "chain_a", "d_right", "chain_b", "d_left", "d_root" };

                // Act
                source.CopyTableSchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfCyclesAndSelfReferences()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "ring_2", "node_b", "loop_tail", "ring_1", "employee", "loop_a", "node_a", "ring_3", "loop_b", "loop_base" };

                // Act
                source.CopyTableSchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlCopyTableSchemasToOfTablesInDifferentSchemasAndWithTheSameName()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "item_ref", "sales.item", "public.item", "country", "Person", "sales.order_line" }, target));
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTablesWithNamesThatNeedQuoting()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "public.\"odd name\"", "public.\"odd.name\"", "public.\"odd\"\"name\"" };

                // Act
                source.CopyTableSchemaTo(tables, target);

                // Assert
                AssertTargetMatchesSource(tables);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTheWholeDatabase()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                source.CopyTableSchemaTo(tables, target);

                // Assert
                CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTheWholeDatabaseInTheReverseOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                source.CopyTableSchemaTo(Enumerable.Reverse(tables), target);

                // Assert
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTheTableThatIsGivenMoreThanOnce()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "public.country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(1, results.Count);
                AssertTargetMatchesSource("country");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithRequestedExistsBehavior()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, results.Single().Action);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithoutTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(Enumerable.Empty<string>(), target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(0, results.Count);
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTableThatReferencesATableThatAlreadyExistsInTheTarget()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" }, target);

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        #endregion

        #region Transactions

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithTransactionThatIsRolledBack()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(new[] { "country", "Person" }, target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithTransactionThatIsCommitted()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(new[] { "country", "Person" }, target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithErrorCallbackInTransactionThatTheServerAborts()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" }, target);
                var errors = new List<CopyTableSchemaError>();

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo(new[] { "country", "Person" }, target, errorCallback: errors.Add, transaction: transaction);

                    // Assert
                    Assert.IsTrue(errors.Count > 1);
                    Assert.IsTrue(errors.All(e => e.Exception is PostgresException));
                    Assert.AreEqual("42P07", ((PostgresException)errors[0].Exception).SqlState);
                    Assert.AreEqual("25P02", ((PostgresException)errors[1].Exception).SqlState);
                    transaction.Rollback();
                }
            }
        }

        #endregion

        #region Exceptions

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheReferencedTableIsNotPartOfThemAndDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyTableSchemaTo(new[] { "Person" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableAlreadyExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" }, target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableDoesNotExistInTheSource()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "country", "Missing" }, target));
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "country" }, target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsBlankOrTheTableNamesAreNull()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "country", " " }, target));
                Assert.Throws<ArgumentNullException>(() => source.CopyTableSchemaTo((IEnumerable<string>)null, target));
            }
        }

        #endregion

        #region Error callback

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToDoesNotCallTheErrorCallbackIfNothingFails()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var errors = new List<CopyTableSchemaError>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" }, target);
                var errors = new List<CopyTableSchemaError>();

                // Act
                source.CopyTableSchemaTo(new[] { "country" }, target, errorCallback: errors.Add);

                // Assert
                var error = errors.Single();
                Assert.AreEqual("country", error.TableName);
                Assert.AreEqual("public", error.SchemaName);
                Assert.AreEqual(0, error.StatementIndex);
                StringAssert.StartsWith(error.Statement, "CREATE TABLE \"public\".\"country\"", StringComparison.Ordinal);
                Assert.IsInstanceOfType<PostgresException>(error.Exception);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithErrorCallbackKeepsTheObjectsThatWereCreated()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var created = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                var person = Helper.GetTargetSchema("Person");
                Assert.AreEqual(10, person.Columns.Count);
                Assert.AreEqual(2, person.Indexes.Count);
                Assert.AreEqual(0, person.ForeignKeys.Count);
                Assert.AreEqual(1, created.Single().Errors.Count);
                Assert.AreEqual(CopyTableSchemaOutcome.Failed, created.Single().Outcome);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "no_key" }, target);
                var created = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "no_key", "chain_a", "chain_b" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                Assert.AreEqual(3, created.Count);
                Assert.AreEqual(1, created.Single(r => r.TableName == "no_key").Errors.Count);
                Assert.AreEqual(0, created.Where(r => r.TableName != "no_key").Sum(r => r.Errors.Count));
                AssertTargetMatchesSource("no_key", "chain_a", "chain_b");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithErrorCallbackCanCopyTheWholeDatabaseTwice()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();
                source.CopyTableSchemaTo(tables, target);
                var errors = new List<CopyTableSchemaError>();
                var created = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(tables, target, createdCallback: created.Add, errorCallback: errors.Add);

                // Assert
                Assert.IsTrue(errors.Count >= tables.Count);
                Assert.IsTrue(errors.All(e => e.Exception is PostgresException));
                Assert.AreEqual(tables.Count, created.Count);
                Assert.IsTrue(created.All(r => r.Errors.Count > 0 && r.Outcome == CopyTableSchemaOutcome.Failed));
                Assert.AreEqual(errors.Count, created.Sum(r => r.Errors.Count));
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "no_key" }, target);
                var created = new List<string>();

                // Act/Assert
                Assert.Throws<NotSupportedException>(() =>
                    source.CopyTableSchemaTo(new[] { "no_key", "chain_a" }, target, createdCallback: r => created.Add(r.TableName), errorCallback: _ => throw new NotSupportedException()));

                // Assert
                Assert.AreEqual(0, created.Count);
                CollectionAssert.AreEqual(new[] { "public.no_key" }, Helper.GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "no_key" }, target);
                CopyTableSchemaError raised = null;

                // Act
                var exception = Assert.Throws<PostgresException>(() =>
                    source.CopyTableSchemaTo(new[] { "no_key" }, target, errorCallback: e =>
                    {
                        raised = e;
                        throw e.Exception;
                    }));

                // Assert
                Assert.AreSame(raised.Exception, exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithErrorCallbackIfThereIsNoMappedSchemaReader()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                var errors = new List<CopyTableSchemaError>();
                Assert.Throws<MissingMappingException>(() => source.CopyTableSchemaTo(new[] { "country" }, target, errorCallback: errors.Add));
                Assert.AreEqual(0, errors.Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsync()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person", "country" }, target);

                // Assert
                AssertTargetMatchesSource("country", "Person");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncOfTablesThatReferenceEachOther()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "node_b", "node_a", "ring_3", "ring_1", "ring_2" }, target);

                // Assert
                AssertTargetMatchesSource("node_a", "node_b", "ring_1", "ring_2", "ring_3");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncOfTheWholeDatabase()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = Helper.GetSourceTables();

                // Act
                await source.CopyTableSchemaToAsync(tables, target);

                // Assert
                CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
                AssertTargetMatchesSource(tables.ToArray());
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncCallbackResult()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(10, results.Single(r => r.TableName == "Person").ColumnCount);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncWithoutTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopyTableSchemaToAsync(Enumerable.Empty<string>(), target);

                // Assert
                Assert.AreEqual(0, Helper.GetTargetTables().Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableAlreadyExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "country" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync(new[] { "country" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => source.CopyTableSchemaToAsync(new[] { "country" }, target));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncCallsTheErrorCallbackWithTheDetailsOfTheError()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "country" }, target);
                var errors = new List<CopyTableSchemaError>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "country" }, target, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual("country", errors.Single().TableName);
                Assert.IsInstanceOfType<PostgresException>(errors.Single().Exception);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "no_key" }, target);
                var created = new List<CopyTableSchemaResult>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "no_key", "chain_a", "chain_b" }, target, createdCallback: created.Add, errorCallback: _ => { });

                // Assert
                Assert.AreEqual(1, created.Single(r => r.TableName == "no_key").Errors.Count);
                AssertTargetMatchesSource("no_key", "chain_a", "chain_b");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncStopsIfTheErrorCallbackThrows()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync(new[] { "no_key" }, target);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    source.CopyTableSchemaToAsync(new[] { "no_key", "chain_a" }, target, errorCallback: _ => throw new NotSupportedException()));
                CollectionAssert.AreEqual(new[] { "public.no_key" }, Helper.GetTargetTables());
            }
        }

        #endregion
    }
}
