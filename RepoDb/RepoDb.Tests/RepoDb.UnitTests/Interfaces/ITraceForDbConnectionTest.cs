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
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        private readonly IStatementBuilder m_statementBuilder = new SqlServerStatementBuilder();

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

            public IEnumerable<DbField> GetFields(string connectionString, string tableName, IDbTransaction transaction = null)
            {
                return GetFields((DbConnection)null, tableName);
            }

            public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                return new[]
                {
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null, Helper.DbSetting),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null, Helper.DbSetting)
                };
            }

            public Task<IEnumerable<DbField>> GetFieldsAsync(string connectionString, string tableName, IDbTransaction transaction = null)
            {
                return GetFieldsAsync((DbConnection)null, tableName);
            }

            public Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                return Task.FromResult<IEnumerable<DbField>>(new[]
                {
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null, Helper.DbSetting),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null, Helper.DbSetting)
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
                OrderField.Ascending<TraceEntity>(t => t.Id, Helper.DbSetting).AsEnumerable(),
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
                OrderField.Ascending<TraceEntity>(t => t.Id, Helper.DbSetting).AsEnumerable(),
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
                OrderField.Ascending<TraceEntity>(t => t.Id, Helper.DbSetting).AsEnumerable(),
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
                OrderField.Ascending<TraceEntity>(t => t.Id, Helper.DbSetting).AsEnumerable(),
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
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name", Helper.DbSetting),
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
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name", Helper.DbSetting),
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
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name", Helper.DbSetting),
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
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name", Helper.DbSetting),
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
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
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
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
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(Helper.DbSetting),
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
