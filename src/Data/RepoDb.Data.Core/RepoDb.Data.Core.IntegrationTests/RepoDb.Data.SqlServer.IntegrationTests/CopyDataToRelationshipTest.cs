#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.SqlServer.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.SqlServer.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table together with the rows of the tables that are related to it (the Country, Person, Pet and Toy tables
    /// form a tree: each one references the previous one), as defined by the <see cref="CopyRelationshipBehavior"/>.
    /// </summary>
    [TestClass]
    public class CopyDataToRelationshipTest
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

        private static readonly string[] Tree = { "[dbo].[Country]", "[dbo].[Person]", "[dbo].[Pet]", "[dbo].[Toy]" };

        private static void AssertTable(string quotedName,
            string orderBy = "[Id]") =>
            Helper.AssertRowsEqual(Helper.SourceRows(quotedName, orderBy), Helper.TargetRows(quotedName, orderBy));

        private static void AssertTree(int expectedTables = 4)
        {
            CollectionAssert.AreEqual(Tree.Take(expectedTables).ToArray(), Helper.TargetTables().ToArray());
            foreach (var table in Tree.Take(expectedTables))
            {
                AssertTable(table);
            }
            if (expectedTables >= 2)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Person]", "[CountryId]", "[dbo].[Country]", "[Id]"));
            }
            if (expectedTables >= 3)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Pet]", "[PersonId]", "[dbo].[Person]", "[Id]"));
            }
            if (expectedTables >= 4)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Toy]", "[PetId]", "[dbo].[Pet]", "[Id]"));
            }
        }

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestCopyDataToWithTableOnlyCopiesTheRowsOfTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person", "Pet", "Toy");
                source.CopyDataTo("Country", target);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.TableOnly).RowCount;

                // Assert
                Assert.AreEqual(6, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Country]"));
                Assert.AreEqual(6, Helper.TargetCount("[dbo].[Person]"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Pet]"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Toy]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithTableOnlyDoesNotCreateTheRelatedTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.TableOnly);

                // Assert
                CollectionAssert.AreEqual(new[] { "[dbo].[Country]" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheTableAndItsParents()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                AssertTree(2);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheParentsOfTheParents()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Toy", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableWithoutParentsCopiesTheTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertTree(1);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsDoesNotCopyTheChildrenOrTheTablesThatAreNotRelated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Pet]");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Toy]");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Currency]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfASelfReferencingTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("[dbo].[Employee]");
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Employee]", "[ManagerId]", "[dbo].[Employee]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableInACycle()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("CycleA", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                AssertTable("[dbo].[CycleA]");
                AssertTable("[dbo].[CycleB]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableThatReferencesATableOfAnotherSchemaCopiesTheParentIntoTheSchemaOfTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Ledger", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(2 + 2, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Invoice]", "[dbo].[Ledger]" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("[Sales].[Invoice]", "[Id]"), Helper.TargetRows("[dbo].[Invoice]", "[Id]"));
                AssertTable("[dbo].[Ledger]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableOfAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Sales.InvoiceLine", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(2 + 3, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Invoice]", "[Sales].[InvoiceLine]" }, Helper.TargetTables().ToArray());
                AssertTable("[Sales].[Invoice]");
                AssertTable("[Sales].[InvoiceLine]");
            }
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheTableAndItsChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheRowsOfTheChildrenThatReferenceTablesThatAlreadyExistInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenDoesNotCopyTheParentsOrTheTablesThatAreNotRelated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Currency]");
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Country]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenOfASelfReferencingTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("[dbo].[Employee]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenOfATableOfAnotherSchemaCopiesTheChildrenIntoThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Sales.Invoice", target, relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Invoice]", "[Sales].[InvoiceLine]", "[Sales].[Ledger]" }, Helper.TargetTables().ToArray());
                AssertTable("[Sales].[Invoice]");
                AssertTable("[Sales].[InvoiceLine]");
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Ledger]", "[Id]"), Helper.TargetRows("[Sales].[Ledger]", "[Id]"));
            }
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        [DataRow("Country")]
        [DataRow("Person")]
        [DataRow("Pet")]
        [DataRow("Toy")]
        public void TestCopyDataToWithParentsAndChildrenCopiesTheWholeTreeOfTheTable(string tableName)
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo(tableName, target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenDoesNotCopyTheTablesThatAreNotRelated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Currency]");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Employee]");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "[dbo].[Number]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfASelfReferencingTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("[dbo].[Employee]");
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Employee]", "[ManagerId]", "[dbo].[Employee]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfTablesInACycle()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("CycleB", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren).RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                AssertTable("[dbo].[CycleA]");
                AssertTable("[dbo].[CycleB]");
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[CycleA]", "[BId]", "[dbo].[CycleB]", "[Id]"));
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[CycleB]", "[AId]", "[dbo].[CycleA]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfATableOfAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Sales.InvoiceLine", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren).RowCount;

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Invoice]", "[Sales].[InvoiceLine]", "[Sales].[Ledger]" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #region Filter

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 2, actual);
                AssertTable("[dbo].[Country]");
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Person]", "[Id]", "[Age] > 40"), Helper.TargetRows("[dbo].[Person]", "[Id]"));
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Person]", "[CountryId]", "[dbo].[Country]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Name", "Japan")), relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(1 + 6 + 5 + 5, actual);
                Assert.AreEqual(1, Helper.TargetCount("[dbo].[Country]"));
                AssertTable("[dbo].[Person]");
                AssertTable("[dbo].[Pet]");
                AssertTable("[dbo].[Toy]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren).RowCount;

                // Assert
                Assert.AreEqual(3 + 2 + 5 + 5, actual);
                AssertTable("[dbo].[Country]");
                Assert.AreEqual(2, Helper.TargetCount("[dbo].[Person]"));
                AssertTable("[dbo].[Pet]");
                AssertTable("[dbo].[Toy]");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterThatMatchesNoRowsCopiesTheParentsOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 1000)), relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Country]"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionAndParentsFiltersTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo<Entities.Person>(target, p => p.CountryId == 2, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Person]", "[Id]", "[CountryId] = 2"), Helper.TargetRows("[dbo].[Person]", "[Id]"));
            }
        }

        #endregion

        #region Existing Tables

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsIfTheRowsOfTheParentsAlreadyExistInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Country]"));
                Assert.AreEqual(6, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAddsTheRowsToTheParentsThatAlreadyExistInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");
                Helper.ExecuteOnTarget("SET IDENTITY_INSERT [dbo].[Country] ON; INSERT INTO [dbo].[Country] ([Id], [Name]) VALUES (100, N'Atlantis'); SET IDENTITY_INSERT [dbo].[Country] OFF;");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(4, Helper.TargetCount("[dbo].[Country]"));
                Assert.AreEqual(1, Helper.TargetCount("[dbo].[Country]", "[Name] = N'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                foreach (var behavior in new[] { CopyRelationshipBehavior.Parents, CopyRelationshipBehavior.Children, CopyRelationshipBehavior.ParentsAndChildren })
                {
                    Assert.Throws<ArgumentException>(() => source.CopyDataTo("Pet", "PetCopy", target, relationshipBehavior: behavior));
                }
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheRelationshipBehaviorIsNotTableOnlyAndThereIsNoSchemaReaderForTheSourceConnection()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParents()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Toy", target, relationshipBehavior: CopyRelationshipBehavior.Parents)).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Country", target, relationshipBehavior: CopyRelationshipBehavior.Children)).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndChildren()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren)).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndAFilter()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents)).RowCount;

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Assert.AreEqual(2, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithChildrenOfATableOfAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Sales.Invoice", target, relationshipBehavior: CopyRelationshipBehavior.Children)).RowCount;

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Invoice]", "[Sales].[InvoiceLine]", "[Sales].[Ledger]" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<SqlException>(() => source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => source.CopyDataToAsync("Pet", "PetCopy", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
            }
        }

        #endregion
    }
}
