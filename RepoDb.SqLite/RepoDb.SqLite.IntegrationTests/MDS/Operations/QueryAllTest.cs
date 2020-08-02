using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionQueryAllWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                connection.QueryAll<MdsCompleteTable>(hints: "WhatEver");
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

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionQueryAllAsyncWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                connection.QueryAllAsync<MdsCompleteTable>(hints: "WhatEver").Wait();
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionQueryAllViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                connection.Query(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionQueryAllAsyncViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var queryResult = connection.QueryAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>()).Result;

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateMdsCompleteTables(1).First();

            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Act
                connection.QueryAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
