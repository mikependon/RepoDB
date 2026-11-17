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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the aurora db postgre sql schema related tables test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaRelatedTablesTest
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
        /// Performs get related tables.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <param name="tableNames">The table names.</param>
        private static string[] GetRelatedTables(RelatedTableBehavior behavior,
            params string[] tableNames)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                return new AuroraDbPostgreSqlSchemaReader().GetRelatedTables(connection,
                    tableNames,
                    behavior,
                    transaction: null).ToArray();
            }
        }

        /// <summary>
        /// Performs get related tables async.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <param name="tableNames">The table names.</param>
        private static async Task<string[]> GetRelatedTablesAsync(RelatedTableBehavior behavior,
            params string[] tableNames)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                return (await new AuroraDbPostgreSqlSchemaReader().GetRelatedTablesAsync(connection,
                    tableNames,
                    behavior,
                    transaction: null)).ToArray();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static string[] GetTargetTables()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                return new AuroraDbPostgreSqlSchemaReader().GetTables(connection,
                    transaction: null).ToArray();
            }
        }

        /// <summary>
        /// Performs assert same.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        private static void AssertSame(string[] expected,
            string[] actual) => CollectionAssert.AreEquivalent(expected,
            actual,
            string.Join(", ",
                actual));
        #endregion

        #region TableOnly

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with table only.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "chain_b");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with table only keeps the given tables in order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "chain_c",
                "public.chain_a",
                "chain_c");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.chain_a" },
                actual);
        }

        #endregion

        #region Parents

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_c");
            // Assert
            AssertSame(new[] { "public.chain_c", "public.chain_b", "public.chain_a" },
                actual);
            Assert.AreEqual("public.chain_c",
                actual[0]);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents does not include the children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_b");
            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of atable without parents.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_a");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "d_leaf");
            // Assert
            AssertSame(new[] { "public.d_leaf", "public.d_left", "public.d_right", "public.d_root" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents across schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "sales.order_line");
            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "item_ref");
            // Assert
            AssertSame(new[] { "public.item_ref", "public.item", "sales.item" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of tables with the same name in different schemas does not mix them.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemasDoesNotMixThem()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "sales.item");
            // Assert
            CollectionAssert.AreEqual(new[] { "sales.item" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of multiple foreign keys to the same table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfMultipleForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "transfer");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.transfer", "public.account" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of acomposite foreign key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfACompositeForeignKey()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "shipment");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.shipment", "public.region_code" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents of tables with special names.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "public.\"odd name\"");
            // Assert
            AssertSame(new[] { "public.\"odd name\"", "public.\"odd.name\"" },
                actual);
        }

        #endregion

        #region Children

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "chain_a");
            // Assert
            AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                actual);
            Assert.AreEqual("public.chain_a",
                actual[0]);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children does not include the parents.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "chain_b");
            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_c" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children of atable without children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "chain_c");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "d_root");
            // Assert
            AssertSame(new[] { "public.d_root", "public.d_left", "public.d_right", "public.d_leaf" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children across schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "country");
            // Assert
            AssertSame(new[] { "public.country", "public.\"Person\"", "sales.order_line" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with children of atable with many children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "fan_parent");
            // Assert
            AssertSame(new[] { "public.fan_parent", "public.fan_1", "public.fan_2", "public.fan_3" },
                actual);
        }

        #endregion

        #region ParentsAndChildren

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "chain_b");
            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" },
                actual);
            Assert.AreEqual("public.chain_b",
                actual[0]);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children gives the same tables from any table of the tree.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "chain_a");
            var fromTheLeaf = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "chain_c");
            // Assert
            AssertSame(fromTheRoot,
                fromTheLeaf);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "d_left");
            // Assert
            AssertSame(new[] { "public.d_left", "public.d_root", "public.d_right", "public.d_leaf" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children reaches the other branches.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "sales.order_line");
            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children does not include the unrelated tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "chain_a");
            // Assert
            Assert.IsFalse(actual.Contains("public.country"));
            Assert.IsFalse(actual.Contains("public.account"));
            Assert.IsFalse(actual.Contains("public.no_key"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables with parents and children of atable without relationships.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "no_key");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.no_key" },
                actual);
        }

        #endregion

        #region Cycles and Self References

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables of acycle.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[]
            {
                RelatedTableBehavior.Parents,
                RelatedTableBehavior.Children,
                RelatedTableBehavior.ParentsAndChildren
            }

            )
            {
                // Act
                var actual = GetRelatedTables(behavior,
                    "node_a");
                // Assert
                AssertSame(new[] { "public.node_a", "public.node_b" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables of acycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[]
            {
                RelatedTableBehavior.Parents,
                RelatedTableBehavior.Children,
                RelatedTableBehavior.ParentsAndChildren
            }

            )
            {
                // Act
                var actual = GetRelatedTables(behavior,
                    "ring_1");
                // Assert
                AssertSame(new[] { "public.ring_1", "public.ring_2", "public.ring_3" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables of acycle with tables around it.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(RelatedTableBehavior.Parents,
                "loop_b");
            var children = GetRelatedTables(RelatedTableBehavior.Children,
                "loop_a");
            var endToEnd = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "loop_a");
            // Assert
            AssertSame(new[] { "public.loop_b", "public.loop_a", "public.loop_base" },
                parents);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_tail" },
                children);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_base", "public.loop_tail" },
                endToEnd);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables of aself reference.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[]
            {
                RelatedTableBehavior.Parents,
                RelatedTableBehavior.Children,
                RelatedTableBehavior.ParentsAndChildren
            }

            )
            {
                // Act
                var actual = GetRelatedTables(behavior,
                    "employee");
                // Assert
                CollectionAssert.AreEqual(new[] { "public.employee" },
                    actual);
            }
        }

        #endregion

        #region Multiple Tables

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables of multiple tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_c",
                "d_left");
            // Assert
            AssertSame(new[] { "public.chain_c", "public.d_left", "public.chain_b", "public.chain_a", "public.d_root" },
                actual);
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.d_left" },
                actual.Take(2).ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables does not repeat atable.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "chain_a",
                "chain_b",
                "chain_c");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables without tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);
            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables are compared with the case.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "\"Person\"");
            // Assert
            AssertSame(new[] { "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables can be given to get dependency order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                var reader = new AuroraDbPostgreSqlSchemaReader();
                var related = reader.GetRelatedTables(connection,
                    new[] { "chain_c" },
                    RelatedTableBehavior.Parents,
                    transaction: null);
                var actual = reader.GetDependencyOrder(connection,
                    related,
                    transaction: null).Select(r => r.Schema.Table.Name).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" },
                    actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables async with each behavior.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" },
                await GetRelatedTablesAsync(RelatedTableBehavior.TableOnly,
                    "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Parents,
                    "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_c" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Children,
                    "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" },
                await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                    "chain_b"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema related tables async of acycle.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                "node_a");
            // Assert
            AssertSame(new[] { "public.node_a", "public.node_b" },
                actual);
        }

        #endregion

        #region Arguments

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => new AuroraDbPostgreSqlSchemaReader().GetRelatedTables(connection,
                        null,
                        RelatedTableBehavior.ParentsAndChildren,
                        transaction: null));
            }
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema related tables if the behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetRelatedTables((RelatedTableBehavior)99,
                    "chain_b"));
        }

        #endregion

        #region CopyTableSchemaTo

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to with parents.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithParents()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "chain_c" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Parents);
                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables());
                Helper.AssertTargetMatchesSource("chain_a");
                Helper.AssertTargetMatchesSource("chain_b");
                Helper.AssertTargetMatchesSource("chain_c");
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to with children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "chain_a" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children);
                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to with parents and children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                var created = new List<string>();
                // Act
                source.CopyTableSchemaTo(new[] { "chain_b" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.Target.Table.Table.Name));
                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables());
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" },
                    created);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to with table only does not copy the related tables.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act/Assert
                Assert.ThrowsExactly<AuroraDbException>(() => source.CopyTableSchemaTo(new[] { "chain_b" },
                        target,
                        relationshipBehavior: RelatedTableBehavior.TableOnly));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to with parents and children of acycle.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "loop_a" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "public.loop_base", "public.loop_a", "public.loop_b", "public.loop_tail" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to async with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                await source.CopyTableSchemaToAsync(new[] { "chain_b" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to of asingle table with parents.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("chain_c",
                    target,
                    relationshipBehavior: RelatedTableBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "chain_c",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("chain_c",
                    result.Target.Table.Table.Name);
                Assert.AreEqual(TableOutcome.Created,
                    result.Outcome);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to async of asingle table with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                var result = (await source.CopyTableSchemaToAsync("chain_b",
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "chain_b",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("chain_b",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to of asingle table with special names and children.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("public.\"odd.name\"",
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "Odd.Name",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("odd.name",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "public.\"odd.name\"", "public.\"odd name\"" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to of asingle table with table only returns the result.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("chain_a",
                    target,
                    relationshipBehavior: RelatedTableBehavior.TableOnly).Items.Single();
                // Assert
                Assert.AreEqual("chain_a",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "public.chain_a" },
                    GetTargetTables().ToArray());
            }
        }
        #endregion
    }
}
