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
using Npgsql;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(NpgsqlConnection source) =>
            SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

        private static List<string> GetTargetTables()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).GetTables().ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country", "Person" }, target, "sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.SourceSchema == "public" && r.DestinationSchema == "sales"));
                var person = GetTargetSchema("sales.\"Person\"");
                Assert.AreEqual(new TableInfo("country", "sales"), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "sales.order_line", "Person", "country" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "public.order_line", "public.\"Person\"", "public.country" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == "public"));
                Assert.AreEqual("sales", results.Single(r => r.TableName == "order_line").SourceSchema);
                Assert.AreEqual(new TableInfo("Person", "public"), GetTargetSchema("public.order_line").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("country", target, "sales");

                // Assert
                Assert.AreEqual("public", result.SourceSchema);
                Assert.AreEqual("sales", result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { "sales.country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country" }, target, "sales");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "country" }, target, "sales", createdCallback: results.Add);
                source.CopySchemaTo(new[] { "country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { "sales.country", "public.country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlCopySchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopySchemaTo(new[] { "country" }, target, "missing"));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "country", "Person" }, target, "sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == "sales"));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "country", "Person" }, target);

                // Act
                var result = await source.CopySchemaToAsync("sales.order_line", target);

                // Assert
                Assert.AreEqual("sales", result.SourceSchema);
                Assert.AreEqual("public", result.DestinationSchema);
                Assert.IsTrue(GetTargetTables().Contains("public.order_line"));
            }
        }

        #endregion
    }
}
