#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.AuroraDb.Npgsql;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.AuroraDbPostgreSql.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.AuroraDbPostgreSql.IntegrationTests
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

        private static readonly string[] Tree = { "\"public\".\"Country\"", "\"public\".\"Person\"", "\"public\".\"Pet\"", "\"public\".\"Toy\"" };

        private static void AssertTable(string quotedName,
            string orderBy = "\"Id\"") =>
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
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Person\"", "\"CountryId\"", "\"public\".\"Country\"", "\"Id\""));
            }
            if (expectedTables >= 3)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Pet\"", "\"PersonId\"", "\"public\".\"Person\"", "\"Id\""));
            }
            if (expectedTables >= 4)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Toy\"", "\"PetId\"", "\"public\".\"Pet\"", "\"Id\""));
            }
        }

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestCopyDataToWithTableOnlyCopiesTheRowsOfTheRequestedTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person", "Pet", "Toy");
                source.CopyDataTo("Country", target);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual(6, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
                Assert.AreEqual(6, Helper.TargetCount("\"public\".\"Person\""));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Pet\""));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Toy\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithTableOnlyDoesNotCreateTheRelatedTables()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.TableOnly);

                // Assert
                CollectionAssert.AreEqual(new[] { "\"public\".\"Country\"" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheTableAndItsParents()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                AssertTree(2);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheParentsOfTheParents()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Toy", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableWithoutParentsCopiesTheTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTree(1);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsDoesNotCopyTheChildrenOrTheTablesThatAreNotRelated()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Pet\"");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Toy\"");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Currency\"");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfASelfReferencingTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("\"public\".\"Employee\"");
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Employee\"", "\"ManagerId\"", "\"public\".\"Employee\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsOfATableInACycleBecauseTheRowsReferenceRowsThatAreNotCopiedYet()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("CycleA", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableThatReferencesATableOfAnotherSchemaCopiesTheParentIntoTheSchemaOfTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Ledger", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(2 + 2, actual);
                CollectionAssert.AreEqual(new[] { "\"public\".\"Invoice\"", "\"public\".\"Ledger\"" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("\"Sales\".\"Invoice\"", "\"Id\""), Helper.TargetRows("\"public\".\"Invoice\"", "\"Id\""));
                AssertTable("\"public\".\"Ledger\"");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsOfATableOfAnotherSchemaBecauseTheBulkCopyOfAuroraDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Sales.InvoiceLine", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
            }
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheTableAndItsChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheRowsOfTheChildrenThatReferenceTablesThatAlreadyExistInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<RepoDb.Connector.AuroraDb.Npgsql.AuroraDbException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenDoesNotCopyTheParentsOrTheTablesThatAreNotRelated()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Currency\"");
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenOfASelfReferencingTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("\"public\".\"Employee\"");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithChildrenOfATableOfAnotherSchemaCopiesTheChildrenIntoThatSchemaBecauseTheBulkCopyOfAuroraDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Sales.Invoice", target, relationshipBehavior: CopyRelationshipBehavior.Children));
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
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo(tableName, target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenDoesNotCopyTheTablesThatAreNotRelated()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Currency\"");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Employee\"");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "\"public\".\"Number\"");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfASelfReferencingTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("\"public\".\"Employee\"");
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Employee\"", "\"ManagerId\"", "\"public\".\"Employee\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsAndChildrenOfTablesInACycleBecauseTheRowsReferenceRowsThatAreNotCopiedYet()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("CycleB", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsAndChildrenOfATableOfAnotherSchemaBecauseTheBulkCopyOfAuroraDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Sales.InvoiceLine", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren));
            }
        }

        #endregion

        #region Filter

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                AssertTable("\"public\".\"Country\"");
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Person\"", "\"Id\"", "\"Age\" > 40"), Helper.TargetRows("\"public\".\"Person\"", "\"Id\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Person\"", "\"CountryId\"", "\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithChildrenAndAFilterBecauseTheRowsOfTheChildrenReferenceRowsThatAreNotCopied()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Name", "Japan")), relationshipBehavior: CopyRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsAndChildrenAndAFilterBecauseTheRowsOfTheChildrenReferenceRowsThatAreNotCopied()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterThatMatchesNoRowsCopiesTheParentsOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 1000)), relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionAndParentsFiltersTheRequestedTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo<Entities.Person>(target, p => p.CountryId == 2, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Person\"", "\"Id\"", "\"CountryId\" = 2"), Helper.TargetRows("\"public\".\"Person\"", "\"Id\""));
            }
        }

        #endregion

        #region Existing Tables

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithParentsIfTheRowsOfTheParentsAlreadyExistInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents));
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
                Assert.AreEqual(6, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAddsTheRowsToTheParentsThatAlreadyExistInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Country\" (\"Id\", \"Name\") VALUES (100, 'Atlantis');");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(4, Helper.TargetCount("\"public\".\"Country\""));
                Assert.AreEqual(1, Helper.TargetCount("\"public\".\"Country\"", "\"Name\" = 'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Toy", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Country", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndChildren()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndAFilter()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Assert.AreEqual(2, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithChildrenOfATableOfAnotherSchemaBecauseTheBulkCopyOfAuroraDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<PostgresException>(() => source.CopyDataToAsync("Sales.Invoice", target, relationshipBehavior: CopyRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<RepoDb.Connector.AuroraDb.Npgsql.AuroraDbException>(() => source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
