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
using RepoDb.Schema.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Turso.IntegrationTests
{
    [TestClass]
    public class TursoSchemaTargetSchemaTest
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
            SchemaReaderMapper.Add<SqliteConnection>(new TursoSchemaReader(source), true);

        private static List<string> GetTargetTables(string schemaName = null)
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader(connection).GetTables(schemaName).ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopySchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
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
        public void ThrowExceptionOnCopySchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<SqliteException>(() => source.CopySchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
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
