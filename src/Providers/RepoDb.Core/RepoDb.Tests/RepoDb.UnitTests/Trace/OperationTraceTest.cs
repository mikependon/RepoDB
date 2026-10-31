#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Options;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Verifies each public operation with focused, explicitly named trace tests.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class OperationTraceTest
    {
        #region Public Methods

        #region Average

        [TestMethod]
        public async Task TestAverageTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestAverageTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestAverageTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestAverageTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestAverageTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestAverageTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestAverageAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region AverageAll

        [TestMethod]
        public async Task TestAverageAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestAverageAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestAverageAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestAverageAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestAverageAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestAverageAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region BatchQuery

        [TestMethod]
        public async Task TestBatchQueryTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBatchQueryTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestBatchQueryTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestBatchQueryTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestBatchQueryTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestBatchQueryTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestBatchQueryTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBatchQueryTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Count

        [TestMethod]
        public async Task TestCountTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestCountTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestCountTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestCountTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestCountTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCountTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestCountTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestCountTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestCountAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestCountAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestCountAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestCountAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestCountAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region CountAll

        [TestMethod]
        public async Task TestCountAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestCountAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestCountAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestCountAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestCountAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestCountAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestCountAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Delete

        [TestMethod]
        public async Task TestDeleteTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestDeleteTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestDeleteTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestDeleteTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestDeleteTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDeleteTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestDeleteAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public async Task TestDeleteAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestDeleteAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestDeleteAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestDeleteAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDeleteAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Exists

        [TestMethod]
        public async Task TestExistsTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExistsTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestExistsTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestExistsTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestExistsTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestExistsTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExistsTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestExistsTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestExistsTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExistsTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExistsTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExistsAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestExistsAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Insert

        [TestMethod]
        public async Task TestInsertTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public async Task TestInsertAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region InsertAllBatch

        [TestMethod]
        public async Task TestInsertAllBatchTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertAllBatchTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Max

        [TestMethod]
        public async Task TestMaxTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMaxTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMaxTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMaxTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMaxTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMaxTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMaxAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region MaxAll

        [TestMethod]
        public async Task TestMaxAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMaxAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMaxAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMaxAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMaxAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMaxAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Merge

        [TestMethod]
        public async Task TestMergeTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public async Task TestMergeAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region MergeAllBatch

        [TestMethod]
        public async Task TestMergeAllBatchTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeAllBatchTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Min

        [TestMethod]
        public async Task TestMinTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMinTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMinTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMinTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMinTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMinTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMinTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMinTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMinAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMinAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMinAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMinAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMinAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region MinAll

        [TestMethod]
        public async Task TestMinAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMinAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMinAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMinAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMinAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMinAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestMinAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Query

        [TestMethod]
        public async Task TestQueryTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public async Task TestQueryAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region QueryFirst

        [TestMethod]
        public async Task TestQueryFirstTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryFirstTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryFirstTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryFirstTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryFirstTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryFirstTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryFirstTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryFirstTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region QueryMultiple

        [TestMethod]
        public async Task TestQueryMultipleTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryMultipleTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryMultipleTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region QuerySingle

        [TestMethod]
        public async Task TestQuerySingleTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQuerySingleTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQuerySingleTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQuerySingleTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQuerySingleTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQuerySingleTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQuerySingleTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQuerySingleTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region SkipQuery

        [TestMethod]
        public async Task TestSkipQueryTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSkipQueryTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSkipQueryTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSkipQueryTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSkipQueryTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSkipQueryTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSkipQueryTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSkipQueryTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Sum

        [TestMethod]
        public async Task TestSumTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSumTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSumTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSumTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSumTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSumTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSumTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSumTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSumAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSumAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSumAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSumAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSumAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region SumAll

        [TestMethod]
        public async Task TestSumAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSumAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSumAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSumAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSumAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSumAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestSumAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Truncate

        [TestMethod]
        public async Task TestTruncateTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestTruncateTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestTruncateTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestTruncateTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestTruncateTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestTruncateTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestTruncateTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestTruncateTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestTruncateTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestTruncateTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestTruncateTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestTruncateAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region Update

        [TestMethod]
        public async Task TestUpdateTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public async Task TestUpdateAllTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateAllTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateAllTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateAllTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateAllTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        #region UpdateAllBatch

        [TestMethod]
        public async Task TestUpdateAllBatchTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceCompletesEveryCommand()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Connection.ExecutionCount > 0);
            Assert.AreEqual(execution.Connection.ExecutionCount, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(execution.Trace.BeforeLogs.Count, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTracePreservesSessionAndKey()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(execution.Trace.BeforeLogs.All(log => log.SessionId != Guid.Empty));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.SessionId), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.SessionId));
            Assert.AreSequenceEqual(execution.Trace.BeforeLogs.Select(log => log.Key), execution.Trace.AfterLogs.Cast<TraceLog>().Select(log => log.Key), StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTracePreservesStartingContext()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterBeforeLogs.Select((log, index) => ReferenceEquals(log, execution.Trace.BeforeLogs[index])).All(same => same));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().Select((log, index) => ReferenceEquals(log.Context, execution.Trace.BeforeLogs[index].Context)).All(same => same));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTracePreservesActivity()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.TraceId.ToString(), log.TraceId, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(execution.Activity.SpanId.ToString(), log.SpanId, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceReportsResultAndDuration()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterExecutionTimes.All(duration => duration > TimeSpan.Zero));
            Assert.IsTrue(execution.Trace.AfterResults.All(result => result != null));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceUsesExpectedCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Trace.AfterLogs.Count, execution.Trace.AfterTokens.Count);
            Assert.IsTrue(execution.Trace.AfterTokens.All(token => token == execution.Cancellation.Token));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceReportsOriginalExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTracePreservesErrorStack()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            StringAssert.Contains(execution.Error.StackTrace, nameof(CustomTracedDbCommand), StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceReportsBeforeObserverError()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceBeforeObserverErrorPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceSilentCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceSilentCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceCancellationDoesNotPreventCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        #endregion

        [TestMethod]
        public async Task TestQueryMultipleTraceReportsReaderError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.ReaderFactory = () => new CustomFailingDbDataReader(execution.Failure);

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceReportsSingleStatementFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomSingleStatementSupportDbSetting(), true);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsReaderError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.ReaderFactory = () => new CustomFailingDbDataReader(execution.Failure);

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceReportsSingleStatementFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomSingleStatementSupportDbSetting(), true);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestAverageTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Average");

            // Act
            await execution.ExecuteOperationAsync("Average", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestAverageAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("AverageAll");

            // Act
            await execution.ExecuteOperationAsync("AverageAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestBatchQueryTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestBatchQueryAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("BatchQuery");

            // Act
            await execution.ExecuteOperationAsync("BatchQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Count");

            // Act
            await execution.ExecuteOperationAsync("Count", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestCountAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("CountAll");

            // Act
            await execution.ExecuteOperationAsync("CountAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Delete");

            // Act
            await execution.ExecuteOperationAsync("Delete", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestDeleteAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("DeleteAll");

            // Act
            await execution.ExecuteOperationAsync("DeleteAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestExistsTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestExistsAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Exists");

            // Act
            await execution.ExecuteOperationAsync("Exists", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Insert");

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAll");

            // Act
            await execution.ExecuteOperationAsync("InsertAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllBatchTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestInsertAllBatchAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("InsertAllBatch");

            // Act
            await execution.ExecuteOperationAsync("InsertAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Max");

            // Act
            await execution.ExecuteOperationAsync("Max", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMaxAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MaxAll");

            // Act
            await execution.ExecuteOperationAsync("MaxAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Merge");

            // Act
            await execution.ExecuteOperationAsync("Merge", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAll");

            // Act
            await execution.ExecuteOperationAsync("MergeAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllBatchTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMergeAllBatchAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MergeAllBatch");

            // Act
            await execution.ExecuteOperationAsync("MergeAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Min");

            // Act
            await execution.ExecuteOperationAsync("Min", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestMinAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("MinAll");

            // Act
            await execution.ExecuteOperationAsync("MinAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Query");

            // Act
            await execution.ExecuteOperationAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryAll");

            // Act
            await execution.ExecuteOperationAsync("QueryAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryFirstTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryFirstAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryFirst");

            // Act
            await execution.ExecuteOperationAsync("QueryFirst", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryMultipleTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QueryMultiple");

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQuerySingleTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestQuerySingleAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("QuerySingle");

            // Act
            await execution.ExecuteOperationAsync("QuerySingle", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSkipQueryTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSkipQueryAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("SkipQuery");

            // Act
            await execution.ExecuteOperationAsync("SkipQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Sum");

            // Act
            await execution.ExecuteOperationAsync("Sum", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestSumAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("SumAll");

            // Act
            await execution.ExecuteOperationAsync("SumAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestTruncateTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestTruncateAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Truncate");

            // Act
            await execution.ExecuteOperationAsync("Truncate", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("Update");

            // Act
            await execution.ExecuteOperationAsync("Update", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAll");

            // Act
            await execution.ExecuteOperationAsync("UpdateAll", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        [TestMethod]
        public async Task TestUpdateAllBatchAsyncTraceCapturesConnectionMetadata()
        {
            // Setup
            using var execution = new TraceTestExecution("UpdateAllBatch");

            // Act
            await execution.ExecuteOperationAsync("UpdateAllBatch", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals("TestDatabase", log.Database, StringComparison.Ordinal)));
            Assert.IsTrue(execution.Trace.AfterLogs.Cast<TraceLog>().All(log => string.Equals(nameof(CustomTracedDbConnection), log.ConnectionType, StringComparison.Ordinal)));
        }

        #endregion
    }
}
