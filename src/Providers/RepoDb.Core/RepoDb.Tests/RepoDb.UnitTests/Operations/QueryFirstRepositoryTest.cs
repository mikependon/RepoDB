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
    public class QueryFirstRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = repository.QueryFirst<CoverageEntity, int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = repository.QueryFirst<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryFirst<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity, int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryFirstAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = repository.QueryFirst(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryFirst(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryFirst(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQueryFirstDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryFirst(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QueryFirstAsync(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryFirstAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryFirstAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQueryFirstAsyncDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryFirstAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
