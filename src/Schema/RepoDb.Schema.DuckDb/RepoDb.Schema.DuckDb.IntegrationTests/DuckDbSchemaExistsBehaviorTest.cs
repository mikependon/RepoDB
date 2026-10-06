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
        public void TestCopySchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results.Single(r => r.TableName == "Country").Outcome);
                Assert.AreEqual(true, results.Single(r => r.TableName == "Country").TableExisted);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(false, results.Single(r => r.TableName == "Person").TableExisted);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "Country", "Person" }, target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopySchemaOutcome.Skipped));
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act
                var result = source.CopySchemaTo("Country", target);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results.Single(r => r.TableName == "Country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(1, CountCountries(target));
            }
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    source.CopySchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw);

                // Assert
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        [TestMethod]
        public void TestCopySchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopySchemaOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.TableName == "Person").Script, "DROP TABLE IF EXISTS \"main\".\"Person\" CASCADE", StringComparison.Ordinal);
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Dropped, results.Single(r => r.TableName == "Country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country", "Person");
                InsertCountry(target);

                // Act
                await source.CopySchemaToAsync(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        [TestMethod]
        public void TestCopySchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                var result = results.Single();
                Assert.AreEqual(CopySchemaOutcome.Aligned, result.Outcome);
                Assert.AreEqual(true, result.TableExisted);
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" }, result.AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), columns);
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                source.CopySchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Aligned, results.Single().Outcome);
                Assert.AreEqual(0, results.Single().AddedColumns.Count);
                Assert.AreEqual(0, results.Single().AddedIndexes.Count);
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Aligned, results.Single(r => r.TableName == "Country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" }, results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" }, results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestCopySchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyToTarget("Country");
                InsertCountry(target);

                // Act
                var result = source.CopySchemaTo("Person", target, tableExistenceBehavior: CopySchemaExistsBehavior.Skip, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion
    }
}
