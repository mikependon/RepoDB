#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.CockroachDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    /// <summary>
    /// Contains the cockroach db schema exists behavior test behavior.
    /// </summary>
    [TestClass]
    public class CockroachDbSchemaExistsBehaviorTest
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
        private static void MapSchemaReaderConnection(CockroachDbConnection source) => SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(),
            true);

        /// <summary>
        /// Performs count countries.
        /// </summary>
        /// <param name="target">The target.</param>
        private static int CountCountries(CockroachDbConnection target) => Convert.ToInt32(target.ExecuteScalar("SELECT COUNT(*) FROM country;"));

        /// <summary>
        /// Performs insert country.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void InsertCountry(CockroachDbConnection target) => target.ExecuteNonQuery("INSERT INTO country (id, name) VALUES (1, 'Philippines');");

        /// <summary>
        /// Performs create small person.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void CreateSmallPerson(CockroachDbConnection target) => target.ExecuteNonQuery("CREATE TABLE \"Person\" (\"Id\" bigint NOT NULL, \"Name\" varchar(50) NOT NULL);");
        #endregion

        #region Skip

        /// <summary>
        /// Verifies cockroach db copy table schemas to with skip leaves the existing table as it is.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Skip,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results.Single(r => r.Target.Table.Table.Name == "country").Outcome);
                Assert.AreEqual(true,
                    results.Single(r => r.Target.Table.Table.Name == "country").TableExisted);
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
        /// Verifies cockroach db copy table schemas to with the default behavior can be run twice.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Skipped));
                Assert.AreEqual(2,
                    results.Count);
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schema to of asingle table with skip returns the skipped result.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                // Act
                var result = source.CopyTableSchemaTo("country",
                    target).Items.Single();
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    result.Outcome);
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to async with skip leaves the existing table as it is.
        /// </summary>
        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Skip,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results.Single(r => r.Target.Table.Table.Name == "country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Assert.AreEqual(1,
                    CountCountries(target));
            }
        }

        #endregion

        #region Throw

        /// <summary>
        /// Verifies throw exception on cockroach db copy table schemas to with throw if atable already exists.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCockroachDbCopyTableSchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo(new[] { "country", "Person" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        /// <summary>
        /// Verifies throw exception on cockroach db copy table schemas to async with throw if atable already exists.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbCopyTableSchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync(new[] { "country", "Person" },
                        target,
                        tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to with throw creates the tables that do not exist.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Throw);
                // Assert
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        /// <summary>
        /// Verifies cockroach db copy table schemas to with drop creates the existing tables again.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country",
                    "Person");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop,
                    createdCallback: results.Add);
                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == TableOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.Target.Table.Table.Name == "Person").Script,
                    "DROP TABLE IF EXISTS \"public\".\"Person\";",
                    StringComparison.Ordinal);
                Assert.AreEqual(0,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to with drop creates the tables that do not exist.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Dropped,
                    results.Single(r => r.Target.Table.Table.Name == "country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to async with drop creates the existing tables again.
        /// </summary>
        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country",
                    "Person");
                InsertCountry(target);
                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Drop);
                // Assert
                Assert.AreEqual(0,
                    CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        /// <summary>
        /// Verifies cockroach db copy table schemas to with align adds the missing columns and indexes.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
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
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" },
                    result.AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" },
                    result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(),
                    columns);
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" },
                    Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to with align can be run twice.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
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
        /// Verifies cockroach db copy table schemas to with align creates the tables that do not exist.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    tableExistenceBehavior: TableExistsBehavior.Align,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Aligned,
                    results.Single(r => r.Target.Table.Table.Name == "country").Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results.Single(r => r.Target.Table.Table.Name == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        /// <summary>
        /// Verifies cockroach db copy table schemas to async with align adds the missing columns and indexes.
        /// </summary>
        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
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
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" },
                    results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" },
                    results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        /// <summary>
        /// Verifies cockroach db copy table schema to with parents and skip only creates the missing parents.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
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
