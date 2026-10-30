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

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Contains the tests of the <c>TraceContext</c> class.
    /// </summary>
    [TestClass]
    public class TraceContextTest
    {
        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
        }

        [TestMethod]
        public void TestTraceContextEmptyHasNoInformation()
        {
            // Act
            var context = TraceContext.Empty;

            // Assert
            Assert.AreEqual(0, context.ThreadId);
            Assert.IsNull(context.PcName);
            Assert.IsNull(context.IpAddress);
            Assert.IsNull(context.TraceId);
            Assert.IsNull(context.SpanId);
            Assert.IsNull(context.ConnectionType);
            Assert.IsNull(context.Database);
        }

        [TestMethod]
        public void TestTraceContextCaptureTheExecution()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var command = (DbCommand)connection.CreateCommand();

            // Act
            var context = TraceContext.Capture(command);

            // Assert
            Assert.AreEqual(Environment.CurrentManagedThreadId, context.ThreadId);
            Assert.AreEqual(Environment.MachineName, context.PcName);
            Assert.AreEqual(TraceEnvironment.IpAddress, context.IpAddress);
            Assert.AreEqual("CustomTracedDbConnection", context.ConnectionType);
            Assert.AreEqual("TestDatabase", context.Database);
        }

        [TestMethod]
        public void TestTraceContextCaptureWithoutAnActivity()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();

            // Act
            var context = TraceContext.Capture((DbCommand)connection.CreateCommand());

            // Assert
            Assert.IsNull(context.TraceId);
            Assert.IsNull(context.SpanId);
        }

        [TestMethod]
        public void TestTraceContextCaptureTheActivityOfTheW3CFormat()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var activity = new Activity("Payroll");
            activity.SetIdFormat(ActivityIdFormat.W3C);
            activity.Start();

            // Act
            var context = TraceContext.Capture((DbCommand)connection.CreateCommand());
            activity.Stop();

            // Assert
            Assert.AreEqual(activity.TraceId.ToString(), context.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(), context.SpanId);
        }

        [TestMethod]
        public void TestTraceContextDoesNotCaptureTheActivityThatIsNotOfTheW3CFormat()
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var activity = new Activity("Payroll");
            activity.SetIdFormat(ActivityIdFormat.Hierarchical);
            activity.Start();

            // Act
            var context = TraceContext.Capture((DbCommand)connection.CreateCommand());
            activity.Stop();

            // Assert
            Assert.IsNull(context.TraceId);
            Assert.IsNull(context.SpanId);
        }

        [TestMethod]
        public void TestTraceContextCaptureWithoutACommand()
        {
            // Act
            var context = TraceContext.Capture(null);

            // Assert
            Assert.IsNull(context.ConnectionType);
            Assert.IsNull(context.Database);
            Assert.AreEqual(Environment.CurrentManagedThreadId, context.ThreadId);
        }

        [TestMethod]
        public void TestTraceContextCaptureTheConnectionThatCannotTellItsDatabase()
        {
            // Setup
            using var connection = new CustomThrowingDatabaseDbConnection();

            // Act
            var context = TraceContext.Capture((DbCommand)connection.CreateCommand());

            // Assert
            Assert.AreEqual("CustomThrowingDatabaseDbConnection", context.ConnectionType);
            Assert.IsNull(context.Database);
        }

        #endregion
    }
}
