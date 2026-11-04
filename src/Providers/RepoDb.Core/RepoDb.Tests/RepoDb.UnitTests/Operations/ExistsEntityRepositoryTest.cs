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
    public class ExistsEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsDynamicWith(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            // Act
            var actual = repository.Exists(
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = repository.Exists<int>(what: what,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsDynamicWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.Exists(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsDynamicWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.Exists(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsDynamicWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.Exists(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestExistsDynamicWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.Exists(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncDynamicWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = await repository.ExistsAsync(what: what,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = await repository.ExistsAsync<int>(what: what,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncDynamicWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.ExistsAsync(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncDynamicWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.ExistsAsync(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncDynamicWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.ExistsAsync(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestExistsAsyncDynamicWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.ExistsAsync(where: where,
                transaction: transaction);

            // Assert
            Assert.IsTrue(actual);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
