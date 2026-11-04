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
    public class BatchQueryEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.BatchQuery(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.BatchQuery(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.BatchQueryAsync(tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.BatchQueryAsync(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
