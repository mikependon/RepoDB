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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the postgre sql schema related tables test behavior.
    /// </summary>
    [TestClass]
    public class PostgreSqlSchemaRelatedTablesTest
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
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return new PostgreSqlSchemaReader().GetRelatedTables(connection,
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
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return (await new PostgreSqlSchemaReader().GetRelatedTablesAsync(connection,
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
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader().GetTables(connection,
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
        /// Verifies postgre sql schema related tables with table only.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "chain_b");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with table only keeps the given tables in order.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
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
        /// Verifies postgre sql schema related tables with parents.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParents()
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
        /// Verifies postgre sql schema related tables with parents does not include the children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_b");
            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of atable without parents.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "chain_a");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of adiamond.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "d_leaf");
            // Assert
            AssertSame(new[] { "public.d_leaf", "public.d_left", "public.d_right", "public.d_root" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents across schemas.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "sales.order_line");
            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "item_ref");
            // Assert
            AssertSame(new[] { "public.item_ref", "public.item", "sales.item" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of tables with the same name in different schemas does not mix them.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemasDoesNotMixThem()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "sales.item");
            // Assert
            CollectionAssert.AreEqual(new[] { "sales.item" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of multiple foreign keys to the same table.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfMultipleForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "transfer");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.transfer", "public.account" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of acomposite foreign key.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfACompositeForeignKey()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "shipment");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.shipment", "public.region_code" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents of tables with special names.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
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
        /// Verifies postgre sql schema related tables with children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildren()
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
        /// Verifies postgre sql schema related tables with children does not include the parents.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "chain_b");
            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_c" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with children of atable without children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "chain_c");
            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "d_root");
            // Assert
            AssertSame(new[] { "public.d_root", "public.d_left", "public.d_right", "public.d_leaf" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with children across schemas.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "country");
            // Assert
            AssertSame(new[] { "public.country", "public.\"Person\"", "sales.order_line" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with children of atable with many children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
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
        /// Verifies postgre sql schema related tables with parents and children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildren()
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
        /// Verifies postgre sql schema related tables with parents and children gives the same tables from any table of the tree.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
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
        /// Verifies postgre sql schema related tables with parents and children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "d_left");
            // Assert
            AssertSame(new[] { "public.d_left", "public.d_root", "public.d_right", "public.d_leaf" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents and children reaches the other branches.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "sales.order_line");
            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables with parents and children does not include the unrelated tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
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
        /// Verifies postgre sql schema related tables with parents and children of atable without relationships.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
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
        /// Verifies postgre sql schema related tables of acycle.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycle()
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
        /// Verifies postgre sql schema related tables of acycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycleOfThreeTables()
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
        /// Verifies postgre sql schema related tables of acycle with tables around it.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycleWithTablesAroundIt()
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
        /// Verifies postgre sql schema related tables of aself reference.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfASelfReference()
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
        /// Verifies postgre sql schema related tables of multiple tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfMultipleTables()
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
        /// Verifies postgre sql schema related tables does not repeat atable.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesDoesNotRepeatATable()
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
        /// Verifies postgre sql schema related tables without tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);
            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables are compared with the case.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "\"Person\"");
            // Assert
            AssertSame(new[] { "public.\"Person\"", "public.country" },
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema related tables can be given to get dependency order.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new PostgreSqlSchemaReader();
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
        /// Verifies postgre sql schema related tables async with each behavior.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlSchemaRelatedTablesAsyncWithEachBehavior()
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
        /// Verifies postgre sql schema related tables async of acycle.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlSchemaRelatedTablesAsyncOfACycle()
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
        /// Verifies throw exception on postgre sql schema related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => new PostgreSqlSchemaReader().GetRelatedTables(connection,
                        null,
                        RelatedTableBehavior.ParentsAndChildren,
                        transaction: null));
            }
        }

        /// <summary>
        /// Verifies throw exception on postgre sql schema related tables if the behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetRelatedTables((RelatedTableBehavior)99,
                    "chain_b"));
        }

        #endregion

        #region CopyTableSchemaTo

        /// <summary>
        /// Verifies postgre sql copy table schema to with parents.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToWithParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to with children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToWithChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to with parents and children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to with table only does not copy the related tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
                    true);
                // Act/Assert
                Assert.ThrowsExactly<PostgresException>(() => source.CopyTableSchemaTo(new[] { "chain_b" },
                        target,
                        relationshipBehavior: RelatedTableBehavior.TableOnly));
            }
        }

        /// <summary>
        /// Verifies postgre sql copy table schema to with parents and children of acycle.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to async with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to of asingle table with parents.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to async of asingle table with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to of asingle table with special names and children.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
        /// Verifies postgre sql copy table schema to of asingle table with table only returns the result.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(),
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
