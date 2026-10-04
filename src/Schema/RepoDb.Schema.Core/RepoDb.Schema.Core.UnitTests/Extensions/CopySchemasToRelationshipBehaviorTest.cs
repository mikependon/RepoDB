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
    [TestClass]
    public class CopySchemasToRelationshipBehaviorTest
    {
        public class RelationshipTestEntity
        {
            public int Id { get; set; }
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

        private static TableSchema GetSchema(string tableName) =>
            new TableSchema(tableName, "dbo") { Columns = { new ColumnInfo() } };

        // The reader expands the given tables with the related ones, and then returns the relationships of the expanded tables
        private static Mock<ISchemaReader> MapReader(string[] related, params string[] ordered)
        {
            var relationships = ordered.Select(name => new RelationshipInfo { Schema = GetSchema(name) }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>())).Returns(related);
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).ReturnsAsync(related);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(GetSchema);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((n, _) => Task.FromResult(GetSchema(n)));
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

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestCopySchemasToWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>()), Times.Never);
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopySchemasToWithTableOnlyDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection(), relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>()), Times.Never);
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestCopySchemasToWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
                MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

                // Act
                new CustomDbConnection().CopySchemaTo(new[] { "Person" }, new CustomDbConnection(), relationshipBehavior: behavior);

                // Assert
                reader.Verify(r => r.GetRelatedTables(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })), behavior), Times.Once);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup (the reader expands Person with its parent Country)
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person" },
                destination,
                relationshipBehavior: CopySchemaRelationshipBehavior.Parents,
                createdCallback: r => results.Add(r.TableName));

            // Assert (the tables that were expanded are the ones that are ordered, composed and executed)
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" }))), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results);
        }

        [TestMethod]
        public void TestCopySchemasToWithRelationshipBehaviorAndNoTablesDoesNothing()
        {
            // Setup (nothing is read, as there is nothing to expand)
            var reader = MapReader(new string[0]);
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(Enumerable.Empty<string>(), destination, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>()), Times.Never);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>()), Times.Never);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
                MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
                using (var source = new CancellationTokenSource())
                {
                    // Act
                    await new CustomDbConnection().CopySchemaToAsync(new[] { "Person" },
                        new CustomDbConnection(),
                        relationshipBehavior: behavior,
                        cancellationToken: source.Token);

                    // Assert
                    reader.Verify(r => r.GetRelatedTablesAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })), behavior, source.Token), Times.Once);
                }
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup (the reader expands Person with its parent Country)
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person" },
                destination,
                relationshipBehavior: CopySchemaRelationshipBehavior.Parents,
                createdCallback: r => results.Add(r.TableName));

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })), It.IsAny<CancellationToken>()), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Single Table

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTable()
        {
            // Setup (the given table is created last, so it is not simply the last result)
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Country", destination, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

            // Assert (the related table is copied too)
            reader.Verify(r => r.GetRelatedTables(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })), CopySchemaRelationshipBehavior.Children), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            Assert.AreEqual("Country", result.TableName);
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableWhenItIsCreatedFirst()
        {
            // Setup
            MapReader(new[] { "Person", "Country" }, "Person", "Country");
            MapComposer("CREATE TABLE Person;", "CREATE TABLE Country;");

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection(), relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual("Person", result.TableName);
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup (there is nothing related, so there is nothing to find)
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopySchemaTo("Person", new CustomDbConnection(), relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual("Person", result.TableName);
            reader.Verify(r => r.GetTableSchema(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestCopySchemaToOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" }, "Country", nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopySchemaTo<RelationshipTestEntity>(destination, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), CopySchemaRelationshipBehavior.Parents), Times.Once);
            Assert.AreEqual(2, destination.ExecutedCommands.Count);
            Assert.AreEqual(nameof(RelationshipTestEntity), result.TableName);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTable()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            var result = await new CustomDbConnection().CopySchemaToAsync("Country", destination, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })), CopySchemaRelationshipBehavior.Children, It.IsAny<CancellationToken>()), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            Assert.AreEqual("Country", result.TableName);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = await new CustomDbConnection().CopySchemaToAsync("Person", new CustomDbConnection(), relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual("Person", result.TableName);
            reader.Verify(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" }, "Country", nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();

            // Act
            var result = await new CustomDbConnection().CopySchemaToAsync<RelationshipTestEntity>(destination, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), CopySchemaRelationshipBehavior.Parents, It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(2, destination.ExecutedCommands.Count);
            Assert.AreEqual(nameof(RelationshipTestEntity), result.TableName);
        }

        #endregion
    }
}
