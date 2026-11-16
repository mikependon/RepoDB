#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the aurora db postgre sql schema relationship test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaRelationshipTest
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
        public void Cleanup() => Database.Cleanup();
        #region Private Methods

        /// <summary>
        /// Performs create reader.
        /// </summary>
        private static AuroraDbPostgreSqlSchemaReader CreateReader() => new AuroraDbPostgreSqlSchemaReader();

        /// <summary>
        /// Performs order.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        private static List<RelationshipInfo> Order(params string[] tableNames)
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource);
            return CreateReader().GetDependencyOrder(operationConnection,
                tableNames).ToList();
        }

        /// <summary>
        /// Performs get.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        /// <param name="tableName">The table name.</param>
        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships,
            string tableName) => relationships.Single(r => r.Schema.Table.Name == tableName);

        /// <summary>
        /// Performs index of.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        /// <param name="tableName">The table name.</param>
        private static int IndexOf(IEnumerable<RelationshipInfo> relationships,
            string tableName) => relationships.Select(r => r.Schema.Table.Name).ToList().IndexOf(tableName);
        #endregion

        #region Dependency chain / diamond

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of dependency chain.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfDependencyChain()
        {
            // Act
            var actual = Order("chain_c",
                "chain_a",
                "chain_b");
            // Assert
            CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" },
                Helper.GetTableNames(actual));
            Assert.AreEqual(0,
                Get(actual,
                    "chain_a").Parents.Count);
            Assert.AreSame(Get(actual,
                    "chain_a"),
                Get(actual,
                    "chain_b").Parents.Single());
            Assert.AreSame(Get(actual,
                    "chain_c"),
                Get(actual,
                    "chain_b").Children.Single());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of diamond.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfDiamond()
        {
            // Act
            var actual = Order("d_leaf",
                "d_right",
                "d_left",
                "d_root");
            // Assert
            Assert.AreEqual("d_root",
                actual[0].Schema.Table.Name);
            Assert.AreEqual("d_leaf",
                actual[3].Schema.Table.Name);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of diamond parents and children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfDiamondParentsAndChildren()
        {
            // Act
            var actual = Order("d_root",
                "d_left",
                "d_right",
                "d_leaf");
            // Assert
            Assert.AreEqual(2,
                Get(actual,
                    "d_root").Children.Count);
            Assert.AreEqual(2,
                Get(actual,
                    "d_leaf").Parents.Count);
            Assert.AreEqual(0,
                Get(actual,
                    "d_leaf").Children.Count);
            Assert.AreEqual(1,
                Get(actual,
                    "d_left").Parents.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of diamond in any order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfDiamondInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("d_leaf",
                "d_right",
                "d_left",
                "d_root");
            // Assert
            foreach (var table in new[]
            {
                "d_root",
                "d_left",
                "d_right",
                "d_leaf"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of table with two foreign keys to the same table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = Order("transfer",
                "account");
            // Assert
            CollectionAssert.AreEqual(new[] { "account", "transfer" },
                Helper.GetTableNames(actual));
            Assert.AreEqual(1,
                Get(actual,
                    "transfer").Parents.Count);
            Assert.AreEqual(1,
                Get(actual,
                    "account").Children.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of table with many children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfTableWithManyChildren()
        {
            // Act
            var actual = Order("fan_3",
                "fan_1",
                "fan_parent",
                "fan_2");
            // Assert
            Assert.AreEqual("fan_parent",
                actual[0].Schema.Table.Name);
            Assert.AreEqual(3,
                Get(actual,
                    "fan_parent").Children.Count);
        }

        #endregion

        #region Cycles

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of cycle of three tables keeps the given order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = Order("ring_2",
                "ring_3",
                "ring_1");
            // Assert
            CollectionAssert.AreEqual(new[] { "ring_2", "ring_3", "ring_1" },
                Helper.GetTableNames(actual));
            Assert.IsTrue(actual.All(r => r.Parents.Count == 1 && r.Children.Count == 1));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of cycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfCycleOfThreeTables()
        {
            // Act
            Helper.CopyAllToTarget("ring_3",
                "ring_1",
                "ring_2");
            // Assert
            foreach (var table in new[]
            {
                "ring_1",
                "ring_2",
                "ring_3"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of cycle with the tables that it depends on and that depend on it.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            var actual = Order("loop_tail",
                "loop_b",
                "loop_a",
                "loop_base");
            // Assert
            CollectionAssert.AreEqual(new[] { "loop_base", "loop_b", "loop_a", "loop_tail" },
                Helper.GetTableNames(actual));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of cycle with the tables that it depends on and that depend on it.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            Helper.CopyAllToTarget("loop_tail",
                "loop_a",
                "loop_b",
                "loop_base");
            // Assert
            foreach (var table in new[]
            {
                "loop_base",
                "loop_a",
                "loop_b",
                "loop_tail"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of self reference is ignored.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfSelfReferenceIsIgnored()
        {
            // Act
            var actual = Order("employee");
            // Assert
            Assert.AreEqual(0,
                actual[0].Parents.Count);
            Assert.AreEqual(0,
                actual[0].Children.Count);
        }

        #endregion

        #region Names

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = Order("item_ref",
                "sales.item",
                "public.item");
            // Assert
            Assert.AreEqual(3,
                actual.Count);
            Assert.AreEqual("item_ref",
                actual[2].Schema.Table.Name);
            Assert.AreEqual(2,
                actual[2].Parents.Count);
            CollectionAssert.AreEquivalent(new[] { "public", "sales" },
                actual[2].Parents.Select(p => p.Schema.Table.Schema).ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            Helper.CopyAllToTarget("item_ref",
                "sales.item",
                "public.item");
            // Assert
            Helper.AssertTargetMatchesSource("public.item");
            Helper.AssertTargetMatchesSource("sales.item");
            Helper.AssertTargetMatchesSource("item_ref");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader reads the table with adot in its name.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderReadsTheTableWithADotInItsName()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var schema = CreateReader().GetTableSchema(operationConnection,
                "\"odd.name\"");
            // Assert
            Assert.AreEqual("odd.name",
                schema.Table.Name);
            Assert.AreEqual("public",
                schema.Table.Schema);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader reads the table with aspace in its name.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderReadsTheTableWithASpaceInItsName()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var schema = CreateReader().GetTableSchema(operationConnection,
                "public.\"odd name\"");
            // Assert
            Assert.AreEqual("odd name",
                schema.Table.Name);
            Assert.AreEqual(new TableInfo("odd.name",
                    "public"),
                schema.ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader reads the table with aquote in its name.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderReadsTheTableWithAQuoteInItsName()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var schema = CreateReader().GetTableSchema(operationConnection,
                "\"odd\"\"name\"");
            // Assert
            Assert.AreEqual("odd\"name",
                schema.Table.Name);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get tables quotes the names that need it.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetTablesQuotesTheNamesThatNeedIt()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var tables = CreateReader().GetTables(operationConnection,
                "public").ToList();
            // Assert
            CollectionAssert.Contains(tables,
                "public.\"odd.name\"");
            CollectionAssert.Contains(tables,
                "public.\"odd name\"");
            CollectionAssert.Contains(tables,
                "public.\"odd\"\"name\"");
            CollectionAssert.Contains(tables,
                "public.\"Person\"");
            CollectionAssert.Contains(tables,
                "public.country");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of tables with adot in their names.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfTablesWithADotInTheirNames()
        {
            // Act
            var actual = Order("public.\"odd name\"",
                "public.\"odd.name\"");
            // Assert
            CollectionAssert.AreEqual(new[] { "odd.name", "odd name" },
                Helper.GetTableNames(actual));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of tables with names that need quoting.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesWithNamesThatNeedQuoting()
        {
            // Act
            Helper.CopyAllToTarget("public.\"odd name\"",
                "public.\"odd.name\"",
                "public.\"odd\"\"name\"");
            // Assert
            foreach (var table in new[]
            {
                "public.\"odd name\"",
                "public.\"odd.name\"",
                "public.\"odd\"\"name\""
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Direct references only

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship only considers the direct references between the given tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOnlyConsidersTheDirectReferencesBetweenTheGivenTables()
        {
            // Act
            var actual = Order("chain_c",
                "chain_a");
            // Assert
            CollectionAssert.AreEqual(new[] { "chain_c", "chain_a" },
                Helper.GetTableNames(actual));
            Assert.AreEqual(0,
                actual[0].Parents.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship keeps the given order of the tables that are not related.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipKeepsTheGivenOrderOfTheTablesThatAreNotRelated()
        {
            // Act
            var actual = Order("ticket",
                "country",
                "no_key",
                "account");
            // Assert
            CollectionAssert.AreEqual(new[] { "ticket", "country", "no_key", "account" },
                Helper.GetTableNames(actual));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship of table that references atable that is not given.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipOfTableThatReferencesATableThatIsNotGiven()
        {
            // Act
            var actual = Order("chain_b");
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual(1,
                actual[0].Schema.ForeignKeys.Count);
            Assert.AreEqual(0,
                actual[0].Parents.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationship ignores the table that is given more than once.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            // Act
            var actual = Order("chain_b",
                "chain_a",
                "public.chain_a");
            // Assert
            CollectionAssert.AreEqual(new[] { "chain_a", "chain_b" },
                Helper.GetTableNames(actual));
        }

        #endregion

        #region The whole database

        /// <summary>
        /// Verifies aurora db postgre sql schema relationships of the whole database are ordered in any given order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipsOfTheWholeDatabaseAreOrderedInAnyGivenOrder()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var tables = Helper.GetSourceTables();
            var forward = CreateReader().GetDependencyOrder(operationConnection,
                tables).ToList();
            var backward = CreateReader().GetDependencyOrder(operationConnection,
                Enumerable.Reverse(tables)).ToList();
            // Assert
            foreach (var relationships in new[]
            {
                forward,
                backward
            }

            )
            {
                Assert.AreEqual(tables.Count,
                    relationships.Count);
                foreach (var relationship in relationships)
                {
                    foreach (var parent in relationship.Parents)
                    {
                        var isCycle = parent.Parents.Any(p => ReferenceEquals(p,
                                relationship)) || IsInCycle(parent,
                            relationship);
                        Assert.IsTrue(isCycle || relationships.IndexOf(parent) < relationships.IndexOf(relationship),
                            $"{parent.Schema.Table.Name} before {relationship.Schema.Table.Name}.");
                    }
                }
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationships of the whole database cover the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelationshipsOfTheWholeDatabaseCoverTheForeignKeys()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var relationships = CreateReader().GetDependencyOrder(operationConnection,
                Helper.GetSourceTables()).ToList();
            // Assert
            foreach (var relationship in relationships)
            {
                var referenced = relationship.Schema.ForeignKeys.Select(fk => fk.ReferencedTable).Distinct().Count(table => table != relationship.Schema.Table);
                Assert.AreEqual(referenced,
                    relationship.Parents.Count,
                    relationship.Schema.Table.Name);
                Assert.IsTrue(relationship.Parents.All(p => p.Children.Contains(relationship)));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of the whole database in the reverse order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTheWholeDatabaseInTheReverseOrder()
        {
            // Act
            var tables = Helper.GetSourceTables();
            Helper.CopyAllToTarget(Enumerable.Reverse(tables).ToArray());
            // Assert
            CollectionAssert.AreEquivalent(tables,
                Helper.GetTargetTables());
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema relationships async of the whole database are the same as the sync ones.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaRelationshipsAsyncOfTheWholeDatabaseAreTheSameAsTheSyncOnes()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var tables = Helper.GetSourceTables();
            var sync = CreateReader().GetDependencyOrder(operationConnection,
                tables).ToList();
            var async = (await CreateReader().GetDependencyOrderAsync(operationConnection,
                tables)).ToList();
            // Assert
            CollectionAssert.AreEqual(Helper.GetTableNames(sync),
                Helper.GetTableNames(async));
            CollectionAssert.AreEqual(sync.Select(r => r.Parents.Count).ToArray(),
                async.Select(r => r.Parents.Count).ToArray());
        }

        /// <summary>
        /// Performs is in cycle.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="relationship">The relationship.</param>
        private static bool IsInCycle(RelationshipInfo parent,
            RelationshipInfo relationship)
        {
            var visited = new HashSet<RelationshipInfo>();
            var pending = new Stack<RelationshipInfo>(parent.Parents);
            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (ReferenceEquals(current,
                    relationship))
                {
                    return true;
                }

                if (visited.Add(current))
                {
                    foreach (var next in current.Parents)
                    {
                        pending.Push(next);
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
