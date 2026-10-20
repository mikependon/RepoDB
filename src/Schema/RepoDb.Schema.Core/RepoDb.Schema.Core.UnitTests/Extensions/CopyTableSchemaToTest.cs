#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopySchemaToTest
    {
        public class CopySchemaToTestEntity
        {
        }

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

        private static TableSchema GetSchema() =>
            new TableSchema("Person", "dbo")
            {
                Columns = { new ColumnInfo(), new ColumnInfo(), new ColumnInfo() },
                Indexes = { new IndexInfo(null) },
                ForeignKeys = { new ForeignKeyInfo(null), new ForeignKeyInfo(null) },
                UniqueConstraints = { new UniqueConstraintInfo(null) },
                CheckConstraints = { new CheckConstraintInfo(null) }
            };

        private static Mock<ISchemaReader> MapReader(TableSchema schema)
        {
            var reader = new Mock<ISchemaReader>();
            var relationships = new List<RelationshipInfo> { new RelationshipInfo { Schema = schema } };
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
            return reader;
        }

        private static Mock<ISchemaComposer> MapComposer(params string[] statements)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns(statements);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns(statements);
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
            return composer;
        }

        #endregion

        #region CopySchemaTo

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                connection.CopySchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheTableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheTableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo("  ", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetSchema());

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopySchemaToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopySchemaToReadsTheSchemaOfTheGivenTable()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopySchemaTo("dbo.Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemaToComposesTheSchemaThatWasRead()
        {
            // Setup
            var schema = GetSchema();
            MapReader(schema);
            var composer = MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection());

            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(t => t.SequenceEqual(new[] { schema }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemaToResult()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection());

            // Assert
            Assert.AreEqual("Person", result.TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", result.SourceSchema, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), result.SourceDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), result.DestinationDatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
            Assert.AreEqual(3, result.ColumnCount);
            Assert.AreEqual(1, result.IndexCount);
            Assert.AreEqual(2, result.ForeignKeyCount);
            Assert.AreEqual(1, result.UniqueConstraintCount);
            Assert.AreEqual(1, result.CheckConstraintCount);
            Assert.AreEqual(
                "CREATE TABLE [Person] ([Id] int);" + Environment.NewLine + "CREATE INDEX [IX_Person] ON [Person] ([Id]);",
                result.Script,
                StringComparer.Ordinal);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        [TestMethod]
        public void TestCopySchemaToResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection(), tableExistenceBehavior: CopySchemaExistsBehavior.Drop);

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.Drop, result.Action);
        }

        [TestMethod]
        public void TestCopySchemaToResultActionIsSkipByDefault()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection());

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.Skip, result.Action);
        }

        [TestMethod]
        public void TestCopySchemaToViaEntityUsesTheMappedTableName()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopySchemaTo<CopySchemaToTestEntity>(new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { nameof(CopySchemaToTestEntity) }))), Times.Once);
        }

        #endregion

        #region CopySchemaToAsync

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                connection.CopySchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfTheTableNameIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaToAsync((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopySchemaToAsync("Person", (IDbConnection)null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetSchema());

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemaToAsync("Person", destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncReadsTheSchemaOfTheGivenTable()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            await new CustomDbConnection().CopySchemaToAsync("dbo.Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncResult()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = await new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection());

            // Assert
            Assert.AreEqual("Person", result.TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", result.SourceSchema, StringComparer.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
            Assert.AreEqual(3, result.ColumnCount);
            Assert.AreEqual(1, result.IndexCount);
            Assert.AreEqual(2, result.ForeignKeyCount);
            Assert.AreEqual(1, result.UniqueConstraintCount);
            Assert.AreEqual(1, result.CheckConstraintCount);
            Assert.AreEqual("CREATE TABLE [Person] ([Id] int);", result.Script, StringComparer.Ordinal);
            Assert.IsTrue(result.EndTime >= result.StartTime);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = await new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection(), tableExistenceBehavior: CopySchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(CopySchemaExistsBehavior.Align, result.Action);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncViaEntityUsesTheMappedTableName()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            await new CustomDbConnection().CopySchemaToAsync<CopySchemaToTestEntity>(new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { nameof(CopySchemaToTestEntity) })), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
