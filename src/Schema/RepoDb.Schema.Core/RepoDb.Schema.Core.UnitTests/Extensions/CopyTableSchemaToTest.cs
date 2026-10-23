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
    public class CopyTableSchemaToTest
    {
        public class CopyTableSchemaToTestEntity
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

        #region CopyTableSchemaTo

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                connection.CopyTableSchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheTableNameIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheTableNameIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("  ", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetSchema());

            // Act/Assert
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopyTableSchemaToExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemaToReadsTheSchemaOfTheGivenTable()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("dbo.Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemaToComposesTheSchemaThatWasRead()
        {
            // Setup
            var schema = GetSchema();
            MapReader(schema);
            var composer = MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection());

            // Assert
            composer.Verify(c => c.ComposeSchemas(It.Is<IEnumerable<TableSchema>>(t => t.SequenceEqual(new[] { schema }))), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemaToResult()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");

            // Act
            var run = new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection());
            var result = run.Items.Single();

            // Assert
            Assert.AreEqual("Person", result.Target.Table.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", result.Source.Table.Table.Schema, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), result.Source.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomDbConnection), result.Target.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
            Assert.AreEqual(3, result.ColumnCount);
            Assert.AreEqual(1, result.IndexCount);
            Assert.AreEqual(2, result.ForeignKeyCount);
            Assert.AreEqual(1, result.UniqueConstraintCount);
            Assert.AreEqual(1, result.CheckConstraintCount);
            Assert.AreEqual(
                "CREATE TABLE [Person] ([Id] int);" + Environment.NewLine + "CREATE INDEX [IX_Person] ON [Person] ([Id]);",
                result.Script,
                StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
        }

        [TestMethod]
        public void TestCopyTableSchemaToResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection(), tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop).Items.Single();

            // Assert
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, result.Action);
        }

        [TestMethod]
        public void TestCopyTableSchemaToResultActionIsSkipByDefault()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection()).Items.Single();

            // Assert
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, result.Action);
        }

        [TestMethod]
        public void TestCopyTableSchemaToViaEntityUsesTheMappedTableName()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            new CustomDbConnection().CopyTableSchemaTo<CopyTableSchemaToTestEntity>(new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { nameof(CopyTableSchemaToTestEntity) }))), Times.Once);
        }

        #endregion

        #region CopyTableSchemaToAsync

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheConnectionIsNull()
        {
            // Setup
            IDbConnection connection = null;

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                connection.CopyTableSchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheTableNameIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync((string)null, new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync("Person", (IDbConnection)null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfThereIsNoMappedSchemaReader()
        {
            // Setup
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfThereIsNoMappedSchemaComposer()
        {
            // Setup
            MapReader(GetSchema());

            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncExecutesTheComposedStatementsInOrder()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);");
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", destination);

            // Assert
            CollectionAssert.AreEqual(
                new[] { "CREATE TABLE [Person] ([Id] int);", "CREATE INDEX [IX_Person] ON [Person] ([Id]);" },
                destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncReadsTheSchemaOfTheGivenTable()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("dbo.Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "dbo.Person" })), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncResult()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var run = await new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection());
            var result = run.Items.Single();

            // Assert
            Assert.AreEqual("Person", result.Target.Table.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", result.Source.Table.Table.Schema, StringComparer.Ordinal);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
            Assert.AreEqual(3, result.ColumnCount);
            Assert.AreEqual(1, result.IndexCount);
            Assert.AreEqual(2, result.ForeignKeyCount);
            Assert.AreEqual(1, result.UniqueConstraintCount);
            Assert.AreEqual(1, result.CheckConstraintCount);
            Assert.AreEqual("CREATE TABLE [Person] ([Id] int);", result.Script, StringComparer.Ordinal);
            Assert.IsTrue(run.EndTime >= run.StartTime);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncResultActionIsTheRequestedExistsBehavior()
        {
            // Setup
            MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection(), tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align)).Items.Single();

            // Assert
            Assert.AreEqual(CopyTableSchemaExistsBehavior.Align, result.Action);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncViaEntityUsesTheMappedTableName()
        {
            // Setup
            var reader = MapReader(GetSchema());
            MapComposer("CREATE TABLE [Person] ([Id] int);");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync<CopyTableSchemaToTestEntity>(new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { nameof(CopyTableSchemaToTestEntity) })), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
