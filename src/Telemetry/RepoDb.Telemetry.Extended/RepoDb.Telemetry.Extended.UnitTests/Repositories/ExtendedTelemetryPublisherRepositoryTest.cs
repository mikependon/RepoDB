#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies completed event publishing and compression.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryPublisherRepositoryTest
    {
        #region Public Methods

        /// <summary>
        /// Retains operations across publishing ticks.
        /// </summary>
        [TestMethod]
        public void RunningOperationRemainsBuffered()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();
            var log = new TestCommandLog();
            trace.BeforeExecution(log);

            // Act
            trace.Flush();
            trace.Flush();

            // Assert
            Assert.AreEqual(log.SessionId,
                trace.GetItem(log).SessionId);
        }

        /// <summary>
        /// Publishes failures received after earlier ticks.
        /// </summary>
        [TestMethod]
        public async Task RunningOperationPublishesItsFinalOutcome()
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);
            var log = new TestCommandLog();
            trace.BeforeExecution(log);
            trace.Flush();

            // Act
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                new InvalidOperationException("Late failure")));
            using var payload = await FlushAsync(trace,
                collector);

            // Assert
            Assert.AreEqual("Late failure",
                payload.RootElement[0].GetProperty("ErrorMessage").GetString());
            Assert.AreEqual(320d,
                payload.RootElement[0].GetProperty("Elapsed").GetDouble());
        }

        /// <summary>
        /// Recovers the command from an after-log.
        /// </summary>
        [TestMethod]
        public async Task ResultRecoversItsBeforeExecutionLog()
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);
            var log = new TestCommandLog();

            // Act
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            using var payload = await FlushAsync(trace,
                collector);

            // Assert
            Assert.AreEqual(log.SessionId,
                payload.RootElement[0].GetProperty("SessionId").GetGuid());
            Assert.AreEqual(3L,
                payload.RootElement[0].GetProperty("RowsAffected").GetInt64());
        }

        /// <summary>
        /// Preserves cancellation exception behavior.
        /// </summary>
        /// <param name="throwException">Whether cancellation throws.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CancellationPreservesExceptionBehavior(bool throwException)
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);
            var log = new TestCommandLog();
            trace.BeforeExecution(log);

            // Act
            log.Cancel(throwException);
            using var payload = await FlushAsync(trace,
                collector);

            // Assert
            Assert.IsTrue(payload.RootElement[0].GetProperty("IsCancelled").GetBoolean());
            Assert.AreEqual(throwException,
                payload.RootElement[0].GetProperty("IsThrowException").GetBoolean());
        }

        /// <summary>
        /// Preserves captured fields through compressed HTTP publishing.
        /// </summary>
        /// <param name="field">The published field.</param>
        /// <param name="expected">The expected JSON value.</param>
        [TestMethod]
        [DataRow("Application", "\"PayrollSystem\"")]
        [DataRow("Elapsed", "320")]
        [DataRow("RowsAffected", "3")]
        [DataRow("Result", "null")]
        [DataRow("ResultType", "\"System.Int32\"")]
        [DataRow("TraceLevel", "3")]
        [DataRow("Database", "\"Northwind\"")]
        [DataRow("Table", "\"dbo.Warehouse\"")]
        [DataRow("ConnectionName", "\"Northwind (SQL Server)\"")]
        [DataRow("CommandType", "1")]
        [DataRow("Level", "3")]
        [DataRow("ParameterCount", "1")]
        [DataRow("Message", "\"Delete was slower than expected.\"")]
        public async Task CaptureToHttpPreservesField(string field,
            string expected)
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);

            // Act
            using var payload = await PublishOperationAsync(trace,
                collector);

            // Assert
            Assert.AreEqual(expected,
                payload.RootElement[0].GetProperty(field).GetRawText());
        }

        /// <summary>
        /// Publishes authentication and route metadata.
        /// </summary>
        [TestMethod]
        public async Task PublishingUsesConfiguredRouteAndApiKey()
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);

            // Act
            using var payload = await PublishOperationAsync(trace,
                collector);

            // Assert
            Assert.AreEqual("/v1/telemetry/extended/publish",
                collector.Path);
            Assert.AreEqual("test-key",
                collector.ApiKey);
        }

        /// <summary>
        /// Compresses all publishing entry points.
        /// </summary>
        /// <param name="mode">The publishing entry point.</param>
        [TestMethod]
        [DataRow("Single")]
        [DataRow("Many")]
        [DataRow("SingleAsync")]
        [DataRow("ManyAsync")]
        public async Task PublishingEntryPointsCompressAndPreserveEvents(string mode)
        {
            // Setup
            using var collector = new TestCollector();
            var repository = new ExtendedTelemetryPublisherRepository(collector.Host);
            var item = new ExtendedTelemetryItem
            {
                TraceId = "trace-id",
                SpanId = "span-id"
            };
            var received = collector.ReceiveAsync();

            // Act
            await PublishAsync(repository,
                item,
                mode);
            using var payload = await received;

            // Assert
            Assert.IsTrue(collector.IsGzipCompressed);
            Assert.AreEqual(mode.StartsWith("Many",
                StringComparison.Ordinal) ? 2 : 1,
                payload.RootElement.GetArrayLength());
            Assert.AreEqual(item.TraceId,
                payload.RootElement[0].GetProperty("TraceId").GetString());
        }

        /// <summary>
        /// Reports unsuccessful HTTP responses.
        /// </summary>
        [TestMethod]
        public async Task HttpFailureInvokesErrorCallback()
        {
            // Setup
            using var collector = new TestCollector();
            Exception error = null;
            var repository = new ExtendedTelemetryPublisherRepository(collector.Host,
                errorCallback: exception => error = exception);
            var received = collector.ReceiveAsync(500);

            // Act
            await repository.PublishAsync(new ExtendedTelemetryItem());
            using var payload = await received;

            // Assert
            Assert.IsInstanceOfType<InvalidOperationException>(error);
            Assert.IsNotNull(error.InnerException);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates an enriched publishing trace.
        /// </summary>
        /// <param name="collector">The receiving collector.</param>
        /// <returns>The configured trace.</returns>
        private static ObservableExtendedTelemetryTrace CreateTrace(TestCollector collector) => new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
            ApiKey = "test-key",
            Host = collector.Host,
            Enrich = EnrichItem
        });

        /// <summary>
        /// Adds application metadata.
        /// </summary>
        /// <param name="item">The event to enrich.</param>
        /// <param name="log">The command log.</param>
        private static void EnrichItem(ExtendedTelemetryItem item,
            CancellableTraceLog log)
        {
            item.CommandType = CommandType.Text;
            item.ConnectionName = "Northwind (SQL Server)";
            item.Database = "Northwind";
            item.Level = LogEventLevel.Warning;
            item.Message = "Delete was slower than expected.";
            item.Table = "dbo.Warehouse";
            item.Properties["Operation"] = log.Key;
            item.Properties["ElapsedMilliseconds"] = 320;
        }

        /// <summary>
        /// Publishes pending events.
        /// </summary>
        /// <param name="trace">The trace under test.</param>
        /// <param name="collector">The receiving collector.</param>
        /// <returns>The decoded payload.</returns>
        private static async Task<JsonDocument> FlushAsync(ObservableExtendedTelemetryTrace trace,
            TestCollector collector)
        {
            var received = collector.ReceiveAsync();
            await Task.Run(trace.Flush);
            return await received;
        }

        /// <summary>
        /// Invokes a publishing entry point.
        /// </summary>
        /// <param name="repository">The publishing repository.</param>
        /// <param name="item">The event to publish.</param>
        /// <param name="mode">The entry point.</param>
        /// <returns>The publishing task.</returns>
        private static Task PublishAsync(ExtendedTelemetryPublisherRepository repository,
            TelemetryItem item,
            string mode) => mode switch
        {
            "Single" => Task.Run(() => repository.Publish(item)),
            "Many" => Task.Run(() => repository.PublishMany(new[] { item, item })),
            "SingleAsync" => repository.PublishAsync(item),
            _ => repository.PublishManyAsync(new[] { item, item })};

        /// <summary>
        /// Captures and publishes a successful command.
        /// </summary>
        /// <param name="trace">The trace under test.</param>
        /// <param name="collector">The receiving collector.</param>
        /// <returns>The decoded payload.</returns>
        private static Task<JsonDocument> PublishOperationAsync(ObservableExtendedTelemetryTrace trace,
            TestCollector collector)
        {
            var log = new TestCommandLog();
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            return FlushAsync(trace,
                collector);
        }

        #endregion
    }
}
