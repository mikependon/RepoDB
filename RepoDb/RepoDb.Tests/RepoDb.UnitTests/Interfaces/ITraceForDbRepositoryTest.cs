using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbRepositoryTest
    {
        private readonly IStatementBuilder m_statementBuilder = new SqlServerStatementBuilder();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(CustomDbConnectionForDbRepositoryITrace), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForDbRepositoryITrace), Helper.DbValidator, true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbRepositoryITrace), new DbRepositoryCustomDbHelper(), true);
            DbOperationMapper.Add(typeof(CustomDbConnectionForDbRepositoryITrace), new DbRepositoryCustomDbOperationProvider(), true);
        }

        #region SubClasses

        private class CustomDbConnectionForDbRepositoryITrace : CustomDbConnection { }

        private class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DbRepositoryCustomDbHelper : IDbHelper
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
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null)
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
                    new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                    new DbField("Name", false, false, true, typeof(string), null, null, null, null)
                });
            }
        }

        private class DbRepositoryCustomDbOperationProvider : IDbOperation
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
        public void TestDbRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsert<TraceEntity>(entities);

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsert<TraceEntity>(entities);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsertAsync<TraceEntity>(entities).Wait();

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsertAsync<TraceEntity>(entities).Wait();

            // Assert
            trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync<TraceEntity>((object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync<TraceEntity>((object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync<TraceEntity>(0).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync<TraceEntity>(0).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 }).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 }).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert(ClassMappedNameCache.Get<TraceEntity>(), new { Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert(ClassMappedNameCache.Get<TraceEntity>(), new { Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync<TraceEntity>(new TraceEntity { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync<TraceEntity>(new TraceEntity { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name")).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name")).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAll<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAll<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAllAsync<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAllAsync<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAsync<TraceEntity>(te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAsync<TraceEntity>(te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAllAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync<TraceEntity>().Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>()).Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                new { Id = 1 });

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                new { Id = 1 });

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                new { Id = 1 }).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                new { Id = 1 }).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAll<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAll<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAllAsync<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAllAsync<TraceEntity>(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryITrace>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
