#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Telemetry.Core;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies the complete capture, compression, and HTTP publishing pipeline.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryPublisherRepositoryTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies a captured event survives the complete publishing pipeline.
        /// </summary>
        [TestMethod]
        public async Task CaptureToHttpPreservesExtendedMetadata()
        {
            using var collector = new TestCollector();
            Exception error = null;
            var trace = new ExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem")
            {
                ApiKey = "test-key",
                Host = collector.Host,
                Enrich = (item, log) =>
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
            },
                exception => error = exception);
            var log = new TestCommandLog();
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<int>(log,
                3));
            log.Cancel(false);
            var received = collector.ReceiveAsync();
            await Task.Run(() => TraceTestHelper.Flush(trace));
            using var payload = await received;
            var item = payload.RootElement[0];
            Assert.IsNull(error);
            Assert.AreEqual("/v1/telemetry/extended/publish",
                collector.Path);
            Assert.AreEqual("test-key",
                collector.ApiKey);
            AssertPublishedEvent(item);
        }

        /// <summary>
        /// Verifies all publishing entry points retain derived properties.
        /// </summary>
        /// <param name="mode">The publishing entry point.</param>
        [TestMethod]
        [DataRow("Single")]
        [DataRow("Many")]
        [DataRow("SingleAsync")]
        [DataRow("ManyAsync")]
        public async Task PublishingEntryPointsPreserveDerivedFields(string mode)
        {
            using var collector = new TestCollector();
            var repository = new ExtendedTelemetryPublisherRepository(collector.Host);
            var item = new ExtendedTelemetryItem
            {
                SpanId = "6be302d56be302d5",
                TraceId = "538453d7538453d7538453d7538453d7"
            };
            var received = collector.ReceiveAsync();
            await Task.Run(async () =>
            {
                switch (mode)
                {
                    case "Single":
                        repository.Publish(item);
                        break;
                    case "Many":
                        repository.PublishMany(new TelemetryItem[] { item, item });
                        break;
                    case "SingleAsync":
                        await repository.PublishAsync(item);
                        break;
                    default:
                        await repository.PublishManyAsync(new TelemetryItem[] { item, item });
                        break;
                }
            });
            using var payload = await received;
            Assert.AreEqual(mode.StartsWith("Many", StringComparison.Ordinal) ? 2 : 1,
                payload.RootElement.GetArrayLength());
            Assert.AreEqual(item.TraceId,
                payload.RootElement[0].GetProperty("TraceId").GetString());
            Assert.AreEqual(item.SpanId,
                payload.RootElement[0].GetProperty("SpanId").GetString());
        }

        /// <summary>
        /// Verifies collector failures reach the configured error callback.
        /// </summary>
        [TestMethod]
        public async Task HttpFailureInvokesErrorCallback()
        {
            using var collector = new TestCollector();
            Exception error = null;
            var repository = new ExtendedTelemetryPublisherRepository(collector.Host,
                errorCallback: exception => error = exception);
            var received = collector.ReceiveAsync(500);
            await repository.PublishAsync(new ExtendedTelemetryItem());
            using var payload = await received;
            Assert.IsInstanceOfType<InvalidOperationException>(error);
            Assert.IsNotNull(error.InnerException);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies the common fields in the decoded event.
        /// </summary>
        /// <param name="item">The decoded event.</param>
        private static void AssertPublishedEvent(JsonElement item)
        {
            Assert.AreEqual("PayrollSystem",
                item.GetProperty("Application").GetString());
            Assert.AreEqual(320d,
                item.GetProperty("Elapsed").GetDouble());
            Assert.IsTrue(item.GetProperty("IsCancelled").GetBoolean());
            Assert.AreEqual(3L,
                item.GetProperty("RowsAffected").GetInt64());
            Assert.AreEqual("Northwind",
                item.GetProperty("Database").GetString());
            Assert.AreEqual("dbo.Warehouse",
                item.GetProperty("Table").GetString());
            Assert.AreEqual("Northwind (SQL Server)",
                item.GetProperty("ConnectionName").GetString());
            Assert.AreEqual((int)CommandType.Text,
                item.GetProperty("CommandType").GetInt32());
            Assert.AreEqual((int)LogEventLevel.Warning,
                item.GetProperty("Level").GetInt32());
            Assert.AreEqual("Delete was slower than expected.",
                item.GetProperty("Message").GetString());
            Assert.AreEqual(320,
                item.GetProperty("Properties").GetProperty("ElapsedMilliseconds").GetInt32());
            Assert.AreEqual(1,
                item.GetProperty("ParameterCount").GetInt32());
            Assert.IsTrue(item.GetProperty("ThreadId").GetInt32() > 0);
        }

        #endregion
    }
}

