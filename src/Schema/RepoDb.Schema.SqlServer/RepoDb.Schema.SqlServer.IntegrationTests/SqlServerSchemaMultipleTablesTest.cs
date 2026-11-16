#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    /// <summary>
    /// Contains the sql server schema multiple tables test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaMultipleTablesTest
    {
        private static readonly string[] CircularTables =
        {
            "dbo.CycleA",
            "dbo.CycleB",
            "dbo.RingX",
            "dbo.RingY",
            "dbo.RingZ",
            "dbo.LoopA",
            "dbo.LoopB",
            "dbo.LoopLeaf"
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader().GetTables(connection,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader().GetTables(connection,
                    transaction: null).ToList();
            }
        }

        #endregion

        #region Dependency chain

        /// <summary>
        /// Verifies sql server schema multiple tables of dependency chain in any order.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainInAnyOrder()
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
        /// Verifies sql server schema multiple tables of dependency chain in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SqlServerSchemaReader().GetDependencyOrder(connection,
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
        /// Verifies sql server schema multiple tables of dependency chain keeps the foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("Parent",
                "Child",
                "GrandChild");
            // Assert
            Assert.AreEqual(new TableInfo("Parent",
                    "dbo"),
                Helper.GetTargetSchema("Child").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("Child",
                    "dbo"),
                Helper.GetTargetSchema("GrandChild").ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies throw exception on multiple tables if the referenced table is not part of them.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<SqlException>(() => Helper.CopyAllToTarget("Shipment"));
        }

        #endregion

        #region Circular references

        /// <summary>
        /// Verifies sql server schema multiple tables with circular references.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA",
                "CycleB");
            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        /// <summary>
        /// Verifies sql server schema multiple tables with circular references keeps both foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("CycleB",
                "CycleA");
            // Assert
            Assert.AreEqual(new TableInfo("CycleB",
                    "dbo"),
                Helper.GetTargetSchema("CycleA").ForeignKeys.Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("CycleA",
                    "dbo"),
                Helper.GetTargetSchema("CycleB").ForeignKeys.Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies throw exception on tables with circular references if they are created one by one.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert
            Assert.Throws<SqlException>(() => Helper.CopyToTarget("CycleA",
                    "CycleB"));
        }

        #endregion

        #region Statements

        /// <summary>
        /// Verifies sql server schema multiple tables statements are ordered by kind.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());
            // Act
            var statements = new SqlServerSchemaComposer().ComposeSchemas(schemas).ToList();
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
        /// Verifies sql server schema multiple tables of the whole database.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfTheWholeDatabase()
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
        /// Verifies sql server schema multiple tables of the whole database counts the objects.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
        {
            // Setup
            var tables = GetSourceTables();
            var sources = Helper.GetSourceSchemas(tables);
            // Act
            Helper.CopyAllToTarget(tables.ToArray());
            // Assert
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                var reader = new SqlServerSchemaReader();
                var copied = tables.Select(value => reader.GetTableSchema(connection,
                        value,
                        transaction: null)).ToList();
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
        }

        /// <summary>
        /// Verifies sql server schema multiple tables of the whole database in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SqlServerSchemaReader().GetDependencyOrder(connection,
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
        /// Verifies sql server schema multiple tables dependency order puts the referenced tables first.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema,
                    s.Table.Name),
                StringComparer.OrdinalIgnoreCase);
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act
                var order = Helper.GetTableNames(new SqlServerSchemaReader().GetDependencyOrder(connection,
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
