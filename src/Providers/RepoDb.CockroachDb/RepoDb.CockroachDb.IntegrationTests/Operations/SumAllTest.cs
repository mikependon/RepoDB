#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
{
    [TestClass]
    public class SumAllTest
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
        public void TestCockroachDbConnectionSumAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll<CompleteTable>(e => e.ColumnInteger);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbConnectionSumAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.SumAll<CompleteTable>(e => e.ColumnInteger,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionSumAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnInteger).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbConnectionSumAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAllAsync<CompleteTable>(e => e.ColumnInteger,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestCockroachDbConnectionSumAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First());

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbConnectionSumAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionSumAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First()).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbConnectionSumAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
