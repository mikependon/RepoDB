using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryAll<MdsCompleteTable>(hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionQueryAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(async () =>
                    await connection.QueryAllAsync<MdsCompleteTable>(hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryAllViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<MdsCompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Query(ClassMappedNameCache.Get<MdsCompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAllAsyncViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                Assert.Throws<NotSupportedException>(async () =>
                    await connection.QueryAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
