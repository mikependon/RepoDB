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
    public class CountAllEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestCountAllDynamicWith(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            // Act
            var actual = repository.CountAll(
                transaction: transaction);

            // Assert
            Assert.AreEqual(1L, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestCountAllAsyncDynamicWith(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            // Act
            var actual = await repository.CountAllAsync(
                transaction: transaction);

            // Assert
            Assert.AreEqual(1L, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
