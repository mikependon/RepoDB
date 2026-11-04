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
    public class UpdateAllEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllDynamicWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.UpdateAll(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllDynamicWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.UpdateAll(entities: entities,
                qualifiers: qualifiers,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllDynamicWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.UpdateAll(entities: entities,
                qualifiers: qualifiers,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestUpdateAllAsyncDynamicWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.UpdateAllAsync(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestUpdateAllAsyncDynamicWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.UpdateAllAsync(entities: entities,
                qualifiers: qualifiers,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestUpdateAllAsyncDynamicWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.UpdateAllAsync(entities: entities,
                qualifiers: qualifiers,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
