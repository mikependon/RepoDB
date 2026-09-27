#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class MaxAllTest
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
        public void TestAuroraDbConnectionMaxAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll<CompleteTable>(e => e.ColumnInteger);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionMaxAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.MaxAll<CompleteTable>(e => e.ColumnInteger,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionMaxAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInteger).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInteger,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionMaxAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First());

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionMaxAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionMaxAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First()).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionMaxAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
