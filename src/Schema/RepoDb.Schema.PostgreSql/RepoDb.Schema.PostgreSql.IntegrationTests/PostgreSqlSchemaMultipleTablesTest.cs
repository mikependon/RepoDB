#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaMultipleTablesTest
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

        private static PostgreSqlSchemaComposer Composer => new PostgreSqlSchemaComposer();

        #endregion

        #region Tests

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesOfDependencyChainInAnyOrder()
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
        public void TestPostgreSqlSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
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
        public void TestPostgreSqlSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("chain_c", "chain_b", "chain_a");

            // Assert
            Assert.AreEqual("public.chain_a", Helper.GetTargetSchema("chain_b").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual("public.chain_b", Helper.GetTargetSchema("chain_c").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<PostgresException>(() => Helper.CopyAllToTarget("chain_c", "chain_b"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("node_a", "node_b");

            // Assert
            Helper.AssertTargetMatchesSource("node_a");
            Helper.AssertTargetMatchesSource("node_b");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("node_b", "node_a");

            // Assert
            Assert.AreEqual("public.node_a", Helper.GetTargetSchema("node_b").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual("public.node_b", Helper.GetTargetSchema("node_a").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert (the first table already references the second one)
            Assert.Throws<PostgresException>(() => Helper.CopyToTarget("node_a", "node_b"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Act
            var statements = Composer.ComposeSchemas(Helper.GetSourceSchemas(new[] { "product", "chain_b", "chain_a" })).ToList();

            // Assert (the tables, then the indexes, then the foreign keys)
            var kinds = statements.Select(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal) ? 0 : s.Contains("INDEX") ? 1 : 2).ToList();
            CollectionAssert.AreEqual(kinds.OrderBy(k => k).ToList(), kinds);
            Assert.AreEqual(3, kinds.Count(k => k == 0));
            Assert.AreEqual(5, kinds.Count(k => k == 1));
            Assert.AreEqual(1, kinds.Count(k => k == 2));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesOfTheWholeDatabase()
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
        public void TestPostgreSqlSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
        {
            // Act
            var tables = Helper.GetSourceTables();
            Helper.CopyAllToTarget(tables.ToArray());

            // Assert
            var source = Helper.GetSourceSchemas(tables);
            var target = Helper.GetSourceSchemas(Enumerable.Empty<string>());
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                var reader = new PostgreSqlSchemaReader(connection);
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
        public void TestPostgreSqlSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Act (the reader orders the tables, and the order is used to create them)
            var tables = Helper.GetSourceTables();
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                var ordered = new PostgreSqlSchemaReader(connection).GetDependencyOrder(Enumerable.Reverse(tables)).Select(r => r.Table).ToList();
                Helper.ExecuteOnTarget(Composer.ComposeSchemas(ordered));
            }

            // Assert
            CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Act
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                var ordered = Helper.GetTableNames(new PostgreSqlSchemaReader(connection).GetDependencyOrder(new[] { "chain_c", "chain_b", "chain_a" }));

                // Assert
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, ordered);
            }
        }

        #endregion
    }
}
