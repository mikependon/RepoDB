#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Contains the tests of the tracing of the <c>ExecuteNonQuery</c> and <c>ExecuteNonQueryAsync</c> extension methods: what the traces are told before and after
    /// an execution, that they are always told that it has completed (with the error if it has failed), and that an error is never hidden from the caller.
    /// </summary>
    [TestClass]
    public class ExecuteNonQueryTraceTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that the tests execute.
        /// </summary>
        private const string Statement = "UPDATE [Table] SET [Value] = 1";

        #endregion

        #region Public Methods

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<CustomTracedDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<CustomTracedDbConnection>(new CustomStatementBuilder(), true);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
        }

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryTellsTheTraceBeforeAndAfterTheExecution(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection { RowsAffected = 3 };
            var trace = new RecordingTrace();

            // Act
            var result = await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(3, result);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            var after = trace.GetAfterLog<int>();
            Assert.AreEqual(3, after.Result);
            Assert.IsNull(after.Error);
            Assert.AreEqual(TraceLevel.Info, after.Level);
            Assert.AreSame(trace.BeforeLogs[0], after.BeforeExecutionLog);
            Assert.AreEqual(trace.BeforeLogs[0].SessionId, after.SessionId);
            Assert.AreEqual(TraceKeys.ExecuteNonQuery, after.Key);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryCapturesTheContextOfTheExecution(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();
            var threadId = Environment.CurrentManagedThreadId;

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var before = trace.BeforeLogs.Single();
            Assert.AreEqual(TraceLevel.Info, before.Level);
            Assert.AreEqual(threadId, before.ThreadId);
            Assert.AreEqual(Environment.MachineName, before.MachineName);
            Assert.AreEqual(TraceEnvironment.IpAddress, before.IpAddress);
            Assert.AreEqual(nameof(CustomTracedDbConnection), before.ConnectionType);
            Assert.AreEqual("TestDatabase", before.Database);
            Assert.IsNull(before.TraceId);
            Assert.IsNull(before.SpanId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryGivesTheAfterLogTheSameContextAsTheBeforeLog(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();
            using var activity = StartActivity(ActivityIdFormat.W3C);

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var before = trace.BeforeLogs.Single();
            var after = trace.GetAfterLog<int>();
            Assert.AreEqual(before.ThreadId, after.ThreadId);
            Assert.AreEqual(before.MachineName, after.MachineName);
            Assert.AreEqual(before.IpAddress, after.IpAddress);
            Assert.AreEqual(before.ConnectionType, after.ConnectionType);
            Assert.AreEqual(before.Database, after.Database);
            Assert.AreEqual(before.TraceId, after.TraceId);
            Assert.AreEqual(before.SpanId, after.SpanId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryCapturesTheTraceAndTheSpanOfTheActiveActivity(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();
            using var activity = StartActivity(ActivityIdFormat.W3C);

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var before = trace.BeforeLogs.Single();
            Assert.AreEqual(activity.TraceId.ToString(), before.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(), before.SpanId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryDoesNotCreateASpanSoTheOperationsOfTheSameActivityShareIt(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();
            using var activity = StartActivity(ActivityIdFormat.W3C);

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(2, trace.BeforeLogs.Count);
            Assert.AreEqual(trace.BeforeLogs[0].TraceId, trace.BeforeLogs[1].TraceId);
            Assert.AreEqual(trace.BeforeLogs[0].SpanId, trace.BeforeLogs[1].SpanId);
            Assert.AreNotEqual(trace.BeforeLogs[0].SessionId, trace.BeforeLogs[1].SessionId);
            Assert.AreSame(activity, Activity.Current);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryDoesNotCaptureTheIdentifiersOfAnActivityThatIsNotW3C(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();
            using var activity = StartActivity(ActivityIdFormat.Hierarchical);

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var before = trace.BeforeLogs.Single();
            Assert.IsNull(before.TraceId);
            Assert.IsNull(before.SpanId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryTellsTheTraceThatTheExecutionHasFailedAndThrowsTheSameErrorToTheCaller(bool asynchronous)
        {
            // Setup
            var failure = new InvalidOperationException("The statement failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace();

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            var after = trace.GetAfterLog<int>();
            Assert.AreSame(failure, after.Error);
            Assert.AreEqual(TraceLevel.Error, after.Level);
            Assert.AreEqual(0, after.Result);
            Assert.AreEqual(trace.BeforeLogs[0].SessionId, after.SessionId);
            Assert.AreEqual(nameof(CustomTracedDbConnection), after.ConnectionType);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryKeepsTheStackTraceOfTheError(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection { Failure = new InvalidOperationException("The statement failed.") };

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(connection, new RecordingTrace(), asynchronous)).ConfigureAwait(false);
            StringAssert.Contains(thrown.StackTrace, nameof(CustomTracedDbCommand.ExecuteNonQuery), StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryDoesNotLetAFailingTraceHideTheError(bool asynchronous)
        {
            // Setup
            var failure = new InvalidOperationException("The statement failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace { ThrowOnAfter = new NotSupportedException("The trace failed.") };

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(1, trace.AfterLogs.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryStillThrowsAFailingTraceWhenTheExecutionSucceeds(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace { ThrowOnAfter = new NotSupportedException("The trace failed.") };

            // Act/Assert
            await Assert.ThrowsExactlyAsync<NotSupportedException>(() => Execute(connection, trace, asynchronous)).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryDoesNotExecuteWhenCancelledButStillReportsCompletion(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection { RowsAffected = 5 };
            var trace = new RecordingTrace { CancelBefore = true };

            // Act
            var result = await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(0, connection.ExecutionCount);
            Assert.AreEqual(1, trace.AfterLogs.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryWithoutATraceExecutesAndThrowsTheError(bool asynchronous)
        {
            // Setup
            var failure = new InvalidOperationException("The statement failed.");
            using var succeeding = new CustomTracedDbConnection { RowsAffected = 2 };
            using var failing = new CustomTracedDbConnection { Failure = failure };

            // Act
            var result = await Execute(succeeding, null, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(2, result);
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(failing, null, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure, thrown);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryMeasuresTheLengthOfTheExecution(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection { Delay = TimeSpan.FromMilliseconds(60) };
            var trace = new RecordingTrace();

            // Act
            await Execute(connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var executionTime = trace.GetAfterLog<int>().ExecutionTime;
            Assert.IsTrue(executionTime >= TimeSpan.FromMilliseconds(40), $"The execution time was {executionTime}.");
            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(30), $"The execution time was {executionTime}.");
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ExecuteNonQueryMeasuresTheLengthOfAFailedExecution(bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection
            {
                Delay = TimeSpan.FromMilliseconds(60),
                Failure = new InvalidOperationException("The statement failed.")
            };
            var trace = new RecordingTrace();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.IsTrue(trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(40));
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsyncGivesTheTraceTheCancellationTokenWhenTheExecutionSucceeds()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace();

            // Act
            await Execute(connection, trace, true, source.Token).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(source.Token, trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsyncStillTellsTheTraceAboutAFailureWhenTheTokenHasBeenCancelled()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace { OnBefore = source.Cancel };

            // Act/Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => Execute(connection, trace, true, source.Token)).ConfigureAwait(false);
            var after = trace.GetAfterLog<int>();
            Assert.IsInstanceOfType<OperationCanceledException>(after.Error);
            Assert.AreEqual(TraceLevel.Error, after.Level);
            Assert.AreEqual(CancellationToken.None, trace.AfterTokens.Single());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the statement with the sync or the async extension method.
        /// </summary>
        /// <param name="connection">The connection to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation. The task result is the number of rows affected.</returns>
        private static async Task<int> Execute(IDbConnection connection,
            ITrace trace,
            bool asynchronous,
            CancellationToken cancellationToken = default)
        {
            return asynchronous
                ? await connection.ExecuteNonQueryAsync(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false)
                : connection.ExecuteNonQuery(Statement, trace: trace);
        }

        /// <summary>
        /// Starts an activity of the format, which becomes the current one.
        /// </summary>
        /// <param name="format">The format of the identifiers of the activity.</param>
        /// <returns>The activity that was started.</returns>
        private static Activity StartActivity(ActivityIdFormat format)
        {
            var activity = new Activity("Payroll");
            activity.SetIdFormat(format);
            return activity.Start();
        }

        #endregion
    }
}
