using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Enumerations;
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
    public class ITraceForBaseRepositoryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(CustomDbConnectionForBaseRepositoryITrace), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForBaseRepositoryITrace), Helper.DbValidator, true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForBaseRepositoryITrace), new BaseRepositoryCustomDbHelper(), true);
            DbOperationMapper.Add(typeof(CustomDbConnectionForBaseRepositoryITrace), new BaseRepositoryCustomDbOperationProvider(), true);
        }

        #region SubClasses

        private class CustomDbConnectionForBaseRepositoryITrace : CustomDbConnection { }

        private class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TraceEntityRepository : BaseRepository<TraceEntity, CustomDbConnectionForBaseRepositoryITrace>
        {
            public TraceEntityRepository(ITrace trace) :
                base("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace,
                Helper.StatementBuilder)
            { }
        }

        private class BaseRepositoryCustomDbHelper : IDbHelper
        {
            public IResolver<string, Type> DbTypeResolver { get; set; }

            public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                if (tableName == ClassMappedNameCache.Get<TraceEntity>())
                {
                    return new[]
                    {
                        new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                        new DbField("Name", false, false, true, typeof(string), null, null, null, null)
                    };
                }
                return null;
            }

            public Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null) where TDbConnection : IDbConnection
            {
                if (tableName == ClassMappedNameCache.Get<TraceEntity>())
                {
                    return Task.FromResult<IEnumerable<DbField>>(new[]
                    {
                        new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                        new DbField("Name", false, false, true, typeof(string), null, null, null, null)
                    });
                }
                return null;
            }
        }

        private class BaseRepositoryCustomDbOperationProvider : IDbOperation
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
        public void TestBaseRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQuery(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQuery(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region BatchQueryAsnc

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQueryAsync(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQueryAsync(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
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
            var repository = new TraceEntityRepository(trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsert(entities);

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsert(entities);

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
            var repository = new TraceEntityRepository(trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsertAsync(entities).Wait();

            // Assert
            trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBulkInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);
            var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

            // Act
            repository.BulkInsertAsync(entities).Wait();

            // Assert
            trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Count((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Count((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAsync((object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAsync((object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAll();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAll();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAllAsync().Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAllAsync().Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Delete(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Delete(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAsync(0).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAsync(0).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAll();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAll();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAllAsync().Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAllAsync().Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Insert(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Insert(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAsync(new TraceEntity { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAsync(new TraceEntity { Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAll(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAll(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAllAsync(new[] { new TraceEntity { Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAllAsync(new[] { new TraceEntity { Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Merge(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Merge(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAsync(new TraceEntity { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAsync(new TraceEntity { Id = 1, Name = "Name" }).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAll(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAll(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAllAsync(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAllAsync(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAsync(te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAsync(te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAll();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAll();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAllAsync();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAllAsync();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Truncate();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Truncate();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.TruncateAsync().Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.TruncateAsync().Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Update(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Update(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAsync(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAsync(
                new TraceEntity { Id = 1, Name = "Name" },
                whereOrPrimaryKey: 1).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAll(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAll(new[] { new TraceEntity { Id = 1, Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAllAsync(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAllAsync(new[] { new TraceEntity { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
