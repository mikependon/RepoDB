#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.MySqlConnector.IntegrationTests
{
    [TestClass]
    public class MySqlConnectorSchemaRelatedTablesTest
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

        private static string[] GetRelatedTables(RelatedTableBehavior behavior, params string[] tableNames)
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                return new MySqlConnectorSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(RelatedTableBehavior behavior, params string[] tableNames)
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                return (await new MySqlConnectorSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                return new MySqlConnectorSchemaReader(connection).GetTables().ToList();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly, "Child");

            // Assert
            CollectionAssert.AreEqual(new[] { "Child" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly, "GrandChild", "Parent", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "GrandChild");

            // Assert
            AssertSame(new[] { "GrandChild", "Child", "Parent" }, actual);
            Assert.AreEqual("GrandChild", actual[0]);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "DiamondD");

            // Assert
            AssertSame(new[] { "DiamondD", "DiamondB", "DiamondC", "DiamondA" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Ledger");

            // Assert
            AssertSame(new[] { "Ledger", $"{Database.SourceSalesName}.Invoice" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "ItemRef");

            // Assert
            AssertSame(new[] { "ItemRef", $"{Database.SourceSalesName}.Item" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Shipment");

            // Assert
            AssertSame(new[] { "Shipment", "OrderLine", "Country" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "`Odd.Child`");

            // Assert
            AssertSame(new[] { "`Odd.Child`", "`Odd.Name`" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Parent");

            // Assert
            AssertSame(new[] { "Parent", "Child", "GrandChild" }, actual);
            Assert.AreEqual("Parent", actual[0]);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Child");

            // Assert
            AssertSame(new[] { "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "DiamondA");

            // Assert
            AssertSame(new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, $"{Database.SourceSalesName}.Invoice");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", "Ledger" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, actual);
            Assert.AreEqual("Child", actual[0]);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent");
            var fromTheLeaf = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "GrandChild");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "DiamondB");

            // Assert
            AssertSame(new[] { "DiamondB", "DiamondA", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference", "OrderLine" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent");

            // Assert
            Assert.IsFalse(actual.Contains("Person"));
            Assert.IsFalse(actual.Contains("Country"));
            Assert.IsFalse(actual.Contains("NoKey"));
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "NoKey");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { RelatedTableBehavior.Parents, RelatedTableBehavior.Children, RelatedTableBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "CycleA");

                // Assert
                AssertSame(new[] { "CycleA", "CycleB" }, actual);
            }
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[] { RelatedTableBehavior.Parents, RelatedTableBehavior.Children, RelatedTableBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "RingX");

                // Assert
                AssertSame(new[] { "RingX", "RingY", "RingZ" }, actual);
            }
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(RelatedTableBehavior.Parents, "LoopB");
            var children = GetRelatedTables(RelatedTableBehavior.Children, "LoopA");
            var endToEnd = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "LoopA");

            // Assert
            AssertSame(new[] { "LoopB", "LoopA", "LoopRoot" }, parents);
            AssertSame(new[] { "LoopA", "LoopB", "LoopLeaf" }, children);
            AssertSame(new[] { "LoopA", "LoopB", "LoopRoot", "LoopLeaf" }, endToEnd);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { RelatedTableBehavior.Parents, RelatedTableBehavior.Children, RelatedTableBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "Employee");

                // Assert
                CollectionAssert.AreEqual(new[] { "Employee" }, actual);
            }
        }

        #endregion

        #region Multiple Tables

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "GrandChild", "DiamondB");

            // Assert
            AssertSame(new[] { "GrandChild", "DiamondB", "Child", "Parent", "DiamondA" }, actual);
            CollectionAssert.AreEqual(new[] { "GrandChild", "DiamondB" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent", "Child", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Parent", "`Parent`");

            // Assert
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(3, actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new MySqlConnectorSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "GrandChild" }, RelatedTableBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestMySqlConnectorSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "Child" }, await GetRelatedTablesAsync(RelatedTableBehavior.TableOnly, "Child"));
            AssertSame(new[] { "Child", "Parent" }, await GetRelatedTablesAsync(RelatedTableBehavior.Parents, "Child"));
            AssertSame(new[] { "Child", "GrandChild" }, await GetRelatedTablesAsync(RelatedTableBehavior.Children, "Child"));
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren, "Child"));
        }

        [TestMethod]
        public async Task TestMySqlConnectorSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren, "CycleA");

            // Assert
            AssertSame(new[] { "CycleA", "CycleB" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new MySqlConnectorSchemaReader(connection).GetRelatedTables(null, RelatedTableBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetRelatedTables((RelatedTableBehavior)99, "Child"));
        }

        #endregion

        #region CopyTableSchemaTo

        [TestMethod]
        public void TestCopyTableSchemaToWithParents()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "GrandChild" }, target, relationshipBehavior: RelatedTableBehavior.Parents);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
                Helper.AssertTargetMatchesSource("Parent");
                Helper.AssertTargetMatchesSource("Child");
                Helper.AssertTargetMatchesSource("GrandChild");
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithChildren()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "Parent" }, target, relationshipBehavior: RelatedTableBehavior.Children);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopyTableSchemaTo(new[] { "Child" }, target,
                    relationshipBehavior: RelatedTableBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.Target.Table.Table.Name));

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, created);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                Assert.ThrowsExactly<MySqlException>(() =>
                    source.CopyTableSchemaTo(new[] { "Child" }, target, relationshipBehavior: RelatedTableBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "LoopA" }, target, relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Child" }, target, relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }


        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("GrandChild", target, relationshipBehavior: RelatedTableBehavior.Parents).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "GrandChild", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("GrandChild", result.Target.Table.Table.Name);
                Assert.AreEqual(TableOutcome.Created, result.Outcome);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Child", target, relationshipBehavior: RelatedTableBehavior.ParentsAndChildren)).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Child", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("Child", result.Target.Table.Table.Name);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("`Odd.Name`", target, relationshipBehavior: RelatedTableBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Odd.Name", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("Odd.Name", result.Target.Table.Table.Name);
                AssertSame(new[] { "`Odd.Name`", "`Odd.Child`" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<MySqlConnection>(new MySqlConnectorSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("Parent", target, relationshipBehavior: RelatedTableBehavior.TableOnly).Items.Single();

                // Assert
                Assert.AreEqual("Parent", result.Target.Table.Table.Name);
                AssertSame(new[] { "Parent" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
