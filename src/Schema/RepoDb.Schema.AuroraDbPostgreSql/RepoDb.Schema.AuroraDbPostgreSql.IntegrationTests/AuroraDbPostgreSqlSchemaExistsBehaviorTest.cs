#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaExistsBehaviorTest
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

        private static void MapSchemaReaderConnection(AuroraDbConnection source) =>
            SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(source), true);

        private static int CountCountries(AuroraDbConnection target) =>
            Convert.ToInt32(target.ExecuteScalar("SELECT COUNT(*) FROM country;"));

        private static void InsertCountry(AuroraDbConnection target) =>
            target.ExecuteNonQuery("INSERT INTO country (id, name) VALUES (1, 'Philippines');");

        private static void CreateSmallPerson(AuroraDbConnection target) =>
            target.ExecuteNonQuery("CREATE TABLE \"Person\" (\"Id\" bigint NOT NULL, \"Name\" varchar(50) NOT NULL);");

        #endregion

        #region Skip

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(true, results.Single(r => r.TableName == "country").TableExisted);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(false, results.Single(r => r.TableName == "Person").TableExisted);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Skipped));
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act
                var result = source.CopyTableSchemaTo("country", target);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(1, CountCountries(target));
            }
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlCopyTableSchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbPostgreSqlCopyTableSchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    source.CopyTableSchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country", "Person");
                InsertCountry(target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopyTableSchemaOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.TableName == "Person").Script, "DROP TABLE IF EXISTS \"public\".\"Person\";", StringComparison.Ordinal);
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Dropped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country", "Person");
                InsertCountry(target);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                var result = results.Single();
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, result.Outcome);
                Assert.AreEqual(true, result.TableExisted);
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" }, result.AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), columns);
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, results.Single().Outcome);
                Assert.AreEqual(0, results.Single().AddedColumns.Count);
                Assert.AreEqual(0, results.Single().AddedIndexes.Count);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Aligned, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopyTableSchemaResult>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Person" }, target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" }, results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);

                // Act
                var result = source.CopyTableSchemaTo("Person", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion
    }
}
