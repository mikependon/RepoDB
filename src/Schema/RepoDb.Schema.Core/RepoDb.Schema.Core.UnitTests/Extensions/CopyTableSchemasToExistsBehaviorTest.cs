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
    public class CopyTableSchemasToExistsBehaviorTest
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

        private static TableSchema GetCountry() =>
            GetSchema("Country", new[] { "Id", "Name" }, new[] { "IX_Country_Name" }, new string[0]);

        private static TableSchema GetPerson() =>
            GetSchema("Person", new[] { "Id", "CountryId" }, new[] { "IX_Person_Id" }, new[] { "FK_Person_Country" });

        private static TableSchema GetSchema(string tableName, string[] columns, string[] indexes, string[] foreignKeys)
        {
            var schema = new TableSchema(tableName, "dbo");
            foreach (var column in columns)
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column, false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            }
            foreach (var index in indexes)
            {
                schema.Indexes.Add(new IndexInfo(index));
            }
            foreach (var foreignKey in foreignKeys)
            {
                schema.ForeignKeys.Add(new ForeignKeyInfo(foreignKey));
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

        private static Mock<ISchemaComposer> MapComposer(bool compose = true)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
            composer.Setup(c => c.ComposeDropTable(It.IsAny<string>())).Returns<string>(n => $"DROP TABLE {n};");
            composer.Setup(c => c.ComposeAddColumn(It.IsAny<string>(), It.IsAny<ColumnInfo>())).Returns<string, ColumnInfo>((n, c) => $"ADD COLUMN {c.Field.Name} TO {n};");
            composer.Setup(c => c.ComposeCreateIndex(It.IsAny<string>(), It.IsAny<IndexInfo>())).Returns<string, IndexInfo>((n, i) => $"CREATE INDEX {i.Name} ON {n};");
            if (compose)
            {
                composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
                composer.Setup(c => c.ComposeColumnExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, c) => $"COLUMN_EXISTS {n}.{c}");
                composer.Setup(c => c.ComposeIndexExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, i) => $"INDEX_EXISTS {n}.{i}");
            }
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
            return composer;
        }

        private static CustomDbConnection GetDestination(params string[] existing) =>
            new CustomDbConnection { ScalarResult = statement => existing.Contains(statement) ? 1 : 0 };

        private static CopyTableSchemaResult Get(IEnumerable<CopyTableSchemaResult> results, string tableName) =>
            results.Single(r => r.TableName == tableName);

        #endregion

        #region Skip

        [TestMethod]
        public void TestCopyTableSchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Person;", "CREATE INDEX IX_Person_Id ON Person;", "ADD FK_Person_Country TO Person;" }, destination.ExecutedCommands);
            var country = Get(results, "Country");
            Assert.AreEqual(CopyTableSchemaOutcome.Skipped, country.Outcome);
            Assert.AreEqual(true, country.TableExisted);
            Assert.AreEqual(string.Empty, country.Script);
            var person = Get(results, "Person");
            Assert.AreEqual(CopyTableSchemaOutcome.Created, person.Outcome);
            Assert.AreEqual(false, person.TableExisted);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithSkipAndAllTheTablesExistingDoesNotExecuteAnything()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "TABLE_EXISTS Person");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Skipped));
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToReportsTheSkippedTablesBeforeTheStatementsAreExecuted()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            var executedWhenReported = new List<(string Table, int Executed)>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination,
                createdCallback: r => executedWhenReported.Add((r.TableName, destination.ExecutedCommands.Count)));

            // Assert
            Assert.AreEqual(("Country", 0), executedWhenReported[0]);
            Assert.AreEqual(("Person", 3), executedWhenReported[1]);
        }

        [TestMethod]
        public void TestCopyTableSchemasToChecksTheExistenceOfEachTableOnce()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination);

            // Assert
            CollectionAssert.AreEqual(new[] { "TABLE_EXISTS Country", "TABLE_EXISTS Person" }, destination.ExecutedScalars);
        }

        #endregion

        #region Unknown

        [TestMethod]
        public void TestCopyTableSchemasToWithAComposerThatDoesNotComposeTheExistenceStatementsCreatesTheTables()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer(compose: false);
            var destination = GetDestination("TABLE_EXISTS Country");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedScalars.Count);
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Country;", "CREATE INDEX IX_Country_Name ON Country;" }, destination.ExecutedCommands);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single().Outcome);
            Assert.IsNull(results.Single().TableExisted);
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithThrowIfATableAlreadyExists()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Person");

            // Act/Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            StringAssert.Contains(exception.Message, "Person", StringComparison.Ordinal);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithThrowAndNoExistingTableCreatesTheTables()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

            // Assert
            Assert.AreEqual(5, destination.ExecutedCommands.Count);
        }

        #endregion

        #region Drop

        [TestMethod]
        public void TestCopyTableSchemasToWithDropDropsTheExistingTablesAndCreatesThemAgain()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "TABLE_EXISTS Person");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "DROP TABLE Person;", "DROP TABLE Country;",
                "CREATE TABLE Country;", "CREATE TABLE Person;",
                "CREATE INDEX IX_Country_Name ON Country;", "CREATE INDEX IX_Person_Id ON Person;",
                "ADD FK_Person_Country TO Person;"
            }, destination.ExecutedCommands);
            Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Dropped));
            Assert.IsTrue(results.All(r => r.TableExisted == true));
            StringAssert.StartsWith(Get(results, "Person").Script, "DROP TABLE Person;", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithDropOnlyDropsTheTablesThatExist()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, destination.ExecutedCommands.Count(c => c.StartsWith("DROP", StringComparison.Ordinal)));
            Assert.AreEqual("DROP TABLE Country;", destination.ExecutedCommands[0]);
            Assert.AreEqual(CopyTableSchemaOutcome.Dropped, Get(results, "Country").Outcome);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, Get(results, "Person").Outcome);
            Assert.IsFalse(Get(results, "Person").Script.Contains("DROP"));
        }

        #endregion

        #region Align

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD COLUMN Name TO Country;", "CREATE INDEX IX_Country_Name ON Country;" }, destination.ExecutedCommands);
            var result = results.Single();
            Assert.AreEqual(CopyTableSchemaOutcome.Aligned, result.Outcome);
            CollectionAssert.AreEqual(new[] { "Name" }, result.AddedColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "IX_Country_Name" }, result.AddedIndexes.ToArray());
            Assert.AreEqual(true, result.TableExisted);
            Assert.AreEqual("ADD COLUMN Name TO Country;" + Environment.NewLine + "CREATE INDEX IX_Country_Name ON Country;", result.Script);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotAddWhatExists()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id", "COLUMN_EXISTS Country.Name", "INDEX_EXISTS Country.IX_Country_Name");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(CopyTableSchemaOutcome.Aligned, results.Single().Outcome);
            Assert.AreEqual(0, results.Single().AddedColumns.Count);
            Assert.AreEqual(0, results.Single().AddedIndexes.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id", "COLUMN_EXISTS Country.Name", "INDEX_EXISTS Country.IX_Country_Name");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country", "Person" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Person;", "CREATE INDEX IX_Person_Id ON Person;", "ADD FK_Person_Country TO Person;" }, destination.ExecutedCommands);
            Assert.AreEqual(CopyTableSchemaOutcome.Aligned, Get(results, "Country").Outcome);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, Get(results, "Person").Outcome);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignDoesNotCheckTheIndexesWithoutName()
        {
            // Setup
            var country = GetCountry();
            country.Indexes.Add(new IndexInfo(null));
            MapReader(country);
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id", "COLUMN_EXISTS Country.Name", "INDEX_EXISTS Country.IX_Country_Name");

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignReportsTheTableAfterItsLastStatement()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            var executedWhenReported = new List<int>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align,
                createdCallback: _ => executedWhenReported.Add(destination.ExecutedCommands.Count));

            // Assert
            CollectionAssert.AreEqual(new[] { 3 }, executedWhenReported);
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAndAFailedStatementReportsTheTableAsFailed()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id");
            destination.FailWhen = statement => statement.StartsWith("ADD COLUMN", StringComparison.Ordinal);
            var results = new List<CopyTableSchemaResult>();
            var errors = new List<CopyTableSchemaError>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add, errorCallback: errors.Add);

            // Assert
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Country", errors[0].TableName);
            Assert.AreEqual(CopyTableSchemaOutcome.Failed, results.Single().Outcome);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            var results = new List<CopyTableSchemaResult>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE TABLE Person;", "CREATE INDEX IX_Person_Id ON Person;", "ADD FK_Person_Country TO Person;" }, destination.ExecutedCommands);
            Assert.AreEqual(CopyTableSchemaOutcome.Skipped, Get(results, "Country").Outcome);
            Assert.AreEqual(CopyTableSchemaOutcome.Created, Get(results, "Person").Outcome);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Person");

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country", "Person" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithDropDropsTheExistingTablesAndCreatesThemAgain()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "TABLE_EXISTS Person");
            var results = new List<CopyTableSchemaResult>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "DROP TABLE Person;", "DROP TABLE Country;" }, destination.ExecutedCommands.Take(2).ToArray());
            Assert.AreEqual(7, destination.ExecutedCommands.Count);
            Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Dropped));
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            // Setup
            MapReader(GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id");
            var results = new List<CopyTableSchemaResult>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Country" }, destination, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

            // Assert
            CollectionAssert.AreEqual(new[] { "ADD COLUMN Name TO Country;", "CREATE INDEX IX_Country_Name ON Country;" }, destination.ExecutedCommands);
            CollectionAssert.AreEqual(new[] { "Name" }, results.Single().AddedColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "IX_Country_Name" }, results.Single().AddedIndexes.ToArray());
        }

        #endregion
    }
}
