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
    public class QuerySingleRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = repository.QuerySingle<CoverageEntity, int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = repository.QuerySingle<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QuerySingle<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            int what = 1;

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity, int>(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWhatint(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            int what = 1;

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity, int>(what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWherePredicate(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where = item => item.Id == 1;

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncTypedWithWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QuerySingleAsync<CoverageEntity>(where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = repository.QuerySingle(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = repository.QuerySingle(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QuerySingle(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestQuerySingleDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QuerySingle(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncDynamicWithTableNamestringWhatobject(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            object what = new { Id = 1 };

            // Act
            var actual = await repository.QuerySingleAsync(tableName: tableName,
                what: what,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncDynamicWithTableNamestringWhereQueryField(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryField where = new QueryField("Id", 1);

            // Act
            var actual = await repository.QuerySingleAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncDynamicWithTableNamestringWhereQueryFields(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            IEnumerable<QueryField> where = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QuerySingleAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestQuerySingleAsyncDynamicWithTableNamestringWhereQueryGroup(bool useTransaction)
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName = "CoverageEntity";
            QueryGroup where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QuerySingleAsync(tableName: tableName,
                where: where,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
