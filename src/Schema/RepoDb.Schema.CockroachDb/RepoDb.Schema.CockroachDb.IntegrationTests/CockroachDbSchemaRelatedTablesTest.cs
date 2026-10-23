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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    [TestClass]
    public class CockroachDbSchemaRelatedTablesTest
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

        private static string[] GetRelatedTables(CopyTableSchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                return new CockroachDbSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                return (await new CockroachDbSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static string[] GetTargetTables()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                return new CockroachDbSchemaReader(connection).GetTables().ToArray();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.TableOnly, "chain_b");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.TableOnly, "chain_c", "public.chain_a", "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.chain_a" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "chain_c");

            // Assert
            AssertSame(new[] { "public.chain_c", "public.chain_b", "public.chain_a" }, actual);
            Assert.AreEqual("public.chain_c", actual[0]);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "chain_a");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "d_leaf");

            // Assert
            AssertSame(new[] { "public.d_leaf", "public.d_left", "public.d_right", "public.d_root" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "sales.order_line");

            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "item_ref");

            // Assert
            AssertSame(new[] { "public.item_ref", "public.item", "sales.item" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemasDoesNotMixThem()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "sales.item");

            // Assert
            CollectionAssert.AreEqual(new[] { "sales.item" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfMultipleForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "transfer");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.transfer", "public.account" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfACompositeForeignKey()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "shipment");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.shipment", "public.region_code" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "public.\"odd name\"");

            // Assert
            AssertSame(new[] { "public.\"odd name\"", "public.\"odd.name\"" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "chain_a");

            // Assert
            AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, actual);
            Assert.AreEqual("public.chain_a", actual[0]);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "d_root");

            // Assert
            AssertSame(new[] { "public.d_root", "public.d_left", "public.d_right", "public.d_leaf" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "country");

            // Assert
            AssertSame(new[] { "public.country", "public.\"Person\"", "sales.order_line" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "fan_parent");

            // Assert
            AssertSame(new[] { "public.fan_parent", "public.fan_1", "public.fan_2", "public.fan_3" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" }, actual);
            Assert.AreEqual("public.chain_b", actual[0]);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_a");
            var fromTheLeaf = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_c");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "d_left");

            // Assert
            AssertSame(new[] { "public.d_left", "public.d_root", "public.d_right", "public.d_leaf" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "sales.order_line");

            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_a");

            // Assert
            Assert.IsFalse(actual.Contains("public.country"));
            Assert.IsFalse(actual.Contains("public.account"));
            Assert.IsFalse(actual.Contains("public.no_key"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "no_key");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.no_key" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "node_a");

                // Assert
                AssertSame(new[] { "public.node_a", "public.node_b" }, actual);
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "ring_1");

                // Assert
                AssertSame(new[] { "public.ring_1", "public.ring_2", "public.ring_3" }, actual);
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "loop_b");
            var children = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "loop_a");
            var endToEnd = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "loop_a");

            // Assert
            AssertSame(new[] { "public.loop_b", "public.loop_a", "public.loop_base" }, parents);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_tail" }, children);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_base", "public.loop_tail" }, endToEnd);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "employee");

                // Assert
                CollectionAssert.AreEqual(new[] { "public.employee" }, actual);
            }
        }

        #endregion

        #region Multiple Tables

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "chain_c", "d_left");

            // Assert
            AssertSame(new[] { "public.chain_c", "public.d_left", "public.chain_b", "public.chain_a", "public.d_root" }, actual);
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.d_left" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_a", "chain_b", "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "\"Person\"");

            // Assert
            AssertSame(new[] { "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                var reader = new CockroachDbSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "chain_c" }, CopyTableSchemaRelationshipBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.TableOnly, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.Parents, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_c" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.Children, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "chain_b"));
        }

        [TestMethod]
        public async Task TestCockroachDbSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "node_a");

            // Assert
            AssertSame(new[] { "public.node_a", "public.node_b" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new CockroachDbSchemaReader(connection).GetRelatedTables(null, CopyTableSchemaRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetRelatedTables((CopyTableSchemaRelationshipBehavior)99, "chain_b"));
        }

        #endregion

        #region CopyTableSchemaTo

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithParents()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "chain_c" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
                Helper.AssertTargetMatchesSource("chain_a");
                Helper.AssertTargetMatchesSource("chain_b");
                Helper.AssertTargetMatchesSource("chain_c");
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithChildren()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "chain_a" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopyTableSchemaTo(new[] { "chain_b" }, target,
                    relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.Target.Table.Table.Name));

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, created);
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act/Assert
                Assert.ThrowsExactly<CockroachDbException>(() =>
                    source.CopyTableSchemaTo(new[] { "chain_b" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "loop_a" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "public.loop_base", "public.loop_a", "public.loop_b", "public.loop_tail" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "chain_b" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
            }
        }


        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("chain_c", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "chain_c", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("chain_c", result.Target.Table.Table.Name);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                var result = (await source.CopyTableSchemaToAsync("chain_b", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "chain_b", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("chain_b", result.Target.Table.Table.Name);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("public.\"odd.name\"", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Odd.Name", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("odd.name", result.Target.Table.Table.Name);
                AssertSame(new[] { "public.\"odd.name\"", "public.\"odd name\"" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("chain_a", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly).Items.Single();

                // Assert
                Assert.AreEqual("chain_a", result.Target.Table.Table.Name);
                AssertSame(new[] { "public.chain_a" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
