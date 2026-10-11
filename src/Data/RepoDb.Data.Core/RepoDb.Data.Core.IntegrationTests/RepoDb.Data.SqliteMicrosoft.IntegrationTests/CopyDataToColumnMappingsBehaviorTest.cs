#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.SqliteMicrosoft.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.SqliteMicrosoft.IntegrationTests
{
    /// <summary>
    /// Copies the rows with the column mappings behavior. SQLite has no bulk operations, so the rows are inserted in batches (the <c>InsertAll</c> of the library),
    /// and only the <see cref="CopySchemaColumnMappingsBehavior.Automatic"/> behavior is available.
    /// </summary>
    [TestClass]
    public class CopyDataToColumnMappingsBehaviorTest
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
        public void TestCopyDataToWithoutABehaviorIsAutomatic()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"))");

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopySchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictIgnoresTheBehaviorBecauseTheProviderHasNoBulkOperations()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopySchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassIgnoresTheBehaviorBecauseTheProviderHasNoBulkOperations()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopySchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"))");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopySchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictIgnoresTheBehaviorBecauseTheProviderHasNoBulkOperations()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopySchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        #endregion
    }
}
