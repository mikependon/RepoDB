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
    [TestClass]
    public class PostgreSqlSchemaRelatedTablesTest
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

        private static string[] GetRelatedTables(CopySchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return new PostgreSqlSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(CopySchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return (await new PostgreSqlSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static string[] GetTargetTables()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).GetTables().ToArray();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "chain_b");

            // Assert (the table is resolved with its schema, and nothing else is returned)
            CollectionAssert.AreEqual(new[] { "public.chain_b" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "chain_c", "public.chain_a", "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.chain_a" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "chain_c");

            // Assert (the parent and the parent of the parent, up to the top of the tree)
            AssertSame(new[] { "public.chain_c", "public.chain_b", "public.chain_a" }, actual);
            Assert.AreEqual("public.chain_c", actual[0]);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "chain_a");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "d_leaf");

            // Assert
            AssertSame(new[] { "public.d_leaf", "public.d_left", "public.d_right", "public.d_root" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "sales.order_line");

            // Assert (the parent is the table with the case-sensitive name, and its own parent comes too)
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "item_ref");

            // Assert (it references both of them)
            AssertSame(new[] { "public.item_ref", "public.item", "sales.item" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemasDoesNotMixThem()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "sales.item");

            // Assert
            CollectionAssert.AreEqual(new[] { "sales.item" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfMultipleForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "transfer");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.transfer", "public.account" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfACompositeForeignKey()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "shipment");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.shipment", "public.region_code" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "public.\"odd name\"");

            // Assert
            AssertSame(new[] { "public.\"odd name\"", "public.\"odd.name\"" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "chain_a");

            // Assert (the child and the child of the child, down to the end of the tree)
            AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, actual);
            Assert.AreEqual("public.chain_a", actual[0]);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "d_root");

            // Assert
            AssertSame(new[] { "public.d_root", "public.d_left", "public.d_right", "public.d_leaf" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "country");

            // Assert (the child is in another schema, one level down)
            AssertSame(new[] { "public.country", "public.\"Person\"", "sales.order_line" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "fan_parent");

            // Assert
            AssertSame(new[] { "public.fan_parent", "public.fan_1", "public.fan_2", "public.fan_3" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_b");

            // Assert
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" }, actual);
            Assert.AreEqual("public.chain_b", actual[0]);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_a");
            var fromTheLeaf = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_c");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "d_left");

            // Assert
            AssertSame(new[] { "public.d_left", "public.d_root", "public.d_right", "public.d_leaf" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "sales.order_line");

            // Assert
            AssertSame(new[] { "sales.order_line", "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_a");

            // Assert
            Assert.IsFalse(actual.Contains("public.country"));
            Assert.IsFalse(actual.Contains("public.account"));
            Assert.IsFalse(actual.Contains("public.no_key"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "no_key");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.no_key" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "node_a");

                // Assert
                AssertSame(new[] { "public.node_a", "public.node_b" }, actual);
            }
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "ring_1");

                // Assert
                AssertSame(new[] { "public.ring_1", "public.ring_2", "public.ring_3" }, actual);
            }
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "loop_b");
            var children = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "loop_a");
            var endToEnd = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "loop_a");

            // Assert
            AssertSame(new[] { "public.loop_b", "public.loop_a", "public.loop_base" }, parents);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_tail" }, children);
            AssertSame(new[] { "public.loop_a", "public.loop_b", "public.loop_base", "public.loop_tail" }, endToEnd);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
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
        public void TestPostgreSqlSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "chain_c", "d_left");

            // Assert (the given tables come first, in the given order)
            AssertSame(new[] { "public.chain_c", "public.d_left", "public.chain_b", "public.chain_a", "public.d_root" }, actual);
            CollectionAssert.AreEqual(new[] { "public.chain_c", "public.d_left" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_a", "chain_b", "chain_c");

            // Assert
            CollectionAssert.AreEqual(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act (the names are case-sensitive, so "Person" and "person" are not the same table, and only the existing one has relatives)
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "\"Person\"");

            // Assert
            AssertSame(new[] { "public.\"Person\"", "public.country" }, actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new PostgreSqlSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "chain_c" }, CopySchemaRelationshipBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert (the tables are ordered so that a table comes after the tables that it references)
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "public.chain_b" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.TableOnly, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Parents, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_c" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Children, "chain_b"));
            AssertSame(new[] { "public.chain_b", "public.chain_a", "public.chain_c" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "chain_b"));
        }

        [TestMethod]
        public async Task TestPostgreSqlSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "node_a");

            // Assert
            AssertSame(new[] { "public.node_a", "public.node_b" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new PostgreSqlSchemaReader(connection).GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetRelatedTables((CopySchemaRelationshipBehavior)99, "chain_b"));
        }

        #endregion

        #region CopySchemaTo

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "chain_c" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
                Helper.AssertTargetMatchesSource("chain_a");
                Helper.AssertTargetMatchesSource("chain_b");
                Helper.AssertTargetMatchesSource("chain_c");
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "chain_a" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopySchemaTo(new[] { "chain_b" }, target,
                    relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.TableName));

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
                CollectionAssert.AreEqual(new[] { "chain_a", "chain_b", "chain_c" }, created);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act/Assert (the foreign key can not be created, as the parent is not part of the copy)
                Assert.ThrowsExactly<PostgresException>(() =>
                    source.CopySchemaTo(new[] { "chain_b" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "loop_a" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "public.loop_base", "public.loop_a", "public.loop_b", "public.loop_tail" }, GetTargetTables());
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                await source.CopySchemaToAsync(new[] { "chain_b" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables());
            }
        }


        [TestMethod]
        public void TestPostgreSqlCopySchemaToOfASingleTableWithParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("chain_c", target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert (the result is the one of the given table, and the parents are copied too)
                Assert.AreEqual("chain_c", result.TableName);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                var result = await source.CopySchemaToAsync("chain_b", target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual("chain_b", result.TableName);
                AssertSame(new[] { "public.chain_a", "public.chain_b", "public.chain_c" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("public.\"odd.name\"", target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert (the table is identified even if its name needs quoting)
                Assert.AreEqual("odd.name", result.TableName);
                AssertSame(new[] { "public.\"odd.name\"", "public.\"odd name\"" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("chain_a", target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual("chain_a", result.TableName);
                AssertSame(new[] { "public.chain_a" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
