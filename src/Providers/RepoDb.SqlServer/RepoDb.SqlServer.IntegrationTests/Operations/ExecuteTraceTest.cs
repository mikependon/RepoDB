#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    /// <summary>
    /// Verifies against a real SQL Server what the traces are told when a statement is executed with <c>ExecuteScalar</c>, <c>ExecuteReader</c>, <c>ExecuteQuery</c>,
    /// <c>ExecuteQueryFirst</c> and <c>ExecuteQuerySingle</c> (sync and async): the result of a successful execution, and the error of a failed one that is also thrown to the caller.
    /// </summary>
    [TestClass]
    public class ExecuteTraceTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that returns one row of one column.
        /// </summary>
        private const string OneRowStatement = "SELECT 7 AS [Value];";

        /// <summary>
        /// The statement that returns two rows.
        /// </summary>
        private const string TwoRowsStatement = "SELECT 1 AS [Value] UNION ALL SELECT 2;";

        /// <summary>
        /// The statement that returns no rows.
        /// </summary>
        private const string NoRowsStatement = "SELECT 1 AS [Value] WHERE 1 = 0;";

        /// <summary>
        /// The statement that SQL Server cannot parse (error 102: incorrect syntax).
        /// </summary>
        private const string InvalidStatement = "SELECT 1 + ;";

        /// <summary>
        /// The statement that is parsed, but that the server rejects when it is executed (error 208: invalid object name).
        /// </summary>
        private const string MissingTableStatement = "SELECT [Value] FROM [dbo].[TableThatDoesNotExist];";

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
            Database.Initialize();
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        public async Task TestSqlConnectionExecuteTellsTheTraceAboutASuccessfulExecution(string operation,
            bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            await Execute(operation, connection, OneRowStatement, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            Assert.AreEqual(1, trace.AfterLogs.Count);
            Assert.IsNull(trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, trace.AfterLevels.Single());
            Assert.AreEqual("SqlConnection", trace.BeforeLogs.Single().ConnectionType);
            Assert.AreEqual(new SqlConnectionStringBuilder(Database.ConnectionString).InitialCatalog,
                trace.BeforeLogs.Single().Database);
            Assert.AreEqual(OneRowStatement, trace.BeforeLogs.Single().Statement);
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarTellsTheTraceTheResult()
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync<int>(OneRowStatement, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(7, result);
            Assert.AreEqual(7, trace.GetAfterLog<int>().Result);
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryFirstTellsTheTraceTheRow()
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync<ValueRow>(OneRowStatement, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(7, result.Value);
            Assert.AreEqual(7, trace.GetAfterLog<ValueRow>().Result.Value);
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        public async Task TestSqlConnectionExecuteTellsTheTraceAboutAStatementThatSqlServerCannotParse(string operation,
            bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(operation, connection, InvalidStatement, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(102, thrown.Number);
            Assert.AreEqual(1, trace.AfterLogs.Count);
            Assert.AreSame(thrown, trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
            Assert.IsFalse(string.IsNullOrEmpty(thrown.StackTrace));
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        public async Task TestSqlConnectionExecuteTellsTheTraceAboutAStatementThatTheServerRejectsWhenItIsExecuted(string operation,
            bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(operation, connection, MissingTableStatement, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(208, thrown.Number);
            Assert.AreSame(thrown, trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        public async Task TestSqlConnectionExecuteDoesNotLetAFailingTraceHideTheSqlException(string operation,
            bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace { ThrowOnAfter = new NotSupportedException("The trace failed.") };
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<SqlException>(() => Execute(operation, connection, InvalidStatement, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(102, thrown.Number);
            Assert.AreEqual(1, trace.AfterLogs.Count);
        }

        [TestMethod]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        public async Task TestSqlConnectionExecuteQueryFirstAndSingleTellTheTraceAboutAQueryThatHasNoRows(string operation,
            bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<EmptyException>(() => Execute(operation, connection, NoRowsStatement, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(thrown, trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TestSqlConnectionExecuteQuerySingleTellsTheTraceAboutAQueryThatHasMoreThanOneRow(bool asynchronous)
        {
            // Setup
            var trace = new RecordingTrace();
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<MultipleRowsFoundException>(() => Execute("Single", connection, TwoRowsStatement, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(thrown, trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
        }

        [TestMethod]
        [DataRow("Scalar")]
        [DataRow("Query")]
        [DataRow("First")]
        public async Task TestSqlConnectionExecuteAsyncStillTellsTheTraceAboutAFailureWhenTheTokenHasBeenCancelled(string operation)
        {
            // Setup
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace { OnBefore = source.Cancel };
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => Execute(operation, connection, OneRowStatement, trace, true, source.Token)).ConfigureAwait(false);
            Assert.IsInstanceOfType<OperationCanceledException>(trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
            Assert.AreEqual(CancellationToken.None, trace.AfterTokens.Single());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the statement with the sync or the async extension method of the operation.
        /// </summary>
        /// <param name="operation">The operation to execute: Scalar, Reader, Query, First or Single.</param>
        /// <param name="connection">The connection to be used.</param>
        /// <param name="statement">The statement to be executed.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation.</returns>
        private static async Task Execute(string operation,
            SqlConnection connection,
            string statement,
            RecordingTrace trace,
            bool asynchronous,
            CancellationToken cancellationToken = default)
        {
            switch (operation)
            {
                case "Scalar":
                    if (asynchronous) await connection.ExecuteScalarAsync<int>(statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteScalar<int>(statement, trace: trace);
                    break;
                case "Reader":
                    if (asynchronous) (await connection.ExecuteReaderAsync(statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))?.Dispose();
                    else connection.ExecuteReader(statement, trace: trace)?.Dispose();
                    break;
                case "Query":
                    if (asynchronous) await connection.ExecuteQueryAsync<ValueRow>(statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuery<ValueRow>(statement, trace: trace)?.ToList();
                    break;
                case "First":
                    if (asynchronous) await connection.ExecuteQueryFirstAsync<ValueRow>(statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQueryFirst<ValueRow>(statement, trace: trace);
                    break;
                case "Single":
                    if (asynchronous) await connection.ExecuteQuerySingleAsync<ValueRow>(statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuerySingle<ValueRow>(statement, trace: trace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// The entity that the queries map to.
        /// </summary>
        public class ValueRow
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public int Value { get; set; }
        }

        #endregion
    }
}
