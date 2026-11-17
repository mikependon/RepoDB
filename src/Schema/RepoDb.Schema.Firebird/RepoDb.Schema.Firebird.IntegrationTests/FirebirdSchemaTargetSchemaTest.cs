#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;

namespace RepoDb.Schema.Firebird.IntegrationTests
{
    /// <summary>
    /// Contains the firebird schema target schema test behavior.
    /// </summary>
    [TestClass]
    public class FirebirdSchemaTargetSchemaTest
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
        private static void MapSchemaReaderConnection(FbConnection source) => SchemaReaderMapper.Add<FbConnection>(new FirebirdSchemaReader(),
            true);

        /// <summary>
        /// Performs get target tables.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new FbConnection(Database.ConnectionStringForTarget))
            {
                return new FirebirdSchemaReader().GetTables(connection,
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
            using (var connection = new FbConnection(Database.ConnectionStringForTarget))
            {
                return new FirebirdSchemaReader().GetTableSchema(connection,
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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
        /// Verifies copy table schemas to async without target schema creates the tables in the default schema.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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

        /// <summary>
        /// Verifies throw exception on copy table schemas to if the target schema is given.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaIsGiven()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                Assert.Throws<NotSupportedException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target,
                        "Sales"));
                Assert.AreEqual(0,
                    GetTargetTables().Count);
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schemas to async if the target schema is given.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheTargetSchemaIsGiven()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() => source.CopyTableSchemaToAsync(new[] { "Country" },
                        target,
                        "Sales"));
                Assert.AreEqual(0,
                    GetTargetTables().Count);
            }
        }
        #endregion
    }
}
