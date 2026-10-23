#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SqliteSchemaExistsBehaviorTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Helpers

        private static void MapSchemaReaderConnection(SqliteConnection source) =>
            SchemaReaderMapper.Add<SqliteConnection>(new SqliteSchemaReader(source), true);

        private static int CountCountries(SqliteConnection target) =>
            target.ExecuteScalar<int>("SELECT COUNT(*) FROM [Country];");

        private static void InsertCountry(SqliteConnection target) =>
            target.ExecuteNonQuery("INSERT INTO [Country] ([Name]) VALUES ('Philippines');");

        private static void CreateSmallPerson(SqliteConnection target) =>
            target.ExecuteNonQuery("CREATE TABLE [Person] ([Id] BIGINT NOT NULL, [Name] VARCHAR(128) NOT NULL);");

        #endregion

        #region Skip

        [TestMethod]
        public void TestCopyTableSchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(true, results.Single(r => r.Target.Table.Table.Name == "Country").TableExisted);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(false, results.Single(r => r.Target.Table.Table.Name == "Person").TableExisted);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Skipped));
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act
                var result = source.CopyTableSchemaTo("Country", target).Items.Single();

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(1, CountCountries(target));
            }
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        [TestMethod]
        public void TestCopyTableSchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.Target.Table.Table.Name == "Person").Script, "DROP TABLE IF EXISTS [Person];", StringComparison.Ordinal);
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Dropped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                var result = results.Single();
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, result.Outcome);
                Assert.AreEqual(true, result.TableExisted);
                CollectionAssert.AreEquivalent(new[] { "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" }, result.AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), columns);
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCanBeRunTwice()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, results.Single().Outcome);
                Assert.AreEqual(0, results.Single().AddedColumns.Count);
                Assert.AreEqual(0, results.Single().AddedIndexes.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "NameUpper", "Age", "CountryId", "Salary", "CreatedDateUtc" }, results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);

                // Act
                var result = source.CopyTableSchemaTo("Person", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Person", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion
    }
}
