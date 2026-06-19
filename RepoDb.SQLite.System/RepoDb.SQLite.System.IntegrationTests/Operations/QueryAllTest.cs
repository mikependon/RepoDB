using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryAll()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllWithHints()
        {
            // Setup
            var table = Database.CreateSdsCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryAll<SdsCompleteTable>(hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionQueryAllAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncWithHints()
        {
            // Setup
            var table = Database.CreateSdsCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(async () =>
                    await connection.QueryAllAsync<SdsCompleteTable>(hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryAllViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<SdsCompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateSdsCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAllAsyncViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateSdsCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(async () =>
                    await connection.QueryAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
