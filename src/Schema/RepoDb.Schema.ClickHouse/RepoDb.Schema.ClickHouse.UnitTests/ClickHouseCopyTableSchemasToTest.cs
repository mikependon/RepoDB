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
using RepoDb.Schema.ClickHouse.UnitTests.CustomObjects;

namespace RepoDb.Schema.ClickHouse.UnitTests
{
    /// <summary>
    /// Tests the copy of the schema of multiple tables with the ClickHouse composer: the reader is faked (it returns the ordered relationships
    /// like the ClickHouse reader does) and the destination connection records the statements that are executed on it.
    /// </summary>
    [TestClass]
    public class ClickHouseCopySchemasToTest
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
                .Returns(() => ClickHouseSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ClickHouseSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer() =>
            SchemaComposerMapper.Add<CustomDbConnection>(new ClickHouseSchemaComposer(), true);

        private static string[] Tables(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)).ToArray();

        private static string[] ForeignKeys(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal)).ToArray();

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void TestClickHouseCopySchemasToExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new ClickHouseSchemaComposer().ComposeSchemas(ClickHouseSchemaReader.Order(schemas).Select(r => r.Schema)).Where(c => c.Length > 0).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestClickHouseCopySchemasToCreatesTheReferencedTableFirst()
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
            StringAssert.StartsWith(tables[0], "CREATE TABLE `dbo`.`Country`", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseCopySchemasToWithTheIndexesOfTheTables()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("CIX_Product_Id") { IsUnique = true, IsClustered = true, Columns = { "Id" } });
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" }, DescendingColumns = { "Id" }, Filter = "(`Id`>(0))" });
            MapReader(product);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Product" }, destination);

            // Assert
            Assert.AreEqual(3, destination.ExecutedCommands.Count);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `CIX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1", destination.ExecutedCommands[1], StringComparer.Ordinal);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `IX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1", destination.ExecutedCommands[2], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseCopySchemasToCallbackResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var results = new List<CopySchemaResult>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, results.Select(t => t.TableName).ToArray());
            StringAssert.StartsWith(results[0].Script, "CREATE TABLE `dbo`.`Country`", StringComparison.Ordinal);
            StringAssert.StartsWith(results[1].Script, "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
            Assert.AreEqual(1, results[1].ForeignKeyCount);
            Assert.AreEqual(CopySchemaOutcome.Created, results[0].Outcome);
        }

        [TestMethod]
        public void TestClickHouseCopySchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
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
        public void TestClickHouseCopySchemasToCallsTheCallbackAfterTheIndexesAndTheForeignKeys()
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
        public void TestClickHouseCopySchemasToErrorCallbackOfTheIndexThatFails()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product, Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal) };
            var errors = new List<CopySchemaError>();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Product", "Category" }, destination, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Product", errors[0].TableName, StringComparer.Ordinal);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `IX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1", errors[0].Statement, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseCopySchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection { FailWhen = c => c.StartsWith("CREATE TABLE `dbo`.`Country`", StringComparison.Ordinal) };

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new CustomDbConnection().CopySchemaTo(
                    new[] { "Person", "Country" },
                    destination,
                    errorCallback: _ => throw new NotSupportedException()));

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count);
        }

        #endregion

        #region CopySchemasToAsync

        [TestMethod]
        public async Task TestClickHouseCopySchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemaToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new ClickHouseSchemaComposer().ComposeSchemas(ClickHouseSchemaReader.Order(schemas).Select(r => r.Schema)).Where(c => c.Length > 0).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestClickHouseCopySchemasToAsyncCallbackResult()
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
            StringAssert.Contains(results[1].Script, "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
        }

        #endregion

        private static TableInfo Reference(string name)
        {
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable(name);
            return new TableInfo(table, schema);
        }
    }
}
