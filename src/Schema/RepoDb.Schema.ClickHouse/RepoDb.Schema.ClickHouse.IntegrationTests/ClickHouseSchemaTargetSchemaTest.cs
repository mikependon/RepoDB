#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Contains the click house schema target schema test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaTargetSchemaTest
    {
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
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs map schema reader connection.
        /// </summary>
        /// <param name="source">The source.</param>
        private static void MapSchemaReaderConnection(ClickHouseConnection source) => SchemaReaderMapper.Add<ClickHouseConnection>(new ClickHouseSchemaReader(),
            true);

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                return new ClickHouseSchemaReader().GetTables(connection,
                    schemaName,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get target schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                return new ClickHouseSchemaReader().GetTableSchema(connection,
                    tableName,
                    transaction: null);
            }
        }

        #endregion

        #region Target Schema

        /// <summary>
        /// Verifies copy table schemas to with target schema creates the tables in that schema.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    Database.TargetSalesName,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" },
                    GetTargetTables(Database.TargetSalesName));
                Assert.AreEqual(0,
                    GetTargetTables().Count);
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null && r.Target.Table.Table.Schema == Database.TargetSalesName));
                var person = GetTargetSchema($"{Database.TargetSalesName}.Person");
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(),
                    person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schemas to without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country", "OrderLine" },
                    target,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" },
                    GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == null));
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null));
            }
        }

        /// <summary>
        /// Verifies copy table schema to of asingle table with target schema.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var result = source.CopyTableSchemaTo("Country",
                    target,
                    Database.TargetSalesName).Items.Single();
                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.AreEqual(Database.TargetSalesName,
                    result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { $"{Database.TargetSalesName}.Country" },
                    GetTargetTables(Database.TargetSalesName));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to with target schema checks the tables of that schema.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" },
                    target,
                    Database.TargetSalesName);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "Country" },
                    target,
                    Database.TargetSalesName,
                    createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "Country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results[0].Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country" },
                    GetTargetTables(Database.TargetSalesName));
                CollectionAssert.AreEquivalent(new[] { "Country" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the target schema does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<ClickHouseServerException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target,
                        "Missing"));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async with target schema creates the tables in that schema.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" },
                    target,
                    Database.TargetSalesName,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" },
                    GetTargetTables(Database.TargetSalesName));
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == Database.TargetSalesName));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var result = (await source.CopyTableSchemaToAsync("Country",
                    target)).Items.Single();
                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.IsNull(result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { "Country" },
                    GetTargetTables());
            }
        }
        #endregion
    }
}
