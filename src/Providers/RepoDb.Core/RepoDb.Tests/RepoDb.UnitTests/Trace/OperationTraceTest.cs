#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Verifies operation failures are traced exactly once and propagated unchanged.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class OperationTraceTest
    {
        #region Public Methods

        /// <summary>
        /// Registers the in-memory provider used by operation tests.
        /// </summary>
        /// <param name="context">The test execution context.</param>
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(),
                true);
            DbHelperMapper.Add<CustomTracedDbConnection>(new CustomDbHelper(),
                true);
            StatementBuilderMapper.Add<CustomTracedDbConnection>(new CustomStatementBuilder(),
                true);
        }

        /// <summary>
        /// Restores the default conversion behavior.
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
        }

        /// <summary>
        /// Provides all operation names and both execution modes.
        /// </summary>
        /// <returns>The operation and execution mode combinations.</returns>
        public static IEnumerable<object[]> Operations()
        {
            var operations = new[]
            {
                "Average",
                "AverageAll",
                "BatchQuery",
                "Count",
                "CountAll",
                "Delete",
                "DeleteAll",
                "Exists",
                "Insert",
                "InsertAll",
                "InsertAllBatch",
                "Max",
                "MaxAll",
                "Merge",
                "MergeAll",
                "MergeAllBatch",
                "Min",
                "MinAll",
                "Query",
                "QueryAll",
                "QueryFirst",
                "QueryMultiple",
                "QuerySingle",
                "SkipQuery",
                "Sum",
                "SumAll",
                "Truncate",
                "Update",
                "UpdateAll",
                "UpdateAllBatch",
            };
            foreach (var operation in operations)
            {
                yield return new object[] { operation, false };
                yield return new object[] { operation, true };
            }
        }

        /// <summary>
        /// Verifies a database error reaches both the trace and the caller unchanged.
        /// </summary>
        /// <param name="operation">The operation under test.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Operations))]
        public async Task FailureIsTracedAndRethrown(string operation,
            bool asynchronous)
        {
            var failure = new InvalidOperationException("Command execution failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace();
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                OperationTraceExecutor.ExecuteAsync(connection,
                    trace,
                    operation,
                    asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure,
                thrown);
            AssertFailure(trace,
                failure);
            StringAssert.Contains(thrown.StackTrace,
                nameof(CustomTracedDbCommand),
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies a failing completion observer cannot replace the database error.
        /// </summary>
        /// <param name="operation">The operation under test.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Operations))]
        public async Task CompletionFailurePreservesOriginalError(string operation,
            bool asynchronous)
        {
            var failure = new InvalidOperationException("Command execution failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace
            {
                ThrowOnAfter = new NotSupportedException("Observer failed.")
            };
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                OperationTraceExecutor.ExecuteAsync(connection,
                    trace,
                    operation,
                    asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure,
                thrown);
            AssertFailure(trace,
                failure);
        }

        /// <summary>
        /// Verifies each cancelled command has a completion notification without execution.
        /// </summary>
        /// <param name="operation">The operation under test.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Operations))]
        public async Task SilentCancellationStillCompletesTrace(string operation,
            bool asynchronous)
        {
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace { CancelBefore = true };
            await OperationTraceExecutor.ExecuteAsync(connection,
                trace,
                operation,
                asynchronous).ConfigureAwait(false);
            Assert.AreEqual(0,
                connection.ExecutionCount);
            Assert.AreEqual(trace.BeforeLogs.Count,
                trace.AfterLogs.Count);
            Assert.AreEqual(1,
                trace.AfterLogs.Count);
        }

        /// <summary>
        /// Verifies failures thrown by a before observer are also completed and rethrown.
        /// </summary>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task BeforeFailureIsCompletedAndRethrown(bool asynchronous)
        {
            var failure = new InvalidOperationException("Before observer failed.");
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace { OnBefore = () => throw failure };
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                OperationTraceExecutor.ExecuteAsync(connection,
                    trace,
                    "Insert",
                    asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure,
                thrown);
            AssertFailure(trace,
                failure);
            Assert.AreEqual(0,
                connection.ExecutionCount);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies errors while consuming a multi-query result are traced and rethrown.
        /// </summary>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ReaderFailureIsTracedAndRethrown(bool asynchronous)
        {
            var failure = new InvalidOperationException("Reading failed.");
            using var connection = new CustomTracedDbConnection
            {
                ReaderFactory = () => new CustomFailingDbDataReader(failure)
            };
            var trace = new RecordingTrace();
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                OperationTraceExecutor.ExecuteAsync(connection,
                    trace,
                    "QueryMultiple",
                    asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure,
                thrown);
            AssertFailure(trace,
                failure);
        }

        /// <summary>
        /// Verifies registered observers receive one completion per multi-query command.
        /// </summary>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task GlobalMultiQueryTraceCompletesExactlyOnce(bool asynchronous)
        {
            var previous = GlobalConfiguration.Options.UseRegisteredGlobalTraces;
            using var connection = new CustomTracedDbConnection();
            var local = new RecordingTrace();
            var global = new RecordingTrace();
            GlobalTraceRegistration.Register(global);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            try
            {
                await OperationTraceExecutor.ExecuteAsync(connection,
                    local,
                    "QueryMultiple",
                    asynchronous).ConfigureAwait(false);
                Assert.AreEqual(1,
                    local.AfterLogs.Count);
                Assert.AreEqual(1,
                    global.BeforeLogs.Count);
                Assert.AreEqual(1,
                    global.AfterLogs.Count);
            }
            finally
            {
                GlobalConfiguration.Options.UseRegisteredGlobalTraces = previous;
                GlobalTraceRegistration.GetTracers().Remove(global);
            }
        }

        /// <summary>
        /// Verifies the single-statement multi-query fallback reports command failures.
        /// </summary>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task SingleStatementMultiQueryFailureIsTraced(bool asynchronous)
        {
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomSingleStatementSupportDbSetting(),
                true);
            try
            {
                await FailureIsTracedAndRethrown("QueryMultiple",
                    asynchronous).ConfigureAwait(false);
            }
            finally
            {
                DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(),
                    true);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies all observers are completed even when the local observer throws.
        /// </summary>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        /// <param name="executionFails">Whether the command also fails.</param>
        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public async Task FailingLocalObserverDoesNotSkipGlobalCompletion(bool asynchronous,
            bool executionFails)
        {
            var previous = GlobalConfiguration.Options.UseRegisteredGlobalTraces;
            var failure = new InvalidOperationException("Command failed.");
            var observerError = new NotSupportedException("Observer failed.");
            using var connection = new CustomTracedDbConnection
            {
                Failure = executionFails ? failure : null
            };
            var local = new RecordingTrace { ThrowOnAfter = observerError };
            var global = new RecordingTrace();
            GlobalTraceRegistration.Register(global);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            try
            {
                Func<Task> execute = () => OperationTraceExecutor.ExecuteAsync(connection,
                    local,
                    "Insert",
                    asynchronous);
                Exception thrown = executionFails
                    ? await Assert.ThrowsExactlyAsync<InvalidOperationException>(execute).ConfigureAwait(false)
                    : await Assert.ThrowsExactlyAsync<NotSupportedException>(execute).ConfigureAwait(false);
                Assert.AreSame(executionFails ? failure : observerError,
                    thrown);
                Assert.AreEqual(1,
                    global.AfterLogs.Count);
                Assert.AreSame(executionFails ? failure : null,
                    global.AfterErrors.Single());
            }
            finally
            {
                GlobalConfiguration.Options.UseRegisteredGlobalTraces = previous;
                GlobalTraceRegistration.GetTracers().Remove(global);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies the failed command has one correlated completion and error severity.
        /// </summary>
        /// <param name="trace">The observer recording the execution.</param>
        /// <param name="failure">The expected original exception.</param>
        private static void AssertFailure(RecordingTrace trace,
            Exception failure)
        {
            Assert.AreEqual(1,
                trace.BeforeLogs.Count);
            Assert.AreEqual(1,
                trace.AfterLogs.Count);
            Assert.AreSame(failure,
                trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error,
                trace.AfterLevels.Single());
        }

        #endregion
    }
}

