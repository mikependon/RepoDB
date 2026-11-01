#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Verifies operation capture and value snapshots.
    /// </summary>
    [TestClass]
    public class ExtendedTelemetryTraceTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies captures parameter name.
        /// </summary>
        [TestMethod]
        public void CapturesParameterName()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual("@Id",
                parameter.Name);
        }

        /// <summary>
        /// Verifies captures parameter database type.
        /// </summary>
        [TestMethod]
        public void CapturesParameterDatabaseType()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(DbType.Int32,
                parameter.DbType);
        }

        /// <summary>
        /// Verifies captures parameter direction.
        /// </summary>
        [TestMethod]
        public void CapturesParameterDirection()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(ParameterDirection.Input,
                parameter.Direction);
        }

        /// <summary>
        /// Verifies captures parameter nullability.
        /// </summary>
        [TestMethod]
        public void CapturesParameterNullability()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(true,
                parameter.IsNullable);
        }

        /// <summary>
        /// Verifies captures parameter size.
        /// </summary>
        [TestMethod]
        public void CapturesParameterSize()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(4,
                parameter.Size);
        }

        /// <summary>
        /// Verifies captures parameter precision.
        /// </summary>
        [TestMethod]
        public void CapturesParameterPrecision()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual((byte)8,
                parameter.Precision);
        }

        /// <summary>
        /// Verifies captures parameter scale.
        /// </summary>
        [TestMethod]
        public void CapturesParameterScale()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual((byte)2,
                parameter.Scale);
        }

        /// <summary>
        /// Verifies captures parameter source column.
        /// </summary>
        [TestMethod]
        public void CapturesParameterSourceColumn()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual("Id",
                parameter.SourceColumn);
        }

        /// <summary>
        /// Verifies captures parameter source version.
        /// </summary>
        [TestMethod]
        public void CapturesParameterSourceVersion()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(DataRowVersion.Current,
                parameter.SourceVersion);
        }

        /// <summary>
        /// Verifies captures parameter value type.
        /// </summary>
        [TestMethod]
        public void CapturesParameterValueType()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var parameter = CaptureParameter(trace);

            // Assert
            Assert.AreEqual(typeof(int).FullName,
                parameter.ValueType);
        }

        /// <summary>
        /// Verifies parameter value capture follows option.
        /// </summary>
        /// <param name="enabled">Whether values are enabled.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ParameterValueCaptureFollowsOption(bool enabled)
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                CaptureParameterValues = enabled
            });

            // Act
            var captured = CaptureParameter(trace).Value.HasValue;

            // Assert
            Assert.AreEqual(enabled,
                captured);
        }

        /// <summary>
        /// Verifies result value capture follows option.
        /// </summary>
        /// <param name="enabled">Whether values are enabled.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ResultValueCaptureFollowsOption(bool enabled)
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                CaptureResultValues = enabled
            });

            // Act
            var captured = CaptureResult(trace,
                42).Result.HasValue;

            // Assert
            Assert.AreEqual(enabled,
                captured);
        }

        /// <summary>
        /// Verifies parameter snapshot survives mutation.
        /// </summary>
        [TestMethod]
        public void ParameterSnapshotSurvivesMutation()
        {
            // Setup
            var parameter = new TestParameter();
            var trace = new ObservableExtendedTelemetryTrace();
            var log = CreateParameterLog(parameter);

            // Act
            trace.BeforeExecution(log);
            parameter.Value = 99;

            // Assert
            Assert.AreEqual(42,
                trace.GetItem(log).Parameters[0].Value.Value.GetInt32());
        }

        /// <summary>
        /// Verifies captures scalar result.
        /// </summary>
        [TestMethod]
        public void CapturesScalarResult()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                42,
                "ExecuteScalar");

            // Assert
            Assert.AreEqual(42,
                item.Result.Value.GetInt32());
        }

        /// <summary>
        /// Verifies captures result type.
        /// </summary>
        [TestMethod]
        public void CapturesResultType()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                42,
                "Query");

            // Assert
            Assert.AreEqual(typeof(int).FullName,
                item.ResultType);
        }

        /// <summary>
        /// Verifies captures materialized rows.
        /// </summary>
        [TestMethod]
        public void CapturesMaterializedRows()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                new List<int> { 1, 2 },
                "Query");

            // Assert
            Assert.AreEqual(2,
                item.Result.Value.GetArrayLength());
        }

        /// <summary>
        /// Verifies counts materialized rows.
        /// </summary>
        [TestMethod]
        public void CountsMaterializedRows()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                new[] { 1, 2 },
                "Query");

            // Assert
            Assert.AreEqual(2L,
                item.RowsReturned);
        }

        /// <summary>
        /// Verifies counts affected rows.
        /// </summary>
        [TestMethod]
        public void CountsAffectedRows()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual(3L,
                item.RowsAffected);
        }

        /// <summary>
        /// Verifies scalar query leaves affected rows unknown.
        /// </summary>
        [TestMethod]
        public void ScalarQueryLeavesAffectedRowsUnknown()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                42,
                "ExecuteScalar");

            // Assert
            Assert.IsNull(item.RowsAffected);
        }

        /// <summary>
        /// Verifies deferred results remain unconsumed.
        /// </summary>
        [TestMethod]
        public void DeferredResultsRemainUnconsumed()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                ThrowOnEnumeration(),
                "Query");

            // Assert
            Assert.IsNull(item.Result);
        }

        /// <summary>
        /// Verifies deferred row count remains unknown.
        /// </summary>
        [TestMethod]
        public void DeferredRowCountRemainsUnknown()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                ThrowOnEnumeration(),
                "Query");

            // Assert
            Assert.IsNull(item.RowsReturned);
        }

        /// <summary>
        /// Verifies null result is captured.
        /// </summary>
        [TestMethod]
        public void NullResultIsCaptured()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                (string)null,
                "Query");

            // Assert
            Assert.AreEqual(JsonValueKind.Null,
                item.Result.Value.ValueKind);
        }

        /// <summary>
        /// Verifies database null is captured.
        /// </summary>
        [TestMethod]
        public void DatabaseNullIsCaptured()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                DBNull.Value,
                "Query");

            // Assert
            Assert.AreEqual(JsonValueKind.Null,
                item.Result.Value.ValueKind);
        }

        /// <summary>
        /// Verifies nested deferred values remain unconsumed.
        /// </summary>
        [TestMethod]
        public void NestedDeferredValuesRemainUnconsumed()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                new object[] { ThrowOnEnumeration(), 42 },
                "Query");

            // Assert
            Assert.AreEqual(JsonValueKind.Null,
                item.Result.Value[0].ValueKind);
        }

        /// <summary>
        /// Verifies dictionary values are captured.
        /// </summary>
        [TestMethod]
        public void DictionaryValuesAreCaptured()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                new Dictionary<string, int> {
                ["Id"] = 42
            },
                "Query");

            // Assert
            Assert.AreEqual(42,
                item.Result.Value.GetProperty("Id").GetInt32());
        }

        /// <summary>
        /// Verifies custom snapshot failure does not fail execution.
        /// </summary>
        [TestMethod]
        public void CustomSnapshotFailureDoesNotFailExecution()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                CaptureValue = value => throw new InvalidOperationException("Snapshot failed")
            });

            // Act
            var item = CaptureResult(trace,
                42);

            // Assert
            Assert.IsNull(item.Result);
        }

        /// <summary>
        /// Verifies custom snapshot uses generated metadata.
        /// </summary>
        [TestMethod]
        public void CustomSnapshotUsesGeneratedMetadata()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                CaptureValue = value => JsonSerializer.SerializeToElement((int[])value,
                TestValueJsonSerializerContext.Default.Int32Array)
            });

            // Act
            var item = CaptureResult(trace,
                new[] { 1, 2 });

            // Assert
            Assert.AreEqual(2,
                item.Result.Value.GetArrayLength());
        }

        /// <summary>
        /// Verifies application rows use generated metadata.
        /// </summary>
        [TestMethod]
        public void ApplicationRowsUseGeneratedMetadata()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("Test") {
                ValueSerializerContext = TestValueJsonSerializerContext.Default
            });
            var row = new TestResultRow
            {
                Id = 42,
                Name = "Employee",
                Deferred = ThrowOnEnumeration()
            };

            // Act
            var item = CaptureResult(trace,
                new List<TestResultRow> { row });
            row.Name = "Changed";

            // Assert
            Assert.AreEqual("Employee",
                item.Result.Value[0].GetProperty("Name").GetString());
            Assert.AreEqual(42,
                item.Result.Value[0].GetProperty("Id").GetInt32());
            Assert.IsFalse(item.Result.Value[0].TryGetProperty("Deferred",
                out _));
        }

        /// <summary>
        /// Verifies unregistered application types are omitted.
        /// </summary>
        [TestMethod]
        public void UnregisteredApplicationTypesAreOmitted()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                new TestResultRow {
                Id = 42
            });

            // Assert
            Assert.AreEqual(JsonValueKind.Null,
                item.Result.Value.ValueKind);
        }

        /// <summary>
        /// Verifies captures activity correlation.
        /// </summary>
        [TestMethod]
        public void CapturesActivityCorrelation()
        {
            // Setup
            using var activity = new Activity("Delete").SetIdFormat(ActivityIdFormat.W3C).Start();
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                3);

            // Assert
            Assert.AreEqual(activity.TraceId.ToString(),
                item.TraceId);
            Assert.AreEqual(activity.SpanId.ToString(),
                item.SpanId);
        }

        /// <summary>
        /// Verifies captures thread and parameter count.
        /// </summary>
        [TestMethod]
        public void CapturesThreadAndParameterCount()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();

            // Act
            var item = CaptureResult(trace,
                3);

            // Assert
            Assert.AreEqual(Environment.CurrentManagedThreadId,
                item.ThreadId);
            Assert.AreEqual(1,
                item.ParameterCount);
        }

        /// <summary>
        /// Verifies enrichment sets application.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsApplication()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("PayrollSystem",
                item.Application);
        }

        /// <summary>
        /// Verifies enrichment sets table.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsTable()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("dbo.Warehouse",
                item.Table);
        }

        /// <summary>
        /// Verifies enrichment sets command type.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsCommandType()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual(CommandType.Text,
                item.CommandType);
        }

        /// <summary>
        /// Verifies enrichment sets database.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsDatabase()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("Northwind",
                item.Database);
        }

        /// <summary>
        /// Verifies enrichment sets connection name.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsConnectionName()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("Northwind (SQL Server)",
                item.ConnectionName);
        }

        /// <summary>
        /// Verifies enrichment sets severity.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsSeverity()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual(LogEventLevel.Warning,
                item.Level);
        }

        /// <summary>
        /// Verifies enrichment sets message.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsMessage()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("Delete was slower than expected.",
                item.Message);
        }

        /// <summary>
        /// Verifies enrichment sets custom property.
        /// </summary>
        [TestMethod]
        public void EnrichmentSetsCustomProperty()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace(new ExtendedTelemetryOption("PayrollSystem") {
                Enrich = EnrichItem
            });

            // Act
            var item = CaptureResult(trace,
                3,
                "Delete");

            // Assert
            Assert.AreEqual("Delete",
                item.Properties["Operation"]);
        }

        /// <summary>
        /// Counts asynchronous query results.
        /// </summary>
        [TestMethod]
        public async Task AsyncCaptureRecordsCollectionCount()
        {
            // Setup
            var trace = new ObservableExtendedTelemetryTrace();
            var log = new TestCommandLog("Query");

            // Act
            await trace.BeforeExecutionAsync(log);
            await trace.AfterExecutionAsync(new TestResultLog<int[]>(log,
                new[] { 1, 2 }));

            // Assert
            Assert.AreEqual(2L,
                trace.GetItem(log).RowsReturned);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Captures a command parameter.
        /// </summary>
        /// <param name="trace">The trace under test.</param>
        /// <returns>The captured parameter.</returns>
        private static TelemetryParameter CaptureParameter(ObservableExtendedTelemetryTrace trace)
        {
            var log = CreateParameterLog(new TestParameter());
            trace.BeforeExecution(log);
            return trace.GetItem(log).Parameters[0];
        }

        /// <summary>
        /// Captures an operation result.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="trace">The trace under test.</param>
        /// <param name="result">The operation result.</param>
        /// <param name="operation">The operation name.</param>
        /// <returns>The captured event.</returns>
        private static ExtendedTelemetryItem CaptureResult<TResult>(ObservableExtendedTelemetryTrace trace,
            TResult result,
            string operation = "Query")
        {
            var log = new TestCommandLog(operation);
            trace.BeforeExecution(log);
            trace.AfterExecution(new TestResultLog<TResult>(log,
                result));
            return trace.GetItem(log);
        }

        /// <summary>
        /// Creates a parameterized command.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>The command log.</returns>
        private static CancellableTraceLog CreateParameterLog(IDbDataParameter parameter) => new CancellableTraceLog(Guid.NewGuid(),
            "Query",
            "SELECT @Id",
            new[] { parameter });

        /// <summary>
        /// Supplies application metadata.
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
        }

        /// <summary>
        /// Creates a sequence that must remain unconsumed.
        /// </summary>
        /// <returns>The deferred sequence.</returns>
        private static IEnumerable<int> ThrowOnEnumeration()
        {
            yield return int.Parse("Deferred results must not be enumerated.",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
