using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
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

        private class TraceEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class ErroneousCancellationTrace : ITrace
        {
            public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
            {
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

        private class SilentCancellationTrace : ITrace
        {
            public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
            {
                AfterExecutionInvocationCount++;
            }

            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
                CancellationToken cancellationToken = default)
            {
                AfterExecutionInvocationCount++;
                return Task.CompletedTask;
            }

            public void BeforeExecution(CancellableTraceLog log)
            {
                log.Cancel(false);
                BeforeExecutionInvocationCount++;
            }

            public Task BeforeExecutionAsync(CancellableTraceLog log,
                CancellationToken cancellationToken = default)
            {
                log.Cancel(false);
                BeforeExecutionInvocationCount++;
                return Task.CompletedTask;
            }

            #region Properties

            public int BeforeExecutionInvocationCount { get; private set; }

            public int AfterExecutionInvocationCount { get; private set; }

            #endregion
        }

        private class PropertyValidatorTrace : ITrace
        {
            public bool IsValid { get; private set; }

            public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
            {
                ValidateResultTraceLog(log);
                IsValid = true;
            }

            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
                CancellationToken cancellationToken = default)
            {
                ValidateResultTraceLog(log);
                IsValid = true;
                return Task.CompletedTask;
            }

            public void BeforeExecution(CancellableTraceLog log)
            {
                ValidateCancellableTraceLog(log);
                IsValid = true;
            }

            public Task BeforeExecutionAsync(CancellableTraceLog log,
                CancellationToken cancellationToken = default)
            {
                ValidateCancellableTraceLog(log);
                IsValid = true;
                return Task.CompletedTask;
            }

            private void ValidateResultTraceLog<TResult>(ResultTraceLog<TResult> log)
            {
                if (log == null)
                {
                    throw new NullReferenceException(nameof(log));
                }
                if (string.IsNullOrEmpty(log.Key))
                {
                    throw new InvalidOperationException(nameof(log.Key));
                }
                if (log.ExecutionTime == TimeSpan.MinValue)
                {
                    throw new InvalidOperationException(nameof(log.ExecutionTime));
                }
                if (log.BeforeExecutionLog == null)
                {
                    throw new InvalidOperationException(nameof(log.BeforeExecutionLog));
                }
                //if (log.Result == null)
                //{
                //    throw new InvalidOperationException(nameof(log.Result));
                //}
            }

            private void ValidateCancellableTraceLog(CancellableTraceLog log)
            {
                if (log == null)
                {
                    throw new NullReferenceException(nameof(log));
                }
                if (log.SessionId == Guid.Empty)
                {
                    throw new InvalidOperationException(nameof(log.SessionId));
                }
                if (string.IsNullOrEmpty(log.Key))
                {
                    throw new InvalidOperationException(nameof(log.Key));
                }
                if (log.Parameters == null)
                {
                    throw new InvalidOperationException(nameof(log.Parameters));
                }
                if (log.StartTime == DateTime.MinValue)
                {
                    throw new InvalidOperationException(nameof(log.StartTime));
                }
            }
        }

        #endregion

        #region Cancelled

        #region ExecuteNonQuery

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnExecuteNonQueryCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQuery("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteNonQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQueryAsync("", trace: new ErroneousCancellationTrace())
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
                .ExecuteQuery("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryAsync("", trace: new ErroneousCancellationTrace())
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
                .ExecuteScalar("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExecuteScalarAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalarAsync("", trace: new ErroneousCancellationTrace())
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
                .ExecuteQueryMultiple("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionExecuteQueryMultipleAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultipleAsync("", trace: new ErroneousCancellationTrace())
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
                .Average("", (Field)null, (object)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnAverageAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .AverageAsync("", (Field)null, (object)null, trace: new ErroneousCancellationTrace())
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
                .AverageAll("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnAverageAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .AverageAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .BatchQuery("", 0, 100, null, (object)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBatchQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .BatchQueryAsync("", 0, 100, null, (object)null, trace: new ErroneousCancellationTrace())
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
                .Count("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnCountAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .CountAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .CountAll("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnCountAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .CountAllAsync("", trace: new ErroneousCancellationTrace())
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
                .Delete("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDeleteAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .DeleteAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .DeleteAll("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnDeleteAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .DeleteAllAsync("", trace: new ErroneousCancellationTrace())
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
                .Exists("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExistsAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExistsAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .Insert("", null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .InsertAsync("", null, trace: new ErroneousCancellationTrace())
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
                new { Id = 1 }
            };


            // Act
            connection
                .InsertAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnInsertAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 }
            };

            // Act
            connection
                .InsertAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .InsertAllAsync("", entities, trace: new ErroneousCancellationTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnInsertAllAsyncMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .InsertAllAsync("", entities, trace: new ErroneousCancellationTrace())
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
                .Max("", (Field)null, (object)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MaxAsync("", (Field)null, (object)null, trace: new ErroneousCancellationTrace())
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
                .MaxAll("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MaxAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .Merge("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MergeAsync("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace())
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
                new { Id = 1 }
            };

            // Act
            connection
                .MergeAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 }
            };

            // Act
            connection
                .MergeAllAsync("", entities, trace: new ErroneousCancellationTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnMergeAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .MergeAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMergeAllMultipleEntitiesAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .MergeAllAsync("", entities, trace: new ErroneousCancellationTrace())
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
                .Min("", (Field)null, (object)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MinAsync("", (Field)null, (object)null, trace: new ErroneousCancellationTrace())
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
                .MinAll("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .MinAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .Query("", (QueryField)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryAsync("", (QueryField)null, trace: new ErroneousCancellationTrace())
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
                .QueryAll("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnQueryAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .QueryAllAsync("", trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                    trace: new ErroneousCancellationTrace());
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
                    trace: new ErroneousCancellationTrace())
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
                .Sum("", (Field)null, (object)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSumAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .SumAsync("", (Field)null, (object)null, trace: new ErroneousCancellationTrace())
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
                .SumAll("", (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSumAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .SumAllAsync("", (Field)null, trace: new ErroneousCancellationTrace())
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
                .Truncate("", trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTruncateAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .TruncateAsync("", trace: new ErroneousCancellationTrace())
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
                .Update("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();

            // Act
            connection
                .UpdateAsync("", new { Id = 1 }, (Field)null, trace: new ErroneousCancellationTrace())
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
                new { Id = 1 }
            };

            // Act
            connection
                .UpdateAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAllAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 }
            };

            // Act
            connection
                .UpdateAllAsync("", entities, trace: new ErroneousCancellationTrace())
                .Wait();
        }

        [TestMethod, ExpectedException(typeof(CancelledExecutionException))]
        public void ThrowExceptionOnUpdateAllMultipleEntitiesCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .UpdateAll("", entities, trace: new ErroneousCancellationTrace());
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnUpdateAllMultipleEntitiesAsyncCancelledOperation()
        {
            // Prepare
            var connection = new TraceDbConnection();
            var entities = new[]
            {
                new { Id = 1 },
                new { Id = 2 },
                new { Id = 3 }
            };

            // Act
            connection
                .UpdateAllAsync("", entities, trace: new ErroneousCancellationTrace())
                .Wait();
        }

        #endregion

        #endregion

        #region Silent Cancellation

        #region ExecuteNonQuery

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteNonQuery()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQuery("", trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteNonQueryAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQueryAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteQuery()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQuery("", trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteQueryAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteScalar()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalar("", trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteScalarAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalarAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region ExecuteQueryMultiple

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteQueryMultiple()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultiple("", trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExecuteQueryMultipleAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultipleAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region Average

        #region Average

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverage()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Average<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }


        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region AverageAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region AverageAll

        #region AverageAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForBatchQuery()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForBatchQueryAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCount()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace,
                where: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace,
                where: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDelete()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Exists

        #region Exists

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExists()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists<TraceEntity>(trace: trace,
                what: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExistsViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExistsAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync<TraceEntity>(trace: trace,
                what: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsert()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Max

        #region Max

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMax()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Max<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region MaxAll

        #region MaxAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMerge()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Min

        #region Min

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMin()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Min<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region MinAll

        #region MinAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQuery()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        #region T2

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT2()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T3

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT3()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T4

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT4()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T5

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT5()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T6

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT6()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T7

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleForT7()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region T2

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT2()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T3

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT3()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T4

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT4()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T5

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT5()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T6

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT6()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region T7

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForQueryMultipleAsyncForT7()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #endregion

        #region Sum

        #region Sum

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSum()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region SumAll

        #region SumAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForTruncate()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForTruncateViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForTruncateAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdate()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
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
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
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
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAll()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAllViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAllAsync()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        [TestMethod]
        public void TestDbConnectionTraceSilentCancellationForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new SilentCancellationTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.AreEqual(1, trace.BeforeExecutionInvocationCount);
            Assert.AreEqual(0, trace.AfterExecutionInvocationCount);
        }

        #endregion

        #endregion

        #endregion

        #region Properties Validation

        #region ExecuteNonQuery

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteNonQuery()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQuery("", trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteNonQueryAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteNonQueryAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region ExecuteQuery

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteQuery()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQuery("", trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteQueryAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteScalar()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalar("", trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteScalarAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteScalarAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region ExecuteQueryMultiple

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteQueryMultiple()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultiple("", trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExecuteQueryMultipleAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection
                .ExecuteQueryMultipleAsync("", trace: trace)
                .Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region Average

        #region Average

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverage()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Average<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }


        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Average(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region AverageAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region AverageAll

        #region AverageAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForAverageAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionTracePropertiesForBatchQuery()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForBatchQueryAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCount()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace,
                where: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace,
                where: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDelete()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll<TraceEntity>(trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Exists

        #region Exists

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExists()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists<TraceEntity>(trace: trace,
                what: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExistsViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Exists(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExistsAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync<TraceEntity>(trace: trace,
                what: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForExistsAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<TraceEntity>(),
                what: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsert()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(
                new TraceEntity { Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Max

        #region Max

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMax()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Max<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Max(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region MaxAll

        #region MaxAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMaxAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMerge()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(
                new TraceEntity { Id = 1, Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new { Id = 1, Name = "Name" },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMergeAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Min

        #region Min

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMin()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Min<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Min(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region MinAll

        #region MinAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForMinAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQuery()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        #region T2

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT2()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T3

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT3()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T4

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT4()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T5

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT5()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T6

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT6()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T7

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleForT7()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region T2

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT2()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T3

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT3()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T4

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT4()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T5

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT5()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T6

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT6()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region T7

        [TestMethod]
        public void TestDbConnectionTracePropertiesForQueryMultipleAsyncForT7()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                te => te.Id == 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #endregion

        #region Sum

        #region Sum

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSum()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Sum(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync<TraceEntity>(trace: trace,
                field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                where: (object)null,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region SumAll

        #region SumAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll<TraceEntity>(trace: trace,
                field: e => e.Id);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync<TraceEntity>(trace: trace,
                field: e => e.Id).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForSumAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                field: new Field("Id"),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbConnectionTracePropertiesForTruncate()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForTruncateViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForTruncateAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdate()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
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
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                what: 1,
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
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
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAll()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAllViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace);

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAllAsync()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync<TraceEntity>(
                new[] { new TraceEntity { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        [TestMethod]
        public void TestDbConnectionTracePropertiesForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var trace = new PropertyValidatorTrace();
            var connection = new TraceDbConnection();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Id = 1, Name = "Name" } },
                trace: trace).Wait();

            // Assert
            Assert.IsTrue(trace.IsValid);
        }

        #endregion

        #endregion

        #endregion
    }
}