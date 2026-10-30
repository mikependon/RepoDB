#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies ExtendedTelemetryTrace behavior.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryTraceTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies synchronous capture, correlation, enrichment, and write row counts.
        /// </summary>
        [TestMethod]
        public void CapturesCorrelationAndEnrichment()
        {
            using var activity = new Activity("Delete").SetIdFormat(ActivityIdFormat.W3C).Start();
            var option = new ExtendedTelemetryOption("PayrollSystem")
            {
                Enrich = (item, log) =>
                {
                    item.CommandType = CommandType.Text;
                    item.ConnectionName = "Northwind (SQL Server)";
                    item.Database = "Northwind";
                    item.Level = LogEventLevel.Warning;
                    item.Message = "Delete was slower than expected.";
                    item.Table = "dbo.Warehouse";
                    item.Properties["Operation"] = log.Key;
                }
            };
            var trace = new ExtendedTelemetryTrace(option);
            var log = new TestCommandLog();
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(activity.TraceId.ToString(),
                item.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(),
                item.SpanId);
            Assert.AreEqual(Environment.CurrentManagedThreadId,
                item.ThreadId);
            Assert.AreEqual(1,
                item.ParameterCount);
            Assert.AreEqual(3L,
                item.RowsAffected);
            Assert.IsNull(item.RowsReturned);
            AssertEnrichedMetadata(item);
        }

        /// <summary>
        /// Verifies asynchronous capture and collection row counts.
        /// </summary>
        [TestMethod]
        public async Task AsyncCaptureRecordsCollectionCount()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog("Query");
            await trace.BeforeExecutionAsync(log);
            await trace.AfterExecutionAsync(new TestResultLog<int[]>(log,
                new[] { 1, 2 }));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.AreEqual(2L,
                item.RowsReturned);
            Assert.IsNull(item.RowsAffected);
        }

        /// <summary>
        /// Verifies scalar query results are not mistaken for affected row counts.
        /// </summary>
        [TestMethod]
        public void ScalarQueryKeepsRowCountsUnknown()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog("ExecuteScalar");
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                42));
            var item = TraceTestHelper.GetItem(trace,
                log);
            Assert.IsNull(item.RowsAffected);
            Assert.IsNull(item.RowsReturned);
        }

        /// <summary>
        /// Verifies deferred results are not enumerated by telemetry.
        /// </summary>
        [TestMethod]
        public void DeferredResultsRemainUnconsumed()
        {
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("Test"));
            var log = new TestCommandLog("Query");
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<IEnumerable<int>>(log,
                ThrowOnEnumeration()));
            Assert.IsNull(TraceTestHelper.GetItem(trace,
                log).RowsReturned);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies metadata supplied through the enrichment callback.
        /// </summary>
        /// <param name="item">The enriched event.</param>
        private static void AssertEnrichedMetadata(ExtendedTelemetryItem item)
        {
            Assert.AreEqual("PayrollSystem",
                item.Application);
            Assert.AreEqual("dbo.Warehouse",
                item.Table);
            Assert.AreEqual(CommandType.Text,
                item.CommandType);
            Assert.AreEqual("Northwind",
                item.Database);
            Assert.AreEqual("Northwind (SQL Server)",
                item.ConnectionName);
            Assert.AreEqual(LogEventLevel.Warning,
                item.Level);
            Assert.AreEqual("Delete was slower than expected.",
                item.Message);
            Assert.AreEqual("Delete",
                item.Properties["Operation"]);
        }

        /// <summary>
        /// Creates a deferred sequence that fails if consumed.
        /// </summary>
        /// <returns>A sequence telemetry must not enumerate.</returns>
        private static IEnumerable<int> ThrowOnEnumeration()
        {
            yield return int.Parse("This must never be enumerated",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion
    }
}

