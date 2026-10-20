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
using RepoDb.Schema.Vertica.UnitTests.CustomObjects;

namespace RepoDb.Schema.Vertica.UnitTests
{
    /// <summary>
    /// Tests the copy of the schema of multiple tables with the Vertica composer: the reader is faked (it returns the ordered relationships
    /// like the Vertica reader does) and the destination connection records the statements that are executed on it.
    /// </summary>
    [TestClass]
    public class VerticaCopySchemasToTest
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
            SchemaTable("dbo", tableName, references);

        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>()))
                .Returns(() => VerticaSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => VerticaSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer() =>
            SchemaComposerMapper.Add<CustomDbConnection>(new VerticaSchemaComposer(), true);

        private static string[] Tables(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)).ToArray();

        private static string[] ForeignKeys(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal)).ToArray();

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void TestVerticaCopySchemasToExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new VerticaSchemaComposer().ComposeSchemas(VerticaSchemaReader.Order(schemas).Select(r => r.Schema)).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToCreatesTheReferencedTableFirst()
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
            StringAssert.StartsWith(tables[0], "CREATE TABLE \"dbo\".\"Country\"", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToCreatesTheForeignKeysAfterAllTheTables()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"), Table("Shipment", "Country", "Person"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country", "Shipment" }, destination);

            // Assert
            var lastTable = destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal));
            var firstForeignKey = destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal));
            Assert.AreEqual(3, Tables(destination).Length);
            Assert.AreEqual(3, ForeignKeys(destination).Length);
            Assert.IsTrue(lastTable < firstForeignKey);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToOfTablesThatReferenceEachOther()
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
            Assert.IsTrue(destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)) <
                destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal)));
        }

        [TestMethod]
        public void TestVerticaCopySchemasToOfDiamond()
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
        public void TestVerticaCopySchemasToOfTablesInDifferentSchemas()
        {
            // Setup
            MapReader(SchemaTable("dbo", "Ledger", "Sales.Invoice"), SchemaTable("Sales", "Invoice"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "dbo.Ledger", "Sales.Invoice" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE \"Sales\".\"Invoice\"", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE \"dbo\".\"Ledger\"", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES \"Sales\".\"Invoice\" (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToOfTablesWithNamesThatNeedQuoting()
        {
            // Setup
            MapReader(SchemaTable("dbo", "Odd.Child", VerticaSchemaHelper.FormatTableName("dbo", "Odd.Name")), SchemaTable("dbo", "Odd.Name"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "\"dbo\".\"Odd.Child\"", "\"dbo\".\"Odd.Name\"" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE \"dbo\".\"Odd.Name\"", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE \"dbo\".\"Odd.Child\"", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES \"dbo\".\"Odd.Name\" (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToCallbackResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
            StringAssert.StartsWith(results[0].Script, "CREATE TABLE \"dbo\".\"Country\"", StringComparison.Ordinal);
            StringAssert.StartsWith(results[1].Script, "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
            StringAssert.Contains(results[1].Script, "ALTER TABLE \"dbo\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"", StringComparison.Ordinal);
            Assert.AreEqual(1, results[1].ForeignKeyCount);
            Assert.AreEqual(CopySchemaOutcome.Created, results[0].Outcome);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
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
            Assert.AreEqual(("Person", 4), reported.Single(r => r.Table == "Person"));
            Assert.AreEqual(("Country", 1), reported.Single(r => r.Table == "Country"));
            Assert.AreEqual(("Solo", 3), reported.Single(r => r.Table == "Solo"));
        }

        #endregion

        #region ErrorCallback

        [TestMethod]
        public void TestVerticaCopySchemasToErrorCallbackOfTheForeignKeyThatFails()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(2, errors[0].StatementIndex);
            Assert.AreEqual("Person", errors[0].TableName, StringComparer.Ordinal);
            Assert.AreEqual("dbo", errors[0].SchemaName, StringComparer.Ordinal);
            StringAssert.StartsWith(errors[0].Statement, "ALTER TABLE \"dbo\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToErrorCallbackAddsTheErrorToTheResultOfTheTable()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
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
            StringAssert.StartsWith(created[1].Errors[0].Statement, "ALTER TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE \"dbo\".\"Country\"", StringComparison.Ordinal) };

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
        public void ThrowExceptionOnVerticaCopySchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
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
        public async Task TestVerticaCopySchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new VerticaSchemaComposer().ComposeSchemas(VerticaSchemaReader.Order(schemas).Select(r => r.Schema)).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestVerticaCopySchemasToAsyncOfTablesThatReferenceEachOther()
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
        public async Task TestVerticaCopySchemasToAsyncCallbackResult()
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
            StringAssert.Contains(results[1].Script, "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
        }

        #endregion

        private static TableInfo Reference(string name)
        {
            var (schema, table) = VerticaSchemaHelper.ParseSchemaAndTable(name);
            return new TableInfo(table, schema);
        }
    }
}
