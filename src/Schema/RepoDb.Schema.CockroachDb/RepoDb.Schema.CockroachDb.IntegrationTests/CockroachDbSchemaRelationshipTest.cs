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
using RepoDb.Connector.CockroachDb;
using RepoDb.Schema.Models;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    [TestClass]
    public class CockroachDbSchemaRelationshipTest
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

        private static CockroachDbSchemaReader CreateReader() =>
            new CockroachDbSchemaReader(new CockroachDbConnection(Database.ConnectionStringForSource));

        private static List<RelationshipInfo> Order(params string[] tableNames) =>
            CreateReader().GetDependencyOrder(tableNames).ToList();

        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships, string tableName) =>
            relationships.Single(r => r.Schema.Table.Name == tableName);

        private static int IndexOf(IEnumerable<RelationshipInfo> relationships, string tableName) =>
            relationships.Select(r => r.Schema.Table.Name).ToList().IndexOf(tableName);

        #endregion

        #region Dependency chain / diamond

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfDependencyChain()
        {
            // Act
            var actual = Order("chain_c", "chain_a", "chain_b");

            // Assert
            CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, Helper.GetTableNames(actual));
            Assert.AreEqual(0, Get(actual, "chain_a").Parents.Count);
            Assert.AreSame(Get(actual, "chain_a"), Get(actual, "chain_b").Parents.Single());
            Assert.AreSame(Get(actual, "chain_c"), Get(actual, "chain_b").Children.Single());
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfDiamond()
        {
            // Act
            var actual = Order("d_leaf", "d_right", "d_left", "d_root");

            // Assert
            Assert.AreEqual("d_root", actual[0].Schema.Table.Name);
            Assert.AreEqual("d_leaf", actual[3].Schema.Table.Name);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfDiamondParentsAndChildren()
        {
            // Act
            var actual = Order("d_root", "d_left", "d_right", "d_leaf");

            // Assert
            Assert.AreEqual(2, Get(actual, "d_root").Children.Count);
            Assert.AreEqual(2, Get(actual, "d_leaf").Parents.Count);
            Assert.AreEqual(0, Get(actual, "d_leaf").Children.Count);
            Assert.AreEqual(1, Get(actual, "d_left").Parents.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfDiamondInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("d_leaf", "d_right", "d_left", "d_root");

            // Assert
            foreach (var table in new[] { "d_root", "d_left", "d_right", "d_leaf" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = Order("transfer", "account");

            // Assert
            CollectionAssert.AreEqual(new[] { "account", "transfer" }, Helper.GetTableNames(actual));
            Assert.AreEqual(1, Get(actual, "transfer").Parents.Count);
            Assert.AreEqual(1, Get(actual, "account").Children.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfTableWithManyChildren()
        {
            // Act
            var actual = Order("fan_3", "fan_1", "fan_parent", "fan_2");

            // Assert
            Assert.AreEqual("fan_parent", actual[0].Schema.Table.Name);
            Assert.AreEqual(3, Get(actual, "fan_parent").Children.Count);
        }

        #endregion

        #region Cycles

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = Order("ring_2", "ring_3", "ring_1");

            // Assert
            CollectionAssert.AreEqual(new[] { "ring_2", "ring_3", "ring_1" }, Helper.GetTableNames(actual));
            Assert.IsTrue(actual.All(r => r.Parents.Count == 1 && r.Children.Count == 1));
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfCycleOfThreeTables()
        {
            // Act
            Helper.CopyAllToTarget("ring_3", "ring_1", "ring_2");

            // Assert
            foreach (var table in new[] { "ring_1", "ring_2", "ring_3" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            var actual = Order("loop_tail", "loop_b", "loop_a", "loop_base");

            // Assert
            CollectionAssert.AreEqual(new[] { "loop_base", "loop_b", "loop_a", "loop_tail" }, Helper.GetTableNames(actual));
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            Helper.CopyAllToTarget("loop_tail", "loop_a", "loop_b", "loop_base");

            // Assert
            foreach (var table in new[] { "loop_base", "loop_a", "loop_b", "loop_tail" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfSelfReferenceIsIgnored()
        {
            // Act
            var actual = Order("employee");

            // Assert
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(0, actual[0].Children.Count);
        }

        #endregion

        #region Names

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = Order("item_ref", "sales.item", "public.item");

            // Assert
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual("item_ref", actual[2].Schema.Table.Name);
            Assert.AreEqual(2, actual[2].Parents.Count);
            CollectionAssert.AreEquivalent(new[] { "public", "sales" }, actual[2].Parents.Select(p => p.Schema.Table.Schema).ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            Helper.CopyAllToTarget("item_ref", "sales.item", "public.item");

            // Assert
            Helper.AssertTargetMatchesSource("public.item");
            Helper.AssertTargetMatchesSource("sales.item");
            Helper.AssertTargetMatchesSource("item_ref");
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderReadsTheTableWithADotInItsName()
        {
            // Act
            var schema = CreateReader().GetTableSchema("\"odd.name\"");

            // Assert
            Assert.AreEqual("odd.name", schema.Table.Name);
            Assert.AreEqual("public", schema.Table.Schema);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderReadsTheTableWithASpaceInItsName()
        {
            // Act
            var schema = CreateReader().GetTableSchema("public.\"odd name\"");

            // Assert
            Assert.AreEqual("odd name", schema.Table.Name);
            Assert.AreEqual(new TableInfo("odd.name", "public"), schema.ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderReadsTheTableWithAQuoteInItsName()
        {
            // Act
            var schema = CreateReader().GetTableSchema("\"odd\"\"name\"");

            // Assert
            Assert.AreEqual("odd\"name", schema.Table.Name);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetTablesQuotesTheNamesThatNeedIt()
        {
            // Act
            var tables = CreateReader().GetTables("public").ToList();

            // Assert
            CollectionAssert.Contains(tables, "public.\"odd.name\"");
            CollectionAssert.Contains(tables, "public.\"odd name\"");
            CollectionAssert.Contains(tables, "public.\"odd\"\"name\"");
            CollectionAssert.Contains(tables, "public.\"Person\"");
            CollectionAssert.Contains(tables, "public.country");
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfTablesWithADotInTheirNames()
        {
            // Act
            var actual = Order("public.\"odd name\"", "public.\"odd.name\"");

            // Assert
            CollectionAssert.AreEqual(new[] { "odd.name", "odd name" }, Helper.GetTableNames(actual));
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTablesWithNamesThatNeedQuoting()
        {
            // Act
            Helper.CopyAllToTarget("public.\"odd name\"", "public.\"odd.name\"", "public.\"odd\"\"name\"");

            // Assert
            foreach (var table in new[] { "public.\"odd name\"", "public.\"odd.name\"", "public.\"odd\"\"name\"" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Direct references only

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOnlyConsidersTheDirectReferencesBetweenTheGivenTables()
        {
            // Act
            var actual = Order("chain_c", "chain_a");

            // Assert
            CollectionAssert.AreEqual(new[] { "chain_c", "chain_a" }, Helper.GetTableNames(actual));
            Assert.AreEqual(0, actual[0].Parents.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipKeepsTheGivenOrderOfTheTablesThatAreNotRelated()
        {
            // Act
            var actual = Order("ticket", "country", "no_key", "account");

            // Assert
            CollectionAssert.AreEqual(new[] { "ticket", "country", "no_key", "account" }, Helper.GetTableNames(actual));
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipOfTableThatReferencesATableThatIsNotGiven()
        {
            // Act
            var actual = Order("chain_b");

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual[0].Schema.ForeignKeys.Count);
            Assert.AreEqual(0, actual[0].Parents.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipIgnoresTheTableThatIsGivenMoreThanOnce()
        {
            // Act
            var actual = Order("chain_b", "chain_a", "public.chain_a");

            // Assert
            CollectionAssert.AreEqual(new[] { "chain_a", "chain_b" }, Helper.GetTableNames(actual));
        }

        #endregion

        #region The whole database

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipsOfTheWholeDatabaseAreOrderedInAnyGivenOrder()
        {
            // Act
            var tables = Helper.GetSourceTables();
            var forward = CreateReader().GetDependencyOrder(tables).ToList();
            var backward = CreateReader().GetDependencyOrder(Enumerable.Reverse(tables)).ToList();

            // Assert
            foreach (var relationships in new[] { forward, backward })
            {
                Assert.AreEqual(tables.Count, relationships.Count);
                foreach (var relationship in relationships)
                {
                    foreach (var parent in relationship.Parents)
                    {
                        var isCycle = parent.Parents.Any(p => ReferenceEquals(p, relationship)) || IsInCycle(parent, relationship);
                        Assert.IsTrue(isCycle || relationships.IndexOf(parent) < relationships.IndexOf(relationship), $"{parent.Schema.Table.Name} before {relationship.Schema.Table.Name}.");
                    }
                }
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelationshipsOfTheWholeDatabaseCoverTheForeignKeys()
        {
            // Act
            var relationships = CreateReader().GetDependencyOrder(Helper.GetSourceTables()).ToList();

            // Assert
            foreach (var relationship in relationships)
            {
                var referenced = relationship.Schema.ForeignKeys.Select(fk => fk.ReferencedTable).Distinct().Count(table => table != relationship.Schema.Table);
                Assert.AreEqual(referenced, relationship.Parents.Count, relationship.Schema.Table.Name);
                Assert.IsTrue(relationship.Parents.All(p => p.Children.Contains(relationship)));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTheWholeDatabaseInTheReverseOrder()
        {
            // Act
            var tables = Helper.GetSourceTables();
            Helper.CopyAllToTarget(Enumerable.Reverse(tables).ToArray());

            // Assert
            CollectionAssert.AreEquivalent(tables, Helper.GetTargetTables());
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbSchemaRelationshipsAsyncOfTheWholeDatabaseAreTheSameAsTheSyncOnes()
        {
            // Act
            var tables = Helper.GetSourceTables();
            var sync = CreateReader().GetDependencyOrder(tables).ToList();
            var async = (await CreateReader().GetDependencyOrderAsync(tables)).ToList();

            // Assert
            CollectionAssert.AreEqual(Helper.GetTableNames(sync), Helper.GetTableNames(async));
            CollectionAssert.AreEqual(sync.Select(r => r.Parents.Count).ToArray(), async.Select(r => r.Parents.Count).ToArray());
        }

        private static bool IsInCycle(RelationshipInfo parent, RelationshipInfo relationship)
        {
            var visited = new HashSet<RelationshipInfo>();
            var pending = new Stack<RelationshipInfo>(parent.Parents);
            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (ReferenceEquals(current, relationship))
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
