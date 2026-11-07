#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Verifies that the memory and process I/O usage is recorded in the result trace log only when it is enabled.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class TraceResourceUsageTest
    {
        #region Public Methods

        [TestMethod]
        public void TestTraceResourceUsageIsDisabledByDefault()
        {
            // Act/Assert
            Assert.IsFalse(new Options.GlobalConfigurationOptions().TraceResourceUsage);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceHasNoResourceUsageIfItIsNotEnabled()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceResourceUsage = false;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsNull(execution.Trace.GetAfterLog<int>().ResourceUsage);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceHasTheResourceUsageIfItIsEnabled()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceResourceUsage = true;

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNull(execution.Error);
            Assert.IsNotNull(execution.Trace.GetAfterLog<int>().ResourceUsage);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryTraceHasTheResourceUsageOfAFailedExecution()
        {
            // Setup
            using var execution = new TraceTestExecution();
            GlobalConfiguration.Options.TraceResourceUsage = true;
            execution.Connection.Failure = new InvalidOperationException("Failed");

            // Act
            await execution.ExecuteCommandAsync("NonQuery", false).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(execution.Error);
            Assert.IsNotNull(execution.Trace.GetAfterLog<int>().ResourceUsage);
        }

        #endregion
    }
}
