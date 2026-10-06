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
    [TestClass]
    public class SqlServerSchemaRelatedTablesTest
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetRelatedTables(tableNames, behavior).ToArray();
            }
        }

        private static async Task<string[]> GetRelatedTablesAsync(CopySchemaRelationshipBehavior behavior, params string[] tableNames)
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return (await new SqlServerSchemaReader(connection).GetRelatedTablesAsync(tableNames, behavior)).ToArray();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        private static void AssertSame(string[] expected, string[] actual) =>
            CollectionAssert.AreEquivalent(expected, actual, string.Join(", ", actual));

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithTableOnly()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "Child");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Child" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithTableOnlyKeepsTheGivenTablesInOrder()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.TableOnly, "GrandChild", "dbo.Parent", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild", "dbo.Parent" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "GrandChild");

            // Assert
            AssertSame(new[] { "dbo.GrandChild", "dbo.Child", "dbo.Parent" }, actual);
            Assert.AreEqual("dbo.GrandChild", actual[0]);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Child");

            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.Parent" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfATableWithoutParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Parent" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "DiamondD");

            // Assert
            AssertSame(new[] { "dbo.DiamondD", "dbo.DiamondB", "dbo.DiamondC", "dbo.DiamondA" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Ledger");

            // Assert
            AssertSame(new[] { "dbo.Ledger", "Sales.Invoice" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "ItemRef");

            // Assert
            AssertSame(new[] { "dbo.ItemRef", "Sales.Item" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfMultipleForeignKeys()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "Shipment");

            // Assert
            AssertSame(new[] { "dbo.Shipment", "dbo.OrderLine", "dbo.Country" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsOfTablesWithSpecialNames()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "dbo.[Odd.Child]");

            // Assert
            AssertSame(new[] { "dbo.[Odd.Child]", "dbo.[Odd.Name]" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Parent");

            // Assert
            AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, actual);
            Assert.AreEqual("dbo.Parent", actual[0]);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Child");

            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.GrandChild" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "DiamondA");

            // Assert
            AssertSame(new[] { "dbo.DiamondA", "dbo.DiamondB", "dbo.DiamondC", "dbo.DiamondD" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenAcrossSchemas()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Sales.Invoice");

            // Assert
            AssertSame(new[] { "Sales.Invoice", "Sales.InvoiceLine", "dbo.Ledger" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithChildrenOfATableWithManyChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "Country");

            // Assert
            AssertSame(new[] { "dbo.Country", "dbo.Person", "dbo.Shipment", "dbo.Preference" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildren()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Child");

            // Assert
            AssertSame(new[] { "dbo.Child", "dbo.Parent", "dbo.GrandChild" }, actual);
            Assert.AreEqual("dbo.Child", actual[0]);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenGivesTheSameTablesFromAnyTableOfTheTree()
        {
            // Act
            var fromTheRoot = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent");
            var fromTheLeaf = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "GrandChild");

            // Assert
            AssertSame(fromTheRoot, fromTheLeaf);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenOfADiamond()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "DiamondB");

            // Assert
            AssertSame(new[] { "dbo.DiamondB", "dbo.DiamondA", "dbo.DiamondC", "dbo.DiamondD" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenReachesTheOtherBranches()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Country");

            // Assert
            AssertSame(new[] { "dbo.Country", "dbo.Person", "dbo.Shipment", "dbo.Preference", "dbo.OrderLine" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent");

            // Assert
            Assert.IsFalse(actual.Contains("dbo.Person"));
            Assert.IsFalse(actual.Contains("dbo.Country"));
            Assert.IsFalse(actual.Contains("dbo.NoKey"));
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "NoKey");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.NoKey" }, actual);
        }

        #endregion

        #region Cycles and Self References

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycle()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "CycleA");

                // Assert
                AssertSame(new[] { "dbo.CycleA", "dbo.CycleB" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycleOfThreeTables()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "RingX");

                // Assert
                AssertSame(new[] { "dbo.RingX", "dbo.RingY", "dbo.RingZ" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfACycleWithTablesAroundIt()
        {
            // Act
            var parents = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "LoopB");
            var children = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "LoopA");
            var endToEnd = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "LoopA");

            // Assert
            AssertSame(new[] { "dbo.LoopB", "dbo.LoopA", "dbo.LoopRoot" }, parents);
            AssertSame(new[] { "dbo.LoopA", "dbo.LoopB", "dbo.LoopLeaf" }, children);
            AssertSame(new[] { "dbo.LoopA", "dbo.LoopB", "dbo.LoopRoot", "dbo.LoopLeaf" }, endToEnd);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfASelfReference()
        {
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                // Act
                var actual = GetRelatedTables(behavior, "Employee");

                // Assert
                CollectionAssert.AreEqual(new[] { "dbo.Employee" }, actual);
            }
        }

        #endregion

        #region Multiple Tables

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesOfMultipleTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Parents, "GrandChild", "DiamondB");

            // Assert
            AssertSame(new[] { "dbo.GrandChild", "dbo.DiamondB", "dbo.Child", "dbo.Parent", "dbo.DiamondA" }, actual);
            CollectionAssert.AreEqual(new[] { "dbo.GrandChild", "dbo.DiamondB" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesDoesNotRepeatATable()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren, "Parent", "Child", "GrandChild");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesWithoutTables()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesAreComparedWithoutTheCase()
        {
            // Act
            var actual = GetRelatedTables(CopySchemaRelationshipBehavior.Children, "PARENT", "dbo.child");

            // Assert
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(3, actual.Select(x => x.ToLowerInvariant()).Distinct().Count());
        }

        [TestMethod]
        public void TestSqlServerSchemaRelatedTablesCanBeGivenToGetDependencyOrder()
        {
            // Act
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new SqlServerSchemaReader(connection);
                var related = reader.GetRelatedTables(new[] { "GrandChild" }, CopySchemaRelationshipBehavior.Parents);
                var actual = reader.GetDependencyOrder(related).Select(r => r.Schema.Table.Name).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, actual);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerSchemaRelatedTablesAsyncWithEachBehavior()
        {
            // Act/Assert
            CollectionAssert.AreEqual(new[] { "dbo.Child" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.TableOnly, "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.Parent" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Parents, "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.GrandChild" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.Children, "Child"));
            AssertSame(new[] { "dbo.Child", "dbo.Parent", "dbo.GrandChild" }, await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "Child"));
        }

        [TestMethod]
        public async Task TestSqlServerSchemaRelatedTablesAsyncOfACycle()
        {
            // Act
            var actual = await GetRelatedTablesAsync(CopySchemaRelationshipBehavior.ParentsAndChildren, "CycleA");

            // Assert
            AssertSame(new[] { "dbo.CycleA", "dbo.CycleB" }, actual);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaRelatedTablesIfTheTableNamesAreNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() =>
                    new SqlServerSchemaReader(connection).GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaRelatedTablesIfTheBehaviorIsNotDefined()
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "GrandChild" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
                Helper.AssertTargetMatchesSource("Parent");
                Helper.AssertTargetMatchesSource("Child");
                Helper.AssertTargetMatchesSource("GrandChild");
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "Parent" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);
                var created = new List<string>();

                // Act
                source.CopySchemaTo(new[] { "Child" }, target,
                    relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren,
                    createdCallback: r => created.Add(r.TableName));

                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
                CollectionAssert.AreEqual(new[] { "Parent", "Child", "GrandChild" }, created);
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithTableOnlyDoesNotCopyTheRelatedTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                Assert.ThrowsExactly<SqlException>(() =>
                    source.CopySchemaTo(new[] { "Child" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly));
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithParentsAndChildrenOfACycle()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                source.CopySchemaTo(new[] { "LoopA" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "dbo.LoopRoot", "dbo.LoopA", "dbo.LoopB", "dbo.LoopLeaf" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                await source.CopySchemaToAsync(new[] { "Child" }, target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
            }
        }


        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithParents()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("GrandChild", target, relationshipBehavior: CopySchemaRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual("GrandChild", result.TableName);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfASingleTableWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                var result = await source.CopySchemaToAsync("Child", target, relationshipBehavior: CopySchemaRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual("Child", result.TableName);
                AssertSame(new[] { "dbo.Parent", "dbo.Child", "dbo.GrandChild" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithSpecialNamesAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("dbo.[Odd.Name]", target, relationshipBehavior: CopySchemaRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual("Odd.Name", result.TableName);
                AssertSame(new[] { "dbo.[Odd.Name]", "dbo.[Odd.Child]" }, GetTargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithTableOnlyReturnsTheResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

                // Act
                var result = source.CopySchemaTo("Parent", target, relationshipBehavior: CopySchemaRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual("Parent", result.TableName);
                AssertSame(new[] { "dbo.Parent" }, GetTargetTables().ToArray());
            }
        }

        #endregion
    }
}
