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
    public class SkipQueryRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(tableName: tableName,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFieldsFieldsFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<Field> fields = new[] { new Field("Id") };
            object where = new { Id = 1 };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.SkipQuery<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(tableName: tableName,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            object where = new { Id = 1 };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public async Task TestSkipQueryAsyncTypedWithSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int skip = 0;
            int rowsPerBatch = 1;
            IEnumerable<OrderField> orderBy = new[] { new OrderField("Id", Order.Ascending) };
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.SkipQueryAsync<CoverageEntity>(skip: skip,
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
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestSkipQueryDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSkipQueryAsyncDynamicWithTableNamestringSkipintRowsPerBatchintOrderByOrderFieldsWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
