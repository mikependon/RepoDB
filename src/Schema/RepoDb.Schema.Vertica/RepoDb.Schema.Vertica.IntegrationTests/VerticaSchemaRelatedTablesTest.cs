#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Vertica.IntegrationTests.Setup;

namespace RepoDb.Schema.Vertica.IntegrationTests
{
    [TestClass]
    public class VerticaSchemaRelatedTablesTest
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
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(RelatedTableBehavior behavior, params string[] tableNames)
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return (await new VerticaSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                return new VerticaSchemaReader(connection).GetTables().ToList();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly, "Child");

            // Assert
            CollectionAssert.AreEqual(new[] { "Child" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.TableOnly, "GrandChild", "Parent", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "GrandChild");

            // Assert
            AssertSame(new[] { "GrandChild", "Child", "Parent" }, actual);
            Assert.AreEqual("GrandChild", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "DiamondD");

            // Assert
            AssertSame(new[] { "DiamondD", "DiamondB", "DiamondC", "DiamondA" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, $"{Database.SourceSalesName}.InvoiceLine");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Invoice" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, $"{Database.SourceSalesName}.ItemRef", "Item");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.ItemRef", "Item", $"{Database.SourceSalesName}.Item" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "Shipment");

            // Assert
            AssertSame(new[] { "Shipment", "OrderLine", "Country" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "\"Odd.Child\"");

            // Assert
            AssertSame(new[] { "\"Odd.Child\"", "\"Odd.Name\"" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Parent");

            // Assert
            AssertSame(new[] { "Parent", "Child", "GrandChild" }, actual);
            Assert.AreEqual("Parent", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Child");

            // Assert
            AssertSame(new[] { "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "DiamondA");

            // Assert
            AssertSame(new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, $"{Database.SourceSalesName}.Invoice");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, actual);
            Assert.AreEqual("Child", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent");
            var fromTheLeaf = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "GrandChild");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "DiamondB");

            // Assert
            AssertSame(new[] { "DiamondB", "DiamondA", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference", "OrderLine" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent");

            // Assert
            Assert.IsFalse(actual.Contains("Person"));
            Assert.IsFalse(actual.Contains("Country"));
            Assert.IsFalse(actual.Contains("NoKey"));
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "NoKey");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesOfACycle()
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
        public void TestVerticaSchemaRelatedTablesOfACycleOfThreeTables()
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
        public void TestVerticaSchemaRelatedTablesOfACycleWithTablesAroundIt()
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
        public void TestVerticaSchemaRelatedTablesOfASelfReference()
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
        public void TestVerticaSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Parents, "GrandChild", "DiamondB");

            // Assert
            AssertSame(new[] { "GrandChild", "DiamondB", "Child", "Parent", "DiamondA" }, actual);
            CollectionAssert.AreEqual(new[] { "GrandChild", "DiamondB" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren, "Parent", "Child", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(RelatedTableBehavior.Children, "Parent", "\"Parent\"");

            // Assert
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(3, actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                var reader = new VerticaSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "GrandChild" }, RelatedTableBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "Child" }, await GetRelatedTablesAsync(RelatedTableBehavior.TableOnly, "Child"));
            AssertSame(new[] { "Child", "Parent" }, await GetRelatedTablesAsync(RelatedTableBehavior.Parents, "Child"));
            AssertSame(new[] { "Child", "GrandChild" }, await GetRelatedTablesAsync(RelatedTableBehavior.Children, "Child"));
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren, "Child"));
        }

        [TestMethod]
        public async Task TestVerticaSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(RelatedTableBehavior.ParentsAndChildren, "CycleA");

            // Assert
            AssertSame(new[] { "CycleA", "CycleB" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new VerticaSchemaReader(connection).GetRelatedTables(null, RelatedTableBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaRelatedTablesIfTheBehaviorIsNotDefined()
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
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

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
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "Parent" }, target, relationshipBehavior: RelatedTableBehavior.Children);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);
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
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                Assert.ThrowsExactly<VerticaException>(() =>
                    source.CopyTableSchemaTo(new[] { "Child" }, target, relationshipBehavior: RelatedTableBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "LoopA" }, target, relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Child" }, target, relationshipBehavior: RelatedTableBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }


        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

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
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

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
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("\"Odd.Name\"", target, relationshipBehavior: RelatedTableBehavior.Children).Items.Single(i => string.Equals(i.Source.Table.Table.Name, "Odd.Name", StringComparison.OrdinalIgnoreCase));

                // Assert
                Assert.AreEqual("Odd.Name", result.Target.Table.Table.Name);
                AssertSame(new[] { "\"Odd.Name\"", "\"Odd.Child\"" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

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
