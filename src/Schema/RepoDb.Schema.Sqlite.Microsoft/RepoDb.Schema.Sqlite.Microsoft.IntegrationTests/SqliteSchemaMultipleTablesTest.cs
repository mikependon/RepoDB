#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Sqlite.Microsoft.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    /// <summary>
    /// Contains the sqlite schema multiple tables test behavior.
    /// </summary>
    [TestClass]
    public class SqliteSchemaMultipleTablesTest
    {
        private static readonly string[] CircularTables =
        {
            "CycleA",
            "CycleB",
            "RingX",
            "RingY",
            "RingZ",
            "LoopA",
            "LoopB",
            "LoopLeaf"
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
        /// Performs get source tables.
        /// </summary>
        private static List<string> GetSourceTables()
        {
            using (var connection = Database.CreateSource())
            {
                var reader = new SqliteSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Concat(reader.GetTables(connection,
                        Database.SalesSchema,
                        transaction: null)).ToList();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = Database.CreateTarget())
            {
                var reader = new SqliteSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Concat(reader.GetTables(connection,
                        Database.SalesSchema,
                        transaction: null).Select(t => Database.SalesSchema + t.Substring(Database.SalesSchema.Length))).ToList();
            }
        }

        #endregion

        #region Dependency chain

        /// <summary>
        /// Verifies sqlite schema multiple tables of dependency chain in any order.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfDependencyChainInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("GrandChild",
                "Parent",
                "Child");
            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables of dependency chain in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = Database.CreateSource())
            {
                var order = Helper.GetTableNames(new SqliteSchemaReader().GetDependencyOrder(connection,
                        new[] { "GrandChild", "Parent", "Child" },
                        transaction: null));
                // Act
                Helper.CopyToTarget(order);
            }

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables of dependency chain keeps the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("Parent",
                "Child",
                "GrandChild");
            // Assert
            Assert.AreEqual(new TableInfo("Parent",
                    null),
                Helper.GetTargetSchema("Child").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("Child",
                    null),
                Helper.GetTargetSchema("GrandChild").ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables creates the table even if the referenced table is not part of them.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesCreatesTheTableEvenIfTheReferencedTableIsNotPartOfThem()
        {
            // Act
            Helper.CopyAllToTarget("Shipment");
            // Assert
            Assert.IsTrue(Helper.TargetTableExists("Shipment"));
        }

        #endregion

        #region Circular references

        /// <summary>
        /// Verifies sqlite schema multiple tables with circular references.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA",
                "CycleB");
            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables with circular references keeps both foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("CycleB",
                "CycleA");
            // Assert
            Assert.AreEqual(new TableInfo("CycleB",
                    null),
                Helper.GetTargetSchema("CycleA").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("CycleA",
                    null),
                Helper.GetTargetSchema("CycleB").ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables with circular references created one by one.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesWithCircularReferencesCreatedOneByOne()
        {
            // Act
            Helper.CopyToTarget("CycleA",
                "CycleB");
            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        #endregion

        #region Statements

        /// <summary>
        /// Verifies sqlite schema multiple tables statements are ordered by kind.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());
            // Act
            var statements = new SqliteSchemaComposer().ComposeSchemas(schemas).ToList();
            // Assert
            var lastTable = statements.FindLastIndex(s => s.StartsWith("CREATE TABLE",
                    StringComparison.Ordinal));
            var firstIndex = statements.FindIndex(s => s.Contains("INDEX",
                    StringComparison.Ordinal) && s.StartsWith("CREATE",
                    StringComparison.Ordinal));
            var lastIndex = statements.FindLastIndex(s => s.Contains("INDEX",
                    StringComparison.Ordinal) && s.StartsWith("CREATE",
                    StringComparison.Ordinal));
            var firstForeignKey = statements.FindIndex(string.IsNullOrEmpty);
            Assert.IsTrue(lastTable < firstIndex);
            Assert.IsTrue(lastIndex < firstForeignKey);
            Assert.AreEqual(schemas.Count,
                statements.Count(s => s.StartsWith("CREATE TABLE",
                        StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.Indexes.Count),
                statements.Count(s => s.StartsWith("CREATE",
                        StringComparison.Ordinal) && s.Contains("INDEX",
                        StringComparison.Ordinal)));
            Assert.AreEqual(schemas.Sum(s => s.ForeignKeys.Count),
                statements.Count(string.IsNullOrEmpty));
        }

        #endregion

        #region Whole database

        /// <summary>
        /// Verifies sqlite schema multiple tables of the whole database.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfTheWholeDatabase()
        {
            // Setup
            var tables = GetSourceTables();
            // Act
            Helper.CopyAllToTarget(tables.ToArray());
            // Assert
            CollectionAssert.AreEquivalent(tables,
                GetTargetTables());
            foreach (var table in tables)
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables of the whole database counts the objects.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
        {
            // Setup
            var tables = GetSourceTables();
            var sources = Helper.GetSourceSchemas(tables);
            // Act
            Helper.CopyAllToTarget(tables.ToArray());
            // Assert
            var copied = tables.Select(Helper.GetTargetSchema).ToList();
            Assert.AreEqual(sources.Sum(s => s.Columns.Count),
                copied.Sum(s => s.Columns.Count));
            Assert.AreEqual(sources.Sum(s => s.Indexes.Count),
                copied.Sum(s => s.Indexes.Count));
            Assert.AreEqual(sources.Sum(s => s.ForeignKeys.Count),
                copied.Sum(s => s.ForeignKeys.Count));
            Assert.AreEqual(sources.Sum(s => s.UniqueConstraints.Count),
                copied.Sum(s => s.UniqueConstraints.Count));
            Assert.AreEqual(sources.Sum(s => s.CheckConstraints.Count),
                copied.Sum(s => s.CheckConstraints.Count));
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables of the whole database in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = Database.CreateSource())
            {
                var order = Helper.GetTableNames(new SqliteSchemaReader().GetDependencyOrder(connection,
                        tables,
                        transaction: null)).Where(t => !CircularTables.Contains(t,
                        StringComparer.OrdinalIgnoreCase)).ToArray();
                // Act
                Helper.CopyToTarget(order);
                // Assert
                foreach (var table in order)
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        /// <summary>
        /// Verifies sqlite schema multiple tables dependency order puts the referenced tables first.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema,
                    s.Table.Name),
                StringComparer.OrdinalIgnoreCase);
            using (var connection = Database.CreateSource())
            {
                // Act
                var order = Helper.GetTableNames(new SqliteSchemaReader().GetDependencyOrder(connection,
                        tables,
                        transaction: null)).ToList();
                // Assert
                Assert.AreEqual(tables.Count,
                    order.Count);
                foreach (var schema in schemas.Values)
                {
                    var name=Helper.FormatName(schema.Table.Schema,
                        schema.Table.Name);
                    if (CircularTables.Contains(name,
                        StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    foreach (var foreignKey in schema.ForeignKeys)
                    {
                        var referenced = Helper.FormatName(foreignKey.ReferencedTable.Schema,
                            foreignKey.ReferencedTable.Name);
                        if (string.Equals(referenced,
                            name,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        Assert.IsTrue(order.FindIndex(t => string.Equals(t,
                                    referenced,
                                    StringComparison.OrdinalIgnoreCase)) < order.FindIndex(t => string.Equals(t,
                                    name,
                                    StringComparison.OrdinalIgnoreCase)),
                            $"'{referenced}' must come before '{name}'.");
                    }
                }
            }
        }
        #endregion
    }
}
