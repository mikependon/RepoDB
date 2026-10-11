#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.MariaDb.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.MariaDb.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table together with the rows of the tables that are related to it (the Country, Person, Pet and Toy tables
    /// form a tree: each one references the previous one), as defined by the <see cref="CopyDataRelationshipBehavior"/>.
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

        private static readonly string[] Tree = { "`Country`", "`Person`", "`Pet`", "`Toy`" };

        private static void AssertTable(string quotedName,
            string orderBy = "`Id`") =>
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
                Assert.AreEqual(0, Helper.TargetOrphans("`Person`", "`CountryId`", "`Country`", "`Id`"));
            }
            if (expectedTables >= 3)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("`Pet`", "`PersonId`", "`Person`", "`Id`"));
            }
            if (expectedTables >= 4)
            {
                Assert.AreEqual(0, Helper.TargetOrphans("`Toy`", "`PetId`", "`Pet`", "`Id`"));
            }
        }

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestCopyDataToWithTableOnlyCopiesTheRowsOfTheRequestedTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person", "Pet", "Toy");
                source.CopyDataTo("Country", target);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.TableOnly);

                // Assert
                Assert.AreEqual(6, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Country`"));
                Assert.AreEqual(6, Helper.TargetCount("`Person`"));
                Assert.AreEqual(0, Helper.TargetCount("`Pet`"));
                Assert.AreEqual(0, Helper.TargetCount("`Toy`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithTableOnlyDoesNotCreateTheRelatedTables()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.TableOnly);

                // Assert
                CollectionAssert.AreEqual(new[] { "`Country`" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheTableAndItsParents()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                AssertTree(2);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheParentsOfTheParents()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Toy", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableWithoutParentsCopiesTheTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTree(1);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsDoesNotCopyTheChildrenOrTheTablesThatAreNotRelated()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Pet`");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Toy`");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Currency`");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfASelfReferencingTable()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("`Employee`");
                Assert.AreEqual(0, Helper.TargetOrphans("`Employee`", "`ManagerId`", "`Employee`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableInACycle()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("CycleA", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that reference rows that are not copied yet)
                // Assert
                Assert.AreEqual(2, actual);
                Assert.AreEqual(0, Helper.TargetOrphans("`CycleA`", "`BId`", "`CycleB`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`CycleB`", "`AId`", "`CycleA`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableThatReferencesATableOfAnotherSchemaCopiesTheParentIntoTheSchemaOfTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Ledger", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(2 + 2, actual);
                // (the schema of MySQL is a database: the parent is copied into the database of the same name, that is the one of the source here)
                Assert.AreEqual("`Ledger`", string.Join(",", Helper.TargetTables()));
                AssertTable("`Ledger`");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsOfATableOfAnotherSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("repodb_mdb_source_sales.InvoiceLine", "repodb_mdb_target_sales.InvoiceLine", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(2 + 3, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Invoice`", "`Sales`.`InvoiceLine`" }, Helper.TargetTables().ToArray());
                AssertTable("`Sales`.`Invoice`");
                AssertTable("`Sales`.`InvoiceLine`");
            }
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheTableAndItsChildren()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheRowsOfTheChildrenThatReferenceTablesThatAlreadyExistInTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<MariaDbException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Children));
                Assert.AreEqual(0, Helper.TargetCount("`Person`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenDoesNotCopyTheParentsOrTheTablesThatAreNotRelated()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw);

                // Act
                source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Currency`");
                Assert.AreEqual(3, Helper.TargetCount("`Country`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenOfASelfReferencingTable()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("`Employee`");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenOfATableOfAnotherSchemaCopiesTheChildrenIntoThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("repodb_mdb_source_sales.Invoice", "repodb_mdb_target_sales.Invoice", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Invoice`", "`Sales`.`InvoiceLine`", "`Sales`.`Ledger`" }, Helper.TargetTables().ToArray());
                AssertTable("`Sales`.`Invoice`");
                AssertTable("`Sales`.`InvoiceLine`");
                Helper.AssertRowsEqual(Helper.SourceRows("`Ledger`", "`Id`"), Helper.TargetRows("`Sales`.`Ledger`", "`Id`"));
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
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo(tableName, target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenDoesNotCopyTheTablesThatAreNotRelated()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                source.CopyDataTo("Pet", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Currency`");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Employee`");
                CollectionAssert.DoesNotContain(Helper.TargetTables(), "`Number`");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfASelfReferencingTable()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Employee", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3, actual);
                AssertTable("`Employee`");
                Assert.AreEqual(0, Helper.TargetOrphans("`Employee`", "`ManagerId`", "`Employee`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfTablesInACycle()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("CycleB", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that reference rows that are not copied yet)
                // Assert
                Assert.AreEqual(2, actual);
                Assert.AreEqual(0, Helper.TargetOrphans("`CycleA`", "`BId`", "`CycleB`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`CycleB`", "`AId`", "`CycleA`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenOfATableOfAnotherSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("repodb_mdb_source_sales.InvoiceLine", "repodb_mdb_target_sales.InvoiceLine", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Invoice`", "`Sales`.`InvoiceLine`", "`Sales`.`Ledger`" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #region Filter

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                AssertTable("`Country`");
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`", "`Age` > 40"), Helper.TargetRows("`Person`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Person`", "`CountryId`", "`Country`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Name", "Japan")), relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows of the children that reference the parents that are not copied)
                // Assert
                Assert.AreEqual(1 + 6 + 5 + 5, actual);
                Assert.AreEqual(1, Helper.TargetCount("`Country`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Person`", "`CountryId`", "`Country`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Pet`", "`PersonId`", "`Person`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Toy`", "`PetId`", "`Pet`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows of the children that reference the parents that are not copied)
                // Assert
                Assert.AreEqual(3 + 2 + 5 + 5, actual);
                Assert.AreEqual(2, Helper.TargetCount("`Person`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Person`", "`CountryId`", "`Country`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Pet`", "`PersonId`", "`Person`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Toy`", "`PetId`", "`Pet`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndAFilterThatMatchesNoRowsCopiesTheParentsOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 1000)), relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Country`"));
                Assert.AreEqual(0, Helper.TargetCount("`Person`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionAndParentsFiltersTheRequestedTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo<Entities.Person>(target, p => p.CountryId == 2, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`", "`CountryId` = 2"), Helper.TargetRows("`Person`", "`Id`"));
            }
        }

        #endregion

        #region Existing Tables

        [TestMethod]
        public void TestCopyDataToWithParentsIgnoresTheRowsOfTheParentsThatAlreadyExistInTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that violate a key)
                source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3, Helper.TargetCount("`Country`"));
                Assert.AreEqual(6, Helper.TargetCount("`Person`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAddsTheRowsToTheParentsThatAlreadyExistInTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");
                Helper.ExecuteOnTarget("INSERT INTO `Country` (`Id`, `Name`) VALUES (100, 'Atlantis');");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(4, Helper.TargetCount("`Country`"));
                Assert.AreEqual(1, Helper.TargetCount("`Country`", "`Name` = 'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("`Person`"));
            }
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                foreach (var behavior in new[] { CopyDataRelationshipBehavior.Parents, CopyDataRelationshipBehavior.Children, CopyDataRelationshipBehavior.ParentsAndChildren })
                {
                    Assert.Throws<ArgumentException>(() => source.CopyDataTo("Pet", "PetCopy", target, relationshipBehavior: behavior));
                }
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheRelationshipBehaviorIsNotTableOnlyAndThereIsNoSchemaReaderForTheSourceConnection()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParents()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Toy", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithChildren()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndChildren()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Pet", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                AssertTree();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndAFilter()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Assert.AreEqual(2, Helper.TargetCount("`Person`"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithChildrenOfATableOfAnotherSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("repodb_mdb_source_sales.Invoice", "repodb_mdb_target_sales.Invoice", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(2 + 3 + 2, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Invoice`", "`Sales`.`InvoiceLine`", "`Sales`.`Ledger`" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithChildrenIfTheTableThatTheChildrenReferenceDoesNotExistInTheTarget()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<MariaDbException>(() => source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => source.CopyDataToAsync("Pet", "PetCopy", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
            }
        }

        #endregion
    }
}
