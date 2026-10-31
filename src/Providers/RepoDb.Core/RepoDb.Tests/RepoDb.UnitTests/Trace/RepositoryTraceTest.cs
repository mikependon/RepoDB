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
    /// Verifies scalar, write, and query tracing through both public repository APIs.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class RepositoryTraceTest
    {
        #region Public Methods

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("CountAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("CountAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryCountAllAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("CountAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("CountAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryCountAllAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("CountAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("Insert", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("Insert", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryInsertAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("Insert", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("Insert", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsReturnedValue()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1L, Convert.ToInt64(execution.Result, CultureInfo.InvariantCulture));
            Assert.AreEqual(1L, Convert.ToInt64(execution.Trace.AfterResults.Single(), CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryInsertAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("Insert", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("QueryAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllReturnsMappedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Id);
            Assert.AreEqual("Trace", ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceReportsReturnedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(((IEnumerable<OperationTraceEntity>)execution.Result).Single(), ((IEnumerable<OperationTraceEntity>)execution.Trace.AfterResults.Single()).Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("QueryAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncReturnsMappedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Id);
            Assert.AreEqual("Trace", ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsReturnedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(((IEnumerable<OperationTraceEntity>)execution.Result).Single(), ((IEnumerable<OperationTraceEntity>)execution.Trace.AfterResults.Single()).Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestDbRepositoryQueryAllAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("QueryAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllReturnsMappedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Id);
            Assert.AreEqual("Trace", ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceReportsReturnedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(((IEnumerable<OperationTraceEntity>)execution.Result).Single(), ((IEnumerable<OperationTraceEntity>)execution.Trace.AfterResults.Single()).Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTracePreservesCorrelation()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(execution.Trace.BeforeLogs.Single().SessionId, ((TraceLog)execution.Trace.AfterLogs.Single()).SessionId);
            Assert.AreSame(execution.Trace.BeforeLogs.Single().Context, ((TraceLog)execution.Trace.AfterLogs.Single()).Context);
            Assert.AreEqual("QueryAll", ((TraceLog)execution.Trace.AfterLogs.Single()).Key, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncReturnsMappedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Id);
            Assert.AreEqual("Trace", ((IEnumerable<OperationTraceEntity>)execution.Result).Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsReturnedEntity()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(((IEnumerable<OperationTraceEntity>)execution.Result).Single(), ((IEnumerable<OperationTraceEntity>)execution.Trace.AfterResults.Single()).Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceUsesCompletionTokenForFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceUsesCompletionTokenForBeforeFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceUsesCompletionTokenForCancel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceUsesCompletionTokenForObserverFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReportsTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTokenCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceUsesCompletionTokenForTokenCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestBaseRepositoryQueryAllAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteRepositoryAsync("QueryAll", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        #endregion
    }
}
