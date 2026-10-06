#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.SapHana.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.SapHana.IntegrationTests
{
    [TestClass]
    public class SapHanaSchemaMultipleTablesTest
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
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                var reader = new SapHanaSchemaReader(connection);
                return reader.GetTables().Concat(reader.GetTables(Database.SourceSalesName)).ToList();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new HanaConnection(Database.ConnectionStringForTarget))
            {
                var reader = new SapHanaSchemaReader(connection);
                return reader.GetTables()
                    .Concat(reader.GetTables(Database.TargetSalesName).Select(t => Database.SourceSalesName + t.Substring(Database.TargetSalesName.Length)))
                    .ToList();
            }
        }

        #endregion

        #region Dependency chain

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesOfDependencyChainInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("GrandChild", "Parent", "Child");

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SapHanaSchemaReader(connection).GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }));

                // Act
                Helper.CopyToTarget(order);
            }

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("Parent", "Child", "GrandChild");

            // Assert
            Assert.AreEqual(new TableInfo("Parent", null), Helper.GetTargetSchema("Child").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("Child", null), Helper.GetTargetSchema("GrandChild").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<HanaException>(() => Helper.CopyAllToTarget("Shipment"));
        }

        #endregion

        #region Circular references

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA", "CycleB");

            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("CycleB", "CycleA");

            // Assert
            Assert.AreEqual(new TableInfo("CycleB", null), Helper.GetTargetSchema("CycleA").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("CycleA", null), Helper.GetTargetSchema("CycleB").ForeignKeys.Single().ReferencedTable);
        }

        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert
            Assert.Throws<HanaException>(() => Helper.CopyToTarget("CycleA", "CycleB"));
        }

        #endregion

        #region Statements

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());

            // Act
            var statements = new SapHanaSchemaComposer().ComposeSchemas(schemas).ToList();

            // Assert
            var lastTable = statements.FindLastIndex(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal));
            var firstIndex = statements.FindIndex(s => s.Contains("INDEX", StringComparison.Ordinal) && s.StartsWith("CREATE", StringComparison.Ordinal));
            var lastIndex = statements.FindLastIndex(s => s.Contains("INDEX", StringComparison.Ordinal) && s.StartsWith("CREATE", StringComparison.Ordinal));
            var firstForeignKey = statements.FindIndex(s => s.StartsWith("ALTER TABLE", StringComparison.Ordinal));
            Assert.IsTrue(lastTable < firstIndex);
            Assert.IsTrue(lastIndex < firstForeignKey);
            Assert.AreEqual(schemas.Count, statements.Count(s => s.StartsWith("CREATE TABLE", StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.Indexes.Count), statements.Count(s => s.StartsWith("CREATE", StringComparison.Ordinal) && s.Contains("INDEX", StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count), statements.Count(s => s.StartsWith("ALTER TABLE", StringComparison.Ordinal)));
        }

        #endregion

        #region Whole database

        [TestMethod]
        public void TestSapHanaSchemaMultipleTablesOfTheWholeDatabase()
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
        public void TestSapHanaSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
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
        public void TestSapHanaSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SapHanaSchemaReader(connection).GetDependencyOrder(tables))
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
        public void TestSapHanaSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema, s.Table.Name), StringComparer.OrdinalIgnoreCase);

            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                // Act
                var order = Helper.GetTableNames(new SapHanaSchemaReader(connection).GetDependencyOrder(tables)).ToList();

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
