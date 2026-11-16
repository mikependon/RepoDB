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
    /// Contains the copy table schemas to relationship behavior test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToRelationshipBehaviorTest
    {
        /// <summary>
        /// Contains the relationship test entity behavior.
        /// </summary>
        public class RelationshipTestEntity
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }
        }

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
        private static TableSchema GetSchema(string tableName) => new TableSchema(tableName,
            "dbo")
        {
            Columns =
            {
                new ColumnInfo()
            }
        };

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="related">The related.</param>
        /// <param name="ordered">The ordered.</param>
        private static Mock<ISchemaReader> MapReader(string[] related,
            params string[] ordered)
        {
            var relationships = ordered.Select(name => new RelationshipInfo { Schema = GetSchema(name) }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(related);
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(related);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => GetSchema(name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    n,
                    operationTransaction,
                    _) => Task.FromResult(GetSchema(n)));
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

        #endregion

        #region TableOnly

        /// <summary>
        /// Verifies copy table schemas to without relationship behavior does not read the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
            reader.Verify(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies copy table schemas to with table only does not read the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTableOnlyDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                new CustomDbConnection(),
                relationshipBehavior: RelatedTableBehavior.TableOnly);
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
        }

        #endregion

        #region Sync

        /// <summary>
        /// Verifies copy table schemas to with relationship behavior reads the related tables of the given tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[]
            {
                RelatedTableBehavior.Parents,
                RelatedTableBehavior.Children,
                RelatedTableBehavior.ParentsAndChildren
            }

            )
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" },
                    "Country",
                    "Person");
                MapComposer("CREATE TABLE Country;",
                    "CREATE TABLE Person;");
                // Act
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    new CustomDbConnection(),
                    relationshipBehavior: behavior);
                // Assert
                reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                        It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })),
                        behavior,
                        It.IsAny<System.Data.IDbTransaction>()),
                    Times.Once);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with relationship behavior copies the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Parents,
                createdCallback: r => results.Add(r.Target.Table.Table.Name));
            // Assert
            reader.Verify(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" },
                destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                results);
        }

        /// <summary>
        /// Verifies copy table schemas to with relationship behavior and no tables does nothing.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithRelationshipBehaviorAndNoTablesDoesNothing()
        {
            // Setup
            var reader = MapReader(new string[0]);
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(Enumerable.Empty<string>(),
                destination,
                relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table does not read the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies copy table schemas to async without relationship behavior does not read the related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutRelationshipBehaviorDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies copy table schemas to async with relationship behavior reads the related tables of the given tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithRelationshipBehaviorReadsTheRelatedTablesOfTheGivenTables()
        {
            foreach (var behavior in new[]
            {
                RelatedTableBehavior.Parents,
                RelatedTableBehavior.Children,
                RelatedTableBehavior.ParentsAndChildren
            }

            )
            {
                // Setup
                var reader = MapReader(new[] { "Person", "Country" },
                    "Country",
                    "Person");
                MapComposer("CREATE TABLE Country;",
                    "CREATE TABLE Person;");
                using (var source = new CancellationTokenSource())
                {
                    // Act
                    await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                        new CustomDbConnection(),
                        relationshipBehavior: behavior,
                        cancellationToken: source.Token);
                    // Assert
                    reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                            It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })),
                            behavior,
                            It.IsAny<System.Data.IDbTransaction>(),
                            source.Token),
                        Times.Once);
                }
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with relationship behavior copies the related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithRelationshipBehaviorCopiesTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var results = new List<string>();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Parents,
                createdCallback: r => results.Add(r.Target.Table.Table.Name));
            // Assert
            reader.Verify(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person", "Country" })),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" },
                destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                results);
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table does not read the related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableDoesNotReadTheRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                new CustomDbConnection());
            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Single Table

        /// <summary>
        /// Verifies copy table schema to of asingle table with relationship behavior returns the result of the given table and its related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Country",
                destination,
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })),
                    RelatedTableBehavior.Children,
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" },
                destination.ExecutedCommands);
            Assert.AreEqual(2,
                result.Items.Count);
            Assert.AreEqual(1,
                result.Items.Count(i => i.Target.Table.Table.Name == "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with relationship behavior returns the result of the given table and its related tables when it is created first.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTablesWhenItIsCreatedFirst()
        {
            // Setup
            MapReader(new[] { "Person", "Country" },
                "Person",
                "Country");
            MapComposer("CREATE TABLE Person;",
                "CREATE TABLE Country;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.AreEqual(2,
                result.Items.Count);
            Assert.AreEqual(1,
                result.Items.Count(i => i.Target.Table.Table.Name == "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with relationship behavior and no related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                relationshipBehavior: RelatedTableBehavior.ParentsAndChildren).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                    "Person",
                    StringComparison.OrdinalIgnoreCase));
            // Assert
            Assert.AreEqual("Person",
                result.Target.Table.Table.Name);
            reader.Verify(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies copy table schema to of an entity with relationship behavior.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" },
                "Country",
                nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo<RelationshipTestEntity>(destination,
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    RelatedTableBehavior.Parents,
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Once);
            Assert.AreEqual(2,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(2,
                result.Items.Count);
            Assert.AreEqual(1,
                result.Items.Count(i => i.Target.Table.Table.Name == nameof(RelationshipTestEntity)));
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with relationship behavior returns the result of the given table and its related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithRelationshipBehaviorReturnsTheResultOfTheGivenTableAndItsRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Country",
                destination,
                relationshipBehavior: RelatedTableBehavior.Children));
            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Country" })),
                    RelatedTableBehavior.Children,
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE TABLE Person;" },
                destination.ExecutedCommands);
            Assert.AreEqual(2,
                result.Items.Count);
            Assert.AreEqual(1,
                result.Items.Count(i => i.Target.Table.Table.Name == "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with relationship behavior and no related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithRelationshipBehaviorAndNoRelatedTables()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                new CustomDbConnection(),
                relationshipBehavior: RelatedTableBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                    "Person",
                    StringComparison.OrdinalIgnoreCase));
            // Assert
            Assert.AreEqual("Person",
                result.Target.Table.Table.Name);
            reader.Verify(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies copy table schema to async of an entity with relationship behavior.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfAnEntityWithRelationshipBehavior()
        {
            // Setup
            var reader = MapReader(new[] { nameof(RelationshipTestEntity), "Country" },
                "Country",
                nameof(RelationshipTestEntity));
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE RelationshipTestEntity;");
            var destination = new CustomDbConnection();
            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync<RelationshipTestEntity>(destination,
                relationshipBehavior: RelatedTableBehavior.Parents));
            // Assert
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    RelatedTableBehavior.Parents,
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.AreEqual(2,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(2,
                result.Items.Count);
            Assert.AreEqual(1,
                result.Items.Count(i => i.Target.Table.Table.Name == nameof(RelationshipTestEntity)));
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table calls the created callback of each table while the copy is running.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableCallsTheCreatedCallbackOfEachTableWhileTheCopyIsRunning()
        {
            // Setup
            MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Country",
                destination,
                relationshipBehavior: RelatedTableBehavior.Children,
                createdCallback: item => reported.Add((item.Target.Table.Table.Name, destination.ExecutedCommands.Count)));
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                reported.Select(r => r.Table).ToArray());
            Assert.IsTrue(reported[0].Executed < destination.ExecutedCommands.Count);
            Assert.AreEqual(destination.ExecutedCommands.Count,
                reported[1].Executed);
            Assert.AreEqual(2,
                result.Items.Count);
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table calls the created callback of each table while the copy is running.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableCallsTheCreatedCallbackOfEachTableWhileTheCopyIsRunning()
        {
            // Setup
            MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            var result = await new CustomDbConnection().CopyTableSchemaToAsync("Country",
                destination,
                relationshipBehavior: RelatedTableBehavior.Children,
                createdCallback: item => reported.Add((item.Target.Table.Table.Name, destination.ExecutedCommands.Count)));
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                reported.Select(r => r.Table).ToArray());
            Assert.IsTrue(reported[0].Executed < destination.ExecutedCommands.Count);
            Assert.AreEqual(destination.ExecutedCommands.Count,
                reported[1].Executed);
            Assert.AreEqual(2,
                result.Items.Count);
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table created callback receives the items of the result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableCreatedCallbackReceivesTheItemsOfTheResult()
        {
            // Setup
            MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer("CREATE TABLE Country;",
                "CREATE TABLE Person;");
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Country",
                new CustomDbConnection(),
                relationshipBehavior: RelatedTableBehavior.Children,
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(2,
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
        /// Verifies copy table schema to of asingle table without related tables calls the created callback once.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithoutRelatedTablesCallsTheCreatedCallbackOnce()
        {
            // Setup
            MapReader(new[] { "Person" },
                "Person");
            MapComposer("CREATE TABLE Person;");
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                new CustomDbConnection(),
                createdCallback: created.Add);
            // Assert
            Assert.AreEqual(1,
                created.Count);
            Assert.AreEqual("Person",
                created[0].Target.Table.Table.Name);
        }
        #endregion
    }
}
