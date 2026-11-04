#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class QueryAllRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryAllTypedWithTableNamestring(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";

            // Act
            var actual = repository.QueryAll<CoverageEntity>(tableName: tableName,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryAllTypedWith(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            // Act
            var actual = repository.QueryAll<CoverageEntity>(
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryAllAsyncTypedWithTableNamestring(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";

            // Act
            var actual = await repository.QueryAllAsync<CoverageEntity>(tableName: tableName,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryAllAsyncTypedWith(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            // Act
            var actual = await repository.QueryAllAsync<CoverageEntity>(
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryAllDynamicWithTableNamestring(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";

            // Act
            var actual = repository.QueryAll(tableName: tableName,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryAllAsyncDynamicWithTableNamestring(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";

            // Act
            var actual = await repository.QueryAllAsync(tableName: tableName,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
