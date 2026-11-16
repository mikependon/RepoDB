#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    /// <summary>
    /// Contains the sql server schema related tables test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaRelatedTablesTest
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader().GetRelatedTables(connection,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return (await new SqlServerSchemaReader().GetRelatedTablesAsync(connection,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader().GetTables(connection,
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
        /// Verifies sql server schema related tables with table only.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "Child");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Child" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with table only keeps the given tables in order.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly,
                "GrandChild",
                "dbo.Parent",
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild", "dbo.Parent" },
                actual);
        }

        #endregion

        #region Parents

        /// <summary>
        /// Verifies sql server schema related tables with parents.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "GrandChild");
            // Assert
            AssertSame(new[] { "dbo.GrandChild", "dbo.Child", "dbo.Parent" },
                actual);
            Assert.AreEqual("dbo.GrandChild",
                actual[0]);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents does not include the children.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Child");
            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.Parent" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents of atable without parents.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Parent");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Parent" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents of adiamond.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "DiamondD");
            // Assert
            AssertSame(new[] { "dbo.DiamondD", "dbo.DiamondB", "dbo.DiamondC", "dbo.DiamondA" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents across schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Ledger");
            // Assert
            AssertSame(new[] { "dbo.Ledger", "Sales.Invoice" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "ItemRef");
            // Assert
            AssertSame(new[] { "dbo.ItemRef", "Sales.Item" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents of multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "Shipment");
            // Assert
            AssertSame(new[] { "dbo.Shipment", "dbo.OrderLine", "dbo.Country" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents of tables with special names.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "dbo.[Odd.Child]");
            // Assert
            AssertSame(new[] { "dbo.[Odd.Child]", "dbo.[Odd.Name]" },
                actual);
        }

        #endregion

        #region Children

        /// <summary>
        /// Verifies sql server schema related tables with children.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Parent");
            // Assert
            AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                actual);
            Assert.AreEqual("dbo.Parent",
                actual[0]);
        }

        /// <summary>
        /// Verifies sql server schema related tables with children does not include the parents.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Child");
            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with children of atable without children.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "DiamondA");
            // Assert
            AssertSame(new[] { "dbo.DiamondA", "dbo.DiamondB", "dbo.DiamondC", "dbo.DiamondD" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with children across schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Sales.Invoice");
            // Assert
            AssertSame(new[] { "Sales.Invoice", "Sales.InvoiceLine", "dbo.Ledger" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with children of atable with many children.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "Country");
            // Assert
            AssertSame(new[] { "dbo.Country", "dbo.Person", "dbo.Shipment", "dbo.Preference" },
                actual);
        }

        #endregion

        #region ParentsAndChildren

        /// <summary>
        /// Verifies sql server schema related tables with parents and children.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Child");
            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.Parent", "dbo.GrandChild" },
                actual);
            Assert.AreEqual("dbo.Child",
                actual[0]);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents and children gives the same tables from any table of the tree.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
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
        /// Verifies sql server schema related tables with parents and children of adiamond.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "DiamondB");
            // Assert
            AssertSame(new[] { "dbo.DiamondB", "dbo.DiamondA", "dbo.DiamondC", "dbo.DiamondD" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents and children reaches the other branches.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Country");
            // Assert
            AssertSame(new[] { "dbo.Country", "dbo.Person", "dbo.Shipment", "dbo.Preference", "dbo.OrderLine" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents and children does not include the unrelated tables.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Parent");
            // Assert
            Assert.IsFalse(actual.Contains("dbo.Person"));
            Assert.IsFalse(actual.Contains("dbo.Country"));
            Assert.IsFalse(actual.Contains("dbo.NoKey"));
        }

        /// <summary>
        /// Verifies sql server schema related tables with parents and children of atable without relationships.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "NoKey");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.NoKey" },
                actual);
        }

        #endregion

        #region Cycles and Self References

        /// <summary>
        /// Verifies sql server schema related tables of acycle.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycle()
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
                AssertSame(new[] { "dbo.CycleA", "dbo.CycleB" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies sql server schema related tables of acycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycleOfThreeTables()
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
                AssertSame(new[] { "dbo.RingX", "dbo.RingY", "dbo.RingZ" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies sql server schema related tables of acycle with tables around it.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(RelatedTableBehavior.Parents,
                "LoopB");
            var children = GetRelatedTables(RelatedTableBehavior.Children,
                "LoopA");
            var endToEnd = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "LoopA");
            // Assert
            AssertSame(new[] { "dbo.LoopB", "dbo.LoopA", "dbo.LoopRoot" },
                parents);
            AssertSame(new[] { "dbo.LoopA", "dbo.LoopB", "dbo.LoopLeaf" },
                children);
            AssertSame(new[] { "dbo.LoopA", "dbo.LoopB", "dbo.LoopRoot", "dbo.LoopLeaf" },
                endToEnd);
        }

        /// <summary>
        /// Verifies sql server schema related tables of aself reference.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfASelfReference()
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
                CollectionAssert.AreEqual(new[] { "dbo.Employee" },
                    actual);
            }
        }

        #endregion

        #region Multiple Tables

        /// <summary>
        /// Verifies sql server schema related tables of multiple tables.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents,
                "GrandChild",
                "DiamondB");
            // Assert
            AssertSame(new[] { "dbo.GrandChild", "dbo.DiamondB", "dbo.Child", "dbo.Parent", "dbo.DiamondA" },
                actual);
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild", "dbo.DiamondB" },
                actual.Take(2).ToArray());
        }

        /// <summary>
        /// Verifies sql server schema related tables does not repeat atable.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren,
                "Parent",
                "Child",
                "GrandChild");
            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                actual);
        }

        /// <summary>
        /// Verifies sql server schema related tables without tables.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);
            // Assert
            Assert.AreEqual(0,
                actual.Length);
        }

        /// <summary>
        /// Verifies sql server schema related tables are compared without the case.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesAreComparedWithoutTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children,
                "PARENT",
                "dbo.child");
            // Assert
            Assert.AreEqual(3,
                actual.Length);
            Assert.AreEqual(3,
                actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        /// <summary>
        /// Verifies sql server schema related tables can be given to get dependency order.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new SqlServerSchemaReader();
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
        /// Verifies sql server schema related tables async with each behavior.
        /// </summary>
        [TestMethod]
        public async Task TestSqlServerSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "dbo.Child" },
                await GetRelatedTablesAsync(RelatedTableBehavior.TableOnly,
                    "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.Parent" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Parents,
                    "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.GrandChild" },
                await GetRelatedTablesAsync(RelatedTableBehavior.Children,
                    "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.Parent", "dbo.GrandChild" },
                await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                    "Child"));
        }

        /// <summary>
        /// Verifies sql server schema related tables async of acycle.
        /// </summary>
        [TestMethod]
        public async Task TestSqlServerSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren,
                "CycleA");
            // Assert
            AssertSame(new[] { "dbo.CycleA", "dbo.CycleB" },
                actual);
        }

        #endregion

        #region Arguments

        /// <summary>
        /// Verifies throw exception on sql server schema related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => new SqlServerSchemaReader().GetRelatedTables(connection,
                        null,
                        RelatedTableBehavior.ParentsAndChildren,
                        transaction: null));
            }
        }

        /// <summary>
        /// Verifies throw exception on sql server schema related tables if the behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaRelatedTablesIfTheBehaviorIsNotDefined()
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "GrandChild" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Parents);
                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "Parent" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children);
                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to with parents and children.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                var created = new List<string>();
                // Act
                source.CopyTableSchemaTo(new[] { "Child" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.Target.Table.Table.Name));
                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                Assert.ThrowsExactly<SqlException>(() => source.CopyTableSchemaTo(new[] { "Child" },
                        target,
                        relationshipBehavior: RelatedTableBehavior.TableOnly));
            }
        }

        /// <summary>
        /// Verifies copy table schema to with parents and children of acycle.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                source.CopyTableSchemaTo(new[] { "LoopA" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "dbo.LoopRoot", "dbo.LoopA", "dbo.LoopB", "dbo.LoopLeaf" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to async with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                await source.CopyTableSchemaToAsync(new[] { "Child" },
                    target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);
                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with parents.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
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
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to async of asingle table with parents and children.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
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
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with special names and children.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("dbo.[Odd.Name]",
                    target,
                    relationshipBehavior: RelatedTableBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name,
                        "Odd.Name",
                        StringComparison.OrdinalIgnoreCase));
                // Assert
                Assert.AreEqual("Odd.Name",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "dbo.[Odd.Name]", "dbo.[Odd.Child]" },
                    GetTargetTables().ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with table only returns the result.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
                    true);
                // Act
                var result = source.CopyTableSchemaTo("Parent",
                    target,
                    relationshipBehavior: RelatedTableBehavior.TableOnly).Items.Single();
                // Assert
                Assert.AreEqual("Parent",
                    result.Target.Table.Table.Name);
                AssertSame(new[] { "dbo.Parent" },
                    GetTargetTables().ToArray());
            }
        }
        #endregion
    }
}
