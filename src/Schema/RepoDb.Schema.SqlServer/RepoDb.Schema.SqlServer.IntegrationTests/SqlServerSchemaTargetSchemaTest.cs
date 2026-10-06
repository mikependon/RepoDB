#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaTargetSchemaTest
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

        private static void MapSchemaReaderConnection(SqlConnection source) =>
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

        private static List<string> GetTargetTables()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader(connection).GetTables().ToList();
            }
        }

        private static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                return new SqlServerSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        #endregion

        #region Target Schema

        [TestMethod]
        public void TestCopySchemasToWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country", "Person" }, target, "Sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Sales.Country", "Sales.Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.SourceSchema == "dbo" && r.DestinationSchema == "Sales"));
                var person = GetTargetSchema("Sales.Person");
                Assert.AreEqual(new TableInfo("Country", "Sales"), person.ForeignKeys.Single().ReferencedTable);
                CollectionAssert.AreEqual(Helper.GetSourceSchema("Person").Columns.Select(c => c.Field.Name).ToArray(), person.Columns.Select(c => c.Field.Name).ToArray());
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Sales.InvoiceLine", "Sales.Invoice", "dbo.Ledger" }, target, createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "dbo.Invoice", "dbo.InvoiceLine", "dbo.Ledger" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == "dbo"));
                Assert.AreEqual("Sales", results.Single(r => r.TableName == "Invoice").SourceSchema);
                Assert.AreEqual(new TableInfo("Invoice", "dbo"), GetTargetSchema("dbo.Ledger").ForeignKeys.Single().ReferencedTable);
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfASingleTableWithTargetSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("Country", target, "Sales");

                // Assert
                Assert.AreEqual("dbo", result.SourceSchema);
                Assert.AreEqual("Sales", result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { "Sales.Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void TestCopySchemasToWithTargetSchemaChecksTheTablesOfThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo(new[] { "Country" }, target, "Sales");
                var results = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "Country" }, target, "Sales", createdCallback: results.Add);
                source.CopySchemaTo(new[] { "Country" }, target, createdCallback: results.Add);

                // Assert
                Assert.AreEqual(CopySchemaOutcome.Skipped, results[0].Outcome);
                Assert.AreEqual(CopySchemaOutcome.Created, results[1].Outcome);
                CollectionAssert.AreEquivalent(new[] { "Sales.Country", "dbo.Country" }, GetTargetTables());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemasToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopySchemaTo(new[] { "Country" }, target, "Missing"));
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithTargetSchemaCreatesTheTablesInThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var results = new List<CopySchemaResult>();

                // Act
                await source.CopySchemaToAsync(new[] { "Country", "Person" }, target, "Sales", createdCallback: results.Add);

                // Assert
                CollectionAssert.AreEquivalent(new[] { "Sales.Country", "Sales.Person" }, GetTargetTables());
                Assert.IsTrue(results.All(r => r.DestinationSchema == "Sales"));
            }
        }

        [TestMethod]
        public async Task TestCopySchemasToAsyncWithoutTargetSchemaCreatesTheTablesInTheDefaultSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = await source.CopySchemaToAsync("Sales.Invoice", target);

                // Assert
                Assert.AreEqual("Sales", result.SourceSchema);
                Assert.AreEqual("dbo", result.DestinationSchema);
                CollectionAssert.AreEqual(new[] { "dbo.Invoice" }, GetTargetTables());
            }
        }

        #endregion
    }
}
