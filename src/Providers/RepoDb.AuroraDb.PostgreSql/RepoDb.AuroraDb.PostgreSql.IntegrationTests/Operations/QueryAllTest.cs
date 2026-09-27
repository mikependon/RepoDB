#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Extensions;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.Operations
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
        public void TestAuroraDbConnectionQueryAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryAll<CompleteTable>(hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestAuroraDbConnectionQueryAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAllAsync<CompleteTable>(hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryAllViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = await connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>()).ConfigureAwait(false);

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
