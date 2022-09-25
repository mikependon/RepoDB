using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Trace
{
    [TestClass]
    public class CancellableTraceLogTest
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

        private class CancelTrace : ITrace
        {
            public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
            {
                // Do nothing
            }

            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
                CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public void BeforeExecution(CancellableTraceLog log)
            {
                log.Cancel(true);
            }

            public Task BeforeExecutionAsync(CancellableTraceLog log,
                CancellationToken cancellationToken = default)
            {
                log.Cancel(true);
                return Task.CompletedTask;
            }
        }

        #endregion

        #region Methods

        #region ExecuteNonQuery

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExecuteNonQueryCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQuery("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteNonQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQueryAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region ExecuteQuery

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExecuteQueryCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQuery("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region ExecuteScalar

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExecuteScalarCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalar("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteScalarAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalarAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region ExecuteQueryMultiple

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExecuteQueryMultipleCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultiple("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionExecuteQueryMultipleAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultipleAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Average

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnAverageCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Average("", (Field)null, (object)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnAverageAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .AverageAsync("", (Field)null, (object)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region AverageAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnAverageAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .AverageAll("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnAverageAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .AverageAllAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region BatchQuery

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnBatchQueryCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .BatchQuery("", 0, 100, null, (object)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBatchQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .BatchQueryAsync("", 0, 100, null, (object)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Count

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnCountCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Count("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnCountAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .CountAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region CountAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnCountAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .CountAll("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnCountAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .CountAllAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Delete

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnDeleteCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Delete("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDeleteAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .DeleteAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region DeleteAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnDeleteAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .DeleteAll("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDeleteAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .DeleteAllAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Exists

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExistsCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Exists("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExistsAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExistsAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Insert

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnInsertCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Insert("", null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .InsertAsync("", null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region InsertAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnInsertAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };


            // Act
            connection
                .InsertAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnInsertAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .InsertAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };

            // Act
            connection
                .InsertAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAllAsyncMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .InsertAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Max

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMaxCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Max("", (Field)null, (object)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MaxAsync("", (Field)null, (object)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region MaxAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMaxAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MaxAll("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MaxAllAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Merge

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMergeCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Merge("", null, (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MergeAsync("", null, (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region MergeAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMergeAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };

            // Act
            connection
                .MergeAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };

            // Act
            connection
                .MergeAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMergeAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .MergeAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAllMultipleEntitiesAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .MergeAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Min

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMinCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Min("", (Field)null, (object)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MinAsync("", (Field)null, (object)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region MaxAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMinAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MinAll("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MinAllAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Query

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Query("", (QueryField)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryAsync("", (QueryField)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region QueryAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryAll("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryAllAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region QueryMultiple

        #region T2

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT2CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT2AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region T3

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT3CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT3AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region T4

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT4CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT4AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region T5

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT5CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT5AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region T6

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT6CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT6AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region T7

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnQueryMultipleForT7CancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultiple("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryMultipleForT7AsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryMultipleAsync("", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    "", (QueryField)null,
                    trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #endregion

        #region Sum

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnSumCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Sum("", (Field)null, (object)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSumAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .SumAsync("", (Field)null, (object)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region SumAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnSumAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .SumAll("", (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSumAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .SumAllAsync("", (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Truncate

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnTruncateCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Truncate("", trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTruncateAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .TruncateAsync("", trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region Update

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnUpdateCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .Update("", null, (Field)null, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .UpdateAsync("", null, (Field)null, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #region UpdateAll

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnUpdateAllCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };

            // Act
            connection
                .UpdateAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object()
            };

            // Act
            connection
                .UpdateAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnUpdateAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .UpdateAll("", entities, trace: new CancelTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAllMultipleEntitiesAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new object(),
                new object(),
                new object()
            };

            // Act
            connection
                .UpdateAllAsync("", entities, trace: new CancelTrace())
                .Wait();
        }

        #endregion

        #endregion
    }
}