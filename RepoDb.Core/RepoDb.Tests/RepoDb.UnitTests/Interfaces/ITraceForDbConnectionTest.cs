using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add<TraceDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<TraceDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<TraceDbConnection>(new CustomStatementBuilder(), true);
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region AverageAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterAverageAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterAverageAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>()), Times.Exactly(1));
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
        //    trace.Verify(t => t.AfterBulkInsert(It.IsAny<ResultTraceLog>()), Times.Exactly(1));
        //}

        //#endregion

        //#region BulkInsertAsync

        //[TestMethod]
        //public async Task TestDbConnectionTraceForBeforeBulkInsertAsync()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsertAsync<TraceEntity>(entities,
        //        trace: trace.Object);

        //    // Assert
        //    trace.Verify(t => t.BeforeBulkInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        //}

        //[TestMethod]
        //public async Task TestDbConnectionTraceForAfterBulkInsertAsync()
        //{
        //    // Prepare
        //    var trace = new Mock<ITrace>();
        //    var connection = new TraceDbConnection();
        //    var entities = new[] { new TraceEntity() { Id = 1, Name = "Name" } };

        //    // Act
        //    connection.BulkInsertAsync<TraceEntity>(entities,
        //        trace: trace.Object);

        //    // Assert
        //    trace.Verify(t => t.AfterBulkInsert(It.IsAny<ResultTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<long>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<long>>()), Times.Exactly(1));
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<long>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<long>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<long>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<long>>()), Times.Exactly(1));
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<long>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<long>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Actawait 
            await connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterDeleteAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<bool>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<bool>>()), Times.Exactly(1));
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeExistsAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.ExistsAsync<TraceEntity>(trace: trace.Object,
                what: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterExistsAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.ExistsAsync<TraceEntity>(trace: trace.Object,
                what: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<bool>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<bool>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMaxAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMaxAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMergeAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMinAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMinAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMinAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryAllAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<IEnumerable<TraceEntity>>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        #region T2

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT2()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT2()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #region T3

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT3()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT3()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #region T4

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT4()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT4()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #region T5

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT5()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT5()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #region T6

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT6()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT6()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #region T7

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleForT7()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleForT7()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>,
                IEnumerable<TraceEntity>>>>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region T2

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT2()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT2()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #region T3

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT3()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT3()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #region T4

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT4()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT4()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #region T5

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT5()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT5()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #region T6

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT6()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT6()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #region T7

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeQueryMultipleAsyncForT7()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterQueryMultipleAsyncForT7()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<Tuple<IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>,
                    IEnumerable<TraceEntity>>>>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterSumAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id,
                where: (object)null);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterSumAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<object>>()), Times.Exactly(1));
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterSumAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAllAsync<TraceEntity>(trace: trace.Object,
                field: e => e.Id);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<object>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.TruncateAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.TruncateAsync<TraceEntity>(trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
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
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
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
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
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
            trace.Verify(t =>
                t.AfterExecution(It.IsAny<ResultTraceLog<int>>()), Times.Exactly(1));
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterUpdateAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForBeforeUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.BeforeExecutionAsync(It.IsAny<CancellableTraceLog>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task TestDbConnectionTraceForAfterUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new TraceDbConnection();

            // Act
            await connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace.Object);

            // Assert
            trace.Verify(t =>
                t.AfterExecutionAsync(It.IsAny<ResultTraceLog<int>>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}