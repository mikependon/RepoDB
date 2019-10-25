using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbRepositoryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(TraceDbConnection), new CustomDbSetting(), true);
            DbValidatorMapper.Add(typeof(TraceDbConnection), new CustomDbValidator(), true);
            DbOperationMapper.Add(typeof(TraceDbConnection), new CustomDbOperation(), true);
            DbHelperMapper.Add(typeof(TraceDbConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(TraceDbConnection), new CustomStatementBuilder(), true);
        }

        #region SubClasses

        private class TraceDbConnection : CustomDbConnection { }

        private class TraceEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Average

        #region Average

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Average<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Average<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Average(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Average(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region AverageAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region AverageAll

        #region AverageAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name")).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Max

        #region Max

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMax()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Max<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMax()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Max<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Max(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Max(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region MaxAll

        #region MaxAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Once);
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Min

        #region Min

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMin()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Min<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMin()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Min<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Min(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Min(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region MinAll

        #region MinAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Once);
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1).Wait();

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Sum

        #region Sum

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSum()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Sum<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSum()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Sum<TraceEntity>(e => e.Id,
                (object)null);

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAsync<TraceEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region SumAll

        #region SumAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAll<TraceEntity>(e => e.Id);

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id"));

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAllAsync<TraceEntity>(e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new Field("Id")).Wait();

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Once);
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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

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
            var repository = new DbRepository<TraceDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object);

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(), new[] { new { Id = 1, Name = "Name" } }).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
