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
    /// Verifies configured warning boundaries and measured command severity.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class TraceWarningThresholdTest
    {
        #region Public Methods

        [TestMethod]
        public async Task TestExecuteNonQueryTraceReportsDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(TraceLevel.Info, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceMeasuresDurationWithDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTracePreservesResultWithDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(null, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceReportsWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(TraceLevel.Warning, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceMeasuresDurationWithWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTracePreservesResultWithWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(null, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceReportsFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(TraceLevel.Error, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceMeasuresDurationWithFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTracePreservesResultWithFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(execution.Failure, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceReportsDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(TraceLevel.Info, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceMeasuresDurationWithDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTracePreservesResultWithDisabledLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(null, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceReportsWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.AreEqual(TraceLevel.Warning, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceMeasuresDurationWithWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTracePreservesResultWithWarningLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(null, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceReportsFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.AreEqual(TraceLevel.Error, execution.Trace.GetAfterLog<int>().Level);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTraceMeasuresDurationWithFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreSame(execution.Failure, execution.Error);
            Assert.IsTrue(execution.Trace.GetAfterLog<int>().ExecutionTime >= TimeSpan.FromMilliseconds(1));
            Assert.AreEqual(1, execution.Connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncTracePreservesResultWithFailureLevel()
        {
            // Setup
            using var execution = new TraceTestExecution();
            execution.Connection.Delay = TimeSpan.FromMilliseconds(10);
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(1);
            execution.Connection.Failure = execution.Failure;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, execution.Trace.GetAfterLog<int>().Result);
            Assert.AreSame(execution.Failure, execution.Trace.GetAfterLog<int>().Error);
            Assert.AreSame(execution.Trace.BeforeLogs.Single(), execution.Trace.GetAfterLog<int>().BeforeExecutionLog);
        }

        [TestMethod]
        public void TestTraceWarningThresholdDefaultValue()
        {
            // Setup
            var options = new GlobalConfigurationOptions();

            // Act
            var actual = options.TraceWarningThreshold;

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(1), actual);
        }

        [TestMethod]
        public void TestTraceWarningThresholdConfiguredValue()
        {
            // Setup
            var options = new GlobalConfigurationOptions();

            // Act
            options.TraceWarningThreshold = TimeSpan.FromSeconds(2);

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(2), options.TraceWarningThreshold);
        }

        [TestMethod]
        public void TestTraceWarningThresholdDisabledValue()
        {
            // Setup
            var options = new GlobalConfigurationOptions();

            // Act
            options.TraceWarningThreshold = null;

            // Assert
            Assert.IsNull(options.TraceWarningThreshold);
        }

        [TestMethod]
        public void TestTraceWarningThresholdDisabledBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = null;

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(3000), 0, null, null);

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.AreSame(null, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdBelowBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(1999), 0, null, null);

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.AreSame(null, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdEqualBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(2000), 0, null, null);

            // Assert
            Assert.AreEqual(TraceLevel.Warning, log.Level);
            Assert.AreSame(null, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdAboveBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(3000), 0, null, null);

            // Assert
            Assert.AreEqual(TraceLevel.Warning, log.Level);
            Assert.AreSame(null, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdMissingDurationBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, null, 0, null, null);

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.AreSame(null, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdErrorAboveBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(3000), 0, null, execution.Failure);

            // Assert
            Assert.AreEqual(TraceLevel.Error, log.Level);
            Assert.AreSame(execution.Failure, log.Error);
        }

        [TestMethod]
        public void TestTraceWarningThresholdErrorBelowBoundary()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceWarningThreshold = TimeSpan.FromMilliseconds(2000);

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), TraceKeys.ExecuteNonQuery, TimeSpan.FromMilliseconds(100), 0, null, execution.Failure);

            // Assert
            Assert.AreEqual(TraceLevel.Error, log.Level);
            Assert.AreSame(execution.Failure, log.Error);
        }

        #endregion
    }
}
