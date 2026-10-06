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

        private static string[] GetRelatedTables(CopySchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(CopySchemaRelationshipBehavior behavior, params string[] tableNames)
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
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "Child");

            // Assert
            CollectionAssert.AreEqual(new[] { "Child" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "GrandChild", "Parent", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "GrandChild");

            // Assert
            AssertSame(new[] { "GrandChild", "Child", "Parent" }, actual);
            Assert.AreEqual("GrandChild", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "DiamondD");

            // Assert
            AssertSame(new[] { "DiamondD", "DiamondB", "DiamondC", "DiamondA" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, $"{Database.SourceSalesName}.InvoiceLine");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.InvoiceLine", $"{Database.SourceSalesName}.Invoice" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, $"{Database.SourceSalesName}.ItemRef", "Item");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.ItemRef", "Item", $"{Database.SourceSalesName}.Item" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Shipment");

            // Assert
            AssertSame(new[] { "Shipment", "OrderLine", "Country" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "\"Odd.Child\"");

            // Assert
            AssertSame(new[] { "\"Odd.Child\"", "\"Odd.Name\"" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Parent");

            // Assert
            AssertSame(new[] { "Parent", "Child", "GrandChild" }, actual);
            Assert.AreEqual("Parent", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Child");

            // Assert
            AssertSame(new[] { "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "DiamondA");

            // Assert
            AssertSame(new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenInAnotherSchema()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, $"{Database.SourceSalesName}.Invoice");

            // Assert
            AssertSame(new[] { $"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, actual);
            Assert.AreEqual("Child", actual[0]);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent");
            var fromTheLeaf = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "GrandChild");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "DiamondB");

            // Assert
            AssertSame(new[] { "DiamondB", "DiamondA", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference", "OrderLine" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent");

            // Assert
            Assert.IsFalse(actual.Contains("Person"));
            Assert.IsFalse(actual.Contains("Country"));
            Assert.IsFalse(actual.Contains("NoKey"));
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "NoKey");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
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
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
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
            var parents = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "LoopB");
            var children = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "LoopA");
            var endToEnd = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "LoopA");

            // Assert
            AssertSame(new[] { "LoopB", "LoopA", "LoopRoot" }, parents);
            AssertSame(new[] { "LoopA", "LoopB", "LoopLeaf" }, children);
            AssertSame(new[] { "LoopA", "LoopB", "LoopRoot", "LoopLeaf" }, endToEnd);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
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
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "GrandChild", "DiamondB");

            // Assert
            AssertSame(new[] { "GrandChild", "DiamondB", "Child", "Parent", "DiamondA" }, actual);
            CollectionAssert.AreEqual(new[] { "GrandChild", "DiamondB" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent", "Child", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestVerticaSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Parent", "\"Parent\"");

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
                var related = reader.GetRelatedTables(new[] { "GrandChild" }, CopySchemaRelationshipBehavior.Parents);
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
            CollectionAssert.AreEqual(new[] { "Child" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.TableOnly, "Child"));
            AssertSame(new[] { "Child", "Parent" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Parents, "Child"));
            AssertSame(new[] { "Child", "GrandChild" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Children, "Child"));
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "Child"));
        }

        [TestMethod]
        public async Task TestVerticaSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "CycleA");

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
                    new VerticaSchemaReader(connection).GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetRelatedTables((CopySchemaRelationshipBehavior)99, "Child"));
        }

        #endregion

        #region CopySchemaTo

        [TestMethod]
        public void TestCopySchemaToWithParents()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "GrandChild" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
                Helper.AssertTargetMatchesSource("Parent");
                Helper.AssertTargetMatchesSource("Child");
                Helper.AssertTargetMatchesSource("GrandChild");
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "Parent" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithParentsAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopySchemaTo(new[] { "Child" }, target,
                    relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.TableName));

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, created);
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                Assert.ThrowsExactly<VerticaException>(() =>
                    source.CopySchemaTo(new[] { "Child" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "LoopA" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                await source.CopySchemaToAsync(new[] { "Child" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }


        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithParents()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("GrandChild", target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual("GrandChild", result.TableName);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                var result = await source.CopySchemaToAsync("Child", target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual("Child", result.TableName);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("\"Odd.Name\"", target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual("Odd.Name", result.TableName);
                AssertSame(new[] { "\"Odd.Name\"", "\"Odd.Child\"" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new VerticaConnection(Database.ConnectionStringForSource))
            using (var target = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<VerticaConnection>(new VerticaSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("Parent", target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual("Parent", result.TableName);
                AssertSame(new[] { "Parent" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
