#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core.UnitTests.Helpers;

namespace RepoDb.Telemetry.Core.UnitTests.Traces
{
    /// <summary>
    /// Contains the tests of what TelemetryTrace takes from the context that the library captures when an operation starts, and how it is told that an operation was completed.
    /// </summary>
    [TestClass]
    public class TelemetryTraceOutcomeTest
    {
        #region Public Methods

        [TestMethod]
        public void TestTelemetryTraceTakesTheClientAndTheIpAddressFromTheLog()
        {
            // Setup
            var trace = new ObservableTelemetryTrace();
            var log = new CancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;", null, CreateContext());

            // Act
            trace.BeforeExecution(log);

            // Assert
            var item = trace.Items[0];
            Assert.AreEqual("PC-01", item.Client);
            Assert.AreEqual("10.0.0.7", item.IpAddress);
        }

        [TestMethod]
        public void TestTelemetryTraceUsesTheNameOfTheMachineWhenTheLogHasNoContext()
        {
            // Setup
            var trace = new ObservableTelemetryTrace();
            var log = new CancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;", null, TraceContext.Empty);

            // Act
            trace.BeforeExecution(log);

            // Assert
            var item = trace.Items[0];
            Assert.AreEqual(Environment.MachineName, item.Client);
            Assert.IsNull(item.IpAddress);
        }

        [TestMethod]
        public void TestTelemetryTraceCompletesTheItemWithTheResultOfASuccessfulOperation()
        {
            // Setup
            var trace = new ObservableTelemetryTrace();
            var before = new CancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;", null, CreateContext());
            trace.BeforeExecution(before);

            // Act
            trace.AfterExecution(new ResultTraceLog<int>(before.SessionId, "Query", TimeSpan.FromMilliseconds(5), 7, before, null));

            // Assert
            Assert.AreEqual(1, trace.Results.Count);
            Assert.AreEqual(7, trace.Results[0]);
            Assert.AreEqual(1, trace.Outcomes.Count);
            Assert.AreEqual(TraceLevel.Info, trace.Outcomes[0].Item3);
            Assert.IsNull(trace.Outcomes[0].Item2);
        }

        [TestMethod]
        public void TestTelemetryTraceDoesNotCompleteTheItemWithTheResultOfAFailedOperation()
        {
            // Setup
            var trace = new ObservableTelemetryTrace();
            var before = new CancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;", null, CreateContext());
            var error = new InvalidOperationException("The statement failed.");
            trace.BeforeExecution(before);

            // Act
            trace.AfterExecution(new ResultTraceLog<int>(before.SessionId, "Query", TimeSpan.FromMilliseconds(5), 0, before, error));

            // Assert
            Assert.AreEqual(0, trace.Results.Count);
            Assert.AreEqual(1, trace.Outcomes.Count);
            Assert.AreSame(error, trace.Outcomes[0].Item2);
            Assert.AreEqual(TraceLevel.Error, trace.Outcomes[0].Item3);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the context that the library captures when an operation starts.
        /// </summary>
        /// <returns>The context.</returns>
        private static TraceContext CreateContext() =>
            new TraceContext(4242,
                "PC-01",
                "10.0.0.7",
                "0af7651916cd43dd8448eb211c80319c",
                "b7ad6b7169203331",
                "SqlConnection",
                "Northwind");

        #endregion
    }
}
