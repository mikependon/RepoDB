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
    public class MaxEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldFieldWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldSelectorWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldSelectorWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldSelectorWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldSelectorWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestMaxDynamicWithFieldSelectorWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldFieldWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldFieldWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldFieldWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldFieldWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldFieldWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldSelectorWhereobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldSelectorWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldSelectorWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldSelectorWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestMaxAsyncDynamicWithFieldSelectorWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, object>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Max<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Field field = new Field("Id");
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            object where = new { Id = 1 };

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
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
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, int>> field = item => item.Id;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.MaxAsync<int>(field: field,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
