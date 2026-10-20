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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Turso.UnitTests.CustomObjects;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests
{
    /// <summary>
    /// Tests the copy of the schema of multiple tables with the SQLite composer: the reader is faked (it returns the ordered relationships
    /// like the SQLite reader does) and the destination connection records the statements that are executed on it.
    /// </summary>
    [TestClass]
    public class TursoCopySchemasToTest
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

        private static TableSchema SchemaTable(string schemaName, string tableName, params string[] references)
        {
            var schema = new TableSchema(tableName, schemaName);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Field = new DbField("Id", true, false, false, typeof(int), 4, 10, 0, "int")
            });
            schema.PrimaryKey = new PrimaryKeyInfo($"PK_{tableName}") { Columns = { "Id" } };
            foreach (var reference in references)
            {
                var referenceName = reference.Replace(".", string.Empty);
                schema.Columns.Add(new ColumnInfo
                {
                    Ordinal = schema.Columns.Count + 1,
                    Field = new DbField($"{referenceName}Id", false, false, true, typeof(int), 4, 10, 0, "int")
                });
                schema.ForeignKeys.Add(new ForeignKeyInfo($"FK_{tableName}_{referenceName}")
                {
                    Columns = { $"{referenceName}Id" },
                    ReferencedTable = new TableInfo(Reference(reference).Name, Reference(reference).Schema ?? schemaName),
                    ReferencedColumns = { "Id" }
                });
            }
            return schema;
        }

        private static TableSchema Table(string tableName, params string[] references) =>
            SchemaTable(null, tableName, references);

        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>()))
                .Returns(() => TursoSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => TursoSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer() =>
            SchemaComposerMapper.Add<CustomDbConnection>(new TursoSchemaComposer(), true);

        private static string[] Tables(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)).ToArray();

        private static string[] ForeignKeys(CustomDbConnection connection) =>
            Tables(connection)
                .SelectMany(table => table.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                .Where(line => line.Contains("FOREIGN KEY", StringComparison.Ordinal))
                .Select(line => line.Trim())
                .ToArray();

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void TestTursoCopySchemasToExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new TursoSchemaComposer().ComposeSchemas(TursoSchemaReader.Order(schemas).Select(r => r.Schema)).Where(statement => statement.Length > 0).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestTursoCopySchemasToCreatesTheReferencedTableFirst()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            var tables = Tables(destination);
            Assert.AreEqual(2, tables.Length);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [Country]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [Person]", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToCreatesTheForeignKeysWithTheTables()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"), Table("Shipment", "Country", "Person"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country", "Shipment" }, destination);

            // Assert
            Assert.AreEqual(3, Tables(destination).Length);
            Assert.AreEqual(3, ForeignKeys(destination).Length);
            Assert.AreEqual(3, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestTursoCopySchemasToOfTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(Table("A", "B"), Table("B", "C"), Table("C", "A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var results = new List<CopySchemaResult>();
            new CustomDbConnection().CopySchemaTo(new[] { "A", "B", "C" }, destination, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, results.Select(t => t.TableName).ToArray());
            Assert.AreEqual(3, Tables(destination).Length);
            Assert.AreEqual(3, ForeignKeys(destination).Length);
            Assert.AreEqual(3, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestTursoCopySchemasToOfDiamond()
        {
            // Setup
            MapReader(Table("D", "B", "C"), Table("C", "A"), Table("B", "A"), Table("A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var results = new List<CopySchemaResult>();
            new CustomDbConnection().CopySchemaTo(new[] { "D", "C", "B", "A" }, destination, createdCallback: results.Add);

            // Assert
            var names = results.Select(t => t.TableName).ToList();
            Assert.AreEqual("A", names[0]);
            Assert.AreEqual("D", names[3]);
            Assert.AreEqual(4, ForeignKeys(destination).Length);
        }

        [TestMethod]
        public void TestTursoCopySchemasToWithTheIndexesOfTheTables()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("CIX_Product_Id") { IsUnique = true, IsClustered = true, Columns = { "Id" } });
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" }, DescendingColumns = { "Id" }, Filter = "([Id]>(0))" });
            MapReader(product);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Product" }, destination);

            // Assert
            Assert.AreEqual(3, destination.ExecutedCommands.Count);
            Assert.AreEqual("CREATE UNIQUE INDEX [CIX_Product_Id] ON [Product] ([Id]);", destination.ExecutedCommands[1], StringComparer.Ordinal);
            Assert.AreEqual("CREATE INDEX [IX_Product_Id] ON [Product] ([Id] DESC) WHERE ([Id]>(0));", destination.ExecutedCommands[2], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToOfTablesInDifferentSchemas()
        {
            // Setup
            MapReader(SchemaTable(null, "Ledger", "sales.Invoice"), SchemaTable("sales", "Invoice"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Ledger", "sales.Invoice" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [sales].[Invoice]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [Ledger]", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES [Invoice] ([Id])", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToOfTablesWithNamesThatNeedQuoting()
        {
            // Setup
            MapReader(SchemaTable(null, "Odd.Child", TursoSchemaHelper.FormatTableName(null, "Odd.Name")), SchemaTable(null, "Odd.Name"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "[Odd.Child]", "[Odd.Name]" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [Odd.Name]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [Odd.Child]", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES [Odd.Name] ([Id])", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToCallbackResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
            StringAssert.StartsWith(results[0].Script, "CREATE TABLE [Country]", StringComparison.Ordinal);
            StringAssert.StartsWith(results[1].Script, "CREATE TABLE [Person]", StringComparison.Ordinal);
            StringAssert.Contains(results[1].Script, "CONSTRAINT [FK_Person_Country] FOREIGN KEY ([CountryId]) REFERENCES [Country] ([Id])", StringComparison.Ordinal);
            Assert.AreEqual(1, results[1].ForeignKeyCount);
            Assert.AreEqual(CopySchemaOutcome.Created, results[0].Outcome);
        }

        [TestMethod]
        public void TestTursoCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"), Table("Solo"));
            MapComposer();
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Person", "Country", "Solo" },
                destination,
                createdCallback: r => reported.Add((r.TableName, destination.ExecutedCommands.Count)));

            // Assert
            Assert.AreEqual(3, reported.Count);
            Assert.AreEqual(("Person", 3), reported.Single(r => r.Table == "Person"));
            Assert.AreEqual(("Country", 1), reported.Single(r => r.Table == "Country"));
            Assert.AreEqual(("Solo", 3), reported.Single(r => r.Table == "Solo"));
        }

        [TestMethod]
        public void TestTursoCopySchemasToCallsTheCallbackAfterTheIndexesAndTheForeignKeys()
        {
            // Setup
            var product = Table("Product", "Category");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product, Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Product", "Category" },
                destination,
                createdCallback: r => reported.Add((r.TableName, destination.ExecutedCommands.Count)));

            // Assert
            Assert.AreEqual(("Category", 1), reported[0]);
            Assert.AreEqual(("Product", 3), reported[1]);
        }

        #endregion

        #region ErrorCallback

        [TestMethod]
        public void TestTursoCopySchemasToErrorCallbackOfTheTableThatFails()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE [Person]", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(1, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.IsNull(errors[0].SchemaName);
            StringAssert.StartsWith(errors[0].Statement, "CREATE TABLE [Person]", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToErrorCallbackOfTheIndexThatFails()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product, Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE INDEX", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Product", "Category" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Product", errors[0].TableName, StringComparer.Ordinal);
            Assert.AreEqual("CREATE INDEX [IX_Product_Id] ON [Product] ([Id]);", errors[0].Statement, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToErrorCallbackAddsTheErrorToTheResultOfTheTable()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE [Person]", StringComparison.Ordinal) };
            var created = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(
                new[] { "Person", "Country" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ => { });

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, created.Select(r => r.TableName).ToArray());
            Assert.AreEqual(0, created[0].Errors.Count);
            Assert.AreEqual(CopySchemaOutcome.Created, created[0].Outcome);
            Assert.AreEqual(1, created[1].Errors.Count);
            Assert.AreEqual(CopySchemaOutcome.Failed, created[1].Outcome);
            StringAssert.StartsWith(created[1].Errors[0].Statement, "CREATE TABLE [Person]", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestTursoCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE [Country]", StringComparison.Ordinal) };

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopySchemaTo(
                    new[] { "Person", "Country" },
                    destination,
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnTursoCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE [Person]", StringComparison.Ordinal) };
            CopySchemaError raised = null;

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, errorCallback: e =>
                {
                    raised = e;
                    throw e.Exception;
                }));

            // Assert
            Assert.AreSame(raised.Exception, exception);
        }

        #endregion

        #region CopySchemasToAsync

        [TestMethod]
        public async Task TestTursoCopySchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new TursoSchemaComposer().ComposeSchemas(TursoSchemaReader.Order(schemas).Select(r => r.Schema)).Where(statement => statement.Length > 0).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestTursoCopySchemasToAsyncOfTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(Table("A", "B"), Table("B", "A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var results = new List<CopySchemaResult>();
            await new CustomDbConnection().CopySchemaToAsync(new[] { "A", "B" }, destination, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(2, Tables(destination).Length);
            Assert.AreEqual(2, ForeignKeys(destination).Length);
        }

        [TestMethod]
        public async Task TestTursoCopySchemasToAsyncCallbackResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var results = new List<CopySchemaResult>();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, new CustomDbConnection(), tableExistenceBehavior: CopySchemaExistsBehavior.Throw, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
            Assert.AreEqual(CopySchemaExistsBehavior.Throw, results[1].Action);
            StringAssert.Contains(results[1].Script, "CREATE TABLE [Person]", StringComparison.Ordinal);
        }

        #endregion

        private static TableInfo Reference(string name)
        {
            var (schema, table) = TursoSchemaHelper.ParseSchemaAndTable(name);
            return new TableInfo(table, schema);
        }
    }
}
