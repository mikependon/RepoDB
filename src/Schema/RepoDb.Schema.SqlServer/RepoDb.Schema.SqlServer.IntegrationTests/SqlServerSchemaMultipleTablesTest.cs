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

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaMultipleTablesTest
    {
        // The tables that reference each other, so no creation order exists for them
        private static readonly string[] CircularTables =
        {
            "dbo.CycleA", "dbo.CycleB", "dbo.RingX", "dbo.RingY", "dbo.RingZ", "dbo.LoopA", "dbo.LoopB", "dbo.LoopLeaf"
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        private static List<string> GetTargetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        #endregion

        #region Dependency chain

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainInAnyOrder()
        {
            // Act
            Helper.CopyAllToTarget("GrandChild", "Parent", "Child");

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainInTheDependencyOrder()
        {
            // Setup
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SqlServerSchemaReader(connection).GetDependencyOrder(new[] { "GrandChild", "Parent", "Child" }));

                // Act (each table is created on its own, in the order that the reader gave)
                Helper.CopyToTarget(order);
            }

            // Assert
            Helper.AssertTargetMatchesSource("Parent");
            Helper.AssertTargetMatchesSource("Child");
            Helper.AssertTargetMatchesSource("GrandChild");
        }

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfDependencyChainKeepsTheForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("Parent", "Child", "GrandChild");

            // Assert
            Assert.AreEqual("dbo.Parent", Helper.GetTargetSchema("Child").ForeignKeys.Single().ReferencedTable, StringComparer.Ordinal);
            Assert.AreEqual("dbo.Child", Helper.GetTargetSchema("GrandChild").ForeignKeys.Single().ReferencedTable, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<SqlException>(() => Helper.CopyAllToTarget("Shipment"));
        }

        #endregion

        #region Circular references

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesWithCircularReferences()
        {
            // Act
            Helper.CopyAllToTarget("CycleA", "CycleB");

            // Assert
            Helper.AssertTargetMatchesSource("CycleA");
            Helper.AssertTargetMatchesSource("CycleB");
        }

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesWithCircularReferencesKeepsBothForeignKeys()
        {
            // Act
            Helper.CopyAllToTarget("CycleB", "CycleA");

            // Assert
            Assert.AreEqual("dbo.CycleB", Helper.GetTargetSchema("CycleA").ForeignKeys.Single().ReferencedTable, StringComparer.Ordinal);
            Assert.AreEqual("dbo.CycleA", Helper.GetTargetSchema("CycleB").ForeignKeys.Single().ReferencedTable, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnTablesWithCircularReferencesIfTheyAreCreatedOneByOne()
        {
            // Act/Assert (a table is created together with its foreign keys, so the first one references a table that does not exist yet)
            Assert.Throws<SqlException>(() => Helper.CopyToTarget("CycleA", "CycleB"));
        }

        #endregion

        #region Statements

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesStatementsAreOrderedByKind()
        {
            // Setup
            var schemas = Helper.GetSourceSchemas(GetSourceTables());

            // Act
            var statements = new SqlServerSchemaComposer().ComposeSchemas(schemas).ToList();

            // Assert (all the tables, then all the indexes, then all the foreign keys)
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
        public void TestSqlServerSchemaMultipleTablesOfTheWholeDatabase()
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
                var reader = new SqlServerSchemaReader(connection);
                var copied = tables.Select(reader.GetTableSchema).ToList();
                Assert.AreEqual(sources.Sum(s => s.Columns.Count), copied.Sum(s => s.Columns.Count));
                Assert.AreEqual(sources.Sum(s => s.Indexes.Count), copied.Sum(s => s.Indexes.Count));
                Assert.AreEqual(sources.Sum(s => s.ForeignKeys.Count), copied.Sum(s => s.ForeignKeys.Count));
                Assert.AreEqual(sources.Sum(s => s.UniqueConstraints.Count), copied.Sum(s => s.UniqueConstraints.Count));
                Assert.AreEqual(sources.Sum(s => s.CheckConstraints.Count), copied.Sum(s => s.CheckConstraints.Count));
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesOfTheWholeDatabaseInTheDependencyOrder()
        {
            // Setup
            var tables = GetSourceTables();
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                var order = Helper.GetTableNames(new SqlServerSchemaReader(connection).GetDependencyOrder(tables))
                    .Where(t => !CircularTables.Contains(t, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                // Act (the tables with circular references cannot be created on their own, so they are left out)
                Helper.CopyToTarget(order);

                // Assert
                foreach (var table in order)
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaMultipleTablesDependencyOrderPutsTheReferencedTablesFirst()
        {
            // Setup
            var tables = GetSourceTables();
            var schemas = Helper.GetSourceSchemas(tables).ToDictionary(s => Helper.FormatName(s.Table.Schema, s.Table.Name), StringComparer.OrdinalIgnoreCase);

            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act
                var order = Helper.GetTableNames(new SqlServerSchemaReader(connection).GetDependencyOrder(tables)).ToList();

                // Assert (except the tables that reference each other and themselves)
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
                        if (string.Equals(foreignKey.ReferencedTable, name, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        Assert.IsTrue(
                            order.FindIndex(t => string.Equals(t, foreignKey.ReferencedTable, StringComparison.OrdinalIgnoreCase)) <
                            order.FindIndex(t => string.Equals(t, name, StringComparison.OrdinalIgnoreCase)),
                            $"'{foreignKey.ReferencedTable}' must come before '{name}'.");
                    }
                }
            }
        }

        #endregion
    }
}
