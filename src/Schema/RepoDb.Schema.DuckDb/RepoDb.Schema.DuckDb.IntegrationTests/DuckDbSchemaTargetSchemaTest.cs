#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.DuckDb.IntegrationTests.Setup;

namespace RepoDb.Schema.DuckDb.IntegrationTests
{
    [TestClass]
    public class DuckDbSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(DuckDBConnection source) =>
            SchemaReaderMapper.Add<DuckDBConnection>(new DuckDbSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                return new DuckDbSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                return new DuckDbSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" }, GetTargetTables(Database.TargetSalesName));
                Assert.AreEqual(0, GetTargetTables().Count);
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null && r.Target.Table.Table.Schema == Database.TargetSalesName));
                var person = GetTargetSchema($"{Database.TargetSalesName}.Person");
                Assert.AreEqual(new TableInfo("Country", Database.TargetSalesName), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country", "OrderLine" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "main"));
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null));
                Assert.AreEqual(new TableInfo("Country", null), GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target, Database.TargetSalesName).Items.Single();

                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.AreEqual(Database.TargetSalesName, result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.TargetSalesName);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.TargetSalesName, createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(TableOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country" }, GetTargetTables(Database.TargetSalesName));
                CollectionAssert.AreEquivalent(new[] { "Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<DuckDBException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, Database.TargetSalesName, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.TargetSalesName}.Country", $"{Database.TargetSalesName}.Person" }, GetTargetTables(Database.TargetSalesName));
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == Database.TargetSalesName));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Country", target)).Items.Single();

                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.AreEqual("main", result.Target.Table.Table.Schema, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Country" }, GetTargetTables());
            }
        }

        #endregion
    }
}
