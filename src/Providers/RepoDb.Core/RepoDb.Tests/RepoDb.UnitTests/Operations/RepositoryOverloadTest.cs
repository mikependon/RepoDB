#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class RepositoryOverloadTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestQueryWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.Query<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
        }

        [TestMethod]
        public async Task TestQueryAsyncWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.QueryAsync<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
        }

        [TestMethod]
        public void TestQueryFirstWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.QueryFirst<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.QueryFirstAsync<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
        }

        [TestMethod]
        public void TestQuerySingleWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.QuerySingle<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncWithGenericKey()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.QuerySingleAsync<int>("CoverageEntity", 1);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
        }

        [TestMethod]
        public void TestTruncateSharedLegacy()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            Func<string, int> operation = repository.Truncate<CoverageEntity>;

            // Act
            var actual = operation("truncate");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public async Task TestTruncateAsyncSharedLegacy()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            Func<string, Task<int>> operation = repository.TruncateAsync<CoverageEntity>;

            // Act
            var actual = await operation("truncate");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public async Task TestTruncateAsyncSharedWithToken()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            Func<string, CancellationToken, Task<int>> operation = repository.TruncateAsync<CoverageEntity>;

            // Act
            var actual = await operation("truncate", CancellationToken.None);

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestTruncateEntityLegacy()
        {
            // Setup
            using var repository = new CoverageRepository();
            Func<string, int> operation = repository.Truncate;

            // Act
            var actual = operation("truncate");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public async Task TestTruncateAsyncEntityLegacy()
        {
            // Setup
            using var repository = new CoverageRepository();
            Func<string, Task<int>> operation = repository.TruncateAsync;

            // Act
            var actual = await operation("truncate");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public async Task TestTruncateAsyncEntityWithToken()
        {
            // Setup
            using var repository = new CoverageRepository();
            Func<string, CancellationToken, Task<int>> operation = repository.TruncateAsync;

            // Act
            var actual = await operation("truncate", CancellationToken.None);

            // Assert
            Assert.AreEqual(1, actual);
        }

    }
}
