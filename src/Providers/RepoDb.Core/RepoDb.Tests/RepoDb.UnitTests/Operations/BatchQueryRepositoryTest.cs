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
    public class BatchQueryRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(tableName: tableName,
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
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
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
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFieldsFieldsFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<Field> fields = new[] { new Field("Id") };
            object where = new { Id = 1 };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
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
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
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
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
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
        public void TestBatchQueryTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.BatchQuery<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public async Task TestBatchQueryAsyncTypedWithPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int page = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.BatchQueryAsync<CoverageEntity>(page: page,
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
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestBatchQueryDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestBatchQueryAsyncDynamicWithTableNamestringPageintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
