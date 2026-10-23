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
    public class CopyTableSchemasToForeignKeysTest
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

        private static void MapComposer(bool composeForeignKeyExists = true, Func<CheckConstraintInfo, string> unsupportedReason = null)
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
            if (unsupportedReason != null)
            {
                composer.As<ICheckConstraintSupport>().Setup(c => c.GetUnsupportedReason(It.IsAny<CheckConstraintInfo>())).Returns(unsupportedReason);
            }
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static TableSchema GetPeople()
        {
            var schema = GetSchema("People", new[] { "Id", "BirthDate", "HireDate" }, new string[0], new (string, string)[0]);
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Birthdate") { Expression = "BirthDate < now()" });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_HireDate") { Expression = "HireDate >= '1990-01-01'" });
            return schema;
        }

        private static CustomDbConnection GetDestination(params string[] existing) =>
            new CustomDbConnection { ScalarResult = statement => existing.Contains(statement) ? 1 : 0 };

        #endregion

        #region Align

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheForeignKeyThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
            var orders = results.Single(r => r.Target.Table.Table.Name == "Orders");
            Assert.AreEqual(CopyTableSchemaOutcome.Aligned, orders.Outcome);
            CollectionAssert.AreEqual(new[] { "FK_Orders_Customers" }, orders.AddedForeignKeys.ToList());
            StringAssert.Contains(orders.Script, "ADD FK_Orders_Customers TO Orders;");
            Assert.AreEqual(0, results.Single(r => r.Target.Table.Table.Name == "Customers").AddedForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotAddTheForeignKeyThatTheTableAlreadyHas()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId", "FK_EXISTS Orders.FK_Orders_Customers");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, results.Single(r => r.Target.Table.Table.Name == "Orders").AddedForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotAddTheForeignKeyIfTheComposerDoesNotComposeTheExistenceStatement()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer(composeForeignKeyExists: false);
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheForeignKeyAfterTheTablesThatAreCreated()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotAddTheForeignKeyIfTheReferencedTableIsNotCopiedAndDoesNotExist()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheForeignKeyIfTheReferencedTableIsNotCopiedButExists()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "TABLE_EXISTS Customers", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignDoesNotAddTheForeignKeyIfTheReferencedTableIsNotCopiedAndDoesNotExist()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheForeignKeyIfTheReferencedTableIsNotCopiedButExists()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "TABLE_EXISTS Customers", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheColumnsAndTheIndexesBeforeTheForeignKeys()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD COLUMN CustomerId TO Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignReportsTheTableAfterItsForeignKeysAreAdded()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var executedWhenReported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align,
                createdCallback: r => executedWhenReported.Add((r.Target.Table.Table.Name, destination.ExecutedCommands.Count)));

            // Assert
            Assert.AreEqual(("Customers", 0), executedWhenReported[0]);
            Assert.AreEqual(("Orders", 1), executedWhenReported[1]);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheForeignKeyThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "FK_Orders_Customers" }, results.Single(r => r.Target.Table.Table.Name == "Orders").AddedForeignKeys.ToList());
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheForeignKeyAfterTheTablesThatAreCreated()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders", "COLUMN_EXISTS Orders.Id", "COLUMN_EXISTS Orders.CustomerId", "INDEX_EXISTS Orders.IX_Orders_CustomerId");

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        #endregion

        #region Other Behaviors

        [TestMethod]
        public void TestCopyTableSchemasToWithSkipDoesNotCheckTheForeignKeysOfTheTablesThatExist()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("FK_EXISTS")));
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithDropCreatesTheForeignKeysOnlyOnce()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers", "COLUMN_EXISTS Customers.Id", "TABLE_EXISTS Orders");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count(c => c == "ADD FK_Orders_Customers TO Orders;"));
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("FK_EXISTS")));
        }

        [TestMethod]
        public void TestCopyTableSchemasToCreatesTheForeignKeysOfTheTablesThatDoNotExist()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;", "ADD FK_Orders_Customers TO Orders;" }, destination.ExecutedCommands);
        }

        #endregion

        #region Unsupported Objects

        [TestMethod]
        public void TestCopyTableSchemasToLeavesOutTheForeignKeyIfTheReferencedTableIsNotCopiedAndDoesNotExist()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;" }, destination.ExecutedCommands);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(0, results[0].ForeignKeyCount);
            Assert.AreEqual(0, results[0].Errors.Count);
            Assert.AreEqual(1, results[0].Warnings.Count);
            StringAssert.Contains(results[0].Warnings[0], "FK_Orders_Customers", StringComparison.Ordinal);
            StringAssert.Contains(results[0].Warnings[0], "Customers", StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncLeavesOutTheForeignKeyIfTheReferencedTableIsNotCopiedAndDoesNotExist()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, destination, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Orders;", "CREATE INDEX IX_Orders_CustomerId ON Orders;" }, destination.ExecutedCommands);
            Assert.AreEqual(1, results[0].Warnings.Count);
            Assert.AreEqual(0, results[0].ForeignKeyCount);
        }

        [TestMethod]
        public void TestCopyTableSchemasToCreatesTheForeignKeyIfTheReferencedTableIsCopied()
        {
            // Setup
            MapReader(GetCustomers(), GetOrders());
            MapComposer();
            var destination = GetDestination();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Orders" }, destination, createdCallback: results.Add);

            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands, "ADD FK_Orders_Customers TO Orders;");
            Assert.IsTrue(results.All(result => result.Warnings.Count == 0));
        }

        [TestMethod]
        public void TestCopyTableSchemasToCreatesTheForeignKeyIfTheReferencedTableIsNotCopiedButExists()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Customers");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, createdCallback: results.Add);

            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands, "ADD FK_Orders_Customers TO Orders;");
            Assert.AreEqual(0, results[0].Warnings.Count);
            Assert.AreEqual(1, results[0].ForeignKeyCount);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithDropLeavesOutTheForeignKeyIfTheReferencedTableIsNotCopiedAndDoesNotExist()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            CollectionAssert.DoesNotContain(destination.ExecutedCommands, "ADD FK_Orders_Customers TO Orders;");
            Assert.AreEqual(1, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotWarnAboutATableThatExistsAndIsSkipped()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Orders");
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotChangeTheSchemaOfTheSourceWhenAForeignKeyIsLeftOut()
        {
            // Setup
            var orders = GetOrders();
            MapReader(orders);
            MapComposer();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, GetDestination());

            // Assert
            Assert.AreEqual(1, orders.ForeignKeys.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToLeavesOutTheCheckConstraintThatTheComposerRefuses()
        {
            // Setup
            MapReader(GetPeople());
            MapComposer(unsupportedReason: check => check.Expression.Contains("now()", StringComparison.Ordinal) ? "its expression uses now(), which the destination does not allow in a CHECK constraint" : null);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "People" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results[0].CheckConstraintCount);
            Assert.AreEqual(1, results[0].Warnings.Count);
            StringAssert.Contains(results[0].Warnings[0], "CK_Birthdate", StringComparison.Ordinal);
            StringAssert.Contains(results[0].Warnings[0], "now()", StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncLeavesOutTheCheckConstraintThatTheComposerRefuses()
        {
            // Setup
            MapReader(GetPeople());
            MapComposer(unsupportedReason: check => check.Expression.Contains("now()", StringComparison.Ordinal) ? "its expression uses now(), which the destination does not allow in a CHECK constraint" : null);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "People" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results[0].CheckConstraintCount);
            Assert.AreEqual(1, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToKeepsAllTheCheckConstraintsIfTheComposerAcceptsThem()
        {
            // Setup
            MapReader(GetPeople());
            MapComposer(unsupportedReason: check => null);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "People" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(2, results[0].CheckConstraintCount);
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToKeepsAllTheCheckConstraintsIfTheComposerDoesNotSayWhichOnesItRefuses()
        {
            // Setup
            MapReader(GetPeople());
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "People" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(2, results[0].CheckConstraintCount);
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        #endregion
    }
}
