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
using RepoDb.Schema.PostgreSql.UnitTests.CustomObjects;

namespace RepoDb.Schema.PostgreSql.UnitTests
{
    /// <summary>
    /// Tests the copy of the schema of multiple tables with the SQL Server composer: the reader is faked (it returns the ordered relationships
    /// like the SQL Server reader does) and the destination connection records the statements that are executed on it.
    /// </summary>
    [TestClass]
    public class PostgreSqlCopyTableSchemasToTest
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
            params string[] references) => SchemaTable("public",
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
                    It.IsAny<System.Data.IDbTransaction>())).Returns(() => PostgreSqlSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() => PostgreSqlSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static void MapComposer() => SchemaComposerMapper.Add<CustomDbConnection>(new PostgreSqlSchemaComposer(),
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
        /// Verifies postgre sql copy table schemas to executes the statements that the composer composed.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToExecutesTheStatementsThatTheComposerComposed()
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
            var expected = new PostgreSqlSchemaComposer().ComposeSchemas(PostgreSqlSchemaReader.Order(schemas).Select(r => r.Schema)).ToList();
            CollectionAssert.AreEqual(expected,
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to creates the referenced table first.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCreatesTheReferencedTableFirst()
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
                "CREATE TABLE \"public\".\"Country\"",
                StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1],
                "CREATE TABLE \"public\".\"Person\"",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to creates the foreign keys after all the tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCreatesTheForeignKeysAfterAllTheTables()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"),
                Table("Shipment",
                    "Country",
                    "Person"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country", "Shipment" },
                destination);
            // Assert
            var lastTable = destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE",
                    StringComparison.Ordinal));
            var firstForeignKey = destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal));
            Assert.AreEqual(3,
                Tables(destination).Length);
            Assert.AreEqual(3,
                ForeignKeys(destination).Length);
            Assert.IsTrue(lastTable < firstForeignKey);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to of tables that reference each other.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(Table("A",
                    "B"),
                Table("B",
                    "C"),
                Table("C",
                    "A"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            var results = new List<CopyTableSchemaResultItem>();
            new CustomDbConnection().CopyTableSchemaTo(new[] { "A", "B", "C" },
                destination,
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B", "C" },
                results.Select(t => t.Target.Table.Table.Name).ToArray());
            Assert.AreEqual(3,
                Tables(destination).Length);
            Assert.AreEqual(3,
                ForeignKeys(destination).Length);
            Assert.IsTrue(destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE",
                        StringComparison.Ordinal)) < destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE",
                        StringComparison.Ordinal)));
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to of diamond.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfDiamond()
        {
            // Setup
            MapReader(Table("D",
                    "B",
                    "C"),
                Table("C",
                    "A"),
                Table("B",
                    "A"),
                Table("A"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            var results = new List<CopyTableSchemaResultItem>();
            new CustomDbConnection().CopyTableSchemaTo(new[] { "D", "C", "B", "A" },
                destination,
                createdCallback: results.Add);
            // Assert
            var names = results.Select(t => t.Target.Table.Table.Name).ToList();
            Assert.AreEqual("A",
                names[0]);
            Assert.AreEqual("D",
                names[3]);
            Assert.AreEqual(4,
                ForeignKeys(destination).Length);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to with the indexes of the tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToWithTheIndexesOfTheTables()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("CIX_Product_Id") { IsUnique = true, IsClustered = true, Columns = { "Id" } });
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" }, DescendingColumns = { "Id" }, Filter = "(\"Id\">(0))" });
            MapReader(product);
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Product" },
                destination);
            // Assert
            Assert.AreEqual(3,
                destination.ExecutedCommands.Count);
            Assert.AreEqual("CREATE UNIQUE INDEX \"CIX_Product_Id\" ON \"public\".\"Product\" (\"Id\");",
                destination.ExecutedCommands[1],
                StringComparer.Ordinal);
            Assert.AreEqual("CREATE INDEX \"IX_Product_Id\" ON \"public\".\"Product\" (\"Id\" DESC) WHERE (\"Id\">(0));",
                destination.ExecutedCommands[2],
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to of tables in different schemas.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTablesInDifferentSchemas()
        {
            // Setup
            MapReader(SchemaTable("public",
                    "Ledger",
                    "Sales.Invoice"),
                SchemaTable("Sales",
                    "Invoice"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "public.Ledger", "Sales.Invoice" },
                destination);
            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0],
                "CREATE TABLE \"public\".\"Invoice\"",
                StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1],
                "CREATE TABLE \"public\".\"Ledger\"",
                StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(),
                "REFERENCES \"public\".\"Invoice\" (\"Id\")",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to of tables with names that need quoting.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToOfTablesWithNamesThatNeedQuoting()
        {
            // Setup
            MapReader(SchemaTable("public",
                    "Odd.Child",
                    PostgreSqlSchemaHelper.FormatTableName("public",
                        "Odd.Name")),
                SchemaTable("public",
                    "Odd.Name"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "\"public\".\"Odd.Child\"", "\"public\".\"Odd.Name\"" },
                destination);
            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0],
                "CREATE TABLE \"public\".\"Odd.Name\"",
                StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1],
                "CREATE TABLE \"public\".\"Odd.Child\"",
                StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(),
                "REFERENCES \"public\".\"Odd.Name\" (\"Id\")",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to callback result.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallbackResult()
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
                "CREATE TABLE \"public\".\"Country\"",
                StringComparison.Ordinal);
            StringAssert.StartsWith(results[1].Script,
                "CREATE TABLE \"public\".\"Person\"",
                StringComparison.Ordinal);
            StringAssert.Contains(results[1].Script,
                "ALTER TABLE \"public\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"",
                StringComparison.Ordinal);
            Assert.AreEqual(1,
                results[1].ForeignKeyCount);
            Assert.AreEqual(TableOutcome.Created,
                results[0].Outcome);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to calls the callback once the schema of the table is created.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallsTheCallbackOnceTheSchemaOfTheTableIsCreated()
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
            Assert.AreEqual(("Person", 4),
                reported.Single(r => r.Table == "Person"));
            Assert.AreEqual(("Country", 1),
                reported.Single(r => r.Table == "Country"));
            Assert.AreEqual(("Solo", 3),
                reported.Single(r => r.Table == "Solo"));
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to calls the callback after the indexes and the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToCallsTheCallbackAfterTheIndexesAndTheForeignKeys()
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
            Assert.AreEqual(("Product", 4),
                reported[1]);
        }

        #endregion

        #region ErrorCallback

        /// <summary>
        /// Verifies postgre sql copy table schemas to error callback of the foreign key that fails.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToErrorCallbackOfTheForeignKeyThatFails()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal)
            };
            var errors = new List<ExecutionError>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                errorCallback: errors.Add);
            // Assert
            Assert.AreEqual(1,
                errors.Count);
            Assert.AreEqual(2,
                errors[0].StatementIndex);
            Assert.AreEqual("Person",
                errors[0].TableName,
                StringComparer.Ordinal);
            Assert.AreEqual("public",
                errors[0].SchemaName,
                StringComparer.Ordinal);
            StringAssert.StartsWith(errors[0].Statement,
                "ALTER TABLE \"public\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to error callback of the index that fails.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToErrorCallbackOfTheIndexThatFails()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo("IX_Product_Id") { Columns = { "Id" } });
            MapReader(product,
                Table("Category"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("CREATE INDEX",
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
            Assert.AreEqual("CREATE INDEX \"IX_Product_Id\" ON \"public\".\"Product\" (\"Id\");",
                errors[0].Statement,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to error callback adds the error to the result of the table.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToErrorCallbackAddsTheErrorToTheResultOfTheTable()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal)
            };
            var created = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                createdCallback: created.Add,
                errorCallback: _ =>
            {
            });
            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" },
                created.Select(r => r.Target.Table.Table.Name).ToArray());
            Assert.AreEqual(0,
                created[0].Errors.Count);
            Assert.AreEqual(TableOutcome.Created,
                created[0].Outcome);
            Assert.AreEqual(1,
                created[1].Errors.Count);
            Assert.AreEqual(TableOutcome.Failed,
                created[1].Outcome);
            StringAssert.StartsWith(created[1].Errors[0].Statement,
                "ALTER TABLE \"public\".\"Person\"",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to stops if the error callback throws.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemasToStopsIfTheErrorCallbackThrows()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("CREATE TABLE \"public\".\"Country\"",
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

        /// <summary>
        /// Verifies throw exception on postgre sql copy table schemas to if the error callback throws the captured exception.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlCopyTableSchemasToIfTheErrorCallbackThrowsTheCapturedException()
        {
            // Setup
            MapReader(Table("Person",
                    "Country"),
                Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection
            {
                FailWhen = c => c.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal)
            };
            ExecutionError raised = null;
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                    destination,
                    errorCallback: e =>
            {
                raised = e;
                throw e.Exception;
            }));
            // Assert
            Assert.AreSame(raised.Exception,
                exception);
        }

        #endregion

        #region CopyTableSchemasToAsync

        /// <summary>
        /// Verifies postgre sql copy table schemas to async executes the statements that the composer composed.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
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
            var expected = new PostgreSqlSchemaComposer().ComposeSchemas(PostgreSqlSchemaReader.Order(schemas).Select(r => r.Schema)).ToList();
            CollectionAssert.AreEqual(expected,
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to async of tables that reference each other.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncOfTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(Table("A",
                    "B"),
                Table("B",
                    "A"));
            MapComposer();
            var destination = new CustomDbConnection();
            // Act
            var results = new List<CopyTableSchemaResultItem>();
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "A", "B" },
                destination,
                createdCallback: results.Add);
            // Assert
            Assert.AreEqual(2,
                results.Count);
            Assert.AreEqual(2,
                Tables(destination).Length);
            Assert.AreEqual(2,
                ForeignKeys(destination).Length);
        }

        /// <summary>
        /// Verifies postgre sql copy table schemas to async callback result.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemasToAsyncCallbackResult()
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
                "CREATE TABLE \"public\".\"Person\"",
                StringComparison.Ordinal);
        }

        #endregion

        /// <summary>
        /// Performs reference.
        /// </summary>
        /// <param name="name">The name.</param>
        private static TableInfo Reference(string name)
        {
            var (schema, table) = PostgreSqlSchemaHelper.ParseSchemaAndTable(name);
            return new TableInfo(table,
                schema);
        }
    }
}
