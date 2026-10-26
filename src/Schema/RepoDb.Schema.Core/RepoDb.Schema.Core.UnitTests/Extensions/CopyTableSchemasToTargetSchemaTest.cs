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
    public class CopyTableSchemasToTargetSchemaTest
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
            CustomDbSetting.Map(null);
        }

        #region Helpers

        private static TableSchema GetInvoice() =>
            new TableSchema("Invoice", "Sales") { Columns = { new ColumnInfo() } };

        private static TableSchema GetLedger()
        {
            var schema = new TableSchema("Ledger", "dbo") { Columns = { new ColumnInfo() } };
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Ledger_Invoice") { ReferencedTable = new TableInfo("Invoice", "Sales") });
            return schema;
        }

        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(n => schemas.First(s => s.Table.Name == n));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static Mock<ISchemaComposer> MapComposer(List<TableSchema> composed = null)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => $"{t.Schema}.{t.Name}");
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas =>
            {
                var list = schemas.ToList();
                composed?.AddRange(list);
                return list.Select(s => $"CREATE TABLE {s.Table.Schema}.{s.Table.Name};").ToList();
            });
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Schema}.{s.Table.Name};" });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
            return composer;
        }

        private static CustomDbConnection GetDestination() =>
            new CustomDbConnection();

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            // Setup
            var invoice = GetInvoice();
            var ledger = GetLedger();
            MapReader(invoice, ledger);
            var composed = new List<TableSchema>();
            MapComposer(composed);
            var destination = GetDestination();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice", "Ledger" }, destination, "archive", createdCallback: results.Add);

            // Assert
            Assert.IsTrue(composed.All(s => s.Table.Schema == "archive"));
            Assert.AreEqual(new TableInfo("Invoice", "archive"), composed.Single(s => s.Table.Name == "Ledger").ForeignKeys.Single().ReferencedTable);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE archive.Invoice;", "CREATE TABLE archive.Ledger;" }, destination.ExecutedCommands);
            Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "archive"));
            Assert.AreEqual("Sales", results.Single(r => r.Target.Table.Table.Name == "Invoice").Source.Table.Table.Schema);
            Assert.AreEqual("dbo", results.Single(r => r.Target.Table.Table.Name == "Ledger").Source.Table.Table.Schema);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaDoesNotChangeTheSchemaThatWasRead()
        {
            // Setup
            var ledger = GetLedger();
            MapReader(ledger);
            MapComposer();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Ledger" }, GetDestination(), "archive");

            // Assert
            Assert.AreEqual("dbo", ledger.Table.Schema);
            Assert.AreEqual(new TableInfo("Invoice", "Sales"), ledger.ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaDoesNotUseTheDefaultSchema()
        {
            // Setup
            CustomDbSetting.Map("dbo");
            MapReader(GetInvoice());
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice" }, GetDestination(), "archive", createdCallback: results.Add);

            // Assert
            Assert.AreEqual("archive", results.Single().Target.Table.Table.Schema);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            // Setup
            MapReader(GetInvoice());
            var composer = MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice" }, destination, "archive");

            // Assert
            composer.Verify(c => c.ComposeTableExists("archive.Invoice"), Times.Once);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTheTargetSchemaOfTheTablesDoesNotCreateNewSchemas()
        {
            // Setup (the tables are in the schema, and only reference tables of the schema)
            var country = new TableSchema("Country", "dbo") { Columns = { new ColumnInfo() } };
            var person = new TableSchema("Person", "dbo") { Columns = { new ColumnInfo() } };
            person.ForeignKeys.Add(new ForeignKeyInfo("FK_Person_Country") { ReferencedTable = new TableInfo("Country", "dbo") });
            MapReader(country, person);
            var composed = new List<TableSchema>();
            MapComposer(composed);

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" }, GetDestination(), "dbo");

            // Assert
            Assert.AreSame(country, composed[0]);
            Assert.AreSame(person, composed[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaMovesTheForeignKeysOfATableThatIsAlreadyInTheSchema()
        {
            // Setup (the table is in the schema, but it references a table of another schema)
            MapReader(GetLedger());
            var composed = new List<TableSchema>();
            var composer = MapComposer(composed);
            composer.Setup(c => c.ComposeTableExists("dbo.Invoice")).Returns("INVOICE_EXISTS");
            var destination = new CustomDbConnection { ScalarResult = statement => statement == "INVOICE_EXISTS" ? 1 : 0 };

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Ledger" }, destination, "dbo");

            // Assert
            Assert.AreEqual(new TableInfo("Invoice", "dbo"), composed.Single().ForeignKeys.Single().ReferencedTable);
        }

        #endregion

        #region Default Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchemaOfTheDestination()
        {
            // Setup
            CustomDbSetting.Map("dbo");
            MapReader(GetInvoice(), GetLedger());
            var composed = new List<TableSchema>();
            MapComposer(composed);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice", "Ledger" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.IsTrue(composed.All(s => s.Table.Schema == "dbo"));
            Assert.AreEqual(new TableInfo("Invoice", "dbo"), composed.Single(s => s.Table.Name == "Ledger").ForeignKeys.Single().ReferencedTable);
            Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "dbo"));
            Assert.AreEqual("Sales", results.Single(r => r.Target.Table.Table.Name == "Invoice").Source.Table.Table.Schema);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithASettingWithoutDefaultSchemaKeepsTheSchemaOfTheSourceTables()
        {
            // Setup
            CustomDbSetting.Map(null);
            MapReader(GetInvoice(), GetLedger());
            var composed = new List<TableSchema>();
            MapComposer(composed);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice", "Ledger" }, GetDestination(), createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Sales", "dbo" }, composed.Select(s => s.Table.Schema).ToArray());
            Assert.AreEqual("Sales", results.Single(r => r.Target.Table.Table.Name == "Invoice").Target.Table.Table.Schema);
            Assert.AreEqual("dbo", results.Single(r => r.Target.Table.Table.Name == "Ledger").Target.Table.Table.Schema);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithASettingWithAnEmptyDefaultSchemaKeepsTheSchemaOfTheSourceTables()
        {
            // Setup
            CustomDbSetting.Map(string.Empty);
            MapReader(GetInvoice());
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual("Sales", results.Single().Target.Table.Table.Schema);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithWhiteSpaceTargetSchemaGetsTheDefaultSchema()
        {
            // Setup
            CustomDbSetting.Map("dbo");
            MapReader(GetInvoice());
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice" }, GetDestination(), " ", createdCallback: results.Add);

            // Assert
            Assert.AreEqual("dbo", results.Single().Target.Table.Table.Schema);
        }

        #endregion

        #region Names

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTwoTablesWouldHaveTheSameNameInTheTargetSchema()
        {
            // Setup
            var sales = new TableSchema("Item", "Sales") { Columns = { new ColumnInfo() } };
            var dbo = new TableSchema("Item", "dbo") { Columns = { new ColumnInfo() } };
            CustomDbSetting.Map("dbo");
            MapReader(dbo, sales);
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "dbo.Item", "Sales.Item" }, destination));
            StringAssert.Contains(exception.Message, "Sales.Item", StringComparison.Ordinal);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaKeepsTheTablesWithTheSameNameWhenTheSettingHasNoDefaultSchema()
        {
            // Setup
            var sales = new TableSchema("Item", "Sales") { Columns = { new ColumnInfo() } };
            var dbo = new TableSchema("Item", "dbo") { Columns = { new ColumnInfo() } };
            MapReader(dbo, sales);
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "dbo.Item", "Sales.Item" }, destination);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE dbo.Item;", "CREATE TABLE Sales.Item;" }, destination.ExecutedCommands);
        }

        #endregion

        #region Single Table

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            // Setup
            MapReader(GetInvoice());
            MapComposer();
            var destination = GetDestination();

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Invoice", destination, targetSchema: "archive").Items.Single();

            // Assert
            Assert.AreEqual("archive", result.Target.Table.Table.Schema);
            Assert.AreEqual("Sales", result.Source.Table.Table.Schema);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE archive.Invoice;" }, destination.ExecutedCommands);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            // Setup
            MapReader(GetInvoice(), GetLedger());
            var composed = new List<TableSchema>();
            MapComposer(composed);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Invoice", "Ledger" }, GetDestination(), "archive", createdCallback: results.Add);

            // Assert
            Assert.IsTrue(composed.All(s => s.Table.Schema == "archive"));
            Assert.AreEqual(new TableInfo("Invoice", "archive"), composed.Single(s => s.Table.Name == "Ledger").ForeignKeys.Single().ReferencedTable);
            Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "archive"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchemaOfTheDestination()
        {
            // Setup
            CustomDbSetting.Map("dbo");
            MapReader(GetInvoice(), GetLedger());
            var composed = new List<TableSchema>();
            MapComposer(composed);
            var results = new List<CopyTableSchemaResultItem>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Invoice", "Ledger" }, GetDestination(), createdCallback: results.Add);

            // Assert
            Assert.IsTrue(composed.All(s => s.Table.Schema == "dbo"));
            Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "dbo"));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTwoTablesWouldHaveTheSameNameInTheTargetSchema()
        {
            // Setup
            var sales = new TableSchema("Item", "Sales") { Columns = { new ColumnInfo() } };
            var dbo = new TableSchema("Item", "dbo") { Columns = { new ColumnInfo() } };
            CustomDbSetting.Map("dbo");
            MapReader(dbo, sales);
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "dbo.Item", "Sales.Item" }, destination));
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithTargetSchema()
        {
            // Setup
            MapReader(GetInvoice());
            MapComposer();

            // Act
            var result = (await new CustomDbConnection().CopyTableSchemaToAsync("Invoice", GetDestination(), targetSchema: "archive")).Items.Single();

            // Assert
            Assert.AreEqual("archive", result.Target.Table.Table.Schema);
        }

        #endregion

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaKeepsTheDatabaseAndTheProviderOfTheTables()
        {
            // Setup
            var invoice = GetInvoice();
            invoice.Table.Database = "Billing";
            invoice.Table.Provider = "SqlServer";
            MapReader(invoice);
            var composed = new List<TableSchema>();
            MapComposer(composed);

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Invoice" }, GetDestination(), "archive");

            // Assert
            Assert.AreEqual("archive", composed.Single().Table.Schema);
            Assert.AreEqual("Billing", composed.Single().Table.Database);
            Assert.AreEqual("SqlServer", composed.Single().Table.Provider);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaKeepsTheDatabaseAndTheProviderOfTheReferencedTables()
        {
            // Setup
            var ledger = GetLedger();
            var referenced = ledger.ForeignKeys.Single().ReferencedTable;
            referenced.Database = "Billing";
            referenced.Provider = "SqlServer";
            MapReader(ledger);
            var composed = new List<TableSchema>();
            var composer = MapComposer(composed);
            composer.Setup(c => c.ComposeTableExists("dbo.Invoice")).Returns("INVOICE_EXISTS");
            var destination = new CustomDbConnection { ScalarResult = statement => statement == "INVOICE_EXISTS" ? 1 : 0 };

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Ledger" }, destination, "dbo");

            // Assert
            var actual = composed.Single().ForeignKeys.Single().ReferencedTable;
            Assert.AreEqual("dbo", actual.Schema);
            Assert.AreEqual("Billing", actual.Database);
            Assert.AreEqual("SqlServer", actual.Provider);
        }
    }
}
