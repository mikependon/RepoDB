#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Contains the tests of the information that the trace logs hold: the level, the error, and the context of the execution.
    /// </summary>
    [TestClass]
    public class TraceLogTest
    {
        #region Public Methods

        [TestMethod]
        public void TestTraceLogWithoutAContextHasNoContext()
        {
            // Act
            var log = new TraceLog(Guid.NewGuid(), "key");

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.AreEqual(0, log.ThreadId);
            Assert.IsNull(log.PcName);
            Assert.IsNull(log.IpAddress);
            Assert.IsNull(log.TraceId);
            Assert.IsNull(log.SpanId);
            Assert.IsNull(log.ConnectionType);
            Assert.IsNull(log.Database);
        }

        [TestMethod]
        public void TestCancellableTraceLogWithoutAContextHasNoContext()
        {
            // Act
            var log = new CancellableTraceLog(Guid.NewGuid(), "key", "statement");

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.IsNull(log.ConnectionType);
            Assert.AreEqual(0, log.ThreadId);
        }

        [TestMethod]
        public void TestCancellableTraceLogHasTheContextOfTheExecution()
        {
            // Setup
            var context = CaptureContext();

            // Act
            var log = new CancellableTraceLog(Guid.NewGuid(), "key", "statement", null, context);

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.AreEqual(context.ThreadId, log.ThreadId);
            Assert.AreEqual(context.PcName, log.PcName);
            Assert.AreEqual(context.IpAddress, log.IpAddress);
            Assert.AreEqual("CustomTracedDbConnection", log.ConnectionType);
            Assert.AreEqual("TestDatabase", log.Database);
        }

        [TestMethod]
        public void TestCancellableTraceLogStartTimeDoesNotChange()
        {
            // Setup
            var log = new CancellableTraceLog(Guid.NewGuid(), "key", "statement");

            // Act
            var first = log.StartTime;
            Thread.Sleep(20);
            var second = log.StartTime;

            // Assert
            Assert.AreEqual(first, second);
            Assert.AreEqual(DateTimeKind.Utc, first.Kind);
        }

        [TestMethod]
        public void TestResultTraceLogWithoutAnErrorIsAnInformation()
        {
            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), "key", TimeSpan.FromMilliseconds(5), 7);

            // Assert
            Assert.AreEqual(TraceLevel.Info, log.Level);
            Assert.IsNull(log.Error);
            Assert.AreEqual(7, log.Result);
            Assert.AreEqual(TimeSpan.FromMilliseconds(5), log.ExecutionTime);
        }

        [TestMethod]
        public void TestResultTraceLogWithAnErrorIsAnError()
        {
            // Setup
            var error = new InvalidOperationException("The statement failed.");

            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), "key", TimeSpan.Zero, 0, null, error);

            // Assert
            Assert.AreEqual(TraceLevel.Error, log.Level);
            Assert.AreSame(error, log.Error);
        }

        [TestMethod]
        public void TestResultTraceLogTakesTheContextFromTheLogBeforeTheExecution()
        {
            // Setup
            var before = new CancellableTraceLog(Guid.NewGuid(), "key", "statement", null, CaptureContext());

            // Act
            var log = new ResultTraceLog<int>(before.SessionId, "key", TimeSpan.Zero, 1, before, null);

            // Assert
            Assert.AreSame(before, log.BeforeExecutionLog);
            Assert.AreEqual(before.ThreadId, log.ThreadId);
            Assert.AreEqual(before.PcName, log.PcName);
            Assert.AreEqual(before.IpAddress, log.IpAddress);
            Assert.AreEqual(before.ConnectionType, log.ConnectionType);
            Assert.AreEqual(before.Database, log.Database);
            Assert.AreEqual(before.TraceId, log.TraceId);
            Assert.AreEqual(before.SpanId, log.SpanId);
        }

        [TestMethod]
        public void TestResultTraceLogWithoutALogBeforeTheExecutionHasNoContext()
        {
            // Act
            var log = new ResultTraceLog<int>(Guid.NewGuid(), "key");

            // Assert
            Assert.AreEqual(0, log.ThreadId);
            Assert.IsNull(log.ConnectionType);
            Assert.IsNull(log.Error);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Captures the context of the execution of a command of a connection that has a database.
        /// </summary>
        /// <returns>The context.</returns>
        private static TraceContext CaptureContext()
        {
            using var connection = new CustomTracedDbConnection();
            return TraceContext.Capture((DbCommand)connection.CreateCommand());
        }

        #endregion
    }
}
