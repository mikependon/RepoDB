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
    public class MergeRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge<CoverageEntity>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.Merge<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityCoverageEntityQualifiersSelectorReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.Merge<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge<CoverageEntity>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge<CoverageEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge<CoverageEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.Merge<CoverageEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge<CoverageEntity, int>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge<CoverageEntity, int>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge<CoverageEntity, int>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithEntityCoverageEntityQualifiersSelectorReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = repository.Merge<CoverageEntity, int>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityCoverageEntityQualifiersSelectorReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifiersSelector(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAsync<CoverageEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithEntityCoverageEntityQualifiersSelectorReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };
            Expression<Func<CoverageEntity, object>> qualifiers = item => item.Id;

            // Act
            var actual = await repository.MergeAsync<CoverageEntity, int>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeDynamicWithTableNamestringEntityobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeDynamicWithTableNamestringEntityobjectQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeDynamicWithTableNamestringEntityobjectQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Merge<int>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityobjectQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = repository.Merge<int>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMergeTypedWithTableNamestringEntityobjectQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = repository.Merge<int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncDynamicWithTableNamestringEntityobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncDynamicWithTableNamestringEntityobjectQualifierField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncDynamicWithTableNamestringEntityobjectQualifiersFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.MergeAsync<int>(tableName: tableName,
                entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityobjectQualifierFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            Field qualifier = new Field("Id");

            // Act
            var actual = await repository.MergeAsync<int>(tableName: tableName,
                entity: entity,
                qualifier: qualifier,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMergeAsyncTypedWithTableNamestringEntityobjectQualifiersFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object entity = new CoverageEntity { Id = 1, Name = "Input" };
            IEnumerable<Field> qualifiers = new[] { new Field("Id") };

            // Act
            var actual = await repository.MergeAsync<int>(tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
