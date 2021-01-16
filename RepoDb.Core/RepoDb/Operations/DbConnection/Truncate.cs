using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Truncate<TEntity>

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Truncate<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return TruncateInternal<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected.</returns>
        internal static int TruncateInternal<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                transaction,
                statementBuilder);

            // Return the result
            return TruncateInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> TruncateAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return TruncateAsyncInternal<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        internal static Task<int> TruncateAsyncInternal<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                transaction,
                statementBuilder);

            // Return the result
            return TruncateAsyncInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region Truncate(TableName)

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Truncate(this IDbConnection connection,
            string tableName,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return TruncateInternal(connection: connection,
                tableName: tableName,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected.</returns>
        internal static int TruncateInternal(this IDbConnection connection,
            string tableName,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new TruncateRequest(tableName,
                connection,
                transaction,
                statementBuilder);

            // Return the result
            return TruncateInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region TruncateAsync(TableName)

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public static Task<int> TruncateAsync(this IDbConnection connection,
            string tableName,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return TruncateAsyncInternal(connection: connection,
                tableName: tableName,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        internal static Task<int> TruncateAsyncInternal(this IDbConnection connection,
            string tableName,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var request = new TruncateRequest(tableName,
                connection,
                transaction,
                statementBuilder);

            // Return the result
            return TruncateAsyncInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region TruncateInternalBase

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="TruncateRequest"/> object.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of rows affected.</returns>
        internal static int TruncateInternalBase(this IDbConnection connection,
            TruncateRequest request,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetTruncateText(request);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: request.Type,
                dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
                skipCommandArrayParametersCheck: true);

            // After Execution
            trace?.AfterTruncate(new TraceLog(sessionId, commandText, null, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion

        #region TruncateAsyncInternalBase

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="TruncateRequest"/> object.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        internal static async Task<int> TruncateAsyncInternalBase(this IDbConnection connection,
            TruncateRequest request,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetTruncateText(request);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteNonQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: request.Type,
                dbFields: await DbFieldCache.GetAsync(connection, request.Name, transaction, true, cancellationToken),
                skipCommandArrayParametersCheck: true);

            // After Execution
            trace?.AfterTruncate(new TraceLog(sessionId, commandText, null, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion
    }
}
