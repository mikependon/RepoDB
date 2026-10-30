#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Setup;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    /// <summary>
    /// Verifies native bulk completion and original-error propagation with real SQL Server failures.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class BulkTracingTest
    {
        #region Private Variables

        private string _tableName;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a unique target table so tracing tests cannot alter other fixtures.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            _tableName = "BulkTracing_" + Guid.NewGuid().ToString("N");
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($"CREATE TABLE [dbo].[{_tableName}] ([Id] INT NOT NULL PRIMARY KEY, [Value] INT NOT NULL); INSERT INTO [dbo].[{_tableName}] VALUES (1, 10);");
        }

        /// <summary>
        /// Removes the table created exclusively for this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($"DROP TABLE IF EXISTS [dbo].[{_tableName}];");
        }

        /// <summary>
        /// Supplies each bulk operation, input form, and supported execution mode.
        /// </summary>
        /// <returns>The bulk API combinations.</returns>
        public static IEnumerable<object[]> Cases()
        {
            foreach (var operation in new[] { "Insert", "Update", "Merge", "Delete" })
            {
                foreach (var form in new[] { "Entities", "Table", "Reader" })
                {
                    yield return new object[] { operation, form, false };
                    yield return new object[] { operation, form, true };
                }
            }
            yield return new object[] { "DeleteByKey", "Keys", false };
            yield return new object[] { "DeleteByKey", "Keys", true };
        }

        /// <summary>
        /// Verifies successful bulk workflows complete every started trace once.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task SuccessCompletesEveryTrace(string operation,
            string form,
            bool asynchronous)
        {
            var trace = new BulkRecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);
            var count = await BulkTraceExecutor.ExecuteAsync(connection,
                _tableName,
                operation,
                form,
                asynchronous,
                trace).ConfigureAwait(false);
            Assert.AreEqual(1,
                count);
            AssertCompleted(trace);
            Assert.IsTrue(trace.Errors.Values.All(error => error == null));
        }

        /// <summary>
        /// Verifies cancellation completes the outer trace without writing data.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task CancellationCompletesTrace(string operation,
            string form,
            bool asynchronous)
        {
            var trace = new BulkRecordingTrace { Cancel = true };
            using var connection = new SqlConnection(Database.ConnectionString);
            var count = await BulkTraceExecutor.ExecuteAsync(connection,
                _tableName,
                operation,
                form,
                asynchronous,
                trace).ConfigureAwait(false);
            Assert.AreEqual(0,
                count);
            AssertCompleted(trace);
            Assert.AreEqual(1,
                trace.Starts.Count);
            Assert.AreEqual(1,
                connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM [dbo].[{_tableName}];"));
        }

        /// <summary>
        /// Verifies SQL Server failures are recorded and rethrown without replacement.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task FailureIsRecordedAndRethrown(string operation,
            string form,
            bool asynchronous)
        {
            await AssertFailureAsync(operation,
                form,
                asynchronous,
                false).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies a throwing observer cannot hide a native bulk failure.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task ObserverFailurePreservesBulkFailure(string operation,
            string form,
            bool asynchronous)
        {
            await AssertFailureAsync(operation,
                form,
                asynchronous,
                true).ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Forces a database failure using a target trigger and verifies correlated completion.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="form">The input representation.</param>
        /// <param name="asynchronous">Whether to invoke the asynchronous API.</param>
        /// <param name="throwOnError">Whether the observer also fails.</param>
        /// <returns>A task representing the assertions.</returns>
        private async Task AssertFailureAsync(string operation,
            string form,
            bool asynchronous,
            bool throwOnError)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($"CREATE TRIGGER [dbo].[TR_{_tableName}] ON [dbo].[{_tableName}] AFTER INSERT, UPDATE, DELETE AS THROW 51001, 'Intentional bulk tracing test failure.', 1;");
            var trace = new BulkRecordingTrace { ThrowOnError = throwOnError };
            var thrown = await Assert.ThrowsAsync<Exception>(() =>
                BulkTraceExecutor.ExecuteAsync(connection,
                    _tableName,
                    operation,
                    form,
                    asynchronous,
                    trace)).ConfigureAwait(false);
            AssertCompleted(trace);
            Assert.AreSame(thrown,
                trace.Errors[trace.Starts[0].SessionId]);
            StringAssert.Contains(thrown.StackTrace,
                "RepoDb",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Verifies all bulk and nested SQL executions have one correlated completion.
        /// </summary>
        /// <param name="trace">The recorded notifications.</param>
        private static void AssertCompleted(BulkRecordingTrace trace)
        {
            Assert.IsNotEmpty(trace.Starts);
            foreach (var start in trace.Starts)
            {
                Assert.IsTrue(trace.Completions.TryGetValue(start.SessionId,
                    out var count));
                Assert.AreEqual(1,
                    count);
            }
        }

        #endregion
    }
}

