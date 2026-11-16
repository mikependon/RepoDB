#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests
{
    /// <summary>
    /// Contains the turso schema target schema test behavior.
    /// </summary>
    [TestClass]
    public class TursoSchemaTargetSchemaTest
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
        private static void MapSchemaReaderConnection(SqliteConnection source) => SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(),
            true);

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader().GetTables(connection,
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
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader().GetTableSchema(connection,
                    tableName,
                    transaction: null);
            }
        }

        #endregion

        #region Target Schema

        /// <summary>
        /// Verifies copy table schemas to without target schema creates the tables in the default schema.
        /// </summary>
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
                source.CopyTableSchemaTo(new[] { "Person", "Country", "OrderLine" },
                    target,
                    createdCallback: results.Add);
                // Assert
                CollectionAssert.AreEquivalent(new[] { "Country", "OrderLine", "Person" },
                    GetTargetTables());
                Assert.IsTrue(results.All(r => r.Target.Table.Table.Schema == null));
                Assert.IsTrue(results.All(r => r.Source.Table.Table.Schema == null));
                Assert.AreEqual(new TableInfo("Country",
                        null),
                    GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable);
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the target schema does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<SqliteException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target,
                        "Missing"));
            }
        }

        /// <summary>
        /// Verifies copy table schemas to async without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
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
