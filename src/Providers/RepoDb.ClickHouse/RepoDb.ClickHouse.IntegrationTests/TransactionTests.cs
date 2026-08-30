#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.IntegrationTests
{
    [TestClass]
    public class TransactionTests
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

        [TestMethod]
        public void TestClickHouseConnectionBeginTransactionThrowsNotSupportedException()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act / Assert
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    connection.EnsureOpen().BeginTransaction());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBeginDbTransactionWithIsolationLevelThrowsNotSupportedException()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act / Assert
                Assert.ThrowsExactly<NotSupportedException>(() =>
                    connection.EnsureOpen().BeginTransaction(System.Data.IsolationLevel.ReadCommitted));
            }
        }
    }
}
