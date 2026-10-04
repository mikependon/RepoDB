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
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaRelationshipTest
    {
        // The tables of the source database that are part of a cycle
        private static readonly string[] CyclicTables =
        {
            "dbo.CycleA", "dbo.CycleB", "dbo.RingX", "dbo.RingY", "dbo.RingZ", "dbo.LoopA", "dbo.LoopB"
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetDependencyOrder(tableNames).ToList();
            }
        }

        private static RelationshipInfo Get(IEnumerable<RelationshipInfo> relationships, string tableName) =>
            relationships.Single(r => r.Table.TableName == tableName);

        private static string[] Names(IEnumerable<RelationshipInfo> relationships) =>
            relationships.Select(r => r.Table.TableName).ToArray();

        private static List<string> GetSourceTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        #endregion

        #region Diamond

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfDiamond()
        {
            // Act
            var actual = GetRelationships("DiamondD", "DiamondB", "DiamondC", "DiamondA");

            // Assert (the root comes first and the table that depends on both branches comes last)
            Assert.AreEqual("DiamondA", actual[0].Table.TableName, StringComparer.Ordinal);
            Assert.AreEqual("DiamondD", actual[3].Table.TableName, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "DiamondB", "DiamondC" }, Names(actual.Skip(1).Take(2)));
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfDiamondParentsAndChildren()
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
        public void TestSqlServerSchemaRelationshipOfDiamondInAnyOrder()
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
                Assert.AreEqual("DiamondA", actual[0].Table.TableName, StringComparer.Ordinal);
                Assert.AreEqual("DiamondD", actual[3].Table.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfDiamondInAnyOrder()
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
        public void TestSqlServerSchemaRelationshipOfTableWithTwoForeignKeysToTheSameTable()
        {
            // Act
            var actual = GetRelationships("Transfer", "Account");

            // Assert (the parent is listed once, although the table has 2 foreign keys to it)
            Assert.AreEqual(2, Get(actual, "Transfer").Table.ForeignKeys.Count);
            Assert.AreEqual(1, Get(actual, "Transfer").Parents.Count);
            Assert.AreEqual(1, Get(actual, "Account").Children.Count);
            CollectionAssert.AreEqual(new[] { "Account", "Transfer" }, Names(actual));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithTwoForeignKeysToTheSameTable()
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
        public void TestSqlServerSchemaRelationshipOfCycleOfThreeTables()
        {
            // Act
            var actual = GetRelationships("RingX", "RingY", "RingZ");

            // Assert (the tables are kept in the order that they were given, and each one has a parent and a child)
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
        public void TestSqlServerSchemaRelationshipOfCycleOfThreeTablesKeepsTheGivenOrder()
        {
            // Act
            var actual = GetRelationships("RingZ", "RingX", "RingY");

            // Assert
            CollectionAssert.AreEqual(new[] { "RingZ", "RingX", "RingY" }, Names(actual));
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
        {
            // Act
            var actual = GetRelationships("LoopLeaf", "LoopB", "LoopA", "LoopRoot");

            // Assert (the root comes first, then the cycle in the given order, and the leaf comes last)
            CollectionAssert.AreEqual(new[] { "LoopRoot", "LoopB", "LoopA", "LoopLeaf" }, Names(actual));
            CollectionAssert.AreEqual(new[] { "LoopA" }, Names(Get(actual, "LoopRoot").Children));
            CollectionAssert.AreEqual(new[] { "LoopB" }, Names(Get(actual, "LoopLeaf").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopRoot", "LoopB" }, Names(Get(actual, "LoopA").Parents));
            CollectionAssert.AreEquivalent(new[] { "LoopA", "LoopLeaf" }, Names(Get(actual, "LoopB").Children));
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfCycleWithTheTablesThatItDependsOnAndThatDependOnItInAnyOrder()
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
        public void TestSqlServerSchemaComposerComposedSchemaOfCycleOfThreeTables()
        {
            // Act
            Helper.CopyAllToTarget("RingY", "RingZ", "RingX");

            // Assert
            Helper.AssertTargetMatchesSource("RingX");
            Helper.AssertTargetMatchesSource("RingY");
            Helper.AssertTargetMatchesSource("RingZ");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfCycleWithTheTablesThatItDependsOnAndThatDependOnIt()
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
        public void TestSqlServerSchemaRelationshipOfTableWithManyChildren()
        {
            // Act
            var actual = GetRelationships("FanChild3", "FanChild1", "FanRoot", "FanChild4", "FanChild2");

            // Assert (the children are listed in the order that they were given)
            Assert.AreEqual("FanRoot", actual[0].Table.TableName, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "FanChild3", "FanChild1", "FanChild4", "FanChild2" }, Names(Get(actual, "FanRoot").Children));
            foreach (var child in actual.Skip(1))
            {
                CollectionAssert.AreEqual(new[] { "FanRoot" }, Names(child.Parents));
            }
        }

        #endregion

        #region Same table name in different schemas

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            var actual = GetRelationships("dbo.ItemRef", "dbo.Item", "Sales.Item");

            // Assert (the table references the one of the Sales schema only)
            var reference = actual.Single(r => r.Table.TableName == "ItemRef");
            var sales = actual.Single(r => r.Table.TableName == "Item" && r.Table.SchemaName == "Sales");
            var dbo = actual.Single(r => r.Table.TableName == "Item" && r.Table.SchemaName == "dbo");
            Assert.AreSame(sales, reference.Parents.Single());
            Assert.AreSame(reference, sales.Children.Single());
            Assert.AreEqual(0, dbo.Children.Count);
            Assert.AreEqual(0, dbo.Parents.Count);
            Assert.IsTrue(actual.IndexOf(sales) < actual.IndexOf(reference));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithTheSameNameInDifferentSchemas()
        {
            // Act
            Helper.CopyAllToTarget("dbo.ItemRef", "dbo.Item", "Sales.Item");

            // Assert
            Helper.AssertTargetMatchesSource("dbo.Item");
            Helper.AssertTargetMatchesSource("Sales.Item");
            Helper.AssertTargetMatchesSource("dbo.ItemRef");
        }

        #endregion

        #region Names that need quoting

        [TestMethod]
        public void TestSqlServerSchemaReaderReadsTheTableWithADotInItsName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("[dbo].[Odd.Name]");

                // Assert
                Assert.AreEqual("dbo", actual.SchemaName, StringComparer.Ordinal);
                Assert.AreEqual("Odd.Name", actual.TableName, StringComparer.Ordinal);
                Assert.AreEqual(2, actual.Columns.Count);
                Assert.AreEqual(1, actual.Indexes.Count);
                Assert.IsTrue(reader.TableExists("[dbo].[Odd.Name]"));
                Assert.IsFalse(reader.TableExists("dbo.Odd"));
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderReadsTheTableWithASpaceInItsName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("[Order Details]");

                // Assert
                Assert.AreEqual("Order Details", actual.TableName, StringComparer.Ordinal);
                Assert.AreEqual("Unit Price", actual.Columns.Single(c => c.Field.Name == "Unit Price").Field.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderReadsTheTableWithABracketInItsName()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTableSchema("[dbo].[Weird]]Name]");

                // Assert
                Assert.AreEqual("Weird]Name", actual.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetTablesQuotesTheNamesThatNeedIt()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetTables().ToList();

                // Assert
                CollectionAssert.Contains(actual, "dbo.Person");
                CollectionAssert.Contains(actual, "dbo.[Odd.Name]");
                CollectionAssert.Contains(actual, "dbo.[Order Details]");
                CollectionAssert.Contains(actual, "dbo.[Weird]]Name]");
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfTablesWithADotInTheirNames()
        {
            // Act
            var actual = GetRelationships("[dbo].[Odd.Child]", "[dbo].[Odd.Name]");

            // Assert
            CollectionAssert.AreEqual(new[] { "dbo.[Odd.Name]", "dbo.[Odd.Child]" }, Helper.GetTableNames(actual));
            Assert.AreSame(actual[0], actual[1].Parents.Single());
            Assert.AreEqual("dbo.[Odd.Name]", actual[1].Table.ForeignKeys.Single().ReferencedTable, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithNamesThatNeedQuoting()
        {
            // Act
            Helper.CopyAllToTarget("[dbo].[Odd.Child]", "[dbo].[Odd.Name]", "[Order Details]", "[dbo].[Weird]]Name]");

            // Assert
            Helper.AssertTargetMatchesSource("[dbo].[Odd.Name]");
            Helper.AssertTargetMatchesSource("[dbo].[Odd.Child]");
            Helper.AssertTargetMatchesSource("[Order Details]");
            Helper.AssertTargetMatchesSource("[dbo].[Weird]]Name]");
        }

        #endregion

        #region Limits

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOnlyConsidersTheDirectReferencesBetweenTheGivenTables()
        {
            // Act (the table in the middle of the chain is not given, so nothing relates the 2 tables)
            var actual = GetRelationships("GrandChild", "Parent");

            // Assert
            CollectionAssert.AreEqual(new[] { "GrandChild", "Parent" }, Names(actual));
            Assert.AreEqual(0, actual[0].Parents.Count);
            Assert.AreEqual(0, actual[1].Children.Count);
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipKeepsTheGivenOrderOfTheTablesThatAreNotRelated()
        {
            // Act
            var actual = GetRelationships("NoKey", "Parent", "Country", "OrderLine");

            // Assert
            CollectionAssert.AreEqual(new[] { "NoKey", "Parent", "Country", "OrderLine" }, Names(actual));
        }

        [TestMethod]
        public void TestSqlServerSchemaRelationshipOfTableThatReferencesATableThatIsNotGiven()
        {
            // Act
            var actual = GetRelationships("Shipment");

            // Assert (the foreign keys stay in the schema, but there is no parent to relate to)
            Assert.AreEqual(2, actual[0].Table.ForeignKeys.Count);
            Assert.AreEqual(0, actual[0].Parents.Count);
        }

        #endregion

        #region Whole database

        [TestMethod]
        public void TestSqlServerSchemaRelationshipsOfTheWholeDatabaseAreConsistent()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var actual = GetRelationships(tables.ToArray());

            // Assert
            Assert.AreEqual(tables.Count, actual.Count);
            foreach (var relationship in actual)
            {
                // The parents are listed once, and each parent lists this table as a child (and the other way around)
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
        public void TestSqlServerSchemaRelationshipsOfTheWholeDatabaseCoverTheForeignKeys()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var actual = GetRelationships(tables.ToArray());

            // Assert (every foreign key to another table is a parent, and every parent has a foreign key)
            foreach (var relationship in actual)
            {
                var self = Helper.FormatName(relationship.Table.SchemaName, relationship.Table.TableName);
                var referenced = relationship.Table.ForeignKeys
                    .Select(fk => fk.ReferencedTable)
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
        public void TestSqlServerSchemaRelationshipsOfTheWholeDatabaseAreOrderedInAnyGivenOrder()
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

                // Assert (a table comes after the tables that it references, except for the tables that are part of a cycle)
                Assert.AreEqual(tables.Count, order.Count);
                foreach (var relationship in actual)
                {
                    var name = Helper.FormatName(relationship.Table.SchemaName, relationship.Table.TableName);
                    foreach (var parent in relationship.Parents)
                    {
                        var parentName = Helper.FormatName(parent.Table.SchemaName, parent.Table.TableName);
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
        public void TestSqlServerSchemaRelationshipsOfTheWholeDatabaseKeepTheGivenOrderInsideTheCycles()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            var order = Helper.GetTableNames(GetRelationships(tables.AsEnumerable().Reverse().ToArray())).ToList();

            // Assert (the tables of the same cycle are in the order that they were given, which is the reverse one)
            foreach (var cycle in new[] { new[] { "dbo.RingZ", "dbo.RingY", "dbo.RingX" }, new[] { "dbo.CycleB", "dbo.CycleA" } })
            {
                var positions = cycle.Select(t => order.IndexOf(t)).ToArray();
                CollectionAssert.AreEqual(positions.OrderBy(p => p).ToArray(), positions);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTheWholeDatabaseInTheReverseOrder()
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
        public async Task TestSqlServerSchemaRelationshipsAsyncOfTheWholeDatabaseAreTheSameAsTheSyncOnes()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new SqlServerSchemaReader(connection);

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
