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
    /// The foreign keys that are created depend on the relationship behavior: <c>Customers</c> is the parent of <c>Orders</c>, and <c>Items</c> is the child of <c>Orders</c>
    /// (<c>Items</c> also references <c>Products</c>, which is never copied).
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToRelationshipForeignKeysTest
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

        private static readonly IDictionary<string, TableSchema> Schemas = new Dictionary<string, TableSchema>
        {
            ["Customers"] = GetSchema("Customers"),
            ["Orders"] = GetSchema("Orders",
                ("FK_Orders_Customers", "Customers")),
            ["Items"] = GetSchema("Items",
                ("FK_Items_Orders", "Orders"),
                ("FK_Items_Products", "Products"))
        };

        /// <summary>
        /// Performs get schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="foreignKeys">The foreign keys.</param>
        private static TableSchema GetSchema(string tableName,
            params (string Name, string ReferencedTable)[] foreignKeys)
        {
            var schema = new TableSchema(tableName,
                "dbo");
            schema.Columns.Add(new ColumnInfo { Field = new DbField("Id",
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
            foreach (var (name, referencedTable)in foreignKeys)
            {
                schema.ForeignKeys.Add(new ForeignKeyInfo(name) { ReferencedTable = new TableInfo(referencedTable,
                        "dbo") });
            }

            return schema;
        }

        /// <summary>
        /// Performs get related.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        private static string[] GetRelated(RelatedTableBehavior behavior)
        {
            switch (behavior)
            {
                case RelatedTableBehavior.Parents:
                    return new[]
                    {
                        "Customers",
                        "Orders"
                    };
                case RelatedTableBehavior.Children:
                    return new[]
                    {
                        "Orders",
                        "Items"
                    };
                default:
                    return new[]
                    {
                        "Customers",
                        "Orders",
                        "Items"
                    };
            }
        }

        /// <summary>
        /// Performs map reader.
        /// </summary>
        private static void MapReader()
        {
            IList<RelationshipInfo> Relationships(IEnumerable<string> names) => names.Select(name => new RelationshipInfo { Schema = Schemas[name] }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, IEnumerable<string>, RelatedTableBehavior, System.Data.IDbTransaction>((operationConnection,
                    _,
                    behavior,
                    operationTransaction) => GetRelated(behavior));
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, IEnumerable<string>, RelatedTableBehavior, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    _,
                    behavior,
                    operationTransaction,
                    __) => Task.FromResult<IEnumerable<string>>(GetRelated(behavior)));
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, IEnumerable<string>, System.Data.IDbTransaction>((operationConnection,
                    names,
                    operationTransaction) => Relationships(names));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, IEnumerable<string>, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    names,
                    operationTransaction,
                    _) => Task.FromResult<IEnumerable<RelationshipInfo>>(Relationships(names)));
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => Schemas[name]);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    name,
                    operationTransaction,
                    _) => Task.FromResult(Schemas[name]));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs script.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name};").Concat(list.SelectMany(s => s.ForeignKeys.Select(f => $"ADD {f.Name} TO {s.Table.Name};")));
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object,
                true);
        }

        /// <summary>
        /// Performs foreign keys of.
        /// </summary>
        /// <param name="destination">The destination.</param>
        private static IList<string> ForeignKeysOf(CustomDbConnection destination) => destination.ExecutedCommands.Where(c => c.StartsWith("ADD ",
                StringComparison.Ordinal)).ToList();
        #endregion

        #region Sync

        /// <summary>
        /// Verifies copy table schema to with table only creates no foreign key.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyCreatesNoForeignKey()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.TableOnly,
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;" },
                destination.ExecutedCommands);
            Assert.AreEqual(1,
                results.Count);
            Assert.AreEqual(0,
                results[0].ForeignKeyCount);
            Assert.AreEqual(1,
                results[0].Warnings.Count);
        }

        /// <summary>
        /// Verifies copy table schema to with parents creates only the foreign keys towards the parents.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsCreatesOnlyTheForeignKeysTowardsTheParents()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Orders;", "ADD FK_Orders_Customers TO Orders;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schema to with children creates only the foreign keys of the children towards the table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithChildrenCreatesOnlyTheForeignKeysOfTheChildrenTowardsTheTable()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Children,
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Items_Orders TO Items;" },
                ForeignKeysOf(destination).ToList());
            Assert.AreEqual(2,
                results.Count);
            Assert.IsTrue(results.Single(result => result.Target.Table.Table.Name == "Orders").Warnings.Single().Contains("FK_Orders_Customers",
                    StringComparison.Ordinal));
            Assert.IsTrue(results.Single(result => result.Target.Table.Table.Name == "Items").Warnings.Single().Contains("FK_Items_Products",
                    StringComparison.Ordinal));
        }

        /// <summary>
        /// Verifies copy table schema to with parents and children creates the foreign keys in both directions.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenCreatesTheForeignKeysInBothDirections()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
            // Assert
            CollectionAssert.AreEquivalent(new[] { "ADD FK_Orders_Customers TO Orders;", "ADD FK_Items_Orders TO Items;" },
                ForeignKeysOf(destination).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to with table only creates the foreign key if the referenced table already exists in the destination.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyCreatesTheForeignKeyIfTheReferencedTableAlreadyExistsInTheDestination()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection
            {
                ScalarResult = statement => statement == "TABLE_EXISTS Customers" ? 1 : 0
            };
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.TableOnly);
            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" },
                ForeignKeysOf(destination).ToList());
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies copy table schema to async with table only creates no foreign key.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTableOnlyCreatesNoForeignKey()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.TableOnly);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schema to async with parents creates only the foreign keys towards the parents.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsCreatesOnlyTheForeignKeysTowardsTheParents()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" },
                ForeignKeysOf(destination).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to async with children creates only the foreign keys of the children towards the table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithChildrenCreatesOnlyTheForeignKeysOfTheChildrenTowardsTheTable()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Items_Orders TO Items;" },
                ForeignKeysOf(destination).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to async with parents and children creates the foreign keys in both directions.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildrenCreatesTheForeignKeysInBothDirections()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" },
                destination,
                relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
            // Assert
            CollectionAssert.AreEquivalent(new[] { "ADD FK_Orders_Customers TO Orders;", "ADD FK_Items_Orders TO Items;" },
                ForeignKeysOf(destination).ToList());
        }
        #endregion
    }
}
