#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies the context that is captured when an operation starts, and the error of a failed operation, survive the complete publishing pipeline.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryFailurePublishingTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies a failed operation is published with its error, level and context.
        /// </summary>
        [TestMethod]
        public async Task FailedOperationIsPublishedWithItsErrorAndContext()
        {
            using var collector = new TestCollector();
            Exception callbackError = null;
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem")
            {
                ApiKey = "test-key",
                Host = collector.Host
            },
                exception => callbackError = exception);
            var log = new TestCommandLog(new TraceContext(4242,
                "PC-01",
                "10.0.0.7",
                "0af7651916cd43dd8448eb211c80319c",
                "b7ad6b7169203331",
                "SqlConnection",
                "Northwind"));
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                0,
                CreateThrownError()));
            var received = collector.ReceiveAsync();
            await Task.Run(() => TraceTestHelper.Flush(trace));
            using var payload = await received;
            var item = payload.RootElement[0];
            Assert.IsNull(callbackError);
            Assert.AreEqual((int)LogEventLevel.Error,
                item.GetProperty("Level").GetInt32());
            Assert.AreEqual(typeof(InvalidOperationException).FullName,
                item.GetProperty("ErrorType").GetString());
            Assert.AreEqual("The statement failed.",
                item.GetProperty("ErrorMessage").GetString());
            Assert.IsFalse(string.IsNullOrEmpty(item.GetProperty("ErrorStackTrace").GetString()));
            Assert.AreEqual("SqlConnection",
                item.GetProperty("ConnectionType").GetString());
            Assert.AreEqual("Northwind",
                item.GetProperty("Database").GetString());
            Assert.AreEqual("PC-01",
                item.GetProperty("Client").GetString());
            Assert.AreEqual("10.0.0.7",
                item.GetProperty("IpAddress").GetString());
            Assert.AreEqual(4242,
                item.GetProperty("ThreadId").GetInt32());
            Assert.AreEqual("0af7651916cd43dd8448eb211c80319c",
                item.GetProperty("TraceId").GetString());
            Assert.AreEqual("b7ad6b7169203331",
                item.GetProperty("SpanId").GetString());
        }

        #endregion

        #region Private Methods

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
