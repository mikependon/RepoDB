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
    public class UpdateAllRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllTypedWithTableNamestringEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(tableName: tableName,
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
        public void TestUpdateAllTypedWithTableNamestringEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(tableName: tableName,
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
        public void TestUpdateAllTypedWithTableNamestringEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(tableName: tableName,
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
        public void TestUpdateAllTypedWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllTypedWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(entities: entities,
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
        public void TestUpdateAllTypedWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.UpdateAll<CoverageEntity>(entities: entities,
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
        public async Task TestUpdateAllAsyncTypedWithTableNamestringEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestUpdateAllAsyncTypedWithTableNamestringEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestUpdateAllAsyncTypedWithTableNamestringEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestUpdateAllAsyncTypedWithEntitiesEntities(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(entities: entities,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestUpdateAllAsyncTypedWithEntitiesEntitiesQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(entities: entities,
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
        public async Task TestUpdateAllAsyncTypedWithEntitiesEntitiesQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<CoverageEntity> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.UpdateAllAsync<CoverageEntity>(entities: entities,
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
        public void TestUpdateAllDynamicWithTableNamestringEntitiesObjects(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = repository.UpdateAll(tableName: tableName,
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
        public void TestUpdateAllDynamicWithTableNamestringEntitiesObjectsQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.UpdateAll(tableName: tableName,
                entities: entities,
                qualifier: qualifier,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestUpdateAllDynamicWithTableNamestringEntitiesObjectsQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.UpdateAll(tableName: tableName,
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
        public async Task TestUpdateAllAsyncDynamicWithTableNamestringEntitiesObjects(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };

            // Act
            var actual = await repository.UpdateAllAsync(tableName: tableName,
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
        public async Task TestUpdateAllAsyncDynamicWithTableNamestringEntitiesObjectsQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.UpdateAllAsync(tableName: tableName,
                entities: entities,
                qualifier: qualifier,
                batchSize: 1,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestUpdateAllAsyncDynamicWithTableNamestringEntitiesObjectsQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<object> entities = new[] { new CoverageEntity { Id = 1, Name = "Input" } };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.UpdateAllAsync(tableName: tableName,
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
