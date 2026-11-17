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
    public class ClickHouseCopyTableSchemasToTest
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

        /// <summary>
        /// Performs schema table.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="references">The references.</param>
        private static TableSchema SchemaTable(string schemaName,
            string tableName,
            params string[] references)
        {
            var schema = new TableSchema(tableName,
                schemaName);
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id",
                    true,
                    false,
                    false,
                    typeof(int),
                    4,
                    10,
                    0,
                    "int") });
            schema.PrimaryKey = new PrimaryKeyInfo($"PK_{tableName}")
            {
                Columns =
                {
                    "Id"
                }
            };
            foreach (var reference in references)
            {
                var referenceName = reference.Replace(".",
                    string.Empty);
                schema.Columns.Add(new ColumnInfo { Ordinal = schema.Columns.Count + 1, Field = new DbField($"{referenceName}Id",
                        false,
                        false,
                        true,
                        typeof(int),
                        4,
                        10,
                        0,
                        "int") });
                schema.ForeignKeys.Add(new ForeignKeyInfo($"FK_{tableName}_{referenceName}") { Columns = { $"{referenceName}Id" }, ReferencedTable = new TableInfo(Reference(reference).Name,
                        Reference(reference).Schema ?? schemaName), ReferencedColumns = { "Id" } });
            }

            return schema;
        }

        /// <summary>
        /// Performs table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="references">The references.</param>
        private static TableSchema Table(string tableName,
            params string[] references) => SchemaTable("dbo",
            tableName,
            references);

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(() => ClickHouseSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() => ClickHouseSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static void MapComposer() => SchemaComposerMapper.Add<CustomDbConnection>(new ClickHouseSchemaComposer(),
            true);

        /// <summary>
        /// Performs tables.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static string[] Tables(CustomDbConnection connection) => connection.ExecutedCommands.Where(c => c.StartsWith("CREATE TABLE",
                StringComparison.Ordinal)).ToArray();

        /// <summary>
        /// Performs foreign keys.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static string[] ForeignKeys(CustomDbConnection connection) => connection.ExecutedCommands.Where(c => c.StartsWith("ALTER TABLE",
                StringComparison.Ordinal)).ToArray();
        #endregion

        #region CopyTableSchemasTo

        /// <summary>
        /// Verifies click house copy table schemas to executes the statements that the composer composed.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[]
            {
                Table("Person",
                    "Country"),
                Table("Country")
            };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination);
            // Assert
            var expected = new ClickHouseSchemaComposer().ComposeSchemas(ClickHouseSchemaReader.Order(schemas).Select(r => r.Schema)).Where(c => c.Length > 0).ToList();
            CollectionAssert.AreEqual(expected,
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies click house copy table schemas to creates the referenced table first.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToCreatesTheReferencedTableFirst()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination);
            // Assert
            var tables = Tables(destination);
            Assert.AreEqual(2,
                tables.Length);
            StringAssert.StartsWith(tables[0],
                "CREATE TABLE `dbo`.`Country`",
                StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1],
                "CREATE TABLE `dbo`.`Person`",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies click house copy table schemas to with the indexes of the tables.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToWithTheIndexesOfTheTables()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("CIX_Product_Id") { IsUnique = true, IsClustered = true, Columns = { "Id" } });
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" }, DescendingColumns = { "Id" }, Filter = "(`Id`>(0))" });
            MapReader(product);
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Product" },
                destination);
            // Assert
            Assert.AreEqual(3,
                destination.ExecutedCommands.Count);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `CIX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1",
                destination.ExecutedCommands[1],
                StringComparer.Ordinal);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `IX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1",
                destination.ExecutedCommands[2],
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies click house copy table schemas to callback result.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToCallbackResult()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                new CustomDbConnection(),
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                results.Select(t => t.Target.Table.Table.Name).ToArray());
            StringAssert.StartsWith(results[0].Script,
                "CREATE TABLE `dbo`.`Country`",
                StringComparison.Ordinal);
            StringAssert.StartsWith(results[1].Script,
                "CREATE TABLE `dbo`.`Person`",
                StringComparison.Ordinal);
            Assert.AreEqual(1,
                results[1].ForeignKeyCount);
            Assert.AreEqual(TableOutcome.Created,
                results[0].Outcome);
        }

        /// <summary>
        /// Verifies click house copy table schemas to calls the callback once the schema of the table is created.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"),
                Table("Solo"));
            MapComposer();
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country", "Solo" },
                destination,
                createdCallback: r => reported.Add((r.Target.Table.Table.Name, destination.ExecutedCommands.Count)));
            // Assert
            Assert.AreEqual(3,
                reported.Count);
            Assert.AreEqual(("Person", 3),
                reported.Single(r => r.Table == "Person"));
            Assert.AreEqual(("Country", 1),
                reported.Single(r => r.Table == "Country"));
            Assert.AreEqual(("Solo", 3),
                reported.Single(r => r.Table == "Solo"));
        }

        /// <summary>
        /// Verifies click house copy table schemas to calls the callback after the indexes and the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToCallsTheCallbackAfterTheIndexesAndTheForeignKeys()
        {
            // Setup
            var product = Table("Product",
                "Category");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product,
                Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection();
            var reported = new List<(string Table, int Executed)>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Product", "Category" },
                destination,
                createdCallback: r => reported.Add((r.Target.Table.Table.Name, destination.ExecutedCommands.Count)));
            // Assert
            Assert.AreEqual(("Category", 1),
                reported[0]);
            Assert.AreEqual(("Product", 3),
                reported[1]);
        }

        #endregion

        #region ErrorCallback

        /// <summary>
        /// Verifies click house copy table schemas to error callback of the index that fails.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToErrorCallbackOfTheIndexThatFails()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product,
                Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal)
            };
            var errors = new List<ExecutionError>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Product", "Category" },
                destination,
                errorCallback: errors.Add);
            // Assert
            Assert.AreEqual(1,
                errors.Count);
            Assert.AreEqual("Product",
                errors[0].TableName,
                StringComparer.Ordinal);
            Assert.AreEqual("ALTER TABLE `dbo`.`Product` ADD INDEX `IX_Product_Id` (`Id`) TYPE minmax GRANULARITY 1",
                errors[0].Statement,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies click house copy table schemas to stops if the error callback throws.
        /// </summary>
        [TestMethod]
        public void TestClickHouseCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("CREATE TABLE `dbo`.`Country`",
                    StringComparison.Ordinal)
            };
            // Act/Assert
            Assert.Throws<NotSupportedException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                    destination,
                    errorCallback: _ => throw new NotSupportedException()));
            // Assert
            Assert.AreEqual(1,
                destination.ExecutedCommands.Count);
        }

        #endregion

        #region CopyTableSchemasToAsync

        /// <summary>
        /// Verifies click house copy table schemas to async executes the statements that the composer composed.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseCopyTableSchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[]
            {
                Table("Person",
                    "Country"),
                Table("Country")
            };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination);
            // Assert
            var expected = new ClickHouseSchemaComposer().ComposeSchemas(ClickHouseSchemaReader.Order(schemas).Select(r => r.Schema)).Where(c => c.Length > 0).ToList();
            CollectionAssert.AreEqual(expected,
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies click house copy table schemas to async callback result.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseCopyTableSchemasToAsyncCallbackResult()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                new CustomDbConnection(),
                tableExistenceBehavior: TableExistsBehavior.Throw,
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                results.Select(t => t.Target.Table.Table.Name).ToArray());
            Assert.AreEqual(TableExistsBehavior.Throw,
                results[1].Action);
            StringAssert.Contains(results[1].Script,
                "CREATE TABLE `dbo`.`Person`",
                StringComparison.Ordinal);
        }

        #endregion

        /// <summary>
        /// Performs reference.
        /// </summary>
        /// <param name="name">The name.</param>
        private static TableInfo Reference(string name)
        {
            var (schema, table) = ClickHouseSchemaHelper.ParseSchemaAndTable(name);
            return new TableInfo(table,
                schema);
        }
    }
}
