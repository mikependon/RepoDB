#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SqliteSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(SqliteConnection source) =>
            SchemaReaderMapper.Add<SqliteConnection>(new SqliteSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = Database.CreateTarget())
            {
                return new SqliteSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = Database.CreateTarget())
            {
                return new SqliteSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, Database.SalesSchema, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.SalesSchema}.Country", $"{Database.SalesSchema}.Person" }, GetTargetTables(Database.SalesSchema));
                Assert.AreEqual(0, GetTargetTables().Count);
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null && r.Target.Table.Table.Schema == Database.SalesSchema));
                var person = GetTargetSchema($"{Database.SalesSchema}.Person");
                Assert.AreEqual(new TableInfo("Country", Database.SalesSchema), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Person", "Country", "OrderLine" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == null));
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null));
                Assert.AreEqual(new TableInfo("Country", null), GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target, Database.SalesSchema).Items.Single();

                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.AreEqual(Database.SalesSchema, result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { $"{Database.SalesSchema}.Country" }, GetTargetTables(Database.SalesSchema));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.SalesSchema);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "Country" }, target, Database.SalesSchema, createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(TableOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(TableOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { $"{Database.SalesSchema}.Country" }, GetTargetTables(Database.SalesSchema));
                CollectionAssert.AreEquivalent(new[] { "Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<SqliteException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();

                // Act
                await source.CopyTableSchemaToAsync(new[] { "Country", "Person" }, target, Database.SalesSchema, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { $"{Database.SalesSchema}.Country", $"{Database.SalesSchema}.Person" }, GetTargetTables(Database.SalesSchema));
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == Database.SalesSchema));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Country", target)).Items.Single();

                // Assert
                Assert.IsNull(result.Source.Table.Table.Schema);
                Assert.IsNull(result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { "Country" }, GetTargetTables());
            }
        }

        #endregion
    }
}
