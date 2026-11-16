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
    /// <summary>
    /// Contains the copy table schemas to index names test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToIndexNamesTest
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
        /// Performs get schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="indexes">The indexes.</param>
        private static TableSchema GetSchema(string tableName,
            params string[] indexes)
        {
            var schema = new TableSchema(tableName,
                "dbo");
            schema.Columns.Add(new ColumnInfo { Field = new DbField("Id",
                    false,
                    false,
                    true,
                    typeof(string),
                    null,
                    null,
                    null,
                    "text",
                    false,
                    "Test") });
            foreach (var index in indexes)
            {
                schema.Indexes.Add(new IndexInfo(index) { Columns = new List<string> { "Id" } });
            }

            return schema;
        }

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs script.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name};").Concat(list.SelectMany(s => s.Indexes.Select(i => $"CREATE INDEX {i.Name} ON {s.Table.Name};")));
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        /// <param name="areIndexNamesUniquePerSchema">The are index names unique per schema.</param>
        private static void MapComposer(bool areIndexNamesUniquePerSchema)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
            composer.Setup(c => c.ComposeDropTable(It.IsAny<string>())).Returns<string>(n => $"DROP TABLE {n};");
            composer.Setup(c => c.ComposeCreateIndex(It.IsAny<string>(),
                    It.IsAny<IndexInfo>())).Returns<string, IndexInfo>((n,
                    i) => $"CREATE INDEX {i.Name} ON {n};");
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            composer.Setup(c => c.ComposeColumnExists(It.IsAny<string>(),
                    It.IsAny<string>())).Returns<string, string>((n,
                    c) => $"COLUMN_EXISTS {n}.{c}");
            composer.Setup(c => c.ComposeIndexExists(It.IsAny<string>(),
                    It.IsAny<string>())).Returns<string, string>((n,
                    i) => $"INDEX_EXISTS {n}.{i}");
            composer.Setup(c => c.ComposeIndexNameExists(It.IsAny<string>(),
                    It.IsAny<string>())).Returns<string, string>((n,
                    i) => $"INDEX_NAME_EXISTS {i}");
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object,
                true);
            CustomDbSetting.Map(null,
                areIndexNamesUniquePerSchema);
        }

        /// <summary>
        /// Performs get destination.
        /// </summary>
        /// <param name="existing">The existing.</param>
        private static CustomDbConnection GetDestination(params string[] existing) => new CustomDbConnection
        {
            ScalarResult = statement => existing.Contains(statement) ? 1 : 0
        };
        #endregion

        #region New Tables

        /// <summary>
        /// Verifies copy table schemas to renames the index whose name is used by another table of the copy.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToRenamesTheIndexWhoseNameIsUsedByAnotherTableOfTheCopy()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "PostalCode"),
                GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Employees;", "CREATE INDEX PostalCode ON Customers;", "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to renames the index whose name is used by another table of the copy with some cases.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToRenamesTheIndexWhoseNameIsUsedByAnotherTableOfTheCopyWithSomeCases()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "PostalCode"),
                GetSchema("Employees",
                    "POSTALCODE"));
            MapComposer(true);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Employees" },
                destination);
            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(),
                "CREATE INDEX Employees_POSTALCODE ON Employees;");
        }

        /// <summary>
        /// Verifies copy table schemas to renames the index whose name is the name of atable of the copy.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToRenamesTheIndexWhoseNameIsTheNameOfATableOfTheCopy()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "Region"),
                GetSchema("Region"));
            MapComposer(true);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Region" },
                destination);
            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(),
                "CREATE INDEX Customers_Region ON Customers;");
        }

        /// <summary>
        /// Verifies copy table schemas to renames the index whose name is the name of atable of the copy with some cases.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToRenamesTheIndexWhoseNameIsTheNameOfATableOfTheCopyWithSomeCases()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "REGION"),
                GetSchema("Region"));
            MapComposer(true);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Region" },
                destination);
            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(),
                "CREATE INDEX Customers_REGION ON Customers;");
        }

        /// <summary>
        /// Verifies copy table schemas to keeps the index named like atable if the index names are unique in their table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToKeepsTheIndexNamedLikeATableIfTheIndexNamesAreUniqueInTheirTable()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "Region"),
                GetSchema("Region"));
            MapComposer(false);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Region" },
                destination);
            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(),
                "CREATE INDEX Region ON Customers;");
        }

        /// <summary>
        /// Verifies copy table schemas to keeps the names of the indexes if they are unique in their table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToKeepsTheNamesOfTheIndexesIfTheyAreUniqueInTheirTable()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "PostalCode"),
                GetSchema("Employees",
                    "PostalCode"));
            MapComposer(false);
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Employees;", "CREATE INDEX PostalCode ON Customers;", "CREATE INDEX PostalCode ON Employees;" },
                destination.ExecutedCommands);
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("INDEX_NAME_EXISTS")));
        }

        /// <summary>
        /// Verifies copy table schemas to renames the index whose name already exists in the destination.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToRenamesTheIndexWhoseNameAlreadyExistsInTheDestination()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("INDEX_NAME_EXISTS PostalCode");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Employees;", "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to numbers the index whose table name prefixed name already exists in the destination.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToNumbersTheIndexWhoseTableNamePrefixedNameAlreadyExistsInTheDestination()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("INDEX_NAME_EXISTS PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Employees;", "CREATE INDEX Employees_PostalCode_2 ON Employees;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to does not change the indexes of the source schema.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToDoesNotChangeTheIndexesOfTheSourceSchema()
        {
            // Setup
            var customers = GetSchema("Customers",
                "PostalCode");
            var employees = GetSchema("Employees",
                "PostalCode");
            MapReader(customers,
                employees);
            MapComposer(true);
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Employees" },
                GetDestination());
            // Assert
            Assert.AreEqual("PostalCode",
                customers.Indexes.Single().Name);
            Assert.AreEqual("PostalCode",
                employees.Indexes.Single().Name);
        }

        /// <summary>
        /// Verifies copy table schemas to keeps the definition of the renamed index.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToKeepsTheDefinitionOfTheRenamedIndex()
        {
            // Setup
            var employees = GetSchema("Employees",
                "PostalCode");
            employees.Indexes[0].IsUnique = true;
            MapReader(GetSchema("Customers",
                    "PostalCode"),
                employees);
            MapComposer(true);
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Customers", "Employees" },
                GetDestination(),
                createdCallback: results.Add);
            // Assert
            var result = results.Single(r => r.Target.Table.Table.Name == "Employees");
            Assert.AreEqual(1,
                result.IndexCount);
            StringAssert.Contains(result.Script,
                "CREATE INDEX Employees_PostalCode ON Employees;");
        }

        /// <summary>
        /// Verifies copy table schemas to async renames the index whose name is used by another table of the copy.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncRenamesTheIndexWhoseNameIsUsedByAnotherTableOfTheCopy()
        {
            // Setup
            MapReader(GetSchema("Customers",
                    "PostalCode"),
                GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Customers", "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Customers;", "CREATE TABLE Employees;", "CREATE INDEX PostalCode ON Customers;", "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to async renames the index whose name already exists in the destination.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncRenamesTheIndexWhoseNameAlreadyExistsInTheDestination()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("INDEX_NAME_EXISTS PostalCode");
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Employees" },
                destination);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Employees;", "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }

        #endregion

        #region Existing Tables

        /// <summary>
        /// Verifies copy table schemas to with align renames the missing index whose name is used by another table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithAlignRenamesTheMissingIndexWhoseNameIsUsedByAnotherTable()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_NAME_EXISTS PostalCode");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align,
                createdCallback: results.Add);
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Employees_PostalCode" },
                results.Single().AddedIndexes.ToList());
        }

        /// <summary>
        /// Verifies copy table schemas to with align does not create the index that the table already has.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotCreateTheIndexThatTheTableAlreadyHas()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "Employees_PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_EXISTS Employees.Employees_PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to with align does not create the index that the table has under the name that an earlier copy gave it.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotCreateTheIndexThatTheTableHasUnderTheNameThatAnEarlierCopyGaveIt()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_NAME_EXISTS PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode",
                "INDEX_EXISTS Employees.Employees_PostalCode");
            var results = new List<CopyTableSchemaResultItem>();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align,
                createdCallback: results.Add);
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(0,
                results.Single().AddedIndexes.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to with align does not create the index that the table has under the numbered name that an earlier copy gave it.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotCreateTheIndexThatTheTableHasUnderTheNumberedNameThatAnEarlierCopyGaveIt()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_NAME_EXISTS PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode_2",
                "INDEX_EXISTS Employees.Employees_PostalCode_2");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to async with align does not create the index that the table has under the name that an earlier copy gave it.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignDoesNotCreateTheIndexThatTheTableHasUnderTheNameThatAnEarlierCopyGaveIt()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_NAME_EXISTS PostalCode",
                "INDEX_NAME_EXISTS Employees_PostalCode",
                "INDEX_EXISTS Employees.Employees_PostalCode");
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies copy table schemas to with skip does not check the names of the indexes of the tables that exist.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithSkipDoesNotCheckTheNamesOfTheIndexesOfTheTablesThatExist()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination);
            // Assert
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
            Assert.IsFalse(destination.ExecutedScalars.Any(s => s.StartsWith("INDEX_NAME_EXISTS")));
        }

        /// <summary>
        /// Verifies copy table schemas to with drop keeps the name of the index of the table that is dropped.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithDropKeepsTheNameOfTheIndexOfTheTableThatIsDropped()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_EXISTS Employees.PostalCode",
                "INDEX_NAME_EXISTS PostalCode");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Drop);
            // Assert
            CollectionAssert.AreEqual(new[] { "DROP TABLE Employees;", "CREATE TABLE Employees;", "CREATE INDEX PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }

        /// <summary>
        /// Verifies copy table schemas to with drop renames the index whose name is used by another table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithDropRenamesTheIndexWhoseNameIsUsedByAnotherTable()
        {
            // Setup
            MapReader(GetSchema("Employees",
                    "PostalCode"));
            MapComposer(true);
            var destination = GetDestination("TABLE_EXISTS Employees",
                "INDEX_NAME_EXISTS PostalCode");
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Employees" },
                destination,
                tableExistenceBehavior: TableExistsBehavior.Drop);
            // Assert
            CollectionAssert.AreEqual(new[] { "DROP TABLE Employees;", "CREATE TABLE Employees;", "CREATE INDEX Employees_PostalCode ON Employees;" },
                destination.ExecutedCommands);
        }
        #endregion
    }
}
