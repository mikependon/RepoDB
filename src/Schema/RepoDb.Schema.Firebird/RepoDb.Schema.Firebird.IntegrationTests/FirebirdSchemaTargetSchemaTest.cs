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
    [TestClass]
    public class FirebirdSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(FbConnection source) =>
            SchemaReaderMapper.Add<FbConnection>(new FirebirdSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = new FbConnection(Database.ConnectionStringForTarget))
            {
                return new FirebirdSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new FbConnection(Database.ConnectionStringForTarget))
            {
                return new FirebirdSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopyTableSchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
        public async Task TestCopyTableSchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemasToIfTheTargetSchemaIsGiven()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<NotSupportedException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target, "Sales"));
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemasToAsyncIfTheTargetSchemaIsGiven()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() => source.CopyTableSchemaToAsync(new[] { "Country" }, target, "Sales"));
                Assert.AreEqual(0, GetTargetTables().Count);
            }
        }

        #endregion
    }
}
