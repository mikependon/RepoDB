#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
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
        /// Performs get relationships.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static List<RelationshipInfo> GetRelationships(params TableSchema[] schemas) => schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        private static Mock<ISchemaReader> MapReader(List<RelationshipInfo> relationships)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
            return reader;
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
        /// Performs collect.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="reported">The reported.</param>
        private static Action<CopyTableSchemaResultItem> Collect(CustomDbConnection destination,
            List<(string Table, int Executed)> reported) => result => reported.Add((result.Target.Table.Table.Name, destination.ExecutedCommands.Count));
        #endregion

        #region CopyTableSchemasTo

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => connection.CopyTableSchemaTo(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTableNamesAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaTo((string)null,
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the destination connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    (IDbConnection)null));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if atable name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", null },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if atable name is white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "  " },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if there is no mapped schema reader.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");
            // Act/Assert
            Assert.Throws<MissingMappingException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if there is no mapped schema composer.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            // Act/Assert
            Assert.Throws<MissingMappingException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies copy table schemas to without tables does nothing.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(Enumerable.Empty<string>(),
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(0,
                reported.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to executes the composed statements in order.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        1,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to reads the given tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person")));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "dbo.Person", "Country" },
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person", "Country" })),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies copy table schemas to composes the schemas in the order that the reader gave.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToComposesTheSchemasInTheOrderThatTheReaderGave()
        {
            // Setup
            var country = GetSchema("Country");
            var person = GetSchema("Person");
            MapReader(GetRelationships(country,
                    person));
            var composer = MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                new CustomDbConnection());
            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(s => s.SequenceEqual(new[] { country, person }))),
                Times.Once);
        }

        /// <summary>
        /// Verifies copy table schemas to without callback.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithoutCallback()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                destination,
                createdCallback: null);
            // Assert
            Assert.AreEqual(1,
                destination.ExecutedCommands.Count);
        }

        #endregion

        #region Callback

        /// <summary>
        /// Verifies copy table schemas to calls the callback for each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForEachTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person")));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                reported.Select(r => r.Table).ToArray());
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback once the schema of the table is created.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        1,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(2,
                reported.Count);
            Assert.AreEqual(("Country", 1),
                reported[0]);
            Assert.AreEqual(("Person", 4),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback after the indexes of the table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackAfterTheIndexesOfTheTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person",
                        1,
                        2,
                        0),
                    GetSchema("Solo")));
            MapComposer("CREATE TABLE Person;",
                "CREATE TABLE Solo;",
                "CREATE INDEX IX_1;",
                "CREATE INDEX IX_2;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Solo" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(("Solo", 2),
                reported[0]);
            Assert.AreEqual(("Person", 4),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback after the foreign keys of the tables that reference each other.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackAfterTheForeignKeysOfTheTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("A",
                        1,
                        0,
                        1),
                    GetSchema("B",
                        1,
                        0,
                        1)));
            MapComposer("CREATE TABLE A;",
                "CREATE TABLE B;",
                "ALTER TABLE A ADD FK;",
                "ALTER TABLE B ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "A", "B" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(("A", 3),
                reported[0]);
            Assert.AreEqual(("B", 4),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback of the table without objects before the others are completed.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackOfTheTableWithoutObjectsBeforeTheOthersAreCompleted()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Solo"),
                    GetSchema("Child",
                        1,
                        0,
                        1)));
            MapComposer("CREATE TABLE Solo;",
                "CREATE TABLE Child;",
                "ALTER TABLE Child ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Solo", "Child" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(("Solo", 1),
                reported[0]);
            Assert.AreEqual(("Child", 3),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to calls the callback for all the tables after the script if the statements of the tables are not known.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        0,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(2,
                reported.Count);
            Assert.AreEqual(("Country", 2),
                reported[0]);
            Assert.AreEqual(("Person", 2),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to callback result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country",
                        2),
                    GetSchema("Person",
                        4,
                        1,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            var run = new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                new CustomDbConnection(),
                createdCallback: results.Add);
            // Assert
            var person = results.Single(r => r.Target.Table.Table.Name == "Person");
            Assert.AreEqual("dbo",
                person.Source.Table.Table.Schema,
                StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection),
                person.Source.DatabaseType,
                StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection),
                person.Target.DatabaseType,
                StringComparer.Ordinal);
            Assert.AreEqual(TableOutcome.Created,
                person.Outcome);
            Assert.AreEqual(TableExistsBehavior.Skip,
                person.Action);
            Assert.AreEqual(4,
                person.ColumnCount);
            Assert.AreEqual(1,
                person.IndexCount);
            Assert.AreEqual(1,
                person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;",
                person.Script,
                StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
            Assert.AreEqual(2,
                results.Single(r => r.Target.Table.Table.Name == "Country").ColumnCount);
        }

        /// <summary>
        /// Verifies copy table schemas to callback result action is the requested exists behavior.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallbackResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                new CustomDbConnection(),
                tableExistenceBehavior: TableExistsBehavior.Drop,
                createdCallback: results.Add);
            // Assert
            Assert.AreEqual(TableExistsBehavior.Drop,
                results.Single().Action);
        }

        /// <summary>
        /// Verifies copy table schemas to callback is called once for the table that is given twice.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToCallbackIsCalledOnceForTheTableThatIsGivenTwice()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "dbo.Person" },
                new CustomDbConnection(),
                createdCallback: results.Add);
            // Assert
            Assert.AreEqual(1,
                results.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the callback throws.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheCallbackThrows()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    new CustomDbConnection(),
                    createdCallback: _ => throw new InvalidOperationException()));
        }

        #endregion

        #region CopyTableSchemasToAsync

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if the connection is null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => connection.CopyTableSchemaToAsync(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheTableNamesAreNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaToAsync((string)null,
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if the destination connection is null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                    (IDbConnection)null));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if atable name is white space.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if there is no mapped schema reader.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if there is no mapped schema composer.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies copy table schemas to async without tables does nothing.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(Enumerable.Empty<string>(),
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(0,
                reported.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to async executes the composed statements in order.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        0,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to async reads the given tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies copy table schemas to async calls the callback once the schema of the table is created.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        1,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(2,
                reported.Count);
            Assert.AreEqual(("Country", 1),
                reported[0]);
            Assert.AreEqual(("Person", 4),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to async calls the callback for all the tables after the script if the statements of the tables are not known.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"),
                    GetSchema("Person",
                        1,
                        0,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination,
                createdCallback: Collect(destination,
                    reported));
            // Assert
            Assert.AreEqual(("Country", 2),
                reported[0]);
            Assert.AreEqual(("Person", 2),
                reported[1]);
        }

        /// <summary>
        /// Verifies copy table schemas to async callback result.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country",
                        2),
                    GetSchema("Person",
                        4,
                        1,
                        1)));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;",
                "CREATE INDEX IX_Person;",
                "ALTER TABLE Person ADD FK;");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            var run = await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                new CustomDbConnection(),
                tableExistenceBehavior: TableExistsBehavior.Align,
                createdCallback: results.Add);
            // Assert
            Assert.AreEqual(2,
                results.Count);
            var person = results.Single(r => r.Target.Table.Table.Name == "Person");
            Assert.AreEqual(TableExistsBehavior.Align,
                person.Action);
            Assert.AreEqual(4,
                person.ColumnCount);
            Assert.AreEqual(1,
                person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;",
                person.Script,
                StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
        }
        #endregion
    }
}
