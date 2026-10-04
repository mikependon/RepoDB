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
using RepoDb.Schema.SqlServer.UnitTests.CustomObjects;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    /// <summary>
    /// Tests the copy of the schema of multiple tables with the SQL Server composer: the reader is faked (it returns the ordered relationships
    /// like the SQL Server reader does) and the destination connection records the statements that are executed on it.
    /// </summary>
    [TestClass]
    public class SqlServerCopySchemasToTest
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
            var schema = new TableSchema { SchemaName = schemaName, TableName = tableName };
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Field = new DbField("Id", true, false, false, typeof(int), 4, 10, 0, "int")
            });
            schema.PrimaryKey = new PrimaryKeyInfo { Name = $"PK_{tableName}", Columns = { "Id" } };
            foreach (var reference in references)
            {
                var referenceName = reference.Replace(".", string.Empty);
                schema.Columns.Add(new ColumnInfo
                {
                    Ordinal = schema.Columns.Count + 1,
                    Field = new DbField($"{referenceName}Id", false, false, true, typeof(int), 4, 10, 0, "int")
                });
                schema.ForeignKeys.Add(new ForeignKeyInfo
                {
                    Name = $"FK_{tableName}_{referenceName}",
                    Columns = { $"{referenceName}Id" },
                    ReferencedTable = reference,
                    ReferencedColumns = { "Id" }
                });
            }
            return schema;
        }

        private static TableSchema Table(string tableName, params string[] references) =>
            SchemaTable("dbo", tableName, references);

        // The reader gives the relationships ordered like the SQL Server reader does, from the schemas that the tables have
        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>()))
                .Returns(() => SqlServerSchemaReader.Order(schemas));
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => SqlServerSchemaReader.Order(schemas));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer() =>
            SchemaComposerMapper.Add<CustomDbConnection>(new SqlServerSchemaComposer(), true);

        private static string[] Tables(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)).ToArray();

        private static string[] ForeignKeys(CustomDbConnection connection) =>
            connection.ExecutedCommands.Where(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal)).ToArray();

        #endregion

        #region CopySchemasTo

        [TestMethod]
        public void TestSqlServerCopySchemasToExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new SqlServerSchemaComposer().ComposeSchemas(SqlServerSchemaReader.Order(schemas).Select(r => r.Table)).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToCreatesTheReferencedTableFirst()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, destination);

            // Assert
            var tables = Tables(destination);
            Assert.AreEqual(2, tables.Length);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [dbo].[Country]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToCreatesTheForeignKeysAfterAllTheTables()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"), Table("Shipment", "Country", "Person"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country", "Shipment" }, destination);

            // Assert
            var lastTable = destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal));
            var firstForeignKey = destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal));
            Assert.AreEqual(3, Tables(destination).Length);
            Assert.AreEqual(3, ForeignKeys(destination).Length);
            Assert.IsTrue(lastTable < firstForeignKey);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToOfTablesThatReferenceEachOther()
        {
            // Setup (A -> B -> C -> A)
            MapReader(Table("A", "B"), Table("B", "C"), Table("C", "A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "A", "B", "C" }, destination);

            // Assert (all the tables are created before any foreign key, so the cycle can be created)
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, result.Tables.Select(t => t.TableName).ToArray());
            Assert.AreEqual(3, Tables(destination).Length);
            Assert.AreEqual(3, ForeignKeys(destination).Length);
            Assert.IsTrue(destination.ExecutedCommands.FindLastIndex(c => c.StartsWith("CREATE TABLE", StringComparison.Ordinal)) <
                destination.ExecutedCommands.FindIndex(c => c.StartsWith("ALTER TABLE", StringComparison.Ordinal)));
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToOfDiamond()
        {
            // Setup
            MapReader(Table("D", "B", "C"), Table("C", "A"), Table("B", "A"), Table("A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "D", "C", "B", "A" }, destination);

            // Assert
            var names = result.Tables.Select(t => t.TableName).ToList();
            Assert.AreEqual("A", names[0]);
            Assert.AreEqual("D", names[3]);
            Assert.AreEqual(4, ForeignKeys(destination).Length);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToWithTheIndexesOfTheTables()
        {
            // Setup
            var product = Table("Product");
            product.Indexes.Add(new IndexInfo { Name = "CIX_Product_Id", IsUnique = true, IsClustered = true, Columns = { "Id" } });
            product.Indexes.Add(new IndexInfo { Name = "IX_Product_Id", Columns = { "Id" }, DescendingColumns = { "Id" }, Filter = "([Id]>(0))" });
            MapReader(product);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "Product" }, destination);

            // Assert
            Assert.AreEqual(3, destination.ExecutedCommands.Count);
            Assert.AreEqual("CREATE UNIQUE CLUSTERED INDEX [CIX_Product_Id] ON [dbo].[Product] ([Id]);", destination.ExecutedCommands[1], StringComparer.Ordinal);
            Assert.AreEqual("CREATE INDEX [IX_Product_Id] ON [dbo].[Product] ([Id] DESC) WHERE ([Id]>(0));", destination.ExecutedCommands[2], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToOfTablesInDifferentSchemas()
        {
            // Setup
            MapReader(SchemaTable("dbo", "Ledger", "Sales.Invoice"), SchemaTable("Sales", "Invoice"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "dbo.Ledger", "Sales.Invoice" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [Sales].[Invoice]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [dbo].[Ledger]", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES [Sales].[Invoice] ([Id])", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToOfTablesWithNamesThatNeedQuoting()
        {
            // Setup
            MapReader(SchemaTable("dbo", "Odd.Child", SqlServerNames.Format("dbo", "Odd.Name")), SchemaTable("dbo", "Odd.Name"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            new CustomDbConnection().CopySchemasTo(new[] { "[dbo].[Odd.Child]", "[dbo].[Odd.Name]" }, destination);

            // Assert
            var tables = Tables(destination);
            StringAssert.StartsWith(tables[0], "CREATE TABLE [dbo].[Odd.Name]", StringComparison.Ordinal);
            StringAssert.StartsWith(tables[1], "CREATE TABLE [dbo].[Odd.Child]", StringComparison.Ordinal);
            StringAssert.Contains(ForeignKeys(destination).Single(), "REFERENCES [dbo].[Odd.Name] ([Id])", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerCopySchemasToResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();

            // Act
            var result = new CustomDbConnection().CopySchemasTo(new[] { "Person", "Country" }, new CustomDbConnection());

            // Assert
            Assert.AreEqual(2, result.TableCount);
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
            StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Country]", StringComparison.Ordinal);
            StringAssert.Contains(result.Script, "ALTER TABLE [dbo].[Person] ADD CONSTRAINT [FK_Person_Country]", StringComparison.Ordinal);
            Assert.AreEqual(1, result.Tables[1].ForeignKeyCount);
            StringAssert.StartsWith(result.Tables[1].Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
            Assert.AreEqual(CopySchemaOutcome.Created, result.Tables[0].Outcome);
        }

        #endregion

        #region CopySchemasToAsync

        [TestMethod]
        public async Task TestSqlServerCopySchemasToAsyncExecutesTheStatementsThatTheComposerComposed()
        {
            // Setup
            var schemas = new[] { Table("Person", "Country"), Table("Country") };
            MapReader(schemas);
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            await new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "Country" }, destination);

            // Assert
            var expected = new SqlServerSchemaComposer().ComposeSchemas(SqlServerSchemaReader.Order(schemas).Select(r => r.Table)).ToList();
            CollectionAssert.AreEqual(expected, destination.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestSqlServerCopySchemasToAsyncOfTablesThatReferenceEachOther()
        {
            // Setup
            MapReader(Table("A", "B"), Table("B", "A"));
            MapComposer();
            var destination = new CustomDbConnection();

            // Act
            var result = await new CustomDbConnection().CopySchemasToAsync(new[] { "A", "B" }, destination);

            // Assert
            Assert.AreEqual(2, result.TableCount);
            Assert.AreEqual(2, Tables(destination).Length);
            Assert.AreEqual(2, ForeignKeys(destination).Length);
        }

        [TestMethod]
        public async Task TestSqlServerCopySchemasToAsyncResult()
        {
            // Setup
            MapReader(Table("Person", "Country"), Table("Country"));
            MapComposer();

            // Act
            var result = await new CustomDbConnection().CopySchemasToAsync(new[] { "Person", "Country" }, new CustomDbConnection(), CopySchemaExistsBehavior.ThrowOnExists);

            // Assert
            CollectionAssert.AreEqual(new[] { "Country", "Person" }, result.Tables.Select(t => t.TableName).ToArray());
            Assert.AreEqual(CopySchemaExistsBehavior.ThrowOnExists, result.Action);
            StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
        }

        #endregion
    }
}
