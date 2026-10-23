#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    [TestClass]
    public class CockroachDbSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(CockroachDbConnection source) =>
            SchemaReaderMapper.Add<CockroachDbConnection>(new CockroachDbSchemaReader(source), true);

        private static List<string> GetTargetTables()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                return new CockroachDbSchemaReader(connection).GetTables().ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                return new CockroachDbSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, "sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == "public" && r.Target.Table.Table.Schema == "sales"));
                var person = GetTargetSchema("sales.\"Person\"");
                Assert.AreEqual(new TableInfo("country", "sales"), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "sales.order_line", "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "public.order_line", "public.\"Person\"", "public.country" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "public"));
                Assert.AreEqual("sales", results.Single(r => r.Target.Table.Table.Name == "order_line").Source.Table.Table.Schema);
                Assert.AreEqual(new TableInfo("Person", "public"), GetTargetSchema("public.order_line").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("country", target, "sales").Items.Single();

                // Assert
                Assert.AreEqual("public", result.Source.Table.Table.Schema);
                Assert.AreEqual("sales", result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { "sales.country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestCockroachDbCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" }, target, "sales");
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "country" }, target, "sales", createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(TableOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { "sales.country", "public.country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<CockroachDbException>(() => source.CopyTableSchemaTo(new[] { "country" }, target, "missing"));
            }
        }

        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" }, target, "sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "sales"));
            }
        }

        [TestMethod]
        public async Task TestCockroachDbCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target);

                // Act
                var result = (await source.CopyTableSchemaToAsync("sales.order_line", target)).Items.Single();

                // Assert
                Assert.AreEqual("sales", result.Source.Table.Table.Schema);
                Assert.AreEqual("public", result.Target.Table.Table.Schema);
                Assert.IsTrue(GetTargetTables().Contains("public.order_line"));
            }
        }

        #endregion
    }
}
