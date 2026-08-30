#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class QueryAllTest
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
        public void TestDb2ConnectionQueryAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionQueryAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
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
        public void TestDb2ConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: typed via the mapped-name overload (rather than the untyped/dynamic one) so the
            // result can still go through Helper.AssertPropertiesEquality below.
            var queryResult = connection.QueryAll<CompleteTable>(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionQueryAllWithHintsThrowsNotSupportedException()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: Db2DbSetting.AreTableHintsSupported is false - any non-null/non-whitespace
            // "hints" argument must throw rather than silently being ignored.
            Assert.Throws<System.NotSupportedException>(() => connection.QueryAll<CompleteTable>(hints: "NOLOCK"));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQueryAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
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
        public async Task TestDb2ConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAllAsyncWithHintsThrowsNotSupportedException()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: Db2DbSetting.AreTableHintsSupported is false - any non-null/non-whitespace
            // "hints" argument must throw rather than silently being ignored.
            await Assert.ThrowsAsync<System.NotSupportedException>(() => connection.QueryAllAsync<CompleteTable>(hints: "NOLOCK"));
        }

        #endregion
    }
}
