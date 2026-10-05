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
    public class CopySchemasToTest
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

        private static Action<CopySchemaResult> Collect(CustomDbConnection destination, List<(string Table, int Executed)> reported) =>
            result => reported.Add((result.TableName, destination.ExecutedCommands.Count));

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                connection.CopySchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTableNamesAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person", null }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person", "  " }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopySchemasToWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(Enumerable.Empty<string>(), destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public void TestCopySchemasToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopySchemasToReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "dbo.Person", "Country" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person", "Country" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemasToComposesTheSchemasInTheOrderThatTheReaderGave()
        {
            // Setup
            var country = GetSchema("Country");
            var person = GetSchema("Person");
            MapReader(GetRelationships(country, person));
            var composer = MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(s => s.SequenceEqual(new[] { country, person }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemasToWithoutCallback()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person" }, destination, createdCallback: null);

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count);
        }

        #endregion

        #region Callback

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackForEachTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, reported.Select(r => r.Table).ToArray());
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 1), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackAfterTheIndexesOfTheTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person", 1, 2, 0), GetSchema("Solo")));
            MapComposer("CREATE TABLE Person;", "CREATE TABLE Solo;", "CREATE INDEX IX_1;", "CREATE INDEX IX_2;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Solo" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Solo", 2), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackAfterTheForeignKeysOfTheTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("A", 1, 0, 1), GetSchema("B", 1, 0, 1)));
            MapComposer("CREATE TABLE A;", "CREATE TABLE B;", "ALTER TABLE A ADD FK;", "ALTER TABLE B ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "A", "B" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("A", 3), reported[0]);
            Assert.AreEqual(("B", 4), reported[1]);
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackOfTheTableWithoutObjectsBeforeTheOthersAreCompleted()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Solo"), GetSchema("Child", 1, 0, 1)));
            MapComposer("CREATE TABLE Solo;", "CREATE TABLE Child;", "ALTER TABLE Child ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Solo", "Child" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Solo", 1), reported[0]);
            Assert.AreEqual(("Child", 3), reported[1]);
        }

        [TestMethod]
        public void TestCopySchemasToCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 2), reported[0]);
            Assert.AreEqual(("Person", 2), reported[1]);
        }

        [TestMethod]
        public void TestCopySchemasToCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            var person = results.Single(r => r.TableName == "Person");
            Assert.AreEqual("dbo", person.SourceSchema, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), person.SourceDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), person.DestinationDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
            Assert.AreEqual(CopySchemaExistsBehavior.Skip, person.Action);
            Assert.AreEqual(4, person.ColumnCount);
            Assert.AreEqual(1, person.IndexCount);
            Assert.AreEqual(1, person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;", person.Script, StringComparer.Ordinal);
            Assert.IsTrue(person.EndTime >= person.StartTime);
            Assert.AreEqual(2, results.Single(r => r.TableName == "Country").ColumnCount);
        }

        [TestMethod]
        public void TestCopySchemasToCallbackResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection(), CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.Drop, results.Single().Action);
        }

        [TestMethod]
        public void TestCopySchemasToCallbackIsCalledOnceForTheTableThatIsGivenTwice()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "dbo.Person" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheCallbackThrows()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection(), createdCallback: _ => throw new InvalidOperationException()));
        }

        #endregion

        #region CopySchemasToAsync

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                connection.CopySchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheTableNamesAreNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaToAsync((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(Enumerable.Empty<string>(), destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncReadsTheGivenTables()
        {
            // Setup
            var reader = MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(("Country", 1), reported[0]);
            Assert.AreEqual(("Person", 4), reported[1]);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallsTheCallbackForAllTheTablesAfterTheScriptIfTheStatementsOfTheTablesAreNotKnown()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person", 1, 0, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination, createdCallback: Collect(destination, reported));

            // Assert
            Assert.AreEqual(("Country", 2), reported[0]);
            Assert.AreEqual(("Person", 2), reported[1]);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncCallbackResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var results = new List<CopySchemaResult>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, new CustomDbConnection(), CopySchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(2, results.Count);
            var person = results.Single(r => r.TableName == "Person");
            Assert.AreEqual(CopySchemaExistsBehavior.Align, person.Action);
            Assert.AreEqual(4, person.ColumnCount);
            Assert.AreEqual(1, person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;", person.Script, StringComparer.Ordinal);
            Assert.IsTrue(person.EndTime >= person.StartTime);
        }

        #endregion
    }
}
