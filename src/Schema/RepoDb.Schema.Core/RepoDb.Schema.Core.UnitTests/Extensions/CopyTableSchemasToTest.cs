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
    [TestClass]
    public class CopyTableSchemasToTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Helpers

        private static TableSchema GetSchema(string tableName, int columns = 1, int indexes = 0, int foreignKeys = 0)
        {
            var schema = new TableSchema(tableName, "dbo");
            for (var i = 0; i < columns; i++) schema.Columns.Add(new ColumnInfo());
            for (var i = 0; i < indexes; i++) schema.Indexes.Add(new IndexInfo(null));
            for (var i = 0; i < foreignKeys; i++) schema.ForeignKeys.Add(new ForeignKeyInfo(null));
            return schema;
        }

        private static List<RelationshipInfo> GetRelationships(params TableSchema[] schemas) =>
            schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();

        private static Mock<ISchemaReader> MapReader(List<RelationshipInfo> relationships)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
            return reader;
        }

        private static Mock<ISchemaComposer> MapComposer(params string[] statements)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns(statements);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
            return composer;
        }

        private static Action<CopyTableSchemaResultItem> Collect(CustomDbConnection destination, List<(string Table, int Executed)> reported) =>
            result => reported.Add((result.Target.Table.Table.Name, destination.ExecutedCommands.Count));

        #endregion

        #region CopyTableSchemasTo

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                connection.CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTableNamesAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", null }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "  " }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(Enumerable.Empty<string>(), destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemasToReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "dbo.Person", "Country" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person", "Country" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemasToComposesTheSchemasInTheOrderThatTheReaderGave()
        {
            // Setup
            var country = GetSchema("Country");
            var person = GetSchema("Person");
            MapReader(GetRelationships(country, person));
            var composer = MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(s => s.SequenceEqual(new[] { country, person }))), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutCallback()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, destination, createdCallback: null);

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count);
        }

        #endregion

        #region Callback

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForEachTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, reported.Select(r => r.Table).ToArray());
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 1), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackAfterTheIndexesOfTheTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person", 1, 2, 0), GetSchema("Solo")));
            MapComposer("CREATE TABLE Person;", "CREATE TABLE Solo;", "CREATE INDEX IX_1;", "CREATE INDEX IX_2;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Solo" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Solo", 2), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackAfterTheForeignKeysOfTheTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("A", 1, 0, 1), GetSchema("B", 1, 0, 1)));
            MapComposer("CREATE TABLE A;", "CREATE TABLE B;", "ALTER TABLE A ADD FK;", "ALTER TABLE B ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "A", "B" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("A", 3), reported[0]);
            Assert.AreEqual(("B", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackOfTheTableWithoutObjectsBeforeTheOthersAreCompleted()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Solo"), GetSchema("Child", 1, 0, 1)));
            MapComposer("CREATE TABLE Solo;", "CREATE TABLE Child;", "ALTER TABLE Child ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Solo", "Child" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Solo", 1), reported[0]);
            Assert.AreEqual(("Child", 3), reported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 2), reported[0]);
            Assert.AreEqual(("Person", 2), reported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            var run = new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            var person = results.Single(r => r.Target.Table.Table.Name == "Person");
            Assert.AreEqual("dbo", person.Source.Table.Table.Schema, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), person.Source.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), person.Target.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, person.Action);
            Assert.AreEqual(4, person.ColumnCount);
            Assert.AreEqual(1, person.IndexCount);
            Assert.AreEqual(1, person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;", person.Script, StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
            Assert.AreEqual(2, results.Single(r => r.Target.Table.Table.Name == "Country").ColumnCount);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection(), tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, results.Single().Action);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCallbackIsCalledOnceForTheTableThatIsGivenTwice()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "dbo.Person" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheCallbackThrows()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection(), createdCallback: _ => throw new InvalidOperationException()));
        }

        #endregion

        #region CopyTableSchemasToAsync

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                connection.CopyTableSchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheTableNamesAreNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(Enumerable.Empty<string>(), destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 1), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Country", 2), reported[0]);
            Assert.AreEqual(("Person", 2), reported[1]);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            var run = await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, new CustomDbConnection(), tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(2, results.Count);
            var person = results.Single(r => r.Target.Table.Table.Name == "Person");
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Align, person.Action);
            Assert.AreEqual(4, person.ColumnCount);
            Assert.AreEqual(1, person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;", person.Script, StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
        }

        #endregion
    }
}
