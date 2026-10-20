#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.ClickHouse.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.ClickHouse.IntegrationTests
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
        public void TestCopyDataToWithParentsOfATableWithoutParentsCopiesTheTableOnly()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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

        #endregion

        #region Children

        [TestMethod]
        public void TestCopyDataToWithChildrenDoesNotCopyTheParentsOrTheTablesThatAreNotRelated()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Country", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenDoesNotCopyTheTablesThatAreNotRelated()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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

        #endregion

        #region Filter

        #endregion

        #region Existing Tables

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
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
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingMappingException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => source.CopyDataToAsync("Pet", "PetCopy", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
            }
        }

        #endregion

        [TestMethod]
        public void TestCopyDataToWithParentsCopiesTheRequestedTableOnlyBecauseThereIsNoForeignKey()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act (ClickHouse has no foreign key, so there are no related tables to copy)
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(6, actual);
                CollectionAssert.AreEqual(new[] { "`Person`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`"), Helper.TargetRows("`Person`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithChildrenCopiesTheRequestedTableOnlyBecauseThereIsNoForeignKey()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act (ClickHouse has no foreign key, so there are no related tables to copy)
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(6, actual);
                CollectionAssert.AreEqual(new[] { "`Person`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`"), Helper.TargetRows("`Person`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithParentsAndChildrenCopiesTheRequestedTableOnlyBecauseThereIsNoForeignKey()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act (ClickHouse has no foreign key, so there are no related tables to copy)
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(6, actual);
                CollectionAssert.AreEqual(new[] { "`Person`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`"), Helper.TargetRows("`Person`", "`Id`"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithParentsAndChildrenCopiesTheRequestedTableOnlyBecauseThereIsNoForeignKey()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act (ClickHouse has no foreign key, so there are no related tables to copy)
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

                // Assert
                Assert.AreEqual(6, actual);
                CollectionAssert.AreEqual(new[] { "`Person`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`"), Helper.TargetRows("`Person`", "`Id`"));
            }
        }
    }
}
