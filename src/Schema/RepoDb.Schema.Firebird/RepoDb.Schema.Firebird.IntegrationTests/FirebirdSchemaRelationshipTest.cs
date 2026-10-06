#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;

namespace RepoDb.Schema.Firebird.IntegrationTests
{
    [TestClass]
    public class FirebirdSchemaRelationshipTest
    {
        private static readonly string[] CyclicTables =
        {
            "CycleA", "CycleB", "RingX", "RingY", "RingZ", "LoopA", "LoopB"
        };

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Helpers

        private static List<RelationshipInfo> GetRelationships(params string[] tableNames)
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                return new FirebirdSchemaReader(connection).GetDependencyOrder(tableNames).ToList();
            }
        }

        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships, string tableName) =>
            relationships.Single(r => r.Schema.Table.Name == tableName);

        private static string[] Names(IEnumerable<RelationshipInfo> relationships) =>
            relationships.Select(r => r.Schema.Table.Name).ToArray();

        private static List<string> GetSourceTables()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                var reader = new FirebirdSchemaReader(connection);
                return reader.GetTables().ToList();
            }
        }

        #endregion

        #region Diamond

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfDiamond()
        {
            // Act
            var actual = GetRelationships("DiamondD", "DiamondB", "DiamondC", "DiamondA");

            // Assert
            Assert.AreEqual("DiamondA", actual[0].Schema.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual("DiamondD", actual[3].Schema.Table.Name, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" }, Names(actual.Skip(1).Take(2)));
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfDiamondParentsAndChildren()
        {
            // Act
            var actual = GetRelationships("DiamondD", "DiamondB", "DiamondC", "DiamondA");

            // Assert
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" }, Names(Get(actual, "DiamondA").Children));
            Assert.AreEqual(0, Get(actual, "DiamondA").Parents.Count);
            CollectionAssert.AreEqual(new[] { "DiamondA" }, Names(Get(actual, "DiamondB").Parents));
            CollectionAssert.AreEqual(new[] { "DiamondD" }, Names(Get(actual, "DiamondB").Children));
            CollectionAssert.AreEqual(new[] { "DiamondA" }, Names(Get(actual, "DiamondC").Parents));
            CollectionAssert.AreEqual(new[] { "DiamondD" }, Names(Get(actual, "DiamondC").Children));
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" }, Names(Get(actual, "DiamondD").Parents));
            Assert.AreEqual(0, Get(actual, "DiamondD").Children.Count);
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfDiamondInAnyOrder()
        {
            foreach (var given in new[]
            {
                new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" },
                new[] { "DiamondD", "DiamondC", "DiamondB", "DiamondA" },
                new[] { "DiamondC", "DiamondD", "DiamondA", "DiamondB" },
                new[] { "DiamondB", "DiamondA", "DiamondD", "DiamondC" }
            })
            {
                // Act
                var actual = GetRelationships(given);

                // Assert
                Assert.AreEqual("DiamondA", actual[0].Schema.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", actual[3].Schema.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfDiamondInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("DiamondD", "DiamondC", "DiamondB", "DiamondA");

            // Assert
            foreach (var table in new[] { "DiamondA", "DiamondB", "DiamondC", "DiamondD" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Two foreign keys to the same table

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelationships("Transfer", "Account");

            // Assert
            Assert.AreEqual(2, Get(actual, "Transfer").Schema.ForeignKeys.Count);
            Assert.AreEqual(1, Get(actual, "Transfer").Parents.Count);
            Assert.AreEqual(1, Get(actual, "Account").Children.Count);
            CollectionAssert.AreEqual(new[] { "Account", "Transfer" }, Names(actual));
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            Helper.CopyAllToTarget("Transfer", "Account");

            // Assert
            Helper.AssertTargetMatchesSource("Account");
            Helper.AssertTargetMatchesSource("Transfer");
        }

        #endregion

        #region Cycles

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfCycleOfThreeTables()
        {
            // Act
            var actual = GetRelationships("RingX", "RingY", "RingZ");

            // Assert
            CollectionAssert.AreEqual(new[] { "RingX", "RingY", "RingZ" }, Names(actual));
            foreach (var relationship in actual)
            {
                Assert.AreEqual(1, relationship.Parents.Count);
                Assert.AreEqual(1, relationship.Children.Count);
            }
            CollectionAssert.AreEqual(new[] { "RingZ" }, Names(Get(actual, "RingX").Parents));
            CollectionAssert.AreEqual(new[] { "RingX" }, Names(Get(actual, "RingY").Parents));
            CollectionAssert.AreEqual(new[] { "RingY" }, Names(Get(actual, "RingZ").Parents));
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = GetRelationships("RingZ", "RingX", "RingY");

            // Assert
            CollectionAssert.AreEqual(new[] { "RingZ", "RingX", "RingY" }, Names(actual));
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            var actual = GetRelationships("LoopLeaf", "LoopB", "LoopA", "LoopRoot");

            // Assert
            CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, Names(actual));
            CollectionAssert.AreEqual(new[] { "LoopA" }, Names(Get(actual, "LoopRoot").Children));
            CollectionAssert.AreEqual(new[] { "LoopB" }, Names(Get(actual, "LoopLeaf").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopRoot", "LoopB" }, Names(Get(actual, "LoopA").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopA", "LoopLeaf" }, Names(Get(actual, "LoopB").Children));
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnItInAnyOrder()
        {
            foreach (var given in new[]
            {
                new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" },
                new[] { "LoopLeaf", "LoopA", "LoopB", "LoopRoot" },
                new[] { "LoopB", "LoopLeaf", "LoopRoot", "LoopA" },
                new[] { "LoopA", "LoopRoot", "LoopLeaf", "LoopB" }
            })
            {
                // Act
                var actual = Names(GetRelationships(given)).ToList();

                // Assert
                Assert.AreEqual(0, actual.IndexOf("LoopRoot"));
                Assert.AreEqual(3, actual.IndexOf("LoopLeaf"));
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfCycleOfThreeTables()
        {
            // Act
            Helper.CopyAllToTarget("RingY", "RingZ", "RingX");

            // Assert
            Helper.AssertTargetMatchesSource("RingX");
            Helper.AssertTargetMatchesSource("RingY");
            Helper.AssertTargetMatchesSource("RingZ");
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            Helper.CopyAllToTarget("LoopLeaf", "LoopB", "LoopA", "LoopRoot");

            // Assert
            foreach (var table in new[] { "LoopRoot", "LoopA", "LoopB", "LoopLeaf" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Many children

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfTableWithManyChildren()
        {
            // Act
            var actual = GetRelationships("FanChild3", "FanChild1", "FanRoot", "FanChild4", "FanChild2");

            // Assert
            Assert.AreEqual("FanRoot", actual[0].Schema.Table.Name, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "FanChild3", "FanChild1", "FanChild4", "FanChild2" }, Names(Get(actual, "FanRoot").Children));
            foreach (var child in actual.Skip(1))
            {
                CollectionAssert.AreEqual(new[] { "FanRoot" }, Names(child.Parents));
            }
        }

        #endregion

        #region Same table name in different schemas

        #endregion

        #region Names that need quoting

        [TestMethod]
        public void TestFirebirdSchemaReaderReadsTheTableWithADotInItsName()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("\"Odd.Name\"");

                // Assert
                Assert.IsNull(actual.Table.Schema);
                Assert.AreEqual("Odd.Name", actual.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual(2, actual.Columns.Count);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.IsTrue(reader.TableExists("\"Odd.Name\""));
                Assert.IsFalse(reader.TableExists("Odd"));
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaReaderReadsTheTableWithASpaceInItsName()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("\"Order Details\"");

                // Assert
                Assert.AreEqual("Order Details", actual.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("Unit Price", actual.Columns.Single(c => c.Field.Name == "Unit Price").Field.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaReaderReadsTheTableWithABracketInItsName()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("\"Weird]Name\"");

                // Assert
                Assert.AreEqual("Weird]Name", actual.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaReaderGetTablesQuotesTheNamesThatNeedIt()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new FirebirdSchemaReader(connection);

                // Act
                var actual = reader.GetTables().ToList();

                // Assert
                CollectionAssert.Contains(actual, "Person");
                CollectionAssert.Contains(actual, "\"Odd.Name\"");
                CollectionAssert.Contains(actual, "\"Order Details\"");
                CollectionAssert.Contains(actual, "\"Weird]Name\"");
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfTablesWithADotInTheirNames()
        {
            // Act
            var actual = GetRelationships("\"Odd.Child\"", "\"Odd.Name\"");

            // Assert
            CollectionAssert.AreEqual(new[] { "\"Odd.Name\"", "\"Odd.Child\"" }, Helper.GetTableNames(actual));
            Assert.AreSame(actual[0], actual[1].Parents.Single());
            Assert.AreEqual(new TableInfo("Odd.Name", null), actual[1].Schema.ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfTablesWithNamesThatNeedQuoting()
        {
            // Act
            Helper.CopyAllToTarget("\"Odd.Child\"", "\"Odd.Name\"", "\"Order Details\"", "\"Weird]Name\"");

            // Assert
            Helper.AssertTargetMatchesSource("\"Odd.Name\"");
            Helper.AssertTargetMatchesSource("\"Odd.Child\"");
            Helper.AssertTargetMatchesSource("\"Order Details\"");
            Helper.AssertTargetMatchesSource("\"Weird]Name\"");
        }

        #endregion

        #region Limits

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOnlyConsidersTheDirectReferencesBetweenTheGivenTables()
        {
            // Act
            var actual = GetRelationships("GrandChild", "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, Names(actual));
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(0, actual[1].Children.Count);
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipKeepsTheGivenOrderOfTheTablesThatAreNotRelated()
        {
            // Act
            var actual = GetRelationships("NoKey", "Parent", "Country", "OrderLine");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey", "Parent", "Country", "OrderLine" }, Names(actual));
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipOfTableThatReferencesATableThatIsNotGiven()
        {
            // Act
            var actual = GetRelationships("Shipment");

            // Assert
            Assert.AreEqual(2, actual[0].Schema.ForeignKeys.Count);
            Assert.AreEqual(0, actual[0].Parents.Count);
        }

        #endregion

        #region Whole database

        [TestMethod]
        public void TestFirebirdSchemaRelationshipsOfTheWholeDatabaseAreConsistent()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var actual = GetRelationships(tables.ToArray());

            // Assert
            Assert.AreEqual(tables.Count, actual.Count);
            foreach (var relationship in actual)
            {
                Assert.AreEqual(relationship.Parents.Count, relationship.Parents.Distinct().Count());
                Assert.AreEqual(relationship.Children.Count, relationship.Children.Distinct().Count());
                Assert.IsFalse(relationship.Parents.Contains(relationship));
                Assert.IsFalse(relationship.Children.Contains(relationship));
                foreach (var parent in relationship.Parents)
                {
                    Assert.IsTrue(parent.Children.Contains(relationship));
                    Assert.IsTrue(actual.Contains(parent));
                }
                foreach (var child in relationship.Children)
                {
                    Assert.IsTrue(child.Parents.Contains(relationship));
                }
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipsOfTheWholeDatabaseCoverTheForeignKeys()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var actual = GetRelationships(tables.ToArray());

            // Assert
            foreach (var relationship in actual)
            {
                var self = Helper.FormatName(relationship.Schema.Table.Schema, relationship.Schema.Table.Name);
                var referenced = relationship.Schema.ForeignKeys
                    .Select(fk => Helper.FormatName(fk.ReferencedTable.Schema, fk.ReferencedTable.Name))
                    .Where(t => !string.Equals(t, self, StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                var parents = Helper.GetTableNames(relationship.Parents)
                    .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                CollectionAssert.AreEqual(referenced, parents, $"Parents of '{self}'.");
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipsOfTheWholeDatabaseAreOrderedInAnyGivenOrder()
        {
            // Setup
            var tables = GetSourceTables();
            var random = new Random(1);
            var orders = new List<List<string>>
            {
                tables,
                tables.AsEnumerable().Reverse().ToList(),
                tables.Skip(tables.Count / 2).Concat(tables.Take(tables.Count / 2)).ToList(),
                tables.OrderBy(_ => random.Next()).ToList(),
                tables.OrderBy(_ => random.Next()).ToList(),
                tables.OrderBy(_ => random.Next()).ToList()
            };

            foreach (var given in orders)
            {
                // Act
                var actual = GetRelationships(given.ToArray());
                var order = Helper.GetTableNames(actual).ToList();

                // Assert
                Assert.AreEqual(tables.Count, order.Count);
                foreach (var relationship in actual)
                {
                    var name = Helper.FormatName(relationship.Schema.Table.Schema, relationship.Schema.Table.Name);
                    foreach (var parent in relationship.Parents)
                    {
                        var parentName = Helper.FormatName(parent.Schema.Table.Schema, parent.Schema.Table.Name);
                        if (CyclicTables.Contains(name, StringComparer.OrdinalIgnoreCase) &&
                            CyclicTables.Contains(parentName, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        Assert.IsTrue(order.IndexOf(parentName) < order.IndexOf(name), $"'{parentName}' must come before '{name}'.");
                    }
                }
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaRelationshipsOfTheWholeDatabaseKeepTheGivenOrderInsideTheCycles()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var order = Helper.GetTableNames(GetRelationships(tables.AsEnumerable().Reverse().ToArray())).ToList();

            // Assert
            foreach (var cycle in new[] { new[] { "RingZ", "RingY", "RingX" }, new[] { "CycleB", "CycleA" } })
            {
                var positions = cycle.Select(t => order.IndexOf(t)).ToArray();
                CollectionAssert.AreEqual(positions.OrderBy(p => p).ToArray(), positions);
            }
        }

        [TestMethod]
        public void TestFirebirdSchemaComposerComposedSchemaOfTheWholeDatabaseInTheReverseOrder()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            Helper.CopyAllToTarget(tables.AsEnumerable().Reverse().ToArray());

            // Assert
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public async Task TestFirebirdSchemaRelationshipsAsyncOfTheWholeDatabaseAreTheSameAsTheSyncOnes()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                var reader = new FirebirdSchemaReader(connection);

                // Act
                var expected = reader.GetDependencyOrder(tables).ToList();
                var actual = (await reader.GetDependencyOrderAsync(tables)).ToList();

                // Assert
                CollectionAssert.AreEqual(Helper.GetTableNames(expected), Helper.GetTableNames(actual));
                for (var i = 0; i < expected.Count; i++)
                {
                    CollectionAssert.AreEqual(Helper.GetTableNames(expected[i].Parents), Helper.GetTableNames(actual[i].Parents));
                    CollectionAssert.AreEqual(Helper.GetTableNames(expected[i].Children), Helper.GetTableNames(actual[i].Children));
                }
            }
        }

        #endregion
    }
}
