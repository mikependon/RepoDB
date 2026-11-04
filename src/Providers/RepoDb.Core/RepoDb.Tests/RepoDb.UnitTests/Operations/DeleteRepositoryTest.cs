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
    public class DeleteRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = repository.Delete<CoverageEntity>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = repository.Delete<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = repository.Delete<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Delete<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Delete<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Delete<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Delete<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithEntityCoverageEntity(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            CoverageEntity entity = new CoverageEntity { Id = 1, Name = "Input" };

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(entity: entity,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.DeleteAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = repository.Delete<int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = repository.Delete(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Delete(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Delete(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestDeleteDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Delete(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = await repository.DeleteAsync<int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = await repository.DeleteAsync(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.DeleteAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.DeleteAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestDeleteAsyncDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.DeleteAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
