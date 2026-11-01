#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies failed event publishing.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryFailurePublishingTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves failure details and operation context.
        /// </summary>
        /// <param name="field">The published field.</param>
        /// <param name="expected">The expected JSON value.</param>
        [TestMethod]
        [DataRow("Level", "4")]
        [DataRow("ErrorType", "\"System.InvalidOperationException\"")]
        [DataRow("ErrorMessage", "\"The statement failed.\"")]
        [DataRow("ConnectionType", "\"SqlConnection\"")]
        [DataRow("Database", "\"Northwind\"")]
        [DataRow("Client", "\"PC-01\"")]
        [DataRow("IpAddress", "\"10.0.0.7\"")]
        [DataRow("ThreadId", "4242")]
        [DataRow("TraceId", "\"0af7651916cd43dd8448eb211c80319c\"")]
        [DataRow("SpanId", "\"b7ad6b7169203331\"")]
        public async Task FailedOperationPreservesPublishedField(string field,
            string expected)
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);

            // Act
            using var payload = await PublishFailureAsync(trace,
                collector);

            // Assert
            Assert.AreEqual(expected,
                payload.RootElement[0].GetProperty(field).GetRawText());
        }

        /// <summary>
        /// Preserves the thrown exception stack.
        /// </summary>
        [TestMethod]
        public async Task FailedOperationPublishesErrorStackTrace()
        {
            // Setup
            using var collector = new TestCollector();
            var trace = CreateTrace(collector);

            // Act
            using var payload = await PublishFailureAsync(trace,
                collector);

            // Assert
            StringAssert.Contains(payload.RootElement[0].GetProperty("ErrorStackTrace").GetString(),
                nameof(CreateThrownError),
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Publishes operation failures without a publishing error.
        /// </summary>
        [TestMethod]
        public async Task OperationFailureDoesNotInvokePublishingErrorCallback()
        {
            // Setup
            using var collector = new TestCollector();
            Exception publishingError = null;
            var trace = CreateTrace(collector,
                error => publishingError = error);

            // Act
            using var payload = await PublishFailureAsync(trace,
                collector);

            // Assert
            Assert.IsNull(publishingError);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a contextual command log.
        /// </summary>
        /// <returns>The command log.</returns>
        private static TestCommandLog CreateLog() => new TestCommandLog(new TraceContext(4242,
            "PC-01",
            "10.0.0.7",
            "0af7651916cd43dd8448eb211c80319c",
            "b7ad6b7169203331",
            "SqlConnection",
            "Northwind"));

        /// <summary>
        /// Creates an exception with a stack trace.
        /// </summary>
        /// <returns>The thrown exception.</returns>
        private static InvalidOperationException CreateThrownError()
        {
            try
            {
                throw new InvalidOperationException("The statement failed.");
            }
            catch (InvalidOperationException error)
            {
                return error;
            }
        }

        /// <summary>
        /// Creates a publishing trace.
        /// </summary>
        /// <param name="collector">The receiving collector.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <returns>The configured trace.</returns>
        private static ObservableExtendedTelemetryTrace CreateTrace(TestCollector collector,
            Action<Exception> errorCallback = null) => new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
            Host = collector.Host
        },
            errorCallback);

        /// <summary>
        /// Captures and publishes an operation failure.
        /// </summary>
        /// <param name="trace">The trace under test.</param>
        /// <param name="collector">The receiving collector.</param>
        /// <returns>The decoded payload.</returns>
        private static async Task<JsonDocument> PublishFailureAsync(ObservableExtendedTelemetryTrace trace,
            TestCollector collector)
        {
            var log = CreateLog();
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                CreateThrownError()));
            var received = collector.ReceiveAsync();
            await Task.Run(trace.Flush);
            return await received;
        }

        #endregion
    }
}
