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
            var schema = new TableSchema { SchemaName = "dbo", TableName = tableName };
            for (var i = 0; i < columns; i++) schema.Columns.Add(new ColumnInfo());
            for (var i = 0; i < indexes; i++) schema.Indexes.Add(new IndexInfo());
            for (var i = 0; i < foreignKeys; i++) schema.ForeignKeys.Add(new ForeignKeyInfo());
            return schema;
        }

        private static List<RelationshipInfo> GetRelationships(params TableSchema[] schemas) =>
            schemas.Select(s => new RelationshipInfo { Table = s }).ToList();

        // The reader returns the relationships (the tables in the order that they must be created)
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
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.TableName};" });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
            return composer;
        }

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                connection.CopySchemasTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTableNamesAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemasTo(null, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemasTo(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemasTo(new[] { "Person", null }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemasTo(new[] { "Person", "  " }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemasTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemasTo(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopySchemasToWithoutTablesDoesNothing()
        {
            // Setup (there is no mapping, as nothing is read and nothing is composed)
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopySchemasTo(Enumerable.Empty<string>(), destination);

            // Assert
            Assert.AreEqual(0, result.TableCount);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        [TestMethod]
        public void TestCopySchemasToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "CREATE INDEX IX_Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, destination);

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
            MapComposer("CREATE TABLE Country;");

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "dbo.Person", "Country" }, new CustomDbConnection());

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
            var composer = MapComposer("CREATE TABLE Country;");

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(s => s.SequenceEqual(new[] { country, person }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemasToResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            Assert.AreEqual(2, result.TableCount);
            Assert.AreEqual(nameof(CustomDbConnection), result.SourceDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), result.DestinationDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaExistsBehavior.SkipOnExists, result.Action);
            Assert.AreEqual("CREATE TABLE Country;" + Environment.NewLine + "CREATE TABLE Person;", result.Script, StringComparer.Ordinal);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        [TestMethod]
        public void TestCopySchemasToResultOfEachTable()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert (in the order that the tables were created)
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
            var person = result.Tables[1];
            Assert.AreEqual("dbo", person.SourceSchema, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Created, person.Outcome);
            Assert.AreEqual(4, person.ColumnCount);
            Assert.AreEqual(1, person.IndexCount);
            Assert.AreEqual(1, person.ForeignKeyCount);
            Assert.AreEqual("CREATE TABLE Person;", person.Script, StringComparer.Ordinal);
            Assert.AreEqual(result.StartTime, person.StartTime);
            Assert.AreEqual(result.EndTime, person.EndTime);
            Assert.AreEqual(2, result.Tables[0].ColumnCount);
        }

        [TestMethod]
        public void TestCopySchemasToResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "Person" }, new CustomDbConnection(), CopySchemaExistsBehavior.DropOnExists);

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, result.Action);
            Assert.AreEqual(CopySchemaExistsBehavior.DropOnExists, result.Tables.Single().Action);
        }

        [TestMethod]
        public void TestCopySchemasToResultOfTheSameTableIsNotDuplicated()
        {
            // Setup (the reader returns one relationship for the table that is given twice)
            MapReader(GetRelationships(GetSchema("Person")));
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "Person", "dbo.Person" }, new CustomDbConnection());

            // Assert
            Assert.AreEqual(1, result.TableCount);
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
                connection.CopySchemasToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheTableNamesAreNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemasToAsync(null, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemasToAsync(new[] { "Person" }, (IDbConnection)null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfATableNameIsWhiteSpace()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE Person;");

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemasToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Person")));

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemasToAsync(new[] { "Person" }, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTablesDoesNothing()
        {
            // Setup
            var destination = new CustomDbConnection();

            // Act
            var result = await new CustomDbConnection().CopySchemasToAsync(Enumerable.Empty<string>(), destination);

            // Assert
            Assert.AreEqual(0, result.TableCount);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country"), GetSchema("Person")));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;", "ALTER TABLE Person ADD FK;");
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "Country" }, destination);

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
            await new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncResult()
        {
            // Setup
            MapReader(GetRelationships(GetSchema("Country", 2), GetSchema("Person", 4, 1, 1)));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

            // Act
            var result = await new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "Country" }, new CustomDbConnection(), CopySchemaExistsBehavior.AlignOnExists);

            // Assert
            Assert.AreEqual(2, result.TableCount);
            Assert.AreEqual(CopySchemaExistsBehavior.AlignOnExists, result.Action);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
            Assert.AreEqual(4, result.Tables[1].ColumnCount);
            Assert.AreEqual("CREATE TABLE Country;" + Environment.NewLine + "CREATE TABLE Person;", result.Script, StringComparer.Ordinal);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        #endregion
    }
}
