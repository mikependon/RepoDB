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
using RepoDb.Schema.Models;
using RepoDb.Schema.Vertica.IntegrationTests.Setup;

namespace RepoDb.Schema.Vertica.IntegrationTests
{
    /// <summary>
    /// Contains the vertica schema relationship test behavior.
    /// </summary>
    [TestClass]
    public class VerticaSchemaRelationshipTest
    {
        private static readonly string[] CyclicTables =
        {
            "CycleA",
            "CycleB",
            "RingX",
            "RingY",
            "RingZ",
            "LoopA",
            "LoopB"
        };

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs get relationships.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        private static List<RelationshipInfo> GetRelationships(params string[] tableNames)
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader().GetDependencyOrder(connection,
                    tableNames,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        /// <param name="tableName">The table name.</param>
        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships,
            string tableName) => relationships.Single(r => r.Schema.Table.Name == tableName);

        /// <summary>
        /// Performs names.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        private static string[] Names(IEnumerable<RelationshipInfo> relationships) => relationships.Select(r => r.Schema.Table.Name).ToArray();

        /// <summary>
        /// Performs get source tables.
        /// </summary>
        private static List<string> GetSourceTables()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                var reader = new VerticaSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Concat(reader.GetTables(connection,
                        Database.SourceSalesName,
                        transaction: null)).ToList();
            }
        }

        #endregion

        #region Diamond

        /// <summary>
        /// Verifies vertica schema relationship of diamond.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfDiamond()
        {
            // Act
            var actual = GetRelationships("DiamondD",
                "DiamondB",
                "DiamondC",
                "DiamondA");
            // Assert
            Assert.AreEqual("DiamondA",
                actual[0].Schema.Table.Name,
                StringComparer.Ordinal);
            Assert.AreEqual("DiamondD",
                actual[3].Schema.Table.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" },
                Names(actual.Skip(1).Take(2)));
        }

        /// <summary>
        /// Verifies vertica schema relationship of diamond parents and children.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfDiamondParentsAndChildren()
        {
            // Act
            var actual = GetRelationships("DiamondD",
                "DiamondB",
                "DiamondC",
                "DiamondA");
            // Assert
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" },
                Names(Get(actual,
                        "DiamondA").Children));
            Assert.AreEqual(0,
                Get(actual,
                    "DiamondA").Parents.Count);
            CollectionAssert.AreEqual(new[] { "DiamondA" },
                Names(Get(actual,
                        "DiamondB").Parents));
            CollectionAssert.AreEqual(new[] { "DiamondD" },
                Names(Get(actual,
                        "DiamondB").Children));
            CollectionAssert.AreEqual(new[] { "DiamondA" },
                Names(Get(actual,
                        "DiamondC").Parents));
            CollectionAssert.AreEqual(new[] { "DiamondD" },
                Names(Get(actual,
                        "DiamondC").Children));
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" },
                Names(Get(actual,
                        "DiamondD").Parents));
            Assert.AreEqual(0,
                Get(actual,
                    "DiamondD").Children.Count);
        }

        /// <summary>
        /// Verifies vertica schema relationship of diamond in any order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfDiamondInAnyOrder()
        {
            foreach (var given in new[]
            {
                new[]
                {
                    "DiamondA",
                    "DiamondB",
                    "DiamondC",
                    "DiamondD"
                },
                new[]
                {
                    "DiamondD",
                    "DiamondC",
                    "DiamondB",
                    "DiamondA"
                },
                new[]
                {
                    "DiamondC",
                    "DiamondD",
                    "DiamondA",
                    "DiamondB"
                },
                new[]
                {
                    "DiamondB",
                    "DiamondA",
                    "DiamondD",
                    "DiamondC"
                }
            }

            )
            {
                // Act
                var actual = GetRelationships(given);
                // Assert
                Assert.AreEqual("DiamondA",
                    actual[0].Schema.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("DiamondD",
                    actual[3].Schema.Table.Name,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of diamond in any order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfDiamondInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("DiamondD",
                "DiamondC",
                "DiamondB",
                "DiamondA");
            // Assert
            foreach (var table in new[]
            {
                "DiamondA",
                "DiamondB",
                "DiamondC",
                "DiamondD"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Two foreign keys to the same table

        /// <summary>
        /// Verifies vertica schema relationship of table with two foreign keys to the same table.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelationships("Transfer",
                "Account");
            // Assert
            Assert.AreEqual(2,
                Get(actual,
                    "Transfer").Schema.ForeignKeys.Count);
            Assert.AreEqual(1,
                Get(actual,
                    "Transfer").Parents.Count);
            Assert.AreEqual(1,
                Get(actual,
                    "Account").Children.Count);
            CollectionAssert.AreEqual(new[] { "Account", "Transfer" },
                Names(actual));
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of table with two foreign keys to the same table.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            Helper.CopyAllToTarget("Transfer",
                "Account");
            // Assert
            Helper.AssertTargetMatchesSource("Account");
            Helper.AssertTargetMatchesSource("Transfer");
        }

        #endregion

        #region Cycles

        /// <summary>
        /// Verifies vertica schema relationship of cycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfCycleOfThreeTables()
        {
            // Act
            var actual = GetRelationships("RingX",
                "RingY",
                "RingZ");
            // Assert
            CollectionAssert.AreEqual(new[] { "RingX", "RingY", "RingZ" },
                Names(actual));
            foreach (var relationship in actual)
            {
                Assert.AreEqual(1,
                    relationship.Parents.Count);
                Assert.AreEqual(1,
                    relationship.Children.Count);
            }

            CollectionAssert.AreEqual(new[] { "RingZ" },
                Names(Get(actual,
                        "RingX").Parents));
            CollectionAssert.AreEqual(new[] { "RingX" },
                Names(Get(actual,
                        "RingY").Parents));
            CollectionAssert.AreEqual(new[] { "RingY" },
                Names(Get(actual,
                        "RingZ").Parents));
        }

        /// <summary>
        /// Verifies vertica schema relationship of cycle of three tables keeps the given order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = GetRelationships("RingZ",
                "RingX",
                "RingY");
            // Assert
            CollectionAssert.AreEqual(new[] { "RingZ", "RingX", "RingY" },
                Names(actual));
        }

        /// <summary>
        /// Verifies vertica schema relationship of cycle with the tables that it depends on and that depend on it.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            var actual = GetRelationships("LoopLeaf",
                "LoopB",
                "LoopA",
                "LoopRoot");
            // Assert
            CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" },
                Names(actual));
            CollectionAssert.AreEqual(new[] { "LoopA" },
                Names(Get(actual,
                        "LoopRoot").Children));
            CollectionAssert.AreEqual(new[] { "LoopB" },
                Names(Get(actual,
                        "LoopLeaf").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopRoot", "LoopB" },
                Names(Get(actual,
                        "LoopA").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopA", "LoopLeaf" },
                Names(Get(actual,
                        "LoopB").Children));
        }

        /// <summary>
        /// Verifies vertica schema relationship of cycle with the tables that it depends on and that depend on it in any order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnItInAnyOrder()
        {
            foreach (var given in new[]
            {
                new[]
                {
                    "LoopRoot",
                    "LoopA",
                    "LoopB",
                    "LoopLeaf"
                },
                new[]
                {
                    "LoopLeaf",
                    "LoopA",
                    "LoopB",
                    "LoopRoot"
                },
                new[]
                {
                    "LoopB",
                    "LoopLeaf",
                    "LoopRoot",
                    "LoopA"
                },
                new[]
                {
                    "LoopA",
                    "LoopRoot",
                    "LoopLeaf",
                    "LoopB"
                }
            }

            )
            {
                // Act
                var actual = Names(GetRelationships(given)).ToList();
                // Assert
                Assert.AreEqual(0,
                    actual.IndexOf("LoopRoot"));
                Assert.AreEqual(3,
                    actual.IndexOf("LoopLeaf"));
            }
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of cycle of three tables.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfCycleOfThreeTables()
        {
            // Act
            Helper.CopyAllToTarget("RingY",
                "RingZ",
                "RingX");
            // Assert
            Helper.AssertTargetMatchesSource("RingX");
            Helper.AssertTargetMatchesSource("RingY");
            Helper.AssertTargetMatchesSource("RingZ");
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of cycle with the tables that it depends on and that depend on it.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            Helper.CopyAllToTarget("LoopLeaf",
                "LoopB",
                "LoopA",
                "LoopRoot");
            // Assert
            foreach (var table in new[]
            {
                "LoopRoot",
                "LoopA",
                "LoopB",
                "LoopLeaf"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion

        #region Many children

        /// <summary>
        /// Verifies vertica schema relationship of table with many children.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfTableWithManyChildren()
        {
            // Act
            var actual = GetRelationships("FanChild3",
                "FanChild1",
                "FanRoot",
                "FanChild4",
                "FanChild2");
            // Assert
            Assert.AreEqual("FanRoot",
                actual[0].Schema.Table.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "FanChild3", "FanChild1", "FanChild4", "FanChild2" },
                Names(Get(actual,
                        "FanRoot").Children));
            foreach (var child in actual.Skip(1))
            {
                CollectionAssert.AreEqual(new[] { "FanRoot" },
                    Names(child.Parents));
            }
        }

        #endregion

        #region Same table name in different schemas

        /// <summary>
        /// Verifies vertica schema relationship of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelationships($"{Database.SourceSalesName}.ItemRef",
                "Item",
                $"{Database.SourceSalesName}.Item");
            // Assert
            var reference = actual.Single(r => r.Schema.Table.Name == "ItemRef");
            var sales = actual.Single(r => r.Schema.Table.Name == "Item" && r.Schema.Table.Schema == Database.SourceSalesName);
            var dbo = actual.Single(r => r.Schema.Table.Name == "Item" && r.Schema.Table.Schema == null);
            Assert.AreSame(sales,
                reference.Parents.Single());
            Assert.AreSame(reference,
                sales.Children.Single());
            Assert.AreEqual(0,
                dbo.Children.Count);
            Assert.AreEqual(0,
                dbo.Parents.Count);
            Assert.IsTrue(actual.IndexOf(sales) < actual.IndexOf(reference));
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of tables with the same name in different schemas.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            Helper.CopyAllToTarget($"{Database.SourceSalesName}.ItemRef",
                "Item",
                $"{Database.SourceSalesName}.Item");
            // Assert
            Helper.AssertTargetMatchesSource("Item");
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.Item");
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.ItemRef");
        }

        #endregion

        #region Names that need quoting

        /// <summary>
        /// Verifies vertica schema reader reads the table with aspace in its name.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderReadsTheTableWithASpaceInItsName()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "\"Order Details\"",
                    transaction: null);
                // Assert
                Assert.AreEqual("Order Details",
                    actual.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("Unit Price",
                    actual.Columns.Single(c => c.Field.Name == "Unit Price").Field.Name,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies vertica schema reader reads the table with abracket in its name.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderReadsTheTableWithABracketInItsName()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = reader.GetTableSchema(connection,
                    "\"Weird]Name\"",
                    transaction: null);
                // Assert
                Assert.AreEqual("Weird]Name",
                    actual.Table.Name,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies vertica schema reader get tables quotes the names that need it.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderGetTablesQuotesTheNamesThatNeedIt()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = reader.GetTables(connection,
                    transaction: null).ToList();
                // Assert
                CollectionAssert.Contains(actual,
                    "Person");
                CollectionAssert.Contains(actual,
                    "\"Odd.Name\"");
                CollectionAssert.Contains(actual,
                    "\"Order Details\"");
                CollectionAssert.Contains(actual,
                    "\"Weird]Name\"");
            }
        }

        /// <summary>
        /// Verifies vertica schema relationship of tables with adot in their names.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfTablesWithADotInTheirNames()
        {
            // Act
            var actual = GetRelationships("\"Odd.Child\"",
                "\"Odd.Name\"");
            // Assert
            CollectionAssert.AreEqual(new[] { "\"Odd.Name\"", "\"Odd.Child\"" },
                Helper.GetTableNames(actual));
            Assert.AreSame(actual[0],
                actual[1].Parents.Single());
            Assert.AreEqual(new TableInfo("Odd.Name",
                    null),
                actual[1].Schema.ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of tables with names that need quoting.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesWithNamesThatNeedQuoting()
        {
            // Act
            Helper.CopyAllToTarget("\"Odd.Child\"",
                "\"Odd.Name\"",
                "\"Order Details\"",
                "\"Weird]Name\"");
            // Assert
            Helper.AssertTargetMatchesSource("\"Odd.Name\"");
            Helper.AssertTargetMatchesSource("\"Odd.Child\"");
            Helper.AssertTargetMatchesSource("\"Order Details\"");
            Helper.AssertTargetMatchesSource("\"Weird]Name\"");
        }

        #endregion

        #region Limits

        /// <summary>
        /// Verifies vertica schema relationship only considers the direct references between the given tables.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOnlyConsidersTheDirectReferencesBetweenTheGivenTables()
        {
            // Act
            var actual = GetRelationships("GrandChild",
                "Parent");
            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" },
                Names(actual));
            Assert.AreEqual(0,
                actual[0].Parents.Count);
            Assert.AreEqual(0,
                actual[1].Children.Count);
        }

        /// <summary>
        /// Verifies vertica schema relationship keeps the given order of the tables that are not related.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipKeepsTheGivenOrderOfTheTablesThatAreNotRelated()
        {
            // Act
            var actual = GetRelationships("NoKey",
                "Parent",
                "Country",
                "OrderLine");
            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey", "Parent", "Country", "OrderLine" },
                Names(actual));
        }

        /// <summary>
        /// Verifies vertica schema relationship of table that references atable that is not given.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipOfTableThatReferencesATableThatIsNotGiven()
        {
            // Act
            var actual = GetRelationships("Shipment");
            // Assert
            Assert.AreEqual(2,
                actual[0].Schema.ForeignKeys.Count);
            Assert.AreEqual(0,
                actual[0].Parents.Count);
        }

        #endregion

        #region Whole database

        /// <summary>
        /// Verifies vertica schema relationships of the whole database are consistent.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipsOfTheWholeDatabaseAreConsistent()
        {
            // Setup
            var tables = GetSourceTables();
            // Act
            var actual = GetRelationships(tables.ToArray());
            // Assert
            Assert.AreEqual(tables.Count,
                actual.Count);
            foreach (var relationship in actual)
            {
                Assert.AreEqual(relationship.Parents.Count,
                    relationship.Parents.Distinct().Count());
                Assert.AreEqual(relationship.Children.Count,
                    relationship.Children.Distinct().Count());
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

        /// <summary>
        /// Verifies vertica schema relationships of the whole database cover the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipsOfTheWholeDatabaseCoverTheForeignKeys()
        {
            // Setup
            var tables = GetSourceTables();
            // Act
            var actual = GetRelationships(tables.ToArray());
            // Assert
            foreach (var relationship in actual)
            {
                var self = Helper.FormatName(relationship.Schema.Table.Schema,
                    relationship.Schema.Table.Name);
                var referenced = relationship.Schema.ForeignKeys.Select(fk => Helper.FormatName(fk.ReferencedTable.Schema,
                        fk.ReferencedTable.Name)).Where(t => !string.Equals(t,
                        self,
                        StringComparison.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(t => t,
                    StringComparer.OrdinalIgnoreCase).ToArray();
                var parents = Helper.GetTableNames(relationship.Parents).OrderBy(t => t,
                    StringComparer.OrdinalIgnoreCase).ToArray();
                CollectionAssert.AreEqual(referenced,
                    parents,
                    $"Parents of '{self}'.");
            }
        }

        /// <summary>
        /// Verifies vertica schema relationships of the whole database are ordered in any given order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipsOfTheWholeDatabaseAreOrderedInAnyGivenOrder()
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
                Assert.AreEqual(tables.Count,
                    order.Count);
                foreach (var relationship in actual)
                {
                    var name=Helper.FormatName(relationship.Schema.Table.Schema,
                        relationship.Schema.Table.Name);
                    foreach (var parent in relationship.Parents)
                    {
                        var parentName = Helper.FormatName(parent.Schema.Table.Schema,
                            parent.Schema.Table.Name);
                        if (CyclicTables.Contains(name,
                            StringComparer.OrdinalIgnoreCase) && CyclicTables.Contains(parentName,
                            StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        Assert.IsTrue(order.IndexOf(parentName) < order.IndexOf(name),
                            $"'{parentName}' must come before '{name}'.");
                    }
                }
            }
        }

        /// <summary>
        /// Verifies vertica schema relationships of the whole database keep the given order inside the cycles.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaRelationshipsOfTheWholeDatabaseKeepTheGivenOrderInsideTheCycles()
        {
            // Setup
            var tables = GetSourceTables();
            // Act
            var order = Helper.GetTableNames(GetRelationships(tables.AsEnumerable().Reverse().ToArray())).ToList();
            // Assert
            foreach (var cycle in new[]
            {
                new[]
                {
                    "RingZ",
                    "RingY",
                    "RingX"
                },
                new[]
                {
                    "CycleB",
                    "CycleA"
                }
            }

            )
            {
                var positions = cycle.Select(t => order.IndexOf(t)).ToArray();
                CollectionAssert.AreEqual(positions.OrderBy(p => p).ToArray(),
                    positions);
            }
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of the whole database in the reverse order.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTheWholeDatabaseInTheReverseOrder()
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

        /// <summary>
        /// Verifies vertica schema relationships async of the whole database are the same as the sync ones.
        /// </summary>
        [TestMethod]
        public async Task TestVerticaSchemaRelationshipsAsyncOfTheWholeDatabaseAreTheSameAsTheSyncOnes()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                var reader = new VerticaSchemaReader();
                // Act
                var expected = reader.GetDependencyOrder(connection,
                    tables,
                    transaction: null).ToList();
                var actual = (await reader.GetDependencyOrderAsync(connection,
                    tables,
                    transaction: null)).ToList();
                // Assert
                CollectionAssert.AreEqual(Helper.GetTableNames(expected),
                    Helper.GetTableNames(actual));
                for (var i = 0; i < expected.Count; i++)
                {
                    CollectionAssert.AreEqual(Helper.GetTableNames(expected[i].Parents),
                        Helper.GetTableNames(actual[i].Parents));
                    CollectionAssert.AreEqual(Helper.GetTableNames(expected[i].Children),
                        Helper.GetTableNames(actual[i].Children));
                }
            }
        }
        #endregion
    }
}
