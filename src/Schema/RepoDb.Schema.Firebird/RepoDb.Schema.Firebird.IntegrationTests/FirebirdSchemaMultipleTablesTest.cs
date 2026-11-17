#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Firebird.IntegrationTests
{
    /// <summary>
    /// Contains the firebird schema multiple tables test behavior.
    /// </summary>
    [TestClass]
    public class FirebirdSchemaMultipleTablesTest
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
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                var reader = new FirebirdSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = new FbConnection(Database.ConnectionStringForTarget))
            {
                var reader = new FirebirdSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).ToList();
            }
        }

        #endregion

        #region Dependency chain

        /// <summary>
        /// Verifies firebird schema multiple tables of dependency chain in any order.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfDependencyChainInAnyOrder()
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
        /// Verifies firebird schema multiple tables of dependency chain in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new FirebirdSchemaReader().GetDependencyOrder(connection,
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
        /// Verifies firebird schema multiple tables of dependency chain keeps the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
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
        /// Verifies throw exception on multiple tables if the referenced table is not part of them.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<FbException>(() => Helper.CopyAllToTarget("Shipment"));
        }

        #endregion

        #region Circular references

        /// <summary>
        /// Verifies firebird schema multiple tables with circular references.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA",
                "CycleB");
            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        /// <summary>
        /// Verifies firebird schema multiple tables with circular references keeps both foreign keys.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
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
        /// Verifies throw exception on tables with circular references if they are created one by one.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert
            Assert.Throws<FbException>(() => Helper.CopyToTarget("CycleA",
                    "CycleB"));
        }

        #endregion

        #region Statements

        /// <summary>
        /// Verifies firebird schema multiple tables statements are ordered by kind.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());
            // Act
            var statements = new FirebirdSchemaComposer().ComposeSchemas(schemas).ToList();
            // Assert
            var lastTable = statements.FindLastIndex(s => s.StartsWith("CREATE TABLE",
                    StringComparison.Ordinal));
            var firstIndex = statements.FindIndex(s => s.Contains("INDEX",
                    StringComparison.Ordinal) && s.StartsWith("CREATE",
                    StringComparison.Ordinal));
            var lastIndex = statements.FindLastIndex(s => s.Contains("INDEX",
                    StringComparison.Ordinal) && s.StartsWith("CREATE",
                    StringComparison.Ordinal));
            var firstForeignKey = statements.FindIndex(s => s.StartsWith("ALTER TABLE",
                    StringComparison.Ordinal));
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
                statements.Count(s => s.StartsWith("ALTER TABLE",
                        StringComparison.Ordinal)));
        }

        #endregion

        #region Whole database

        /// <summary>
        /// Verifies firebird schema multiple tables of the whole database.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfTheWholeDatabase()
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
        /// Verifies firebird schema multiple tables of the whole database counts the objects.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
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
        /// Verifies firebird schema multiple tables of the whole database in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new FirebirdSchemaReader().GetDependencyOrder(connection,
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
        /// Verifies firebird schema multiple tables dependency order puts the referenced tables first.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema,
                    s.Table.Name),
                StringComparer.OrdinalIgnoreCase);
            using (var connection = new FbConnection(Database.ConnectionStringForSource))
            {
                // Act
                var order = Helper.GetTableNames(new FirebirdSchemaReader().GetDependencyOrder(connection,
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
