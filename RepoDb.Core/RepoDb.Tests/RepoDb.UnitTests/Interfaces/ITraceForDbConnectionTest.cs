using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        private readonly IStatementBuilder m_statementBuilder = Helper.StatementBuilder;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(CustomDbConnectionForDbConnectionITrace), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForDbConnectionITrace), Helper.DbValidator, true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbConnectionITrace), new CustomerDbConnectionDbHelper(), true);
            DbOperationMapper.Add(typeof(CustomDbConnectionForDbConnectionITrace), new DbConnectionCustomDbOperationProvider(), true);
        }

        #region SubClasses

        private class CustomDbConnectionForDbConnectionITrace : CustomDbConnection { }

        private class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class CustomerDbConnectionDbHelper : IDbHelper
        {
            public IResolver<string, Type> DbTypeResolver { get; set; }

            public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                return new[]
                {
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null)
                };
            }

            public Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                return Task.FromResult<IEnumerable<DbField>>(new[]
                {
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null)
                });
            }
        }

        private class DbConnectionCustomDbOperationProvider : IDbOperation
        {
            public int BulkInsert<TEntity>(IDbConnection connection, IEnumerable<TEntity> entities, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return 1;
            }

            public int BulkInsert<TEntity>(IDbConnection connection, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return 1;
            }

            public int BulkInsert(IDbConnection connection, string tableName, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
            {
                return 1;
            }

            public int BulkInsert<TEntity>(IDbConnection connection, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return 1;
            }

            public int BulkInsert(IDbConnection connection, string tableName, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
            {
                return 1;
            }

            public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, IEnumerable<TEntity> entities, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return Task.FromResult(1);
            }

            public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return Task.FromResult(1);
            }

            public Task<int> BulkInsertAsync(IDbConnection connection, string tableName, DbDataReader reader, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
            {
                return Task.FromResult(1);
            }

            public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null) where TEntity : class
            {
                return Task.FromResult(1);
            }

            public Task<int> BulkInsertAsync(IDbConnection connection, string tableName, DataTable dataTable, DataRowState rowState = DataRowState.Unchanged, IEnumerable<BulkInsertMapItem> mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, int? bulkCopyTimeout = null, int? batchSize = null, IDbTransaction transaction = null)
            {
                return Task.FromResult(1);
            }
        }

        #endregion

        #region Average

        #region Average

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Average<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Average<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region AverageAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region AverageAll

        #region AverageAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region BulkInsert

        #region BulkInsert

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            connection.BulkInsert<TraceEntity>(entities,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            connection.BulkInsert<TraceEntity>(entities,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBulkInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            connection.BulkInsertAsync<TraceEntity>(entities,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            connection.BulkInsertAsync<TraceEntity>(entities,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
            statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Max

        #region Max

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMax()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Max<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMax()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Max<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region MaxAll

        #region MaxAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Min

        #region Min

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMin()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Min<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMin()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Min<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region MinAll

        #region MinAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Sum

        #region Sum

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSum()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Sum<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSum()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Sum<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region SumAll

        #region SumAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnectionForDbConnectionITrace();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}
