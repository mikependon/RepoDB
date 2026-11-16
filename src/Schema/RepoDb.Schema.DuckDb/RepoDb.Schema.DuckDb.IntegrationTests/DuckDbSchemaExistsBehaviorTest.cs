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
    /// <summary>
    /// Contains the duck db schema exists behavior test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaExistsBehaviorTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs map schema reader connection.
        /// </summary>
        /// <param name="source">The source.</param>
        private static void MapSchemaReaderConnection(DuckDBConnection source) => SchemaReaderMapper.Add<DuckDBConnection>(new DuckDbSchemaReader(),
            true);

        /// <summary>
        /// Performs count countries.
        /// </summary>
        /// <param name="target">The target.</param>
        private static int CountCountries(DuckDBConnection target) => target.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Country\"");

        /// <summary>
        /// Performs insert country.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void InsertCountry(DuckDBConnection target) => target.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");

        /// <summary>
        /// Performs create small person.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void CreateSmallPerson(DuckDBConnection target) => target.ExecuteNonQuery("CREATE TABLE \"Person\" (\"Id\" BIGINT NOT NULL, \"Name\" VARCHAR NOT NULL, \"NameUpper\" VARCHAR GENERATED ALWAYS AS (upper(\"Name\")))");
        #endregion

        #region Skip

        /// <summary>
        /// Verifies copy table schemas to with skip leaves the existing table as it is.
        /// </summary>
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
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Skip,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(true,
                    results.Single(r => r.Target.Table.Table.Name == "Country").TableExisted);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(false,
                    results.Single(r => r.Target.Table.Table.Name == "Person").TableExisted);
                Assert.AreEqual(1,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with the default behavior can be run twice.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Skipped));
                Assert.AreEqual(2,
                    results.Count);
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with skip returns the skipped result.
        /// </summary>
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
                var result = source.CopyTableSchemaTo("Country",
                    target).Items.Single();
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    result.Outcome);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with skip leaves the existing table as it is.
        /// </summary>
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
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Skip,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(1,
                    CountCountries(target));
            }
        }

        #endregion

        #region Throw

        /// <summary>
        /// Verifies throw exception on copy table schemas to with throw if atable already exists.
        /// </summary>
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
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "Country", "Person" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async with throw if atable already exists.
        /// </summary>
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
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync(new[] { "Country", "Person" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with throw creates the tables that do not exist.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Throw);
                // Assert
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        /// <summary>
        /// Verifies copy table schemas to with drop creates the existing tables again.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country",
                    "Person");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop,
                    createdCallback: results.Add);
                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.Target.Table.Table.Name == "Person").Script,
                    "DROP TABLE IF EXISTS \"main\".\"Person\" CASCADE",
                    StringComparison.Ordinal);
                Assert.AreEqual(0,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with drop creates the tables that do not exist.
        /// </summary>
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
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Dropped,
                    results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with drop creates the existing tables again.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("Country",
                    "Person");
                InsertCountry(target);
                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop);
                // Assert
                Assert.AreEqual(0,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("Country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        /// <summary>
        /// Verifies copy table schemas to with align adds the missing columns and indexes.
        /// </summary>
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
                source.CopyTableSchemaTo(new[] { "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                var result = results.Single();
                Assert.AreEqual(TableOutcome.Aligned,
                    result.Outcome);
                Assert.AreEqual(true,
                    result.TableExisted);
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    result.AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" },
                    result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(),
                    columns);
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" },
                    Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with align can be run twice.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                source.CopyTableSchemaTo(new[] { "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Aligned,
                    results.Single().Outcome);
                Assert.AreEqual(0,
                    results.Single().AddedColumns.Count);
                Assert.AreEqual(0,
                    results.Single().AddedIndexes.Count);
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with align creates the tables that do not exist.
        /// </summary>
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
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Aligned,
                    results.Single(r => r.Target.Table.Table.Name == "Country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with align adds the missing columns and indexes.
        /// </summary>
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
                await source.CopyTableSchemaToAsync(new[] { "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "Age", "CountryId", "Salary", "CreatedDateUtc" },
                    results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEqual(new[] { "IX_Person_Name" },
                    results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        /// <summary>
        /// Verifies copy table schema to with parents and skip only creates the missing parents.
        /// </summary>
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
                var result = source.CopyTableSchemaTo("Person",
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Skip,
                    relationshipBehavior: RelatedTableBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "Person",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual(TableOutcome.Created,
                    result.Outcome);
                Assert.AreEqual(1,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }
        #endregion
    }
}
