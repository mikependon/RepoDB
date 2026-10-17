#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
    public class CopySchemasToForeignKeysTest
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

        private static TableSchema GetCustomers() =>
            GetSchema("Customers", new string[0], new string[0], new (string, string)[0]);

        private static TableSchema GetOrders() =>
            GetSchema("Orders", new[] { "Id", "CustomerId" }, new[] { "IX_Orders_CustomerId" }, new[] { ("FK_Orders_Customers", "Customers") });

        private static TableSchema GetSchema(string tableName, string[] columns, string[] indexes, (string Name, string ReferencedTable)[] foreignKeys)
        {
            var schema = new TableSchema(tableName, "dbo");
            schema.Columns.Add(new ColumnInfo { Field = new DbField("Id", false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            foreach (var column in columns.Where(column => column != "Id"))
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column, false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            }
            foreach (var index in indexes)
            {
                schema.Indexes.Add(new IndexInfo(index) { Columns = new List<string> { "Id" } });
            }
            foreach (var (name, referencedTable) in foreignKeys)
            {
                schema.ForeignKeys.Add(new ForeignKeyInfo(name) { ReferencedTable = new TableInfo(referencedTable, "dbo") });
            }
            return schema;
        }

        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name};")
                .Concat(list.SelectMany(s => s.Indexes.Select(i => $"CREATE INDEX {i.Name} ON {s.Table.Name};")))
                .Concat(list.SelectMany(s => s.ForeignKeys.Select(f => $"ADD {f.Name} TO {s.Table.Name};")));
        }

        private static void MapComposer(bool composeForeignKeyExists = true)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
            composer.Setup(c => c.ComposeDropTable(It.IsAny<string>())).Returns<string>(n => $"DROP TABLE {n};");
            composer.Setup(c => c.ComposeAddColumn(It.IsAny<string>(), It.IsAny<ColumnInfo>())).Returns<string, ColumnInfo>((n, c) => $"ADD COLUMN {c.Field.Name} TO {n};");
            composer.Setup(c => c.ComposeCreateIndex(It.IsAny<string>(), It.IsAny<IndexInfo>())).Returns<string, IndexInfo>((n, i) => $"CREATE INDEX {i.Name} ON {n};");
            composer.Setup(c => c.ComposeAddForeignKey(It.IsAny<string>(), It.IsAny<ForeignKeyInfo>())).Returns<string, ForeignKeyInfo>((n, f) => $"ADD {f.Name} TO {n};");
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            composer.Setup(c => c.ComposeColumnExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, c) => $"COLUMN_EXISTS {n}.{c}");
            composer.Setup(c => c.ComposeIndexExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, i) => $"INDEX_EXISTS {n}.{i}");
            if (composeForeignKeyExists)
            {
                composer.Setup(c => c.ComposeForeignKeyExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, f) => $"FK_EXISTS {n}.{f}");
            }
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static CustomDbConnection GetDestination(params string[] existing) =>
            new CustomDbConnection { ScalarResult = statement => existing.Contains(statement) ? 1 : 0 };

        #endregion

        #region Align

        [TestMethod]
        public void TestCopySchemasToWithAlignAddsTheForeignKeyThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
            var orders = results.Single(r => r.TableName == "Orders");
            Assert.AreEqual(CopySchemaOutcome.Aligned, orders.Outcome);
            CollectionAssert.AreEqual(new[] { "FK_Orders_Customers" }, orders.AddedForeignKeys.ToList());
            StringAssert.Contains(orders.Script, "ADD FK_Orders_Customers TO Orders;");
            Assert.AreEqual(0, results.Single(r => r.TableName == "Customers").AddedForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignDoesNotAddTheForeignKeyThatTheTableAlreadyHas()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId", "FK_EXISTS Orders.FK_Orders_Customers");
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, results.Single(r => r.TableName == "Orders").AddedForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignDoesNotAddTheForeignKeyIfTheComposerDoesNotComposeTheExistenceStatement()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer(composeForeignKeyExists: false);
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignAddsTheForeignKeyAfterTheTablesThatAreCreated()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignAddsTheColumnsAndTheIndexesBeforeTheForeignKeys()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD COLUMN CustomerId TO Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignReportsTheTableAfterItsForeignKeysAreAdded()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var executedWhenReported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align,
                createdCallback: r => executedWhenReported.Add((r.TableName, destination.ExecutedCommands.Count)));

            // Assert
            Assert.AreEqual(("Customers", 0), executedWhenReported[0]);
            Assert.AreEqual(("Orders", 1), executedWhenReported[1]);
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithAlignAddsTheForeignKeyThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var results = new List<CopySchemaResult>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "FK_Orders_Customers" }, results.Single(r => r.TableName == "Orders").AddedForeignKeys.ToList());
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithAlignAddsTheForeignKeyAfterTheTablesThatAreCreated()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        #endregion

        #region Other Behaviors

        [TestMethod]
        public void TestCopySchemasToWithSkipDoesNotCheckTheForeignKeysOfTheTablesThatExist()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("FK_EXISTS")));
        }

        [TestMethod]
        public void TestCopySchemasToWithDropCreatesTheForeignKeysOnlyOnce()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders");

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopySchemaExistsBehavior.Drop);

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count(c => c == "ADD FK_Orders_Customers TO Orders;"));
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("FK_EXISTS")));
        }

        [TestMethod]
        public void TestCopySchemasToCreatesTheForeignKeysOfTheTablesThatDoNotExist()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Customers", "Orders" }, destination);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        #endregion
    }
}
