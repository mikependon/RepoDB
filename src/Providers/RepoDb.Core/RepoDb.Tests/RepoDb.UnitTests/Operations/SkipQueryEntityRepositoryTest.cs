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
    public class SkipQueryEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.SkipQuery(tableName: tableName,
                skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public void TestSkipQueryDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.SkipQuery(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.SkipQueryAsync(tableName: tableName,
                skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
        public async Task TestSkipQueryAsyncDynamicWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.SkipQueryAsync(skip: skip,
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
