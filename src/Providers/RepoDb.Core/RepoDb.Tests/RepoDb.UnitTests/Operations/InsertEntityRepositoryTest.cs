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
    public class InsertEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestInsertDynamicWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Insert(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestInsertTypedWithEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Insert<int>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestInsertAsyncDynamicWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.InsertAsync(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestInsertAsyncTypedWithEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.InsertAsync<int>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
