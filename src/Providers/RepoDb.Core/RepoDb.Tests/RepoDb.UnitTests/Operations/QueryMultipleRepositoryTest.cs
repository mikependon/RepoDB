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
    public class QueryMultipleRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2objectWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2PredicateWhere3Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2objectWhat3objectWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5objectWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };
            object what6 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5PredicateWhere6Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryFieldWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFieldsWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroupWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5objectWhat6objectWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };
            object what6 = new { Id = 1 };
            object what7 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                what6: what6,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5PredicateWhere6PredicateWhere7Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where7 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryFieldWhere6QueryFieldWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);
            QueryField where6 = new QueryField("Id", 1);
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFieldsWhere6QueryFieldsWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroupWhere6QueryGroupWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2objectWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2PredicateWhere3Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2objectWhat3objectWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5objectWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };
            object what6 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5PredicateWhere6Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryFieldWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFieldsWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroupWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhat1objectWhat2objectWhat3objectWhat4objectWhat5objectWhat6objectWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            object what1 = new { Id = 1 };
            object what2 = new { Id = 1 };
            object what3 = new { Id = 1 };
            object what4 = new { Id = 1 };
            object what5 = new { Id = 1 };
            object what6 = new { Id = 1 };
            object what7 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(what1: what1,
                what2: what2,
                what3: what3,
                what4: what4,
                what5: what5,
                what6: what6,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1PredicateWhere2PredicateWhere3PredicateWhere4PredicateWhere5PredicateWhere6PredicateWhere7Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;
            Expression<Func<CoverageEntity, bool>> where7 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldWhere2QueryFieldWhere3QueryFieldWhere4QueryFieldWhere5QueryFieldWhere6QueryFieldWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryField where1 = new QueryField("Id", 1);
            QueryField where2 = new QueryField("Id", 1);
            QueryField where3 = new QueryField("Id", 1);
            QueryField where4 = new QueryField("Id", 1);
            QueryField where5 = new QueryField("Id", 1);
            QueryField where6 = new QueryField("Id", 1);
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryFieldsWhere2QueryFieldsWhere3QueryFieldsWhere4QueryFieldsWhere5QueryFieldsWhere6QueryFieldsWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithWhere1QueryGroupWhere2QueryGroupWhere3QueryGroupWhere4QueryGroupWhere5QueryGroupWhere6QueryGroupWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5PredicateTableName6stringWhere6Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            string tableName6 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6objectTableName7stringWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };
            string tableName7 = "CoverageEntity";
            object what7 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                tableName7: tableName7,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5PredicateTableName6stringWhere6PredicateTableName7stringWhere7Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            string tableName6 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;
            string tableName7 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where7 = item => item.Id == 1;

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryFieldTableName7stringWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);
            string tableName7 = "CoverageEntity";
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFieldsTableName7stringWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            string tableName7 = "CoverageEntity";
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroupTableName7stringWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            string tableName7 = "CoverageEntity";
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6objectTableName7stringWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };
            string tableName7 = "CoverageEntity";
            object what7 = new { Id = 1 };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                tableName7: tableName7,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryFieldTableName7stringWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);
            string tableName7 = "CoverageEntity";
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFieldsTableName7stringWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            string tableName7 = "CoverageEntity";
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public void TestQueryMultipleDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroupTableName7stringWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            string tableName7 = "CoverageEntity";
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = repository.QueryMultiple(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item2.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item3.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item4.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item5.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5PredicateTableName6stringWhere6Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            string tableName6 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item6.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6objectTableName7stringWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };
            string tableName7 = "CoverageEntity";
            object what7 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                tableName7: tableName7,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1PredicateTableName2stringWhere2PredicateTableName3stringWhere3PredicateTableName4stringWhere4PredicateTableName5stringWhere5PredicateTableName6stringWhere6PredicateTableName7stringWhere7Predicate(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where1 = item => item.Id == 1;
            string tableName2 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where2 = item => item.Id == 1;
            string tableName3 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where3 = item => item.Id == 1;
            string tableName4 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where4 = item => item.Id == 1;
            string tableName5 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where5 = item => item.Id == 1;
            string tableName6 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where6 = item => item.Id == 1;
            string tableName7 = "CoverageEntity";
            Expression<Func<CoverageEntity, bool>> where7 = item => item.Id == 1;

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryFieldTableName7stringWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);
            string tableName7 = "CoverageEntity";
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFieldsTableName7stringWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            string tableName7 = "CoverageEntity";
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncTypedWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroupTableName7stringWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            string tableName7 = "CoverageEntity";
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Item7.Single().Id);
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item2.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item3.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item4.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item5.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item6.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhat1objectTableName2stringWhat2objectTableName3stringWhat3objectTableName4stringWhat4objectTableName5stringWhat5objectTableName6stringWhat6objectTableName7stringWhat7object(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            object what1 = new { Id = 1 };
            string tableName2 = "CoverageEntity";
            object what2 = new { Id = 1 };
            string tableName3 = "CoverageEntity";
            object what3 = new { Id = 1 };
            string tableName4 = "CoverageEntity";
            object what4 = new { Id = 1 };
            string tableName5 = "CoverageEntity";
            object what5 = new { Id = 1 };
            string tableName6 = "CoverageEntity";
            object what6 = new { Id = 1 };
            string tableName7 = "CoverageEntity";
            object what7 = new { Id = 1 };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                what1: what1,
                tableName2: tableName2,
                what2: what2,
                tableName3: tableName3,
                what3: what3,
                tableName4: tableName4,
                what4: what4,
                tableName5: tableName5,
                what5: what5,
                tableName6: tableName6,
                what6: what6,
                tableName7: tableName7,
                what7: what7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldTableName2stringWhere2QueryFieldTableName3stringWhere3QueryFieldTableName4stringWhere4QueryFieldTableName5stringWhere5QueryFieldTableName6stringWhere6QueryFieldTableName7stringWhere7QueryField(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryField where1 = new QueryField("Id", 1);
            string tableName2 = "CoverageEntity";
            QueryField where2 = new QueryField("Id", 1);
            string tableName3 = "CoverageEntity";
            QueryField where3 = new QueryField("Id", 1);
            string tableName4 = "CoverageEntity";
            QueryField where4 = new QueryField("Id", 1);
            string tableName5 = "CoverageEntity";
            QueryField where5 = new QueryField("Id", 1);
            string tableName6 = "CoverageEntity";
            QueryField where6 = new QueryField("Id", 1);
            string tableName7 = "CoverageEntity";
            QueryField where7 = new QueryField("Id", 1);

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryFieldsTableName2stringWhere2QueryFieldsTableName3stringWhere3QueryFieldsTableName4stringWhere4QueryFieldsTableName5stringWhere5QueryFieldsTableName6stringWhere6QueryFieldsTableName7stringWhere7QueryFields(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            IEnumerable<QueryField> where1 = new[] { new QueryField("Id", 1) };
            string tableName2 = "CoverageEntity";
            IEnumerable<QueryField> where2 = new[] { new QueryField("Id", 1) };
            string tableName3 = "CoverageEntity";
            IEnumerable<QueryField> where3 = new[] { new QueryField("Id", 1) };
            string tableName4 = "CoverageEntity";
            IEnumerable<QueryField> where4 = new[] { new QueryField("Id", 1) };
            string tableName5 = "CoverageEntity";
            IEnumerable<QueryField> where5 = new[] { new QueryField("Id", 1) };
            string tableName6 = "CoverageEntity";
            IEnumerable<QueryField> where6 = new[] { new QueryField("Id", 1) };
            string tableName7 = "CoverageEntity";
            IEnumerable<QueryField> where7 = new[] { new QueryField("Id", 1) };

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(false, true)]
        public async Task TestQueryMultipleAsyncDynamicWithTableName1stringWhere1QueryGroupTableName2stringWhere2QueryGroupTableName3stringWhere3QueryGroupTableName4stringWhere4QueryGroupTableName5stringWhere5QueryGroupTableName6stringWhere6QueryGroupTableName7stringWhere7QueryGroup(bool multipleStatements, bool useTransaction)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = useTransaction ? connection.BeginTransaction() : null;
            connection.UseScalarResults();
            string tableName1 = "CoverageEntity";
            QueryGroup where1 = new QueryGroup(new QueryField("Id", 1));
            string tableName2 = "CoverageEntity";
            QueryGroup where2 = new QueryGroup(new QueryField("Id", 1));
            string tableName3 = "CoverageEntity";
            QueryGroup where3 = new QueryGroup(new QueryField("Id", 1));
            string tableName4 = "CoverageEntity";
            QueryGroup where4 = new QueryGroup(new QueryField("Id", 1));
            string tableName5 = "CoverageEntity";
            QueryGroup where5 = new QueryGroup(new QueryField("Id", 1));
            string tableName6 = "CoverageEntity";
            QueryGroup where6 = new QueryGroup(new QueryField("Id", 1));
            string tableName7 = "CoverageEntity";
            QueryGroup where7 = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await repository.QueryMultipleAsync(tableName1: tableName1,
                where1: where1,
                tableName2: tableName2,
                where2: where2,
                tableName3: tableName3,
                where3: where3,
                tableName4: tableName4,
                where4: where4,
                tableName5: tableName5,
                where5: where5,
                tableName6: tableName6,
                where6: where6,
                tableName7: tableName7,
                where7: where7,
                transaction: transaction);

            // Assert
            Assert.AreEqual(1, (object)actual.Item7.Single());
            Assert.IsTrue(connection.ExecutionCount > 0);
        }

    }
}
