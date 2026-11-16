#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Ahtola.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Ahtola.IntegrationTests
{
    /// <summary>
    /// Contains the ahtola schema related tables test behavior.
    /// </summary>
    [TestClass]
    public class AhtolaSchemaRelatedTablesTest
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
            using (var connection = Database.CreateSource())
            {
                return new AhtolaSchemaReader().GetRelatedTables(connection,
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
            using (var connection = Database.CreateSource())
            {
                return (await new AhtolaSchemaReader().GetRelatedTablesAsync(connection,
                    tableNames,
                    behavior,
                    transaction: null)).ToArray();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = Database.CreateTarget())
            {
                return new AhtolaSchemaReader().GetTables(connection,
                    transaction: null).ToList();
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
        /// Verifies ahtola schema related tables with table only.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "Child");
            // Assert
            CollectionAssert.AreEqual(new[] { "Child" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with table only keeps the given tables in order.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "GrandChild",
                "Parent",
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" },
                actual);
        }

        #endregion

        #region Parents

        /// <summary>
        /// Verifies ahtola schema related tables with parents.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "GrandChild");
            // Assert
            AssertSame(new[] { "GrandChild", "Child", "Parent" },
                actual);
            Assert.AreEqual("GrandChild",
                actual[0]);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents does not include the children.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Child");
            // Assert
            AssertSame(new[] { "Child", "Parent" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents of atable without parents.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Parent");
            // Assert
            CollectionAssert.AreEqual(new[] { "Parent" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "DiamondD");
            // Assert
            AssertSame(new[] { "DiamondD", "DiamondB", "DiamondC", "DiamondA" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents in another schema.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                $"{Database.SalesSchema}.InvoiceLine");
            // Assert
            AssertSame(new[] { $"{Database.SalesSchema}.InvoiceLine", $"{Database.SalesSchema}.Invoice" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                $"{Database.SalesSchema}.ItemRef",
                "Item");
            // Assert
            AssertSame(new[] { $"{Database.SalesSchema}.ItemRef", "Item", $"{Database.SalesSchema}.Item" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents of multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Shipment");
            // Assert
            AssertSame(new[] { "Shipment", "OrderLine", "Country" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents of tables with special names.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "[Odd.Child]");
            // Assert
            AssertSame(new[] { "[Odd.Child]", "[Odd.Name]" },
                actual);
        }

        #endregion

        #region Children

        /// <summary>
        /// Verifies ahtola schema related tables with children.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Parent");
            // Assert
            AssertSame(new[] { "Parent", "Child", "GrandChild" },
                actual);
            Assert.AreEqual("Parent",
                actual[0]);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with children does not include the parents.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Child");
            // Assert
            AssertSame(new[] { "Child", "GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with children of atable without children.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "DiamondA");
            // Assert
            AssertSame(new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with children in another schema.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildrenInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                $"{Database.SalesSchema}.Invoice");
            // Assert
            AssertSame(new[] { $"{Database.SalesSchema}.Invoice", $"{Database.SalesSchema}.InvoiceLine" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with children of atable with many children.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Country");
            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference" },
                actual);
        }

        #endregion

        #region ParentsAndChildren

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Child");
            // Assert
            AssertSame(new[] { "Child", "Parent", "GrandChild" },
                actual);
            Assert.AreEqual("Child",
                actual[0]);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children gives the same tables from any table of the tree.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Parent");
            var fromTheLeaf = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "GrandChild");
            // Assert
            AssertSame(fromTheRoot,
                fromTheLeaf);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "DiamondB");
            // Assert
            AssertSame(new[] { "DiamondB", "DiamondA", "DiamondC", "DiamondD" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children reaches the other branches.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Country");
            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference", "OrderLine" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children does not include the unrelated tables.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Parent");
            // Assert
            Assert.IsFalse(actual.Contains("Person"));
            Assert.IsFalse(actual.Contains("Country"));
            Assert.IsFalse(actual.Contains("NoKey"));
        }

        /// <summary>
        /// Verifies ahtola schema related tables with parents and children of atable without relationships.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "NoKey");
            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey" },
                actual);
        }

        #endregion

        #region Cycles and Self References

        /// <summary>
        /// Verifies ahtola schema related tables of acycle.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesOfACycle()
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
                    "CycleA");
                // Assert
                AssertSame(new[] { "CycleA", "CycleB" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies ahtola schema related tables of acycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesOfACycleOfThreeTables()
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
                    "RingX");
                // Assert
                AssertSame(new[] { "RingX", "RingY", "RingZ" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies ahtola schema related tables of acycle with tables around it.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(RelatedTableBehavior.Parents,
                "LoopB");
            var children = GetRelatedTables(RelatedTableBehavior.Children,
                "LoopA");
            var endToEnd = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "LoopA");
            // Assert
            AssertSame(new[] { "LoopB", "LoopA", "LoopRoot" },
                parents);
            AssertSame(new[] { "LoopA", "LoopB", "LoopLeaf" },
                children);
            AssertSame(new[] { "LoopA", "LoopB", "LoopRoot", "LoopLeaf" },
                endToEnd);
        }

        /// <summary>
        /// Verifies ahtola schema related tables of aself reference.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesOfASelfReference()
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
                    "Employee");
                // Assert
                CollectionAssert.AreEqual(new[] { "Employee" },
                    actual);
            }
        }

        #endregion

        #region Multiple Tables

        /// <summary>
        /// Verifies ahtola schema related tables of multiple tables.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "GrandChild",
                "DiamondB");
            // Assert
            AssertSame(new[] { "GrandChild", "DiamondB", "Child", "Parent", "DiamondA" },
                actual);
            CollectionAssert.AreEqual(new[] { "GrandChild", "DiamondB" },
                actual.Take(2).ToArray());
        }

        /// <summary>
        /// Verifies ahtola schema related tables does not repeat atable.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Parent",
                "Child",
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema related tables without tables.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);
            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Verifies ahtola schema related tables are compared with the case.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Parent",
                "[Parent]");
            // Assert
            Assert.AreEqual(3,
                actual.Length);
            Assert.AreEqual(3,
                actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        /// <summary>
        /// Verifies ahtola schema related tables can be given to get dependency order.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = Database.CreateSource())
            {
                var reader = new AhtolaSchemaReader();
                var related = reader.GetRelatedTables(connection,
                    new[] { "GrandChild" },
                    RelatedTableBehavior.Parents,
                    transaction: null);
                var actual = reader.GetDependencyOrder(connection,
                    related,
                    transaction: null).Select(r => r.Schema.Table.Name).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" },
                    actual);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies ahtola schema related tables async with each behavior.
        /// </summary>
        [TestMethod]
        public async Task TestAhtolaSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "Child" },
                await GetRelatedTablesAsync(RelatedTableBehavior.TableOnly,
                    "Child"));
            AssertSame(new[] { "Child", "Parent" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Parents,
                    "Child"));
            AssertSame(new[] { "Child", "GrandChild" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Children,
                    "Child"));
            AssertSame(new[] { "Child", "Parent", "GrandChild" },
                await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                    "Child"));
        }

        /// <summary>
        /// Verifies ahtola schema related tables async of acycle.
        /// </summary>
        [TestMethod]
        public async Task TestAhtolaSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                "CycleA");
            // Assert
            AssertSame(new[] { "CycleA", "CycleB" },
                actual);
        }

        #endregion

        #region Arguments

        /// <summary>
        /// Verifies throw exception on ahtola schema related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = Database.CreateSource())
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetRelatedTables(connection,
                        null,
                        RelatedTableBehavior.ParentsAndChildren,
                        transaction: null));
            }
        }

        /// <summary>
        /// Verifies throw exception on ahtola schema related tables if the behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetRelatedTables((RelatedTableBehavior)99,
                    "Child"));
        }

        #endregion

        #region CopyTableSchemaTo

        /// <summary>
        /// Verifies copy table schema to with parents.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParents()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "GrandChild" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Parents);
                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
                Helper.AssertTargetMatchesSource("Parent");
                Helper.AssertTargetMatchesSource("Child");
                Helper.AssertTargetMatchesSource("GrandChild");
            }
        }

        /// <summary>
        /// Verifies copy table schema to with children.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "Parent" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children);
                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to with parents and children.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                var created = new List<string>();
                // Act
                source.CopyTableSchemaTo(new[] { "Child" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.Target.Table.Table.Name));
                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" },
                    created);
            }
        }

        /// <summary>
        /// Verifies copy table schema to with table only does not copy the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "Child" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.TableOnly);
                // Assert
                AssertSame(new[] { "Child" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to with parents and children of acycle.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "LoopA" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to async with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                await source.CopyTableSchemaToAsync(new[] { "Child" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with parents.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("GrandChild",
                    target,
                    relationshipBehavior: RelatedTableBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "GrandChild",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("GrandChild",
                    result.Target.Table.Table.Name);
                Assert.AreEqual(TableOutcome.Created,
                    result.Outcome);
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                var result = (await source.CopyTableSchemaToAsync("Child",
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "Child",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("Child",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "Parent", "Child", "GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with special names and children.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("[Odd.Name]",
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "Odd.Name",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("Odd.Name",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "[Odd.Name]", "[Odd.Child]" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with table only returns the result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new AhtolaSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("Parent",
                    target,
                    relationshipBehavior: RelatedTableBehavior.TableOnly).Items.Single();
                // Assert
                Assert.AreEqual("Parent",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "Parent" },
                    GetTargetTables().ToArray());
            }
        }
        #endregion
    }
}
