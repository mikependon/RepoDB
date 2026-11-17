#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Contains the click house schema multiple tables test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaMultipleTablesTest
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
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                var reader = new ClickHouseSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Concat(reader.GetTables(connection,
                        Database.SourceSalesName,
                        transaction: null)).ToList();
            }
        }

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                var reader = new ClickHouseSchemaReader();
                return reader.GetTables(connection,
                    transaction: null).Concat(reader.GetTables(connection,
                        Database.TargetSalesName,
                        transaction: null).Select(t => Database.SourceSalesName + t.Substring(Database.TargetSalesName.Length))).ToList();
            }
        }

        #endregion

        #region Dependency chain

        /// <summary>
        /// Verifies throw exception on multiple tables if the referenced table is not part of them.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMultipleTablesIfTheReferencedTableIsNotPartOfThem()
        {
            // Act/Assert
            Assert.Throws<ClickHouseServerException>(() => Helper.CopyAllToTarget("Shipment"));
        }

        #endregion

        #region Circular references

        #endregion

        #region Statements

        #endregion

        #region Whole database

        /// <summary>
        /// Verifies click house schema multiple tables of the whole database.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaMultipleTablesOfTheWholeDatabase()
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
        /// Verifies click house schema multiple tables of the whole database counts the objects.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaMultipleTablesOfTheWholeDatabaseCountsTheObjects()
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
        #endregion
    }
}
