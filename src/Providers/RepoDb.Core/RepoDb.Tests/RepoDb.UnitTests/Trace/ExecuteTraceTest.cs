#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Contains the tests of the tracing of the <c>ExecuteScalar</c>, <c>ExecuteReader</c>, <c>ExecuteQuery</c>, <c>ExecuteQueryFirst</c> and <c>ExecuteQuerySingle</c>
    /// extension methods (dynamic and typed, sync and async): that the traces are always told that the execution has completed, with the error if it has failed,
    /// and that an error is never hidden from the caller.
    /// </summary>
    [TestClass]
    public class ExecuteTraceTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that the tests execute.
        /// </summary>
        private const string Statement = "SELECT [Id] FROM [Table]";

        #endregion

        #region Public Methods

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<CustomTracedDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<CustomTracedDbConnection>(new CustomStatementBuilder(), true);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
        }

        [TestInitialize]
        public void Initialize()
        {
            Activity.Current = null;
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("QueryTyped", false)]
        [DataRow("QueryTyped", true)]
        public async Task ExecuteTellsTheTraceBeforeAndAfterASuccessfulExecution(string operation,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection { ScalarValue = 5 };
            var trace = new RecordingTrace();

            // Act
            await Execute(operation, connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, connection.ExecutionCount);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            Assert.AreEqual(1, trace.AfterLogs.Count);
            Assert.IsNull(trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Info, trace.AfterLevels.Single());
            Assert.AreEqual("CustomTracedDbConnection", trace.BeforeLogs.Single().ConnectionType);
            Assert.AreEqual("TestDatabase", trace.BeforeLogs.Single().Database);
        }

        [TestMethod]
        public async Task ExecuteScalarTellsTheTraceTheResult()
        {
            // Setup
            using var connection = new CustomTracedDbConnection { ScalarValue = 5 };
            var trace = new RecordingTrace();

            // Act
            var result = await connection.ExecuteScalarAsync<int>(Statement, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, trace.AfterResults.Single());
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        [DataRow("QueryTyped", false)]
        [DataRow("QueryTyped", true)]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("FirstTyped", false)]
        [DataRow("FirstTyped", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        [DataRow("SingleTyped", false)]
        [DataRow("SingleTyped", true)]
        public async Task ExecuteTellsTheTraceThatTheExecutionHasFailedAndThrowsTheSameErrorToTheCaller(string operation,
            bool asynchronous)
        {
            // Setup
            var failure = new InvalidOperationException("The execution failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace();

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(operation, connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            Assert.AreEqual(1, trace.AfterLogs.Count);
            Assert.AreSame(failure, trace.AfterErrors.Single());
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
        public async Task ExecuteDoesNotLetAFailingTraceHideTheErrorOfTheExecution(string operation,
            bool asynchronous)
        {
            // Setup
            var failure = new InvalidOperationException("The execution failed.");
            using var connection = new CustomTracedDbConnection { Failure = failure };
            var trace = new RecordingTrace { ThrowOnAfter = new NotSupportedException("The trace failed.") };

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Execute(operation, connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(1, trace.AfterLogs.Count);
        }

        [TestMethod]
        [DataRow("First", false)]
        [DataRow("First", true)]
        [DataRow("FirstTyped", false)]
        [DataRow("FirstTyped", true)]
        [DataRow("Single", false)]
        [DataRow("Single", true)]
        [DataRow("SingleTyped", false)]
        [DataRow("SingleTyped", true)]
        public async Task ExecuteQueryFirstAndSingleTellTheTraceAboutAQueryThatHasNoRows(string operation,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace();

            // Act/Assert
            var thrown = await Assert.ThrowsExactlyAsync<EmptyException>(() => Execute(operation, connection, trace, asynchronous)).ConfigureAwait(false);
            Assert.AreSame(thrown, trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
        }

        [TestMethod]
        [DataRow("Scalar")]
        [DataRow("Reader")]
        [DataRow("Query")]
        public async Task ExecuteAsyncGivesTheTraceTheCancellationTokenWhenTheExecutionSucceeds(string operation)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace();

            // Act
            await Execute(operation, connection, trace, true, source.Token).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(source.Token, trace.AfterTokens.Single());
        }

        [TestMethod]
        [DataRow("Scalar")]
        [DataRow("Reader")]
        [DataRow("Query")]
        [DataRow("QueryTyped")]
        [DataRow("First")]
        [DataRow("Single")]
        public async Task ExecuteAsyncStillTellsTheTraceAboutAFailureWhenTheTokenHasBeenCancelled(string operation)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            using var source = new CancellationTokenSource();
            var trace = new RecordingTrace { OnBefore = source.Cancel };

            // Act/Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => Execute(operation, connection, trace, true, source.Token)).ConfigureAwait(false);
            Assert.IsInstanceOfType<OperationCanceledException>(trace.AfterErrors.Single());
            Assert.AreEqual(TraceLevel.Error, trace.AfterLevels.Single());
            Assert.AreEqual(CancellationToken.None, trace.AfterTokens.Single());
        }

        [TestMethod]
        [DataRow("Scalar", false)]
        [DataRow("Scalar", true)]
        [DataRow("Reader", false)]
        [DataRow("Reader", true)]
        [DataRow("Query", false)]
        [DataRow("Query", true)]
        public async Task ExecuteDoesNotExecuteWhenTheTraceCancelsItAndDoesNotTellTheTraceAfterwards(string operation,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomTracedDbConnection();
            var trace = new RecordingTrace { CancelBefore = true };

            // Act
            await Execute(operation, connection, trace, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, connection.ExecutionCount);
            Assert.AreEqual(1, trace.BeforeLogs.Count);
            Assert.AreEqual(0, trace.AfterLogs.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the statement with the sync or the async extension method of the operation.
        /// </summary>
        /// <param name="operation">The operation to execute: Scalar, Reader, Query, QueryTyped, First, FirstTyped, Single or SingleTyped.</param>
        /// <param name="connection">The connection to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation.</returns>
        private static async Task Execute(string operation,
            IDbConnection connection,
            ITrace trace,
            bool asynchronous,
            CancellationToken cancellationToken = default)
        {
            switch (operation)
            {
                case "Scalar":
                    if (asynchronous) await connection.ExecuteScalarAsync<int>(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteScalar<int>(Statement, trace: trace);
                    break;
                case "Reader":
                    if (asynchronous) (await connection.ExecuteReaderAsync(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))?.Dispose();
                    else connection.ExecuteReader(Statement, trace: trace)?.Dispose();
                    break;
                case "Query":
                    if (asynchronous) await connection.ExecuteQueryAsync(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuery(Statement, trace: trace)?.ToList();
                    break;
                case "QueryTyped":
                    if (asynchronous) await connection.ExecuteQueryAsync<TraceEntity>(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuery<TraceEntity>(Statement, trace: trace)?.ToList();
                    break;
                case "First":
                    if (asynchronous) await connection.ExecuteQueryFirstAsync(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQueryFirst(Statement, trace: trace);
                    break;
                case "FirstTyped":
                    if (asynchronous) await connection.ExecuteQueryFirstAsync<TraceEntity>(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQueryFirst<TraceEntity>(Statement, trace: trace);
                    break;
                case "Single":
                    if (asynchronous) await connection.ExecuteQuerySingleAsync(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuerySingle(Statement, trace: trace);
                    break;
                case "SingleTyped":
                    if (asynchronous) await connection.ExecuteQuerySingleAsync<TraceEntity>(Statement, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
                    else connection.ExecuteQuerySingle<TraceEntity>(Statement, trace: trace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// The entity that the typed queries map to.
        /// </summary>
        private class TraceEntity
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public int Id { get; set; }
        }

        #endregion
    }
}
