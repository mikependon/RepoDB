#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaMultipleTablesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup() =>
            Database.Cleanup();

        #region Helpers

        private static AuroraDbPostgreSqlSchemaComposer Composer => new AuroraDbPostgreSqlSchemaComposer();

        #endregion

        #region Tests

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfDependencyChainInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("chain_c", "chain_a", "chain_b");

            // Assert
            foreach (var table in new[] { "chain_a", "chain_b", "chain_c" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Act
            Helper.CopyToTarget("chain_a", "chain_b", "chain_c");

            // Assert
            foreach (var table in new[] { "chain_a", "chain_b", "chain_c" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("chain_c", "chain_b", "chain_a");

            // Assert
            Assert.AreEqual(new TableInfo("chain_a", "public"), Helper.GetTargetSchema("chain_b").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("chain_b", "public"), Helper.GetTargetSchema("chain_c").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<AuroraDbException>(() => Helper.CopyAllToTarget("chain_c", "chain_b"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("node_a", "node_b");

            // Assert
            Helper.AssertTargetMatchesSource("node_a");
            Helper.AssertTargetMatchesSource("node_b");
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("node_b", "node_a");

            // Assert
            Assert.AreEqual(new TableInfo("node_a", "public"), Helper.GetTargetSchema("node_b").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("node_b", "public"), Helper.GetTargetSchema("node_a").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert
            Assert.Throws<AuroraDbException>(() => Helper.CopyToTarget("node_a", "node_b"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Act
            var statements = Composer.ComposeSchemas(Helper.GetSourceSchemas(new[] { "product", "chain_b", "chain_a" })).ToList();

            // Assert
            var kinds = statements.Select(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal) ? 0 : s.Contains("INDEX") ? 1 : 2).ToList();
            CollectionAssert.AreEqual(kinds.OrderBy(k => k).ToList(), kinds);
            Assert.AreEqual(3, kinds.Count(k => k == 0));
            Assert.AreEqual(5, kinds.Count(k => k == 1));
            Assert.AreEqual(1, kinds.Count(k => k == 2));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfTheWholeDatabase()
        {
            // Act
            var tables = Helper.GetSourceTables();
            Helper.CopyAllToTarget(tables.ToArray());

            // Assert
            CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
        {
            // Act
            var tables = Helper.GetSourceTables();
            Helper.CopyAllToTarget(tables.ToArray());

            // Assert
            var source = Helper.GetSourceSchemas(tables);
            var target = Helper.GetSourceSchemas(Enumerable.Empty<string>());
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                var reader = new AuroraDbPostgreSqlSchemaReader(connection);
                var copied = tables.Select(reader.GetTableSchema).ToList();
                Assert.AreEqual(source.Sum(s => s.Columns.Count), copied.Sum(s => s.Columns.Count));
                Assert.AreEqual(source.Sum(s => s.Indexes.Count), copied.Sum(s => s.Indexes.Count));
                Assert.AreEqual(source.Sum(s => s.ForeignKeys.Count), copied.Sum(s => s.ForeignKeys.Count));
                Assert.AreEqual(source.Sum(s => s.UniqueConstraints.Count), copied.Sum(s => s.UniqueConstraints.Count));
                Assert.AreEqual(source.Sum(s => s.CheckConstraints.Count), copied.Sum(s => s.CheckConstraints.Count));
            }
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Act
            var tables = Helper.GetSourceTables();
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                var ordered = new AuroraDbPostgreSqlSchemaReader(connection).GetDependencyOrder(Enumerable.Reverse(tables)).Select(r => r.Schema).ToList();
                Helper.ExecuteOnTarget(Composer.ComposeSchemas(ordered));
            }

            // Assert
            CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Act
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                var ordered = Helper.GetTableNames(new AuroraDbPostgreSqlSchemaReader(connection).GetDependencyOrder(new[] { "chain_c", "chain_b", "chain_a" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, ordered);
            }
        }

        #endregion
    }
}
