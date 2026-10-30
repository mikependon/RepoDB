#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    /// <summary>
    /// Verifies against a real SQL Server what the traces are told when a statement is executed with <c>ExecuteNonQuery</c> and <c>ExecuteNonQueryAsync</c>:
    /// the context of the execution, and that they are told about a failure, whether the server rejects the statement or the statement fails after it started.
    /// </summary>
    [TestClass]
    public class ExecuteNonQueryTraceTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that inserts a row into the table that has an identity.
        /// </summary>
        private const string InsertStatement = "INSERT INTO [dbo].[IdentityCompleteTable] ([SessionId]) VALUES (@SessionId);";

        /// <summary>
        /// The statement that inserts a row into the table whose primary key is given. The same key cannot be inserted twice.
        /// </summary>
        private const string InsertWithKeyStatement = "INSERT INTO [dbo].[NonIdentityCompleteTable] ([Id], [SessionId]) VALUES (@Id, @SessionId);";

        /// <summary>
        /// The statement that SQL Server cannot parse (error 102: incorrect syntax).
        /// </summary>
        private const string InvalidStatement = "SELECT 1 + ;";

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
            Database.Initialize();
            Database.Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryTellsTheTraceAboutASuccessfulExecution(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            var threadId = Environment.CurrentManagedThreadId;

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            var before = trace.BeforeLogs[0];
            var after = trace.GetAfterLog<int>();
            Assert.AreEqual(InsertStatement, before.Statement);
            Assert.AreEqual(1, before.Parameters.Count());
            Assert.AreEqual(1, after.Result);
            Assert.IsNull(after.Error);
            Assert.AreEqual(TraceLevel.Info, after.Level);
            Assert.AreEqual(before.SessionId, after.SessionId);
            Assert.IsTrue(after.ExecutionTime > TimeSpan.Zero);
            Assert.AreEqual(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[IdentityCompleteTable];"));
            Assert.AreEqual(threadId, before.ThreadId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryCapturesTheContextOfTheExecution(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            await Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, asynchronous).ConfigureAwait(false);

            // Assert
            var after = trace.GetAfterLog<int>();
            Assert.AreEqual("SqlConnection", after.ConnectionType);
            Assert.AreEqual(new SqlConnectionStringBuilder(Database.ConnectionString).InitialCatalog,
                after.Database);
            Assert.AreEqual(Environment.MachineName, after.PcName);
            if (after.IpAddress != null)
            {
                Assert.IsTrue(IPAddress.TryParse(after.IpAddress, out _), $"'{after.IpAddress}' is not an IP address.");
            }
            Assert.IsNull(after.TraceId);
            Assert.IsNull(after.SpanId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryCapturesTheTraceAndTheSpanOfTheActiveActivity(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            var activity = new Activity("Payroll");
            activity.SetIdFormat(ActivityIdFormat.W3C);
            activity.Start();

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            await Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, asynchronous).ConfigureAwait(false);
            await Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, asynchronous).ConfigureAwait(false);
            activity.Stop();

            // Assert
            Assert.AreEqual(2, trace.BeforeLogs.Count);
            Assert.IsTrue(trace.BeforeLogs.All(log => log.TraceId == activity.TraceId.ToString()));
            Assert.IsTrue(trace.BeforeLogs.All(log => log.SpanId == activity.SpanId.ToString()));
            Assert.AreNotEqual(trace.BeforeLogs[0].SessionId, trace.BeforeLogs[1].SessionId);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryTellsTheTraceAboutAStatementThatSqlServerCannotParse(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(connection, InvalidStatement, null, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(102, thrown.Number);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            var after = trace.GetAfterLog<int>();
            Assert.AreSame(thrown, after.Error);
            Assert.AreEqual(TraceLevel.Error, after.Level);
            Assert.AreEqual(0, after.Result);
            Assert.AreEqual("SqlConnection", after.ConnectionType);
            Assert.AreEqual(new SqlConnectionStringBuilder(Database.ConnectionString).InitialCatalog,
                after.Database);
            Assert.IsFalse(string.IsNullOrEmpty(thrown.StackTrace));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryTellsTheTraceAboutAStatementThatFailsOnTheServer(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);
            await Execute(connection, InsertWithKeyStatement, new { Id = 1, SessionId = Guid.NewGuid() }, null, asynchronous).ConfigureAwait(false);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(connection, InsertWithKeyStatement, new { Id = 1, SessionId = Guid.NewGuid() }, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(2627, thrown.Number);
            var after = trace.GetAfterLog<int>();
            Assert.AreSame(thrown, after.Error);
            Assert.AreEqual(TraceLevel.Error, after.Level);
            Assert.AreEqual(trace.BeforeLogs.Single().SessionId, after.SessionId);
            Assert.IsTrue(after.ExecutionTime > TimeSpan.Zero);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteNonQueryDoesNotLetAFailingTraceHideTheSqlException(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace { ThrowOnAfter = new NotSupportedException("The trace failed.") };
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(connection, InvalidStatement, null, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(102, thrown.Number);
            Assert.AreEqual(1, trace.AfterLogs.Count);
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncGivesTheTraceTheCancellationTokenWhenTheExecutionSucceeds()
        {
            // Setup
            var trace = new RecordingTrace();
            using var source = new CancellationTokenSource();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            await Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, true, source.Token).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(source.Token, trace.AfterTokens.Single());
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncStillTellsTheTraceAboutAFailureWhenTheTokenHasBeenCancelled()
        {
            // Setup
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace { OnBefore = source.Cancel };
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => Execute(connection, InsertStatement, new { SessionId = Guid.NewGuid() }, trace, true, source.Token)).ConfigureAwait(false);
            var after = trace.GetAfterLog<int>();
            Assert.IsInstanceOfType<OperationCanceledException>(after.Error);
            Assert.AreEqual(TraceLevel.Error, after.Level);
            Assert.AreEqual(CancellationToken.None, trace.AfterTokens.Single());
            Assert.AreEqual(0, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[IdentityCompleteTable];"));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the statement with the sync or the async extension method.
        /// </summary>
        /// <param name="connection">The connection to be used.</param>
        /// <param name="statement">The statement to be executed.</param>
        /// <param name="parameters">The parameters of the statement, or <c>null</c>.</param>
        /// <param name="trace">The trace object to be used, or <c>null</c>.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation. The task result is the number of rows affected.</returns>
        private static async Task<int> Execute(SqlConnection connection,
            string statement,
            object parameters,
            RecordingTrace trace,
            bool asynchronous,
            CancellationToken cancellationToken = default)
        {
            return asynchronous
                ? await connection.ExecuteNonQueryAsync(statement, parameters, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false)
                : connection.ExecuteNonQuery(statement, parameters, trace: trace);
        }

        #endregion
    }
}
