#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests
{
    [TestClass]
    public class TursoSchemaRelatedTablesTest
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
            using (var connection = Database.CreateSource())
            {
                return new TursoSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = Database.CreateSource())
            {
                return (await new TursoSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader(connection).GetTables().ToList();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.TableOnly, "Child");

            // Assert
            CollectionAssert.AreEqual(new[] { "Child" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.TableOnly, "GrandChild", "Parent", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "GrandChild");

            // Assert
            AssertSame(new[] { "GrandChild", "Child", "Parent" }, actual);
            Assert.AreEqual("GrandChild", actual[0]);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "DiamondD");

            // Assert
            AssertSame(new[] { "DiamondD", "DiamondB", "DiamondC", "DiamondA" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "Shipment");

            // Assert
            AssertSame(new[] { "Shipment", "OrderLine", "Country" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "[Odd.Child]");

            // Assert
            AssertSame(new[] { "[Odd.Child]", "[Odd.Name]" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "Parent");

            // Assert
            AssertSame(new[] { "Parent", "Child", "GrandChild" }, actual);
            Assert.AreEqual("Parent", actual[0]);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "Child");

            // Assert
            AssertSame(new[] { "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "DiamondA");

            // Assert
            AssertSame(new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Child");

            // Assert
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, actual);
            Assert.AreEqual("Child", actual[0]);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Parent");
            var fromTheLeaf = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "GrandChild");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "DiamondB");

            // Assert
            AssertSame(new[] { "DiamondB", "DiamondA", "DiamondC", "DiamondD" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Country");

            // Assert
            AssertSame(new[] { "Country", "Person", "Shipment", "Preference", "OrderLine" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Parent");

            // Assert
            Assert.IsFalse(actual.Contains("Person"));
            Assert.IsFalse(actual.Contains("Country"));
            Assert.IsFalse(actual.Contains("NoKey"));
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "NoKey");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestTursoSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "CycleA");

                // Assert
                AssertSame(new[] { "CycleA", "CycleB" }, actual);
            }
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "RingX");

                // Assert
                AssertSame(new[] { "RingX", "RingY", "RingZ" }, actual);
            }
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "LoopB");
            var children = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "LoopA");
            var endToEnd = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "LoopA");

            // Assert
            AssertSame(new[] { "LoopB", "LoopA", "LoopRoot" }, parents);
            AssertSame(new[] { "LoopA", "LoopB", "LoopLeaf" }, children);
            AssertSame(new[] { "LoopA", "LoopB", "LoopRoot", "LoopLeaf" }, endToEnd);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { CopyTableSchemaRelationshipBehavior.Parents, CopyTableSchemaRelationshipBehavior.Children, CopyTableSchemaRelationshipBehavior.ParentsAndChildren })
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
        public void TestTursoSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Parents, "GrandChild", "DiamondB");

            // Assert
            AssertSame(new[] { "GrandChild", "DiamondB", "Child", "Parent", "DiamondA" }, actual);
            CollectionAssert.AreEqual(new[] { "GrandChild", "DiamondB" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Parent", "Child", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesAreComparedWithTheCase()
        {
            // Act
            var actual = GetRelatedTables(CopyTableSchemaRelationshipBehavior.Children, "Parent", "[Parent]");

            // Assert
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(3, actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        [TestMethod]
        public void TestTursoSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = Database.CreateSource())
            {
                var reader = new TursoSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "GrandChild" }, CopyTableSchemaRelationshipBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestTursoSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "Child" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.TableOnly, "Child"));
            AssertSame(new[] { "Child", "Parent" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.Parents, "Child"));
            AssertSame(new[] { "Child", "GrandChild" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.Children, "Child"));
            AssertSame(new[] { "Child", "Parent", "GrandChild" }, await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "Child"));
        }

        [TestMethod]
        public async Task TestTursoSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(CopyTableSchemaRelationshipBehavior.ParentsAndChildren, "CycleA");

            // Assert
            AssertSame(new[] { "CycleA", "CycleB" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnTursoSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = Database.CreateSource())
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new TursoSchemaReader(connection).GetRelatedTables(null, CopyTableSchemaRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTursoSchemaRelatedTablesIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetRelatedTables((CopyTableSchemaRelationshipBehavior)99, "Child"));
        }

        #endregion

        #region CopyTableSchemaTo

        [TestMethod]
        public void TestCopyTableSchemaToWithParents()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "GrandChild" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

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
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "Parent" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopyTableSchemaTo(new[] { "Child" }, target,
                    relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.TableName));

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, created);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "Child" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly);

                // Assert
                AssertSame(new[] { "Child" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                source.CopyTableSchemaTo(new[] { "LoopA" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Child" }, target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithParents()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("GrandChild", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual("GrandChild", result.TableName);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                var result = await source.CopyTableSchemaToAsync("Child", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual("Child", result.TableName);
                AssertSame(new[] { "Parent", "Child", "GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("[Odd.Name]", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual("Odd.Name", result.TableName);
                AssertSame(new[] { "[Odd.Name]", "[Odd.Child]" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

                // Act
                var result = source.CopyTableSchemaTo("Parent", target, relationshipBehavior: CopyTableSchemaRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual("Parent", result.TableName);
                AssertSame(new[] { "Parent" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
