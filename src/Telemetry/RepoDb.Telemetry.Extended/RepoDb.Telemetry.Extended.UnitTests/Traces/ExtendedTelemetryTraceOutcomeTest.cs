#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies captured context and operation outcomes.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryTraceOutcomeTest
    {
        #region Private Variables

        /// <summary>
        /// The identifier of the span that the captured context has.
        /// </summary>
        private const string CapturedSpanId = "b7ad6b7169203331";

        /// <summary>
        /// The identifier of the trace that the captured context has.
        /// </summary>
        private const string CapturedTraceId = "0af7651916cd43dd8448eb211c80319c";

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
        }

        /// <summary>
        /// Verifies traceid and spanid.
        /// </summary>
        [TestMethod]
        public void CapturedContextPreservesTraceIdAndSpanId()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(CapturedTraceId,
                item.TraceId);
            Assert.AreEqual(CapturedSpanId,
                item.SpanId);
        }

        /// <summary>
        /// Verifies threadid and connectiontype.
        /// </summary>
        [TestMethod]
        public void CapturedContextPreservesThreadIdAndConnectionType()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(4242,
                item.ThreadId);
            Assert.AreEqual("SqlConnection",
                item.ConnectionType);
        }

        /// <summary>
        /// Verifies database and client.
        /// </summary>
        [TestMethod]
        public void CapturedContextPreservesDatabaseAndClient()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual("Northwind",
                item.Database);
            Assert.AreEqual("PC-01",
                item.Client);
        }

        /// <summary>
        /// Verifies ipaddress and traceid.
        /// </summary>
        [TestMethod]
        public void CapturedContextPreservesIpAddressAndTraceId()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual("10.0.0.7",
                item.IpAddress);
            Assert.AreNotEqual(activity.TraceId.ToString(),
                item.TraceId);
        }

        /// <summary>
        /// Verifies traceid and spanid.
        /// </summary>
        [TestMethod]
        public void MissingContextUsesTraceIdAndSpanId()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog();

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(activity.TraceId.ToString(),
                item.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(),
                item.SpanId);
        }

        /// <summary>
        /// Verifies threadid and connectiontype.
        /// </summary>
        [TestMethod]
        public void MissingContextUsesThreadIdAndConnectionType()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog();

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(Environment.CurrentManagedThreadId,
                item.ThreadId);
            Assert.IsNull(item.ConnectionType);
        }

        /// <summary>
        /// Verifies database and client.
        /// </summary>
        [TestMethod]
        public void MissingContextUsesDatabaseAndClient()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog();

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.Database);
            Assert.AreEqual(Environment.MachineName,
                item.Client);
        }

        /// <summary>
        /// Verifies ipaddress.
        /// </summary>
        [TestMethod]
        public void MissingContextUsesIpAddress()
        {
            // Setup
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog();

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.IpAddress);
        }

        /// <summary>
        /// Verifies the application can still replace the captured values through the enrichment.
        /// </summary>
        [TestMethod]
        public void EnrichmentCanReplaceTheCapturedConnectionTypeAndDatabase()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                Enrich = (item,
                log) =>
            {
                item.ConnectionType = "WrappedConnection";
                item.Database = "Replaced";
            }
            });
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual("WrappedConnection",
                item.ConnectionType);
            Assert.AreEqual("Replaced",
                item.Database);
        }

        /// <summary>
        /// Verifies level and errortype.
        /// </summary>
        [TestMethod]
        public void RecordsTheErrorOfAFailedOperationLevelAndErrorType()
        {
            // Setup
            var error = CreateThrownError();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                error));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(LogEventLevel.Error,
                item.Level);
            Assert.AreEqual(typeof(InvalidOperationException).FullName,
                item.ErrorType);
        }

        /// <summary>
        /// Verifies errormessage and errorstacktrace.
        /// </summary>
        [TestMethod]
        public void RecordsTheErrorOfAFailedOperationErrorMessageAndErrorStackTrace()
        {
            // Setup
            var error = CreateThrownError();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                error));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual("The statement failed.",
                item.ErrorMessage);
            Assert.IsFalse(string.IsNullOrEmpty(item.ErrorStackTrace));
        }

        /// <summary>
        /// Verifies errorstacktrace and message.
        /// </summary>
        [TestMethod]
        public void RecordsTheErrorOfAFailedOperationErrorStackTraceAndMessage()
        {
            // Setup
            var error = CreateThrownError();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                error));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            StringAssert.Contains(item.ErrorStackTrace,
                nameof(CreateThrownError),
                StringComparison.Ordinal);
            Assert.AreEqual("The statement failed.",
                item.Message);
        }

        /// <summary>
        /// Verifies rowsaffected and rowsreturned.
        /// </summary>
        [TestMethod]
        public void RecordsTheErrorOfAFailedOperationRowsAffectedAndRowsReturned()
        {
            // Setup
            var error = CreateThrownError();
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                error));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.RowsAffected);
            Assert.IsNull(item.RowsReturned);
        }

        /// <summary>
        /// Verifies the memory and process I/O of the operation are recorded.
        /// </summary>
        [TestMethod]
        public void RecordsTheMemoryAndTheIoOfTheOperation()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                1,
                new ResourceUsage(4096, 10, 20)));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(4096L, item.MemoryBytes);
            Assert.AreEqual(10L, item.ReadBytes);
            Assert.AreEqual(20L, item.WriteBytes);
        }

        /// <summary>
        /// Verifies the memory and process I/O are left empty if they are not recorded.
        /// </summary>
        [TestMethod]
        public void LeavesTheMemoryAndTheIoEmptyIfTheyAreNotRecorded()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                1));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.MemoryBytes);
            Assert.IsNull(item.ReadBytes);
            Assert.IsNull(item.WriteBytes);
        }

        /// <summary>
        /// Verifies the message that the application gave is kept, and the level is an error even if the application gave another one.
        /// </summary>
        [TestMethod]
        public void KeepsTheMessageOfTheApplicationWhenTheOperationFails()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                Enrich = (item,
                log) =>
            {
                item.Level = LogEventLevel.Warning;
                item.Message = "Deleting the warehouse.";
            }
            });
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                CreateThrownError()));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual("Deleting the warehouse.",
                item.Message);
            Assert.AreEqual("The statement failed.",
                item.ErrorMessage);
            Assert.AreEqual(LogEventLevel.Error,
                item.Level);
        }

        /// <summary>
        /// Verifies level and errortype.
        /// </summary>
        [TestMethod]
        public void LeavesTheErrorEmptyWhenTheOperationSucceedsLevelAndErrorType()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.AreEqual(LogEventLevel.Information,
                item.Level);
            Assert.IsNull(item.ErrorType);
        }

        /// <summary>
        /// Verifies errormessage and errorstacktrace.
        /// </summary>
        [TestMethod]
        public void LeavesTheErrorEmptyWhenTheOperationSucceedsErrorMessageAndErrorStackTrace()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.ErrorMessage);
            Assert.IsNull(item.ErrorStackTrace);
        }

        /// <summary>
        /// Verifies message and rowsaffected.
        /// </summary>
        [TestMethod]
        public void LeavesTheErrorEmptyWhenTheOperationSucceedsMessageAndRowsAffected()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());

            // Act
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            var item = TraceTestHelper.GetItem(trace,
                log);

            // Assert
            Assert.IsNull(item.Message);
            Assert.AreEqual(3L,
                item.RowsAffected);
        }

        /// <summary>
        /// Verifies the level of the trace is mapped to the level of the logging.
        /// </summary>
        /// <param name="level">The level of the trace.</param>
        /// <param name="expected">The expected level of the logging.</param>
        [TestMethod]
        [DataRow(TraceLevel.Warning, LogEventLevel.Warning)]
        [DataRow(TraceLevel.Verbose, LogEventLevel.Verbose)]
        [DataRow(TraceLevel.Error, LogEventLevel.Error)]
        [DataRow(TraceLevel.Off, LogEventLevel.Information)]
        public void MapsTheLevelOfTheTrace(TraceLevel level,
            LogEventLevel expected)
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();
            var item = new ExtendedTelemetryItem();

            // Act
            trace.Complete(item,
                0,
                null,
                level);

            // Assert
            Assert.AreEqual(expected,
                item.Level);
            Assert.AreEqual(level,
                item.TraceLevel);
        }

        /// <summary>
        /// Verifies an information keeps the level that the application gave.
        /// </summary>
        [TestMethod]
        public void KeepsTheLevelOfTheApplicationForAnInformation()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();
            var item = new ExtendedTelemetryItem
            {
                Level = LogEventLevel.Warning
            };

            // Act
            trace.Complete(item,
                0,
                null,
                TraceLevel.Info);

            // Assert
            Assert.AreEqual(LogEventLevel.Warning,
                item.Level);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the context that the library captures when an operation starts.
        /// </summary>
        /// <returns>The context.</returns>
        private static TraceContext CreateContext() => new TraceContext(4242,
            "PC-01",
            "10.0.0.7",
            CapturedTraceId,
            CapturedSpanId,
            "SqlConnection",
            "Northwind");

        /// <summary>
        /// Creates an exception that has been thrown, so it has a stack trace.
        /// </summary>
        /// <returns>The exception.</returns>
        private static InvalidOperationException CreateThrownError()
        {
            try
            {
                throw new InvalidOperationException("The statement failed.");
            }
            catch (InvalidOperationException exception)
            {
                return exception;
            }
        }

        #endregion
    }
}
