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
    public class MergeAllRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeAllTypedWithTableNamestringEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.MergeAll<CoverageEntity>(tableName: tableName,
                entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeAllTypedWithTableNamestringEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.MergeAll<CoverageEntity>(tableName: tableName,
                entities: entities,
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
        public void TestMergeAllTypedWithTableNamestringEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.MergeAll<CoverageEntity>(tableName: tableName,
                entities: entities,
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
        public void TestMergeAllTypedWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.MergeAll<CoverageEntity>(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeAllTypedWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.MergeAll<CoverageEntity>(entities: entities,
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
        public void TestMergeAllTypedWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.MergeAll<CoverageEntity>(entities: entities,
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
        public async Task TestMergeAllAsyncTypedWithTableNamestringEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(tableName: tableName,
                entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAllAsyncTypedWithTableNamestringEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(tableName: tableName,
                entities: entities,
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
        public async Task TestMergeAllAsyncTypedWithTableNamestringEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(tableName: tableName,
                entities: entities,
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
        public async Task TestMergeAllAsyncTypedWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAllAsyncTypedWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(entities: entities,
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
        public async Task TestMergeAllAsyncTypedWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAllAsync<CoverageEntity>(entities: entities,
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
        public void TestMergeAllDynamicWithTableNamestringEntitiesObjects(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.MergeAll(tableName: tableName,
                entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeAllDynamicWithTableNamestringEntitiesObjectsQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.MergeAll(tableName: tableName,
                entities: entities,
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
        public async Task TestMergeAllAsyncDynamicWithTableNamestringEntitiesObjects(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.MergeAllAsync(tableName: tableName,
                entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAllAsyncDynamicWithTableNamestringEntitiesObjectsQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAllAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
