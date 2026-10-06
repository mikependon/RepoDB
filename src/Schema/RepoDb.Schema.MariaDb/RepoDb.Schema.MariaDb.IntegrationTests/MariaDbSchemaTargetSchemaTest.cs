#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.MariaDb.IntegrationTests.Setup;

namespace RepoDb.Schema.MariaDb.IntegrationTests
{
    [TestClass]
    public class MariaDbSchemaTargetSchemaTest
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
            SchemaReaderMapper.Add<MariaDbConnection>(new MariaDbSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                return new MariaDbSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                return new MariaDbSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopySchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

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
        public void TestCopySchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Person", "Country", "OrderLine" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == null));
                Assert.IsTrue(results.All(r => r.SourceSchema == null));
                Assert.AreEqual(new TableInfo("Country", null), GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("Country", target, Database.TargetSalesName);

                // Assert
                Assert.IsNull(result.SourceSchema);
                Assert.AreEqual(Database.TargetSalesName, result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "Country" }, target, Database.TargetSalesName);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country" }, target, Database.TargetSalesName, createdCallback: results.Add);
                source.CopySchemaTo(new[] { "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
                CollectionAssert.AreEquivalent(new[] { "Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<MariaDbException>(() => source.CopySchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" }, GetTargetTables(Database.TargetSalesName));
                Assert.IsTrue(results.All(r => r.DestinationSchema == Database.TargetSalesName));
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = await source.CopySchemaToAsync("Country", target);

                // Assert
                Assert.IsNull(result.SourceSchema);
                Assert.IsNull(result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { "Country" }, GetTargetTables());
            }
        }

        #endregion
    }
}
