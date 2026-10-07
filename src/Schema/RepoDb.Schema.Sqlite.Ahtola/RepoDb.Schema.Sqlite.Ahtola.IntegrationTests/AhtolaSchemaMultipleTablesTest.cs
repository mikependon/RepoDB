#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Ahtola.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Ahtola.IntegrationTests
{
    [TestClass]
    public class AhtolaSchemaMultipleTablesTest
    {
        private static readonly string[] CircularTables =
        {
            "CycleA", "CycleB", "RingX", "RingY", "RingZ", "LoopA", "LoopB", "LoopLeaf"
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

        private static List<string> GetSourceTables()
        {
            using (var connection = Database.CreateSource())
            {
                var reader = new AhtolaSchemaReader(connection);
                return reader.GetTables().Concat(reader.GetTables(Database.SalesSchema)).ToList();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = Database.CreateTarget())
            {
                var reader = new AhtolaSchemaReader(connection);
                return reader.GetTables()
                    .Concat(reader.GetTables(Database.SalesSchema).Select(t => Database.SalesSchema + t.Substring(Database.SalesSchema.Length)))
                    .ToList();
            }
        }

        #endregion

        #region Dependency chain

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfDependencyChainInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("GrandChild", "Parent", "Child");

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = Database.CreateSource())
            {
                var order = Helper.GetTableNames(new AhtolaSchemaReader(connection).GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }));

                // Act
                Helper.CopyToTarget(order);
            }

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("Parent", "Child", "GrandChild");

            // Assert
            Assert.AreEqual(new TableInfo("Parent", null), Helper.GetTargetSchema("Child").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("Child", null), Helper.GetTargetSchema("GrandChild").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesCreatesTheTableEvenIfTheReferencedTableIsNotPartOfThem()
        {
            // Act
            Helper.CopyAllToTarget("Shipment");

            // Assert
            Assert.IsTrue(Helper.TargetTableExists("Shipment"));
        }

        #endregion

        #region Circular references

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA", "CycleB");

            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("CycleB", "CycleA");

            // Assert
            Assert.AreEqual(new TableInfo("CycleB", null), Helper.GetTargetSchema("CycleA").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("CycleA", null), Helper.GetTargetSchema("CycleB").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesWithCircularReferencesCreatedOneByOne()
        {
            // Act
            Helper.CopyToTarget("CycleA", "CycleB");

            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        #endregion

        #region Statements

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());

            // Act
            var statements = new AhtolaSchemaComposer().ComposeSchemas(schemas).ToList();

            // Assert
            var lastTable = statements.FindLastIndex(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal));
            var firstIndex = statements.FindIndex(s => s.Contains("INDEX", StringComparison.Ordinal) && s.StartsWith("CREATE", StringComparison.Ordinal));
            var lastIndex = statements.FindLastIndex(s => s.Contains("INDEX", StringComparison.Ordinal) && s.StartsWith("CREATE", StringComparison.Ordinal));
            var firstForeignKey = statements.FindIndex(string.IsNullOrEmpty);
            Assert.IsTrue(lastTable < firstIndex);
            Assert.IsTrue(lastIndex < firstForeignKey);
            Assert.AreEqual(schemas.Count, statements.Count(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.Indexes.Count), statements.Count(s => s.StartsWith("CREATE", StringComparison.Ordinal) && s.Contains("INDEX", StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count), statements.Count(string.IsNullOrEmpty));
        }

        #endregion

        #region Whole database

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfTheWholeDatabase()
        {
            // Setup
            var tables = GetSourceTables();

            // Act
            Helper.CopyAllToTarget(tables.ToArray());

            // Assert
            CollectionAssert.AreEquivalent(tables, GetTargetTables());
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
        {
            // Setup
            var tables = GetSourceTables();
            var sources = Helper.GetSourceSchemas(tables);

            // Act
            Helper.CopyAllToTarget(tables.ToArray());

            // Assert
            var copied = tables.Select(Helper.GetTargetSchema).ToList();
            Assert.AreEqual(sources.Sum(s => s.Columns.Count), copied.Sum(s => s.Columns.Count));
            Assert.AreEqual(sources.Sum(s => s.Indexes.Count), copied.Sum(s => s.Indexes.Count));
            Assert.AreEqual(sources.Sum(s => s.ForeignKeys.Count), copied.Sum(s => s.ForeignKeys.Count));
            Assert.AreEqual(sources.Sum(s => s.UniqueConstraints.Count), copied.Sum(s => s.UniqueConstraints.Count));
            Assert.AreEqual(sources.Sum(s => s.CheckConstraints.Count), copied.Sum(s => s.CheckConstraints.Count));
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = Database.CreateSource())
            {
                var order = Helper.GetTableNames(new AhtolaSchemaReader(connection).GetDependencyOrder(tables))
                    .Where(t => !CircularTables.Contains(t, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                // Act
                Helper.CopyToTarget(order);

                // Assert
                foreach (var table in order)
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestAhtolaSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema, s.Table.Name), StringComparer.OrdinalIgnoreCase);

            using (var connection = Database.CreateSource())
            {
                // Act
                var order = Helper.GetTableNames(new AhtolaSchemaReader(connection).GetDependencyOrder(tables)).ToList();

                // Assert
                Assert.AreEqual(tables.Count, order.Count);
                foreach (var schema in schemas.Values)
                {
                    var name = Helper.FormatName(schema.Table.Schema, schema.Table.Name);
                    if (CircularTables.Contains(name, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    foreach (var foreignKey in schema.ForeignKeys)
                    {
                        var referenced = Helper.FormatName(foreignKey.ReferencedTable.Schema, foreignKey.ReferencedTable.Name);
                        if (string.Equals(referenced, name, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        Assert.IsTrue(
                            order.FindIndex(t => string.Equals(t, referenced, StringComparison.OrdinalIgnoreCase)) <
                            order.FindIndex(t => string.Equals(t, name, StringComparison.OrdinalIgnoreCase)),
                            $"'{referenced}' must come before '{name}'.");
                    }
                }
            }
        }

        #endregion
    }
}
