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
    public class CopyTableSchemasToRelationshipBehaviorTest
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

        private static Mock<ISchemaReader> MapReader(string[] related, params string[] ordered)
        {
            var relationships = ordered.Select(name => new RelationshipInfo { Schema = GetSchema(name) }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>())).Returns(related);
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).ReturnsAsync(related);
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
        public void TestCopyTableSchemasToWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>()), Times.Never);
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" }))), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTableOnlyDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection(), relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>()), Times.Never);
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
                MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");

                // Act
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, new CustomDbConnection(), relationshipBehavior: behavior);

                // Assert
                reader.Verify(r => r.GetRelatedTables(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })), behavior), Times.Once);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                destination,
                relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents,
                createdCallback: r => results.Add(r.Target.Table.Table.Name));

            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" }))), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorAndNoTablesDoesNothing()
        {
            // Setup
            var reader = MapReader(new string[0]);
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(Enumerable.Empty<string>(), destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>()), Times.Never);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>()), Times.Never);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
                MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
                using (var source = new CancellationTokenSource())
                {
                    // Act
                    await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                        new CustomDbConnection(),
                        relationshipBehavior: behavior,
                        cancellationToken: source.Token);

                    // Assert
                    reader.Verify(r => r.GetRelatedTablesAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })), behavior, source.Token), Times.Once);
                }
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                destination,
                relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents,
                createdCallback: r => results.Add(r.Target.Table.Table.Name));

            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })), It.IsAny<CancellationToken>()), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection());

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Single Table

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Country", destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })), CopyTableSchemaRelationshipBehavior.Children), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(1, result.Items.Count(i => i.Target.Table.Table.Name == "Country"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTablesWhenItIsCreatedFirst()
        {
            // Setup
            MapReader(new[] { "Person", "Country" }, "Person", "Country");
            MapComposer("CREATE TABLE Person;", "CREATE TABLE Country;");

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection(), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(1, result.Items.Count(i => i.Target.Table.Table.Name == "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person", new CustomDbConnection(), relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Person", StringComparison.OrdinalIgnoreCase));

            // Assert
            Assert.AreEqual("Person", result.Target.Table.Table.Name);
            reader.Verify(r => r.GetTableSchema(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" }, "Country", nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo<RelationshipTestEntity>(destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), CopyTableSchemaRelationshipBehavior.Parents), Times.Once);
            Assert.AreEqual(2, destination.ExecutedCommands.Count);
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(1, result.Items.Count(i => i.Target.Table.Table.Name == nameof(RelationshipTestEntity)));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer("CREATE TABLE Country;", "CREATE TABLE Person;");
            var destination = new CustomDbConnection();

            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Country", destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children));

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })), CopyTableSchemaRelationshipBehavior.Children, It.IsAny<CancellationToken>()), Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" }, destination.ExecutedCommands);
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(1, result.Items.Count(i => i.Target.Table.Table.Name == "Country"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            MapComposer("CREATE TABLE Person;");

            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Person", new CustomDbConnection(), relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Person", StringComparison.OrdinalIgnoreCase));

            // Assert
            Assert.AreEqual("Person", result.Target.Table.Table.Name);
            reader.Verify(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" }, "Country", nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;", "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();

            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync<RelationshipTestEntity>(destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents));

            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), CopyTableSchemaRelationshipBehavior.Parents, It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(2, destination.ExecutedCommands.Count);
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(1, result.Items.Count(i => i.Target.Table.Table.Name == nameof(RelationshipTestEntity)));
        }

        #endregion
    }
}
