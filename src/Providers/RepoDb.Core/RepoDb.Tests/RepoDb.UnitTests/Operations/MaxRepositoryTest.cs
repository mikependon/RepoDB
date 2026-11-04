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
    public class MaxRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<CoverageEntity>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWherePredicateReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldFieldWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWherePredicateReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithFieldSelectorWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWherePredicateReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldFieldWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWherePredicateReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithFieldSelectorWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<CoverageEntity, int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithTableNamestringFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithTableNamestringFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithTableNamestringFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithTableNamestringFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithTableNamestringFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithTableNamestringFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithTableNamestringFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithTableNamestringFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithTableNamestringFieldFieldWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithTableNamestringFieldFieldWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithTableNamestringFieldFieldWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxTypedWithTableNamestringFieldFieldWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithTableNamestringFieldFieldWhereobjectReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithTableNamestringFieldFieldWhereQueryFieldReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithTableNamestringFieldFieldWhereQueryFieldsReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncTypedWithTableNamestringFieldFieldWhereQueryGroupReturningKey(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<int>(tableName: tableName,
                field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
