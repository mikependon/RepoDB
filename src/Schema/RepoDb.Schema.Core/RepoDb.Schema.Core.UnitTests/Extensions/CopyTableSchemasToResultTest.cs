#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the copy table schemas to result test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToResultTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Private Methods

        /// <summary>
        /// Performs get schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="foreignKeys">The foreign keys.</param>
        private static TableSchema GetSchema(string tableName,
            int columns = 1,
            int indexes = 0,
            int foreignKeys = 0)
        {
            var schema = new TableSchema(tableName,
                "dbo");
            for (var i = 0; i < columns; i++)
                schema.Columns.Add(new ColumnInfo());
            for (var i = 0; i < indexes; i++)
                schema.Indexes.Add(new IndexInfo(null));
            for (var i = 0; i < foreignKeys; i++)
                schema.ForeignKeys.Add(new ForeignKeyInfo(null));
            return schema;
        }

        /// <summary>
        /// Performs get named schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="columns">The columns.</param>
        private static TableSchema GetNamedSchema(string tableName,
            params string[] columns)
        {
            var schema = new TableSchema(tableName,
                "dbo");
            foreach (var column in columns)
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column,
                        false,
                        false,
                        true,
                        typeof(string),
                        null,
                        null,
                        null,
                        "text",
                        false,
                        "Test") });
            }

            return schema;
        }

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => schemas.Single(s => s.Table.Name == name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    name,
                    operationTransaction,
                    _) => Task.FromResult(schemas.Single(s => s.Table.Name == name)));
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        /// <param name="statements">The statements.</param>
        private static Mock<ISchemaComposer> MapComposer(params string[] statements)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns(statements);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object,
                true);
            return composer;
        }

        /// <summary>
        /// Performs map three tables.
        /// </summary>
        private static void MapThreeTables()
        {
            MapReader(GetSchema("Country"),
                GetSchema("Person",
                    1,
                    1,
                    1),
                GetSchema("Solo"));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE TABLE Solo;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
        }

        private static readonly string[] ThreeTables =
        {
            "Country",
            "Person",
            "Solo"
        };
        #endregion

        #region Items

        /// <summary>
        /// Verifies copy table schemas to returns aresult with an item for each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToReturnsAResultWithAnItemForEachTable()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection());
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3,
                result.Items.Count);
            CollectionAssert.AreEquivalent(ThreeTables,
                result.Items.Select(i => i.Target.Table.Table.Name).ToArray());
        }

        /// <summary>
        /// Verifies copy table schemas to async returns aresult with an item for each table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncReturnsAResultWithAnItemForEachTable()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(ThreeTables,
                new CustomDbConnection());
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3,
                result.Items.Count);
            CollectionAssert.AreEquivalent(ThreeTables,
                result.Items.Select(i => i.Target.Table.Table.Name).ToArray());
        }

        /// <summary>
        /// Verifies copy table schemas to items are the same items that the created callback receives.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsAreTheSameItemsThatTheCreatedCallbackReceives()
        {
            // Setup
            MapThreeTables();
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection(),
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(3,
                created.Count);
            Assert.AreEqual(created.Count,
                result.Items.Count);
            for (var i = 0; i < created.Count; i++)
            {
                Assert.AreSame(created[i],
                    result.Items[i]);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async items are the same items that the created callback receives.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncItemsAreTheSameItemsThatTheCreatedCallbackReceives()
        {
            // Setup
            MapThreeTables();
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(ThreeTables,
                new CustomDbConnection(),
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(3,
                created.Count);
            Assert.AreEqual(created.Count,
                result.Items.Count);
            for (var i = 0; i < created.Count; i++)
            {
                Assert.AreSame(created[i],
                    result.Items[i]);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to items are populated without acreated callback.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsArePopulatedWithoutACreatedCallback()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection(),
                createdCallback: null);
            // Assert
            Assert.AreEqual(3,
                result.Items.Count);
            Assert.IsTrue(result.Items.All(i => i.Outcome == TableOutcome.Created));
            Assert.IsTrue(result.Items.All(i => !string.IsNullOrEmpty(i.Script)));
        }

        /// <summary>
        /// Verifies copy table schemas to items have the source and the target of the table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsHaveTheSourceAndTheTargetOfTheTable()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection());
            // Assert
            foreach (var item in result.Items)
            {
                Assert.IsNotNull(item.Source.Table);
                Assert.IsNotNull(item.Target.Table);
                Assert.AreNotSame(item.Source,
                    item.Target);
                Assert.AreEqual(item.Source.Table.Table.Name,
                    item.Target.Table.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual(nameof(CustomDbConnection),
                    item.Source.DatabaseType,
                    StringComparer.Ordinal);
                Assert.AreEqual(nameof(CustomDbConnection),
                    item.Target.DatabaseType,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to item source is the schema that was read and target is the schema that was created.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemSourceIsTheSchemaThatWasReadAndTargetIsTheSchemaThatWasCreated()
        {
            // Setup
            var country = GetSchema("Country");
            var person = GetSchema("Person");
            MapReader(country,
                person);
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" },
                new CustomDbConnection(),
                targetSchema: "Sales");
            // Assert
            var item = result.Items.Single(i => i.Source.Table.Table.Name == "Person");
            Assert.AreSame(person,
                item.Source.Table);
            Assert.AreEqual("dbo",
                item.Source.Table.Table.Schema,
                StringComparer.Ordinal);
            Assert.AreEqual("Sales",
                item.Target.Table.Table.Schema,
                StringComparer.Ordinal);
            Assert.AreNotSame(item.Source.Table,
                item.Target.Table);
        }

        /// <summary>
        /// Verifies copy table schemas to items have the server and the database of the connections.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsHaveTheServerAndTheDatabaseOfTheConnections()
        {
            // Setup
            MapReader(GetSchema("Person"));
            MapComposer("CREATE TABLE Person;");
            var source = new CustomDbConnection();
            var target = new CustomDbConnection();
            // Act
            var result = source.CopyTableSchemaTo(new[] { "Person" },
                target);
            // Assert
            var item = result.Items.Single();
            Assert.AreEqual(source.Database,
                item.Source.Database,
                StringComparer.Ordinal);
            Assert.AreEqual(source.DataSource,
                item.Source.Server,
                StringComparer.Ordinal);
            Assert.AreEqual(target.Database,
                item.Target.Database,
                StringComparer.Ordinal);
            Assert.AreEqual(target.DataSource,
                item.Target.Server,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies copy table schemas to items have the outcome of each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsHaveTheOutcomeOfEachTable()
        {
            // Setup
            MapReader(GetSchema("Country"),
                GetSchema("Person"));
            var composer = MapComposer();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            var destination = new CustomDbConnection
            {
                ScalarResult = statement => statement == "TABLE_EXISTS Country" ? 1 : 0
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Skip);
            // Assert
            Assert.AreEqual(2,
                result.Items.Count);
            var country = result.Items.Single(i => i.Target.Table.Table.Name == "Country");
            var person = result.Items.Single(i => i.Target.Table.Table.Name == "Person");
            Assert.AreEqual(TableOutcome.Skipped,
                country.Outcome);
            Assert.AreEqual(true,
                country.TableExisted);
            Assert.AreEqual(TableOutcome.Created,
                person.Outcome);
            Assert.AreEqual(false,
                person.TableExisted);
            Assert.AreEqual(TableExistsBehavior.Skip,
                country.Action);
            Assert.AreEqual(TableExistsBehavior.Skip,
                person.Action);
        }

        /// <summary>
        /// Verifies copy table schemas to items have the counts of the created objects.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemsHaveTheCountsOfTheCreatedObjects()
        {
            // Setup
            MapReader(GetSchema("Person",
                    4,
                    2,
                    1));
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                new CustomDbConnection());
            // Assert
            var item = result.Items.Single();
            Assert.AreEqual(4,
                item.ColumnCount);
            Assert.AreEqual(2,
                item.IndexCount);
            Assert.AreEqual(1,
                item.ForeignKeyCount);
        }

        #endregion

        #region Times

        /// <summary>
        /// Verifies copy table schemas to result has the start and the end time in utc.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToResultHasTheStartAndTheEndTimeInUtc()
        {
            // Setup
            MapThreeTables();
            var before = DateTime.UtcNow;
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection());
            var after = DateTime.UtcNow;
            // Assert
            Assert.AreEqual(DateTimeKind.Utc,
                result.StartTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc,
                result.EndTime.Kind);
            Assert.IsTrue(result.StartTime >= before);
            Assert.IsTrue(result.EndTime >= result.StartTime);
            Assert.IsTrue(result.EndTime <= after);
            Assert.AreEqual(result.EndTime - result.StartTime,
                result.Duration);
        }

        /// <summary>
        /// Verifies copy table schemas to async result has the start and the end time in utc.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncResultHasTheStartAndTheEndTimeInUtc()
        {
            // Setup
            MapThreeTables();
            var before = DateTime.UtcNow;
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(ThreeTables,
                new CustomDbConnection());
            var after = DateTime.UtcNow;
            // Assert
            Assert.AreEqual(DateTimeKind.Utc,
                result.StartTime.Kind);
            Assert.AreEqual(DateTimeKind.Utc,
                result.EndTime.Kind);
            Assert.IsTrue(result.StartTime >= before);
            Assert.IsTrue(result.EndTime >= result.StartTime);
            Assert.IsTrue(result.EndTime <= after);
        }

        #endregion

        #region Empty

        /// <summary>
        /// Verifies copy table schemas to with no tables returns an empty result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithNoTablesReturnsAnEmptyResult()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new string[0],
                destination);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0,
                result.Items.Count);
            Assert.AreEqual(0,
                result.Errors.Count);
            Assert.AreEqual(0,
                result.Warnings.Count);
            Assert.IsTrue(result.EndTime >= result.StartTime);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to async with no tables returns an empty result.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithNoTablesReturnsAnEmptyResult()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(new string[0],
                destination);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0,
                result.Items.Count);
            Assert.AreEqual(0,
                result.Errors.Count);
            Assert.AreEqual(0,
                result.Warnings.Count);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        #endregion

        #region Errors

        /// <summary>
        /// Verifies copy table schemas to result has no errors if nothing fails.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToResultHasNoErrorsIfNothingFails()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection(),
                errorCallback: _ =>
            {
            });
            // Assert
            Assert.AreEqual(0,
                result.Errors.Count);
            Assert.IsTrue(result.Items.All(i => i.Errors.Count == 0));
        }

        /// <summary>
        /// Verifies copy table schemas to result has the errors of all the tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToResultHasTheErrorsOfAllTheTables()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c == "CREATE TABLE Country;" || c == "CREATE INDEX IX_Person;"
            };
            var raised = new List<ExecutionError>();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                destination,
                errorCallback: raised.Add);
            // Assert
            Assert.AreEqual(2,
                result.Errors.Count);
            Assert.AreEqual(raised.Count,
                result.Errors.Count);
            for (var i = 0; i < raised.Count; i++)
            {
                Assert.AreSame(raised[i],
                    result.Errors[i]);
            }

            CollectionAssert.AreEquivalent(new[] { "Country", "Person" },
                result.Errors.Select(e => e.TableName).ToArray());
        }

        /// <summary>
        /// Verifies copy table schemas to async result has the errors of all the tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncResultHasTheErrorsOfAllTheTables()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c == "CREATE TABLE Country;" || c == "CREATE INDEX IX_Person;"
            };
            var raised = new List<ExecutionError>();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(ThreeTables,
                destination,
                errorCallback: raised.Add);
            // Assert
            Assert.AreEqual(2,
                result.Errors.Count);
            Assert.AreEqual(raised.Count,
                result.Errors.Count);
            CollectionAssert.AreEquivalent(new[] { "Country", "Person" },
                result.Errors.Select(e => e.TableName).ToArray());
        }

        /// <summary>
        /// Verifies copy table schemas to item errors are the errors of the table of the item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToItemErrorsAreTheErrorsOfTheTableOfTheItem()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c == "CREATE TABLE Country;" || c == "CREATE INDEX IX_Person;"
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                destination,
                errorCallback: _ =>
            {
            });
            // Assert
            var country = result.Items.Single(i => i.Target.Table.Table.Name == "Country");
            var person = result.Items.Single(i => i.Target.Table.Table.Name == "Person");
            var solo = result.Items.Single(i => i.Target.Table.Table.Name == "Solo");
            Assert.AreEqual(1,
                country.Errors.Count);
            Assert.AreEqual(1,
                person.Errors.Count);
            Assert.AreEqual(0,
                solo.Errors.Count);
            Assert.AreEqual(TableOutcome.Failed,
                country.Outcome);
            Assert.AreEqual(TableOutcome.Failed,
                person.Outcome);
            Assert.AreEqual(TableOutcome.Created,
                solo.Outcome);
            Assert.AreSame(country.Errors[0],
                result.Errors.Single(e => e.TableName == "Country"));
            Assert.AreSame(person.Errors[0],
                result.Errors.Single(e => e.TableName == "Person"));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if astatement fails without an error callback and no result is returned.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfAStatementFailsWithoutAnErrorCallbackAndNoResultIsReturned()
        {
            // Setup
            MapThreeTables();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal)
            };
            CopyTableSchemaResult result = null;
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                    destination));
            Assert.IsNull(result);
        }

        #endregion

        #region Warnings

        /// <summary>
        /// Performs map reader and composer with warnings.
        /// </summary>
        private static void MapReaderAndComposerWithWarnings()
        {
            var country = GetSchema("Country");
            var person = GetSchema("Person",
                1,
                0,
                1);
            person.ForeignKeys[0] = new ForeignKeyInfo("FK_Person_Missing")
            {
                ReferencedTable = new TableInfo("Missing",
                    "dbo")
            };
            MapReader(country,
                person);
            var composer = MapComposer();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
        }

        /// <summary>
        /// Verifies copy table schemas to result has the warnings of all the tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToResultHasTheWarningsOfAllTheTables()
        {
            // Setup
            MapReaderAndComposerWithWarnings();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" },
                new CustomDbConnection());
            // Assert
            Assert.AreEqual(1,
                result.Warnings.Count);
            StringAssert.Contains(result.Warnings[0],
                "FK_Person_Missing",
                StringComparison.Ordinal);
            var person = result.Items.Single(i => i.Target.Table.Table.Name == "Person");
            var country = result.Items.Single(i => i.Target.Table.Table.Name == "Country");
            CollectionAssert.AreEqual(person.Warnings.ToArray(),
                result.Warnings.ToArray());
            Assert.AreEqual(0,
                country.Warnings.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to async result has the warnings of all the tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncResultHasTheWarningsOfAllTheTables()
        {
            // Setup
            MapReaderAndComposerWithWarnings();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person" },
                new CustomDbConnection());
            // Assert
            Assert.AreEqual(1,
                result.Warnings.Count);
            StringAssert.Contains(result.Warnings[0],
                "FK_Person_Missing",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies copy table schemas to result has no warnings if nothing is skipped.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToResultHasNoWarningsIfNothingIsSkipped()
        {
            // Setup
            MapThreeTables();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(ThreeTables,
                new CustomDbConnection());
            // Assert
            Assert.AreEqual(0,
                result.Warnings.Count);
        }

        #endregion

        #region Single Table

        /// <summary>
        /// Verifies copy table schema to of asingle table returns aresult with one item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableReturnsAResultWithOneItem()
        {
            // Setup
            MapReader(GetSchema("Person",
                    3));
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection());
            // Assert
            Assert.AreEqual(1,
                result.Items.Count);
            Assert.AreEqual("Person",
                result.Items[0].Source.Table.Table.Name,
                StringComparer.Ordinal);
            Assert.AreEqual("Person",
                result.Items[0].Target.Table.Table.Name,
                StringComparer.Ordinal);
            Assert.AreEqual(3,
                result.Items[0].ColumnCount);
            Assert.IsTrue(result.EndTime >= result.StartTime);
            Assert.AreEqual(0,
                result.Errors.Count);
            Assert.AreEqual(0,
                result.Warnings.Count);
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table returns aresult with one item.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableReturnsAResultWithOneItem()
        {
            // Setup
            MapReader(GetSchema("Person",
                    3));
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                new CustomDbConnection());
            // Assert
            Assert.AreEqual(1,
                result.Items.Count);
            Assert.AreEqual("Person",
                result.Items[0].Target.Table.Table.Name,
                StringComparer.Ordinal);
            Assert.AreEqual(3,
                result.Items[0].ColumnCount);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with column mappings returns aresult with one item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithColumnMappingsReturnsAResultWithOneItem()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name",
                    "Age"));
            MapComposer("CREATE TABLE Person;");
            var mappings = new List<ColumnMapping>
            {
                new ColumnMapping("Id"),
                new ColumnMapping("Name",
                    "FullName")
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                mappings);
            // Assert
            var item = result.Items.Single();
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age" },
                item.Source.Table.Columns.Select(c => c.Field.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "Id", "FullName" },
                item.Target.Table.Columns.Select(c => c.Field.Name).ToArray());
            Assert.AreEqual(2,
                item.ColumnCount);
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with column mappings returns aresult with one item.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithColumnMappingsReturnsAResultWithOneItem()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name",
                    "Age"));
            MapComposer("CREATE TABLE Person;");
            var mappings = new List<ColumnMapping>
            {
                new ColumnMapping("Id"),
                new ColumnMapping("Name",
                    "FullName")
            };
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                new CustomDbConnection(),
                mappings);
            // Assert
            var item = result.Items.Single();
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age" },
                item.Source.Table.Columns.Select(c => c.Field.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "Id", "FullName" },
                item.Target.Table.Columns.Select(c => c.Field.Name).ToArray());
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with selected columns keeps all the columns in the source of the item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSelectedColumnsKeepsAllTheColumnsInTheSourceOfTheItem()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name",
                    "Age"));
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                columns: new[] { "Id", "Name" });
            // Assert
            var item = result.Items.Single();
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age" },
                item.Source.Table.Columns.Select(c => c.Field.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "Id", "Name" },
                item.Target.Table.Columns.Select(c => c.Field.Name).ToArray());
            Assert.AreEqual(2,
                item.ColumnCount);
        }

        /// <summary>
        /// Verifies copy table schemas to with table mappings keeps all the columns in the source of the item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTableMappingsKeepsAllTheColumnsInTheSourceOfTheItem()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name",
                    "Age"),
                GetNamedSchema("Country",
                    "Id",
                    "Name"));
            MapComposer("CREATE TABLE Person;",
                "CREATE TABLE Country;");
            var tables = new List<TableMapping>
            {
                new TableMapping("Person",
                    null,
                    new List<ColumnMapping> { new ColumnMapping("Id") })
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                new CustomDbConnection(),
                tables);
            // Assert
            var person = result.Items.Single(i => i.Source.Table.Table.Name == "Person");
            var country = result.Items.Single(i => i.Source.Table.Table.Name == "Country");
            Assert.AreEqual(3,
                person.Source.Table.Columns.Count);
            Assert.AreEqual(1,
                person.Target.Table.Columns.Count);
            Assert.AreEqual(2,
                country.Source.Table.Columns.Count);
            Assert.AreEqual(2,
                country.Target.Table.Columns.Count);
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with column mappings calls the created callback.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithColumnMappingsCallsTheCreatedCallback()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name"));
            MapComposer("CREATE TABLE Person;");
            var created = new List<CopyTableSchemaResultItem>();
            var mappings = new List<ColumnMapping>
            {
                new ColumnMapping("Id")
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                mappings,
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(1,
                created.Count);
            Assert.AreSame(created[0],
                result.Items.Single());
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with column mappings calls the created callback.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithColumnMappingsCallsTheCreatedCallback()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name"));
            MapComposer("CREATE TABLE Person;");
            var created = new List<CopyTableSchemaResultItem>();
            var mappings = new List<ColumnMapping>
            {
                new ColumnMapping("Id")
            };
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                new CustomDbConnection(),
                mappings,
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(1,
                created.Count);
            Assert.AreSame(created[0],
                result.Items.Single());
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with selected columns calls the created callback.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSelectedColumnsCallsTheCreatedCallback()
        {
            // Setup
            MapReader(GetNamedSchema("Person",
                    "Id",
                    "Name"));
            MapComposer("CREATE TABLE Person;");
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                columns: new[] { "Id" },
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(1,
                created.Count);
            Assert.AreSame(created[0],
                result.Items.Single());
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table returns the errors in the result and in the item.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableReturnsTheErrorsInTheResultAndInTheItem()
        {
            // Setup
            MapReader(GetSchema("Person",
                    1,
                    1,
                    0));
            MapComposer("CREATE TABLE Person;",
                "CREATE INDEX IX_Person;");
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("CREATE INDEX",
                    StringComparison.Ordinal)
            };
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                destination,
                errorCallback: _ =>
            {
            });
            // Assert
            Assert.AreEqual(1,
                result.Errors.Count);
            Assert.AreEqual(1,
                result.Items.Single().Errors.Count);
            Assert.AreSame(result.Errors[0],
                result.Items.Single().Errors[0]);
            Assert.AreEqual(TableOutcome.Failed,
                result.Items.Single().Outcome);
        }
        #endregion
    }
}
