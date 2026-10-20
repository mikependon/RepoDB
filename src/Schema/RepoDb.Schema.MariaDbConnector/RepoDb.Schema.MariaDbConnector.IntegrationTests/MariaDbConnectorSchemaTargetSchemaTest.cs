#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.MariaDbConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.MariaDbConnector.IntegrationTests
{
    [TestClass]
    public class MariaDbConnectorSchemaTargetSchemaTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Helpers

        private static void MapSchemaReaderConnection(MariaDbConnection source) =>
            SchemaReaderMapper.Add<MariaDbConnection>(new MariaDbConnectorSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                return new MariaDbConnectorSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                return new MariaDbConnectorSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" }, GetTargetTables(Database.TargetSalesName));
                Assert.AreEqual(0, GetTargetTables().Count);
                Assert.IsTrue(results.All(r => r.SourceSchema == null && r.DestinationSchema == Database.TargetSalesName));
                var person = GetTargetSchema($"{Database.TargetSalesName}.Person");
                Assert.AreEqual(new TableInfo("Country", Database.TargetSalesName), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country", "OrderLine" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == null));
                Assert.IsTrue(results.All(r => r.SourceSchema == null));
                Assert.AreEqual(new TableInfo("Country", null), GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target, Database.TargetSalesName);

                // Assert
                Assert.IsNull(result.SourceSchema);
                Assert.AreEqual(Database.TargetSalesName, result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.TargetSalesName);
                var results = new List<CopyTableSchemaResult>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.TargetSalesName, createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopyTableSchemaOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
                CollectionAssert.AreEquivalent(new[] { "Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<MariaDbException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResult>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" }, GetTargetTables(Database.TargetSalesName));
                Assert.IsTrue(results.All(r => r.DestinationSchema == Database.TargetSalesName));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = await source.CopyTableSchemaToAsync("Country", target);

                // Assert
                Assert.IsNull(result.SourceSchema);
                Assert.IsNull(result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { "Country" }, GetTargetTables());
            }
        }

        #endregion
    }
}
