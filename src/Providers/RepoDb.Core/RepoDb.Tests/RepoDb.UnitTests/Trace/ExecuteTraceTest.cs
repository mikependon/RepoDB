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
    /// Verifies raw execution tracing with explicit success, failure, and cancellation tests.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class ExecuteTraceTest
    {
        #region Public Methods

        [TestMethod]
        public async Task TestExecuteScalarTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceReportsReturnedResult()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Scalar", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceReportsReturnedResult()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Scalar", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteReaderTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Reader", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteReaderAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Reader", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteMultipleTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Multiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteMultipleAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Multiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Query", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Query", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteQueryTypedAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("QueryTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("First", false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("First", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteFirstTypedAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("FirstTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("Single", false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("Single", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCompletesSuccessfulCommand()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceReportsSuccessfulExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
            Assert.IsNull(execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCapturesDatabase()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual("TestDatabase", execution.Trace.BeforeLogs.Single().Database, StringComparer.Ordinal);
            Assert.AreEqual(nameof(CustomTracedDbConnection), execution.Trace.BeforeLogs.Single().ConnectionType, StringComparer.Ordinal);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceReportsOriginalFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCompletesFailedCommandOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceObserverCannotHideExecutionError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Failure = execution.Failure;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCancellationCompletesOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceReportsEmptyQuery()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Table.Rows.Clear();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<EmptyException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceReceivesCallerToken()
        {
            // Setup
            using var execution = new TraceTestExecution();

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(execution.Cancellation.Token, execution.Trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceReportsCallerCancellation()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.Trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, execution.Trace.AfterLevels.Single());
        }

        [TestMethod]
        public async Task TestExecuteSingleTypedAsyncTraceCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Trace.OnBefore = execution.Cancellation.Cancel;

            // Act
            await execution.ExecuteCommandAsync("SingleTyped", true).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.Trace.AfterTokens.Single());
        }

        #endregion
    }
}
