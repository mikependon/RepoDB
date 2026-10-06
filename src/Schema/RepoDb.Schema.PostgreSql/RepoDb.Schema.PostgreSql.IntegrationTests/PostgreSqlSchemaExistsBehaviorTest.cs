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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaExistsBehaviorTest
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

        private static void MapSchemaReaderConnection(NpgsqlConnection source) =>
            SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

        private static int CountCountries(NpgsqlConnection target) =>
            Convert.ToInt32(target.ExecuteScalar("SELECT COUNT(*) FROM country;"));

        private static void InsertCountry(NpgsqlConnection target) =>
            target.ExecuteNonQuery("INSERT INTO country (id, name) VALUES (1, 'Philippines');");

        private static void CreateSmallPerson(NpgsqlConnection target) =>
            target.ExecuteNonQuery("CREATE TABLE \"Person\" (\"Id\" bigint NOT NULL, \"Name\" varchar(50) NOT NULL);");

        #endregion

        #region Skip

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(true, results.Single(r => r.TableName == "country").TableExisted);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(false, results.Single(r => r.TableName == "Person").TableExisted);
                Assert.AreEqual(1, CountCountries(target));
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithTheDefaultBehaviorCanBeRunTwice()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country", "Person" }, target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopySchemaOutcome.Skipped));
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToOfASingleTableWithSkipReturnsTheSkippedResult()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act
                var result = source.CopySchemaTo("country", target);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemasToAsyncWithSkipLeavesTheExistingTableAsItIs()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Skip, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Assert.AreEqual(1, CountCountries(target));
            }
        }

        #endregion

        #region Throw

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlCopySchemasToWithThrowIfATableAlreadyExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() =>
                    source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnPostgreSqlCopySchemasToAsyncWithThrowIfATableAlreadyExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    source.CopySchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithThrowCreatesTheTablesThatDoNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw);

                // Assert
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Drop

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country", "Person");
                InsertCountry(target);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.IsTrue(results.All(r => r.Outcome == CopySchemaOutcome.Dropped));
                Assert.IsTrue(results.All(r => r.TableExisted == true));
                StringAssert.StartsWith(results.Single(r => r.TableName == "Person").Script, "DROP TABLE IF EXISTS \"public\".\"Person\";", StringComparison.Ordinal);
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithDropCreatesTheTablesThatDoNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Dropped, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemasToAsyncWithDropCreatesTheExistingTablesAgain()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country", "Person");
                InsertCountry(target);

                // Act
                await source.CopySchemaToAsync(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(0, CountCountries(target));
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        #endregion

        #region Align

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" }, result.AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, result.AddedIndexes.ToArray());
                var columns = Helper.GetTargetSchema("Person").Columns.Select(c => c.Field.Name).ToArray();
                CollectionAssert.AreEquivalent(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), columns);
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, Helper.GetTargetSchema("Person").Indexes.Select(i => i.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithAlignCanBeRunTwice()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
        public void TestPostgreSqlCopySchemasToWithAlignCreatesTheTablesThatDoNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Aligned, results.Single(r => r.TableName == "country").Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results.Single(r => r.TableName == "Person").Outcome);
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemasToAsyncWithAlignAddsTheMissingColumnsAndIndexes()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                CreateSmallPerson(target);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Person" }, target, tableExistenceBehavior: CopySchemaExistsBehavior.Align, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" }, results.Single().AddedColumns.ToArray());
                CollectionAssert.AreEquivalent(new[] { "ix_person_name", "ux_person_token" }, results.Single().AddedIndexes.ToArray());
            }
        }

        #endregion

        #region Relationships

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithParentsAndSkipOnlyCreatesTheMissingParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                Helper.CopyAllToTarget("country");
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
