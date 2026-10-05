#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    /// <summary>
    /// Tests the ordering and the relationships of the tables, that are done in memory, with the graphs that are hard to build in a database.
    /// </summary>
    [TestClass]
    public class MariaDbSchemaReaderOrderTest
    {
        #region Helpers

        private static TableSchema Table(string name, params string[] references)
        {
            var schema = name.Contains('.') ? name.Substring(0, name.IndexOf('.')) : null;
            var table = name.Contains('.') ? name.Substring(name.IndexOf('.') + 1) : name;
            var tableSchema = new TableSchema(table, schema);
            foreach (var reference in references)
            {
                tableSchema.ForeignKeys.Add(new ForeignKeyInfo($"FK_{table}_{reference}")
                {
                    Columns = { "Id" },
                    ReferencedTable = Reference(reference),
                    ReferencedColumns = { "Id" }
                });
            }
            return tableSchema;
        }

        private static string[] Order(params TableSchema[] schemas) =>
            MariaDbSchemaReader.Order(schemas).Select(r => r.Schema.Table.Name).ToArray();

        private static IList<RelationshipInfo> Relationships(params TableSchema[] schemas) =>
            MariaDbSchemaReader.Order(schemas);

        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships, string tableName) =>
            relationships.Single(r => r.Schema.Table.Name == tableName);

        private static string[] Names(IEnumerable<RelationshipInfo> relationships) =>
            relationships.Select(r => r.Schema.Table.Name).ToArray();

        #endregion

        #region Order

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderWithoutTables()
        {
            Assert.AreEqual(0, Relationships().Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfSingleTable()
        {
            // Act
            var actual = Relationships(Table("A"));

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(0, actual[0].Children.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfChainInReverseOrder()
        {
            // Act
            var actual = Order(Table("C", "B"), Table("B", "A"), Table("A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfChainInOrder()
        {
            // Act
            var actual = Order(Table("A"), Table("B", "A"), Table("C", "B"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfTablesThatAreNotRelatedKeepsTheGivenOrder()
        {
            // Act
            var actual = Order(Table("Z"), Table("M"), Table("A"), Table("K"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Z", "M", "A", "K" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderMovesOnlyWhatMustMove()
        {
            // Act
            var actual = Order(Table("B", "A"), Table("X"), Table("A"), Table("Y"));

            // Assert
            CollectionAssert.AreEqual(new[] { "X", "A", "B", "Y" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfDiamond()
        {
            // Act
            var actual = Order(Table("D", "B", "C"), Table("C", "A"), Table("B", "A"), Table("A"));

            // Assert
            Assert.AreEqual("A", actual[0]);
            Assert.AreEqual("D", actual[3]);
            CollectionAssert.AreEqual(new[] { "C", "B" }, actual.Skip(1).Take(2).ToArray());
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfTableWithManyChildren()
        {
            // Act
            var actual = Order(Table("C3", "R"), Table("C1", "R"), Table("R"), Table("C2", "R"));

            // Assert
            CollectionAssert.AreEqual(new[] { "R", "C3", "C1", "C2" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfLongChain()
        {
            // Setup
            var schemas = Enumerable.Range(1, 500)
                .Select(i => i == 1 ? Table("T1") : Table($"T{i}", $"T{i - 1}"))
                .Reverse()
                .ToArray();

            // Act
            var actual = Order(schemas);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 500).Select(i => $"T{i}").ToArray(), actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfLongCycle()
        {
            // Setup
            var schemas = Enumerable.Range(1, 500)
                .Select(i => Table($"T{i}", $"T{i % 500 + 1}"))
                .ToArray();

            // Act
            var actual = Order(schemas);

            // Assert
            CollectionAssert.AreEqual(schemas.Select(s => s.Table.Name).ToArray(), actual);
        }

        #endregion

        #region References

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderIgnoresTheSelfReference()
        {
            // Act
            var actual = Relationships(Table("Employee", "Employee"));

            // Assert
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(0, actual[0].Children.Count);
            Assert.AreEqual(1, actual[0].Schema.ForeignKeys.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfSelfReferencingTableWithChild()
        {
            // Act
            var actual = Relationships(Table("EmployeeNote", "Employee"), Table("Employee", "Employee"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Employee", "EmployeeNote" }, Names(actual));
            CollectionAssert.AreEqual(new[] { "EmployeeNote" }, Names(Get(actual, "Employee").Children));
            Assert.AreEqual(0, Get(actual, "Employee").Parents.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderListsTheParentOnceForTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = Relationships(Table("Transfer", "Account", "Account"), Table("Account"));

            // Assert
            Assert.AreEqual(2, Get(actual, "Transfer").Schema.ForeignKeys.Count);
            Assert.AreEqual(1, Get(actual, "Transfer").Parents.Count);
            Assert.AreEqual(1, Get(actual, "Account").Children.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderIgnoresTheReferencedTablesThatAreNotGiven()
        {
            // Act
            var actual = Relationships(Table("Shipment", "Country", "OrderLine"));

            // Assert
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(2, actual[0].Schema.ForeignKeys.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderIsNotTransitive()
        {
            // Act
            var actual = Relationships(Table("C", "B"), Table("A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "C", "A" }, Names(actual));
            Assert.AreEqual(0, actual[0].Parents.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderMatchesTheNamesCaseSensitively()
        {
            // Act
            var actual = Relationships(Table("Child", "PARENT"), Table("Parent"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Child", "Parent" }, Names(actual));
            Assert.AreEqual(0, Get(actual, "Child").Parents.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderMatchesTheQuotedNames()
        {
            // Act
            var odd = new TableSchema("Odd.Name", "sales");
            var actual = Relationships(Table("Child", "`sales`.`Odd.Name`"), odd);

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Odd.Name", actual[0].Schema.Table.Name, StringComparer.Ordinal);
            Assert.AreSame(actual[0], actual[1].Parents.Single());
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderDistinguishesTheSchemas()
        {
            // Act
            var actual = Relationships(Table("Ref", "sales.Item"), Table("Item"), Table("sales.Item"));
            var reference = Get(actual, "Ref");
            var sales = actual.Single(r => r.Schema.Table.Schema == "sales");
            var dbo = actual.Single(r => r.Schema.Table.Name == "Item" && r.Schema.Table.Schema == null);

            // Assert
            Assert.AreSame(sales, reference.Parents.Single());
            Assert.AreEqual(0, dbo.Children.Count);
            Assert.IsTrue(actual.IndexOf(sales) < actual.IndexOf(reference));
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderKeepsOneRelationshipForTheTableThatIsGivenMoreThanOnce()
        {
            // Act
            var actual = Relationships(Table("A"), Table("A"), Table("B", "A"));

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(1, Get(actual, "A").Children.Count);
        }

        #endregion

        #region Parents and children

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderParentsAndChildrenOfDiamond()
        {
            // Act
            var actual = Relationships(Table("D", "B", "C"), Table("C", "A"), Table("B", "A"), Table("A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "C", "B" }, Names(Get(actual, "A").Children));
            CollectionAssert.AreEqual(new[] { "A" }, Names(Get(actual, "B").Parents));
            CollectionAssert.AreEqual(new[] { "D" }, Names(Get(actual, "B").Children));
            CollectionAssert.AreEqual(new[] { "C", "B" }, Names(Get(actual, "D").Parents));
            Assert.AreEqual(0, Get(actual, "D").Children.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderParentsAndChildrenShareTheSameInstances()
        {
            // Act
            var actual = Relationships(Table("B", "A"), Table("A"));

            // Assert
            Assert.AreSame(actual[0], actual[1].Parents.Single());
            Assert.AreSame(actual[1], actual[0].Children.Single());
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderKeepsTheSchemaOfTheTable()
        {
            // Setup
            var schema = Table("A");

            // Act
            var actual = Relationships(schema);

            // Assert
            Assert.AreSame(schema, actual[0].Schema);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderParentsAndChildrenAreInTheGivenOrder()
        {
            // Act
            var actual = Relationships(Table("C3", "R"), Table("C1", "R"), Table("R"), Table("C2", "R"));

            // Assert
            CollectionAssert.AreEqual(new[] { "C3", "C1", "C2" }, Names(Get(actual, "R").Children));
        }

        #endregion

        #region Cycles

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfTwoTablesThatReferenceEachOther()
        {
            // Act
            var actual = Relationships(Table("A", "B"), Table("B", "A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B" }, Names(actual));
            Assert.AreSame(actual[1], actual[0].Parents.Single());
            Assert.AreSame(actual[1], actual[0].Children.Single());
            Assert.AreSame(actual[0], actual[1].Parents.Single());
            Assert.AreSame(actual[0], actual[1].Children.Single());
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = Order(Table("Z", "Y"), Table("X", "Z"), Table("Y", "X"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Z", "X", "Y" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleWithATableThatItDependsOn()
        {
            // Act
            var actual = Order(Table("A", "Root", "B"), Table("B", "A"), Table("Root"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Root", "A", "B" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleWithATableThatDependsOnIt()
        {
            // Act
            var actual = Order(Table("Leaf", "B"), Table("A", "B"), Table("B", "A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B", "Leaf" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleWithATableThatItDependsOnAndATableThatDependsOnIt()
        {
            // Act
            var actual = Order(Table("Leaf", "B"), Table("B", "A"), Table("A", "Root", "B"), Table("Root"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Root", "B", "A", "Leaf" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfTwoSeparateCycles()
        {
            // Act
            var actual = Order(Table("A1", "A2"), Table("B1", "B2"), Table("A2", "A1"), Table("B2", "B1"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A1", "A2", "B1", "B2" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleThatDependsOnAnotherCycle()
        {
            // Act
            var actual = Order(Table("B1", "B2", "A1"), Table("B2", "B1"), Table("A1", "A2"), Table("A2", "A1"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A1", "A2", "B1", "B2" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfCycleInsideLongerChain()
        {
            // Act
            var actual = Order(Table("Top", "Mid1"), Table("Mid2", "Mid1", "Base"), Table("Mid1", "Mid2"), Table("Base"));

            // Assert
            CollectionAssert.AreEqual(new[] { "Base", "Mid2", "Mid1", "Top" }, actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfSelfReferenceInsideACycle()
        {
            // Act
            var actual = Relationships(Table("A", "A", "B"), Table("B", "A"));

            // Assert
            CollectionAssert.AreEqual(new[] { "A", "B" }, Names(actual));
            CollectionAssert.AreEqual(new[] { "B" }, Names(Get(actual, "A").Parents));
        }

        #endregion

        #region Any given order

        [TestMethod]
        public void TestMariaDbSchemaReaderOrderOfTheSameGraphInAnyGivenOrder()
        {
            // Setup
            Func<TableSchema[]> graph = () => new[]
            {
                Table("A"), Table("B", "A"), Table("C", "A"), Table("D", "B", "C"),
                Table("Root"), Table("L1", "Root", "L2"), Table("L2", "L1"), Table("Leaf", "L2"),
                Table("Solo")
            };
            var cyclic = new[] { "L1", "L2" };
            var random = new Random(7);

            for (var attempt = 0; attempt < 50; attempt++)
            {
                // Act
                var given = graph().OrderBy(_ => random.Next()).ToArray();
                var actual = MariaDbSchemaReader.Order(given);
                var names = Names(actual).ToList();

                // Assert
                Assert.AreEqual(9, names.Count);
                Assert.AreEqual(9, names.Distinct().Count());
                foreach (var relationship in actual)
                {
                    foreach (var parent in relationship.Parents)
                    {
                        if (cyclic.Contains(relationship.Schema.Table.Name) && cyclic.Contains(parent.Schema.Table.Name))
                        {
                            continue;
                        }
                        Assert.IsTrue(
                            names.IndexOf(parent.Schema.Table.Name) < names.IndexOf(relationship.Schema.Table.Name),
                            $"'{parent.Schema.Table.Name}' must come before '{relationship.Schema.Table.Name}'.");
                    }
                }
            }
        }

        #endregion

        private static TableInfo Reference(string name)
        {
            var (schema, table) = MariaDbSchemaHelper.Parse(name);
            return new TableInfo(table, schema);
        }
    }
}
