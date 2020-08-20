using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(TraceDbConnection), new CustomDbSetting(), true);
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
        public void TestDbConnectionTraceForBeforeAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Average<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverage()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Average<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterAverage(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverage(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterAverageAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeAverageAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region BulkInsert

        //#region BulkInsert

        //[TestMethod]
        //public void TestDbConnectionTraceForBeforeBulkInsert()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsert<TraceEntity>(entities,
        //        trace: trace.Object);

        //    // Assert
        //    trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        //}

        //[TestMethod]
        //public void TestDbConnectionTraceForAfterBulkInsert()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsert<TraceEntity>(entities,
        //        trace: trace.Object);

        //    // Assert
        //    trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        //}

        //#endregion

        //#region BulkInsertAsync

        //[TestMethod]
        //public void TestDbConnectionTraceForBeforeBulkInsertAsync()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsertAsync<TraceEntity>(entities,
        //        trace: trace.Object).Wait();

        //    // Assert
        //    trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        //}

        //[TestMethod]
        //public void TestDbConnectionTraceForAfterBulkInsertAsync()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsertAsync<TraceEntity>(entities,
        //        trace: trace.Object).Wait();

        //    // Assert
        //    trace.Verify(t => t.AfterBulkInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        //}

        //#endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeDeleteAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterDeleteAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Exists

        #region Exists

        [TestMethod]
        public void TestDbConnectionTraceForBeforeExists()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists<TraceEntity>(trace: trace.Object,
                what: (object)null);

            // Assert
            trace.Verify(t => t.BeforeExists(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterExists()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists<TraceEntity>(trace: trace.Object,
                what: (object)null);

            // Assert
            trace.Verify(t => t.AfterExists(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeExistsViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeExists(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterExistsViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterExists(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeExistsAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync<TraceEntity>(trace: trace.Object,
                what: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeExists(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterExistsAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync<TraceEntity>(trace: trace.Object,
                what: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterExists(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeExists(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterExists(It.IsAny<TraceLog>()), Times.Exactly(1));
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
            var connection = new TraceDbConnection();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Max<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMax()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Max<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMax(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMax(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterMaxAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMaxAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterMergeAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMergeAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Min<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMin()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Min<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterMin(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMin(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterMinAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeMinAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Sum<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSum()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            trace.Verify(t => t.AfterSum(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeSum(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id).Wait();

            // Assert
            trace.Verify(t => t.AfterSumAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeSumAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

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
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

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
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

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
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

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
                trace: trace.Object).Wait();

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
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

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
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.BeforeUpdateAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object).Wait();

            // Assert
            trace.Verify(t => t.AfterUpdateAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}