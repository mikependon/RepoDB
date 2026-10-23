#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.DuckDb.IntegrationTests.Setup;

namespace RepoDb.Schema.DuckDb.IntegrationTests
{
    [TestClass]
    public class DuckDbSchemaExistsBehaviorTest
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

        private static void MapSchemaReaderConnection(DuckDBConnection source) =>
            SchemaReaderMapper.Add<DuckDBConnection>(new DuckDbSchemaReader(source), true);

        private static int CountCountries(DuckDBConnection target) =>
            target.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Country\"");

        private static void InsertCountry(DuckDBConnection target) =>
            target.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");

        private static void CreateSmallPerson(DuckDBConnection target) =>
            target.ExecuteNonQuery("CREATE TABLE \"Person\" (\"Id\" BIGINT NOT NULL, \"Name\" VARCHAR NOT NULL, \"NameUpper\" VARCHAR GENERATED ALWAYS AS (upper(\"Name\")))");

        #endregion

        #region Skip

        [TestMethod]
        public void TestCopyTableSchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(true, results.Single(r => r.Target.Table.Table.Name == "Country").TableExisted);
                Assert.AreEqual(TableOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(false, results.Single(r => r.Target.Table.Table.Name == "Person").TableExisted);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Skipped));
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act
                var result = source.CopyTableSchemaTo("Country", target).Items.Single();

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(1, CountCountries(target));
            }
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Throw));

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Throw);

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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.Target.Table.Table.Name == "Person").Script, "DROP TABLE IF EXISTS \"main\".\"Person\" CASCADE", StringComparison.Ordinal);
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Dropped, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Drop);

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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                var result = results.Single();
                Assert.AreEqual(TableOutcome.Aligned, result.Outcome);
                Assert.AreEqual(true, result.TableExisted);
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" }, result.AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), columns);
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Align);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Aligned, results.Single().Outcome);
                Assert.AreEqual(0, results.Single().AddedColumns.Count);
                Assert.AreEqual(0, results.Single().AddedIndexes.Count);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Aligned, results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created, results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person" }, target, tableExistenceBehavior: TableExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" }, results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);

                // Act
                var result = source.CopyTableSchemaTo("Person", target, tableExistenceBehavior: TableExistsBehavior.Skip, relationshipBehavior: RelatedTableBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Person", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual(TableOutcome.Created, result.Outcome);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion
    }
}
