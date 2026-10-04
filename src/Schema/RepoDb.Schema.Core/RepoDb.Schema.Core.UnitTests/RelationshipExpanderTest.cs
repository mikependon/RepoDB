#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class RelationshipExpanderTest
    {
        #region Helpers

        private static TableInfo T(string name, string schema = "dbo") =>
            new TableInfo(name, schema);

        // A table (the child) that references another table (the parent)
        private static (TableInfo Child, TableInfo Parent) Fk(string child, string parent) =>
            (T(child), T(parent));

        private static string Key(TableInfo table) =>
            $"{table.Schema}\u0001{table.Name}";

        private static string[] Expand(string[] tables,
            CopySchemaRelationshipBehavior behavior,
            params (TableInfo Child, TableInfo Parent)[] foreignKeys) =>
            RelationshipExpander.Expand(tables.Select(t => T(t)), foreignKeys, behavior, Key).Select(t => t.Name).ToArray();

        // Root <- Mid <- Leaf, Root <- Sibling, Other <- Mid (Other is a second parent of Mid), Alone (no relationship)
        private static readonly (TableInfo Child, TableInfo Parent)[] Tree =
        {
            Fk("Mid", "Root"),
            Fk("Leaf", "Mid"),
            Fk("Sibling", "Root"),
            Fk("Mid", "Other")
        };

        #endregion

        #region TableOnly

        [TestMethod]
        public void TestRelationshipExpanderTableOnly()
        {
            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.TableOnly, Tree);

            // Assert
            CollectionAssert.AreEqual(new[] { "Mid" }, actual);
        }

        [TestMethod]
        public void TestRelationshipExpanderTableOnlyKeepsTheGivenTablesInOrderWithoutDuplicates()
        {
            // Act
            var actual = Expand(new[] { "B", "A", "B" }, CopySchemaRelationshipBehavior.TableOnly, Tree);

            // Assert
            CollectionAssert.AreEqual(new[] { "B", "A" }, actual);
        }

        #endregion

        #region Parents

        [TestMethod]
        public void TestRelationshipExpanderParents()
        {
            // Act
            var actual = Expand(new[] { "Leaf" }, CopySchemaRelationshipBehavior.Parents, Tree);

            // Assert (the parents of the parents too, but not the siblings)
            CollectionAssert.AreEquivalent(new[] { "Leaf", "Mid", "Root", "Other" }, actual);
            Assert.AreEqual("Leaf", actual[0]);
            Assert.AreEqual("Mid", actual[1]);
        }

        [TestMethod]
        public void TestRelationshipExpanderParentsDoesNotIncludeTheChildren()
        {
            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.Parents, Tree);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "Mid", "Root", "Other" }, actual);
        }

        [TestMethod]
        public void TestRelationshipExpanderParentsOfATableWithoutParents()
        {
            // Act
            var actual = Expand(new[] { "Root" }, CopySchemaRelationshipBehavior.Parents, Tree);

            // Assert
            CollectionAssert.AreEqual(new[] { "Root" }, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestRelationshipExpanderChildren()
        {
            // Act
            var actual = Expand(new[] { "Root" }, CopySchemaRelationshipBehavior.Children, Tree);

            // Assert (the children of the children too, but not the other parent of a child)
            CollectionAssert.AreEquivalent(new[] { "Root", "Mid", "Sibling", "Leaf" }, actual);
            Assert.AreEqual("Root", actual[0]);
            Assert.AreEqual("Leaf", actual[3]);
        }

        [TestMethod]
        public void TestRelationshipExpanderChildrenDoesNotIncludeTheParents()
        {
            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.Children, Tree);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "Mid", "Leaf" }, actual);
        }

        [TestMethod]
        public void TestRelationshipExpanderChildrenOfATableWithoutChildren()
        {
            // Act
            var actual = Expand(new[] { "Leaf" }, CopySchemaRelationshipBehavior.Children, Tree);

            // Assert
            CollectionAssert.AreEqual(new[] { "Leaf" }, actual);
        }

        #endregion

        #region ParentsAndChildren

        [TestMethod]
        public void TestRelationshipExpanderParentsAndChildren()
        {
            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.ParentsAndChildren, Tree);

            // Assert (the whole tree, in any direction)
            CollectionAssert.AreEquivalent(new[] { "Mid", "Root", "Other", "Leaf", "Sibling" }, actual);
            Assert.AreEqual("Mid", actual[0]);
        }

        [TestMethod]
        public void TestRelationshipExpanderParentsAndChildrenStartingFromTheLeaf()
        {
            // Act
            var actual = Expand(new[] { "Leaf" }, CopySchemaRelationshipBehavior.ParentsAndChildren, Tree);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "Leaf", "Mid", "Root", "Other", "Sibling" }, actual);
        }

        [TestMethod]
        public void TestRelationshipExpanderParentsAndChildrenDoesNotIncludeTheUnrelatedTables()
        {
            // Setup
            var foreignKeys = Tree.Concat(new[] { Fk("Island2", "Island1") }).ToArray();

            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.ParentsAndChildren, foreignKeys);

            // Assert
            Assert.IsFalse(actual.Contains("Island1"));
            Assert.IsFalse(actual.Contains("Island2"));
        }

        [TestMethod]
        public void TestRelationshipExpanderParentsAndChildrenOfATableWithoutRelationships()
        {
            // Act
            var actual = Expand(new[] { "Alone" }, CopySchemaRelationshipBehavior.ParentsAndChildren, Tree);

            // Assert
            CollectionAssert.AreEqual(new[] { "Alone" }, actual);
        }

        #endregion

        #region Shapes

        [TestMethod]
        public void TestRelationshipExpanderOfADiamond()
        {
            // Setup (A <- B, A <- C, B <- D, C <- D)
            var diamond = new[] { Fk("B", "A"), Fk("C", "A"), Fk("D", "B"), Fk("D", "C") };

            // Act/Assert
            CollectionAssert.AreEquivalent(new[] { "D", "B", "C", "A" }, Expand(new[] { "D" }, CopySchemaRelationshipBehavior.Parents, diamond));
            CollectionAssert.AreEquivalent(new[] { "A", "B", "C", "D" }, Expand(new[] { "A" }, CopySchemaRelationshipBehavior.Children, diamond));
            CollectionAssert.AreEquivalent(new[] { "B", "A", "C", "D" }, Expand(new[] { "B" }, CopySchemaRelationshipBehavior.ParentsAndChildren, diamond));
            Assert.AreEqual(4, Expand(new[] { "D" }, CopySchemaRelationshipBehavior.Parents, diamond).Distinct().Count());
        }

        [TestMethod]
        public void TestRelationshipExpanderOfACycle()
        {
            // Setup (A -> B -> C -> A)
            var ring = new[] { Fk("A", "B"), Fk("B", "C"), Fk("C", "A") };

            // Act/Assert
            foreach (var behavior in new[] { CopySchemaRelationshipBehavior.Parents, CopySchemaRelationshipBehavior.Children, CopySchemaRelationshipBehavior.ParentsAndChildren })
            {
                CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, Expand(new[] { "A" }, behavior, ring));
            }
        }

        [TestMethod]
        public void TestRelationshipExpanderOfASelfReference()
        {
            // Setup
            var self = new[] { Fk("Employee", "Employee"), Fk("Badge", "Employee") };

            // Act/Assert
            CollectionAssert.AreEqual(new[] { "Employee" }, Expand(new[] { "Employee" }, CopySchemaRelationshipBehavior.Parents, self));
            CollectionAssert.AreEquivalent(new[] { "Employee", "Badge" }, Expand(new[] { "Employee" }, CopySchemaRelationshipBehavior.Children, self));
            CollectionAssert.AreEquivalent(new[] { "Employee", "Badge" }, Expand(new[] { "Employee" }, CopySchemaRelationshipBehavior.ParentsAndChildren, self));
        }

        [TestMethod]
        public void TestRelationshipExpanderWithMultipleForeignKeysToTheSameTable()
        {
            // Setup (Transfer has 2 foreign keys to Account)
            var foreignKeys = new[] { Fk("Transfer", "Account"), Fk("Transfer", "Account") };

            // Act
            var actual = Expand(new[] { "Transfer" }, CopySchemaRelationshipBehavior.Parents, foreignKeys);

            // Assert
            CollectionAssert.AreEqual(new[] { "Transfer", "Account" }, actual);
        }

        #endregion

        #region Given Tables

        [TestMethod]
        public void TestRelationshipExpanderWithMultipleGivenTables()
        {
            // Act
            var actual = Expand(new[] { "Sibling", "Leaf" }, CopySchemaRelationshipBehavior.Parents, Tree);

            // Assert (the given tables come first, in the given order, and nothing is repeated)
            CollectionAssert.AreEquivalent(new[] { "Sibling", "Leaf", "Root", "Mid", "Other" }, actual);
            CollectionAssert.AreEqual(new[] { "Sibling", "Leaf" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestRelationshipExpanderDoesNotRepeatAGivenTableThatIsAlsoRelated()
        {
            // Act
            var actual = Expand(new[] { "Mid", "Root" }, CopySchemaRelationshipBehavior.Parents, Tree);

            // Assert
            Assert.AreEqual(actual.Length, actual.Distinct().Count());
            CollectionAssert.AreEqual(new[] { "Mid", "Root" }, actual.Take(2).ToArray());
        }

        [TestMethod]
        public void TestRelationshipExpanderWithNoGivenTables()
        {
            // Act
            var actual = Expand(new string[0], CopySchemaRelationshipBehavior.ParentsAndChildren, Tree);

            // Assert
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void TestRelationshipExpanderWithNoForeignKeys()
        {
            // Act
            var actual = Expand(new[] { "Person" }, CopySchemaRelationshipBehavior.ParentsAndChildren);

            // Assert
            CollectionAssert.AreEqual(new[] { "Person" }, actual);
        }

        [TestMethod]
        public void TestRelationshipExpanderNearestRelativesComeFirst()
        {
            // Setup (Root <- Mid <- Leaf)
            var chain = new[] { Fk("Mid", "Root"), Fk("Leaf", "Mid") };

            // Act
            var actual = Expand(new[] { "Mid" }, CopySchemaRelationshipBehavior.ParentsAndChildren, chain);

            // Assert
            Assert.AreEqual("Mid", actual[0]);
            CollectionAssert.AreEquivalent(new[] { "Root", "Leaf" }, actual.Skip(1).ToArray());
        }

        #endregion

        #region Identity

        [TestMethod]
        public void TestRelationshipExpanderTablesInDifferentSchemasAreDifferentTables()
        {
            // Setup (dbo.Item <- dbo.ItemRef, but sales.Item has no relationship)
            var foreignKeys = new[] { (T("ItemRef"), T("Item")) };

            // Act
            var actual = RelationshipExpander.Expand(new[] { T("Item", "sales") }, foreignKeys, CopySchemaRelationshipBehavior.ParentsAndChildren, Key);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("sales", actual[0].Schema);
        }

        [TestMethod]
        public void TestRelationshipExpanderUsesTheKeyToIdentifyTheTables()
        {
            // Setup (the key is case-insensitive)
            string CaseInsensitiveKey(TableInfo table) => Key(table).ToLowerInvariant();

            // Act
            var actual = RelationshipExpander.Expand(new[] { T("PERSON") }, new[] { (T("Person"), T("Country")), (T("Order"), T("PERSON")) },
                CopySchemaRelationshipBehavior.ParentsAndChildren, CaseInsensitiveKey);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "PERSON", "Country", "Order" }, actual.Select(t => t.Name).ToArray());
        }

        [TestMethod]
        public void TestRelationshipExpanderKeepsTheNameOfTheRelatedTablesAsRead()
        {
            // Act
            var actual = RelationshipExpander.Expand(new[] { T("Child") }, new[] { (T("Child"), T("Parent", "sales")) },
                CopySchemaRelationshipBehavior.Parents, Key);

            // Assert
            Assert.AreEqual(new TableInfo("Parent", "sales"), actual[1]);
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnRelationshipExpanderIfTheBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Expand(new[] { "Person" }, (CopySchemaRelationshipBehavior)99, Tree));
        }

        #endregion
    }
}
