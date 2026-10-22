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

        private static readonly IDictionary<string, TableSchema> Schemas = new Dictionary<string, TableSchema>
        {
            ["Customers"] = GetSchema("Customers"),
            ["Orders"] = GetSchema("Orders", ("FK_Orders_Customers", "Customers")),
            ["Items"] = GetSchema("Items", ("FK_Items_Orders", "Orders"), ("FK_Items_Products", "Products"))
        };

        private static TableSchema GetSchema(string tableName, params (string Name, string ReferencedTable)[] foreignKeys)
        {
            var schema = new TableSchema(tableName, "dbo");
            schema.Columns.Add(new ColumnInfo { Field = new DbField("Id", false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            foreach (var (name, referencedTable) in foreignKeys)
            {
                schema.ForeignKeys.Add(new ForeignKeyInfo(name) { ReferencedTable = new TableInfo(referencedTable, "dbo") });
            }
            return schema;
        }

        private static string[] GetRelated(CopyTableSchemaRelationshipBehavior behavior)
        {
            switch (behavior)
            {
                case CopyTableSchemaRelationshipBehavior.Parents:
                    return new[] { "Customers", "Orders" };
                case CopyTableSchemaRelationshipBehavior.Children:
                    return new[] { "Orders", "Items" };
                default:
                    return new[] { "Customers", "Orders", "Items" };
            }
        }

        private static void MapReader()
        {
            IList<RelationshipInfo> Relationships(IEnumerable<string> names) =>
                names.Select(name => new RelationshipInfo { Schema = Schemas[name] }).ToList();

            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>())).Returns<IEnumerable<string>, CopyTableSchemaRelationshipBehavior>((_, behavior) => GetRelated(behavior));
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, CopyTableSchemaRelationshipBehavior, CancellationToken>((_, behavior, __) => Task.FromResult<IEnumerable<string>>(GetRelated(behavior)));
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns<IEnumerable<string>>(names => Relationships(names));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, CancellationToken>((names, _) => Task.FromResult<IEnumerable<RelationshipInfo>>(Relationships(names)));
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(name => Schemas[name]);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((name, _) => Task.FromResult(Schemas[name]));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name};")
                .Concat(list.SelectMany(s => s.ForeignKeys.Select(f => $"ADD {f.Name} TO {s.Table.Name};")));
        }

        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static IList<string> ForeignKeysOf(CustomDbConnection destination) =>
            destination.ExecutedCommands.Where(c => c.StartsWith("ADD ", StringComparison.Ordinal)).ToList();

        #endregion

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyCreatesNoForeignKey()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;" }, destination.ExecutedCommands);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(0, results[0].ForeignKeyCount);
            Assert.AreEqual(1, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsCreatesOnlyTheForeignKeysTowardsTheParents()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Orders;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithChildrenCreatesOnlyTheForeignKeysOfTheChildrenTowardsTheTable()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Items_Orders TO Items;" }, ForeignKeysOf(destination).ToList());
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Single(result => result.TableName == "Orders").Warnings.Single().Contains("FK_Orders_Customers", StringComparison.Ordinal));
            Assert.IsTrue(results.Single(result => result.TableName == "Items").Warnings.Single().Contains("FK_Items_Products", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenCreatesTheForeignKeysInBothDirections()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "ADD FK_Orders_Customers TO Orders;", "ADD FK_Items_Orders TO Items;" }, ForeignKeysOf(destination).ToList());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyCreatesTheForeignKeyIfTheReferencedTableAlreadyExistsInTheDestination()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection { ScalarResult = statement => statement == "TABLE_EXISTS Customers" ? 1 : 0 };

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, ForeignKeysOf(destination).ToList());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTableOnlyCreatesNoForeignKey()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsCreatesOnlyTheForeignKeysTowardsTheParents()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, ForeignKeysOf(destination).ToList());
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithChildrenCreatesOnlyTheForeignKeysOfTheChildrenTowardsTheTable()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Items_Orders TO Items;" }, ForeignKeysOf(destination).ToList());
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildrenCreatesTheForeignKeysInBothDirections()
        {
            // Setup
            MapReader();
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "ADD FK_Orders_Customers TO Orders;", "ADD FK_Items_Orders TO Items;" }, ForeignKeysOf(destination).ToList());
        }

        #endregion
    }
}
