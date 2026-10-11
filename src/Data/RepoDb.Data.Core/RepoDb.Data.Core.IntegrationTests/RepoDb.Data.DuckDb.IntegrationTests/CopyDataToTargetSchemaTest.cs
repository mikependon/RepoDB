#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.DuckDb.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.DuckDb.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table into a target table that has a schema (i.e.: <c>Sales.Person</c>). Firebird has no schema, so the schema of a name is not supported.
    /// </summary>
    [TestClass]
    public class CopyDataToTargetSchemaTest
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

        #region Sync

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasASchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<DuckDBException>(() => source.CopyDataTo("Currency", "Sales.Currency", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasAQuotedSchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyDataTo("Currency", "\"Sales\".\"Currency\"", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Money\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_Money\" PRIMARY KEY (\"Code\"))");

                // Act
                var actual = source.CopyDataTo("Currency", "Money", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Money\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableHasASchema()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<DuckDBException>(() => source.CopyDataToAsync("Currency", "Sales.Currency", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Money\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_Money\" PRIMARY KEY (\"Code\"))");

                // Act
                var actual = await source.CopyDataToAsync("Currency", "Money", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Money\""));
            }
        }

        #endregion
    }
}
