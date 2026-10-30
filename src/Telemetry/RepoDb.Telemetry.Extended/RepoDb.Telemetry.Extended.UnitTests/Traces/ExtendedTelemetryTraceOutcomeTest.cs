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
    /// Verifies what ExtendedTelemetryTrace records from the context that the library captures when an operation starts, and from how the operation ends.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryTraceOutcomeTest
    {
        #region Private Variables

        /// <summary>
        /// The identifier of the trace that the captured context has.
        /// </summary>
        private const string CapturedTraceId = "0af7651916cd43dd8448eb211c80319c";

        /// <summary>
        /// The identifier of the span that the captured context has.
        /// </summary>
        private const string CapturedSpanId = "b7ad6b7169203331";

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
        }

        /// <summary>
        /// Verifies the context that the library captured is used, instead of the one that is current when the item is created.
        /// </summary>
        [TestMethod]
        public void UsesTheContextThatWasCapturedWhenTheOperationStarted()
        {
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(CapturedTraceId,
                item.TraceId);
            Assert.AreEqual(CapturedSpanId,
                item.SpanId);
            Assert.AreEqual(4242,
                item.ThreadId);
            Assert.AreEqual("SqlConnection",
                item.ConnectionType);
            Assert.AreEqual("Northwind",
                item.Database);
            Assert.AreEqual("PC-01",
                item.Client);
            Assert.AreEqual("10.0.0.7",
                item.IpAddress);
            Assert.AreNotEqual(activity.TraceId.ToString(),
                item.TraceId);
        }

        /// <summary>
        /// Verifies a log without a context gets the activity and the thread that are current, and no connection information.
        /// </summary>
        [TestMethod]
        public void UsesTheActiveActivityAndThreadWhenTheLogHasNoContext()
        {
            using var activity = new Activity("Ambient").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog();
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(activity.TraceId.ToString(),
                item.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(),
                item.SpanId);
            Assert.AreEqual(Environment.CurrentManagedThreadId,
                item.ThreadId);
            Assert.IsNull(item.ConnectionType);
            Assert.IsNull(item.Database);
            Assert.AreEqual(Environment.MachineName,
                item.Client);
            Assert.IsNull(item.IpAddress);
        }

        /// <summary>
        /// Verifies the application can still replace the captured values through the enrichment.
        /// </summary>
        [TestMethod]
        public void EnrichmentCanReplaceTheCapturedConnectionTypeAndDatabase()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test")
            {
                Enrich = (item, log) =>
                {
                    item.ConnectionType = "WrappedConnection";
                    item.Database = "Replaced";
                }
            });
            var log = new TestCommandLog(CreateContext());
            trace.BeforeExecution(log);
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual("WrappedConnection",
                item.ConnectionType);
            Assert.AreEqual("Replaced",
                item.Database);
        }

        /// <summary>
        /// Verifies the exception of a failed operation is recorded, and the row counts are not.
        /// </summary>
        [TestMethod]
        public void RecordsTheErrorOfAFailedOperation()
        {
            var error = CreateThrownError();
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                error));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(LogEventLevel.Error,
                item.Level);
            Assert.AreEqual(typeof(InvalidOperationException).FullName,
                item.ErrorType);
            Assert.AreEqual("The statement failed.",
                item.ErrorMessage);
            Assert.IsFalse(string.IsNullOrEmpty(item.ErrorStackTrace));
            StringAssert.Contains(item.ErrorStackTrace,
                nameof(CreateThrownError),
                StringComparison.Ordinal);
            Assert.AreEqual("The statement failed.",
                item.Message);
            Assert.IsNull(item.RowsAffected);
            Assert.IsNull(item.RowsReturned);
        }

        /// <summary>
        /// Verifies the message that the application gave is kept, and the level is an error even if the application gave another one.
        /// </summary>
        [TestMethod]
        public void KeepsTheMessageOfTheApplicationWhenTheOperationFails()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test")
            {
                Enrich = (item, log) =>
                {
                    item.Level = LogEventLevel.Warning;
                    item.Message = "Deleting the warehouse.";
                }
            });
            var log = new TestCommandLog(CreateContext());
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                CreateThrownError()));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual("Deleting the warehouse.",
                item.Message);
            Assert.AreEqual("The statement failed.",
                item.ErrorMessage);
            Assert.AreEqual(LogEventLevel.Error,
                item.Level);
        }

        /// <summary>
        /// Verifies a successful operation has no error, and keeps its level and row counts.
        /// </summary>
        [TestMethod]
        public void LeavesTheErrorEmptyWhenTheOperationSucceeds()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog(CreateContext());
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(LogEventLevel.Information,
                item.Level);
            Assert.IsNull(item.ErrorType);
            Assert.IsNull(item.ErrorMessage);
            Assert.IsNull(item.ErrorStackTrace);
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
            var trace = new ObservableExtendedTelemetryTrace();
            var item = new ExtendedTelemetryItem();
            trace.Complete(item,
                0,
                null,
                level);
            Assert.AreEqual(expected,
                item.Level);
        }

        /// <summary>
        /// Verifies an information keeps the level that the application gave.
        /// </summary>
        [TestMethod]
        public void KeepsTheLevelOfTheApplicationForAnInformation()
        {
            var trace = new ObservableExtendedTelemetryTrace();
            var item = new ExtendedTelemetryItem { Level = LogEventLevel.Warning };
            trace.Complete(item,
                0,
                null,
                TraceLevel.Info);
            Assert.AreEqual(LogEventLevel.Warning,
                item.Level);
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
