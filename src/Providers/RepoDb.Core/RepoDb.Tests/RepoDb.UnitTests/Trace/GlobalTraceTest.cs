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
    /// Verifies global registration, cancellation, and observer failures through executed commands.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class GlobalTraceTest
    {
        #region Public Methods

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithoutLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithoutLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithoutLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithoutLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceDisabledWithLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithoutLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithoutLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithoutLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithoutLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithoutLocalTraceSharesNotifications()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.BeforeLogs.Single(), execution.Second.BeforeLogs.Single());
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreEqual(1, execution.First.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceEnabledWithLocalTraceSharesNotifications()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.BeforeLogs.Single(), execution.Second.BeforeLogs.Single());
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreEqual(1, execution.First.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestGlobalTraceFailingObserversPreserveSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.First.AfterErrors.Single());
            Assert.AreSame(null, execution.Second.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceFailingObserversCompleteAfterSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestLocalTraceFailureStillCompletesGlobalTraceAfterSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreSame(null, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceFailingObserversPreserveExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
            Assert.AreSame(execution.Failure, execution.Second.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceFailingObserversCompleteAfterExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestLocalTraceFailureStillCompletesGlobalTraceAfterExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceCancelControlsBeforeCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceCancelCompletesEveryObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceCancelSharesCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreSame(null, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceBeforeFailureControlsBeforeCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceBeforeFailureCompletesEveryObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceBeforeFailureSharesCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleCompletesLocalAndGlobalTraceOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithoutLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithoutLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithoutLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithoutLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncDisabledWithLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithoutLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithoutLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithoutLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithoutLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithoutLocalTraceSharesNotifications()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.BeforeLogs.Single(), execution.Second.BeforeLogs.Single());
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreEqual(1, execution.First.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithLocalTracePreservesCommandResult()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Result);
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithLocalTraceNotifiesFirstObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithLocalTraceNotifiesSecondObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithLocalTraceRespectsLocalObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncEnabledWithLocalTraceSharesNotifications()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.BeforeLogs.Single(), execution.Second.BeforeLogs.Single());
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreEqual(1, execution.First.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncFailingObserversPreserveSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreSame(null, execution.First.AfterErrors.Single());
            Assert.AreSame(null, execution.Second.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncFailingObserversCompleteAfterSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestLocalTraceAsyncFailureStillCompletesGlobalTraceAfterSuccess()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.ObserverFailure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreSame(null, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncFailingObserversPreserveExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
            Assert.AreSame(execution.Failure, execution.Second.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncFailingObserversCompleteAfterExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.ThrowOnAfter = execution.ObserverFailure;
            execution.Second.ThrowOnAfter = new InvalidOperationException("Second observer failed.");
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestLocalTraceAsyncFailureStillCompletesGlobalTraceAfterExecutionFailure()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.Trace.ThrowOnAfter = execution.ObserverFailure;
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteOperationAsync("Insert", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncCancelPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncCancelControlsBeforeCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(1, execution.Second.BeforeLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncCancelCompletesEveryObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncCancelSharesCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.CancelBefore = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreSame(null, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncBeforeFailurePreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncBeforeFailureControlsBeforeCallbacks()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.BeforeLogs.Count);
            Assert.AreEqual(0, execution.Second.BeforeLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncBeforeFailureCompletesEveryObserver()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
            Assert.AreEqual(1, execution.Second.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceAsyncBeforeFailureSharesCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = () => throw execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.First.AfterLogs.Single(), execution.Second.AfterLogs.Single());
            Assert.AreSame(execution.Failure, execution.First.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestQueryMultipleAsyncCompletesLocalAndGlobalTraceOnce()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;

            // Act
            await execution.ExecuteOperationAsync("QueryMultiple", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(1, execution.Trace.AfterLogs.Count);
            Assert.AreEqual(1, execution.First.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestGlobalTraceCallerCancellationPreservesError()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = execution.Cancellation.Cancel;
            execution.First.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreSame(execution.Error, execution.First.AfterErrors.Single());
            Assert.AreSame(execution.Error, execution.Second.AfterErrors.Single());
        }

        [TestMethod]
        public async Task TestGlobalTraceCallerCancellationPreventsExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = execution.Cancellation.Cancel;
            execution.First.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(0, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestGlobalTraceCallerCancellationDoesNotCancelCompletion()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.RegisterGlobal(execution.First);
            execution.RegisterGlobal(execution.Second);
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = true;
            execution.First.OnBefore = execution.Cancellation.Cancel;
            execution.First.ThrowOnAfter = execution.ObserverFailure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true, false).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType<OperationCanceledException>(execution.Error);
            Assert.AreEqual(CancellationToken.None, execution.First.AfterTokens.Single());
            Assert.AreEqual(CancellationToken.None, execution.Second.AfterTokens.Single());
        }

        #endregion
    }
}
