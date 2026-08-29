#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: now that Db2DbSetting.IsMultiStatementExecutable is true, every 10-row InsertAll call
    /// below genuinely batches into a single "SELECT <key> FROM FINAL TABLE (INSERT ... VALUES
    /// (...), (...), ...)" round trip instead of 10 separate single-row round trips. This makes
    /// the "table.Id > 0" and per-row AssertPropertiesEquality (matched by the returned Id)
    /// assertions below the live verification of Db2StatementBuilder.CreateInsertAll's documented
    /// assumption that FINAL TABLE's result rows come back in the same order as the source VALUES
    /// list - if that assumption is ever wrong, a returned identity would be paired with the wrong
    /// entity, and AssertPropertiesEquality's lookup-by-Id would immediately surface a mismatch.
    /// </summary>
    [TestClass]
    public class InsertAllTest
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
            Database.Cleanup();
        }

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionInsertAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.InsertAll<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionInsertAllWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.InsertAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionInsertAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: the mapped-name overload still returns typed CompleteTable rows once queried back below,
            // so this is a genuine additional scenario rather than a re-run of the test above.
            var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionInsertAllAsync()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAllAsync<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionInsertAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.InsertAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionInsertAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion
    }
}
