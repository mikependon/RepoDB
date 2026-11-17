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
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the aurora db postgre sql schema target schema test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaTargetSchemaTest
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
        private static void MapSchemaReaderConnection(AuroraDbConnection source) => SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(),
            true);

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        private static List<string> GetTargetTables()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                return new AuroraDbPostgreSqlSchemaReader().GetTables(connection,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get target schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                return new AuroraDbPostgreSqlSchemaReader().GetTableSchema(connection,
                    tableName,
                    transaction: null);
            }
        }

        #endregion

        #region Target Schema

        /// <summary>
        /// Verifies aurora db postgre sql copy table schemas to with target schema creates the tables in that schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target,
                    "sales",
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" },
                    GetTargetTables());
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == "public" && r.Target.Table.Table.Schema == "sales"));
                var person = GetTargetSchema("sales.\"Person\"");
                Assert.AreEqual(new TableInfo("country",
                        "sales"),
                    person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(),
                    person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schemas to without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "sales.order_line", "Person", "country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "public.order_line", "public.\"Person\"", "public.country" },
                    GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "public"));
                Assert.AreEqual("sales",
                    results.Single(r => r.Target.Table.Table.Name == "order_line").Source.Table.Table.Schema);
                Assert.AreEqual(new TableInfo("Person",
                        "public"),
                    GetTargetSchema("public.order_line").ForeignKeys.Single().ReferencedTable);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schema to of asingle table with target schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var result = source.CopyTableSchemaTo("country",
                    target,
                    "sales").Items.Single();
                // Assert
                Assert.AreEqual("public",
                    result.Source.Table.Table.Schema);
                Assert.AreEqual("sales",
                    result.Target.Table.Table.Schema);
                CollectionAssert.AreEqual(new[] { "sales.country" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schemas to with target schema checks the tables of that schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country" },
                    target,
                    "sales");
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                source.CopyTableSchemaTo(new[] { "country" },
                    target,
                    "sales",
                    createdCallback: results.Add);
                source.CopyTableSchemaTo(new[] { "country" },
                    target,
                    createdCallback: results.Add);
                // Assert
                Assert.AreEqual(TableOutcome.Skipped,
                    results[0].Outcome);
                Assert.AreEqual(TableOutcome.Created,
                    results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { "sales.country", "public.country" },
                    GetTargetTables());
            }
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql copy table schemas to if the target schema does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<AuroraDbException>(() => source.CopyTableSchemaTo(new[] { "country" },
                        target,
                        "missing"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schemas to async with target schema creates the tables in that schema.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopyTableSchemaResultItem>();
                // Act
                await source.CopyTableSchemaToAsync(new[] { "country", "Person" },
                    target,
                    "sales",
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "sales.country", "sales.\"Person\"" },
                    GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == "sales"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql copy table schemas to async without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo(new[] { "country", "Person" },
                    target);
                // Act
                var result = (await source.CopyTableSchemaToAsync("sales.order_line",
                    target)).Items.Single();
                // Assert
                Assert.AreEqual("sales",
                    result.Source.Table.Table.Schema);
                Assert.AreEqual("public",
                    result.Target.Table.Table.Schema);
                Assert.IsTrue(GetTargetTables().Contains("public.order_line"));
            }
        }
        #endregion
    }
}
