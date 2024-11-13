using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <averagemary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </averagemary>
    public static partial class DbConnectionExtension
    {
        #region AverageAll<TEntity>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static object AverageAll<TEntity>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return AverageAllInternal<TEntity, object>(connection: connection,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static object AverageAll<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return AverageAllInternal<TEntity, object>(connection: connection,
                field: Field.Parse<TEntity>(field).First(),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static TResult AverageAll<TEntity, TResult>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return AverageAllInternal<TEntity, TResult>(connection: connection,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static TResult AverageAll<TEntity, TResult>(this IDbConnection connection,
            Expression<Func<TEntity, TResult>> field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return AverageAllInternal<TEntity, TResult>(connection: connection,
                field: Field.Parse<TEntity, TResult>(field).First(),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        internal static TResult AverageAllInternal<TEntity, TResult>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new AverageAllRequest(typeof(TEntity),
                connection,
                transaction,
                field,
                hints,
                statementBuilder);
            var param = (object)null;

            // Return the result
            return AverageAllInternalBase<TResult>(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region AverageAllAsync<TEntity>

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<object> AverageAllAsync<TEntity>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return AverageAllAsyncInternal<TEntity, object>(connection: connection,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<object> AverageAllAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return AverageAllAsyncInternal<TEntity, object>(connection: connection,
                field: Field.Parse<TEntity>(field).First(),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<TResult> AverageAllAsync<TEntity, TResult>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return AverageAllAsyncInternal<TEntity, TResult>(connection: connection,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<TResult> AverageAllAsync<TEntity, TResult>(this IDbConnection connection,
            Expression<Func<TEntity, TResult>> field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return AverageAllAsyncInternal<TEntity, TResult>(connection: connection,
                field: Field.Parse<TEntity, TResult>(field).First(),
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        internal static Task<TResult> AverageAllAsyncInternal<TEntity, TResult>(this IDbConnection connection,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables
            var request = new AverageAllRequest(typeof(TEntity),
                connection,
                transaction,
                field,
                hints,
                statementBuilder);
            var param = (object)null;

            // Return the result
            return AverageAllAsyncInternalBase<TResult>(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region AverageAll(TableName)

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static object AverageAll(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return AverageAllInternal<object>(connection: connection,
                tableName: tableName,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public static TResult AverageAll<TResult>(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return AverageAllInternal<TResult>(connection: connection,
                tableName: tableName,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        internal static TResult AverageAllInternal<TResult>(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            // Variables
            var request = new AverageAllRequest(tableName,
                connection,
                transaction,
                field,
                hints,
                statementBuilder);

            // Return the result
            return AverageAllInternalBase<TResult>(connection: connection,
                request: request,
                param: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region AverageAllAsync(TableName)

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<object> AverageAllAsync(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return AverageAllAsyncInternal<object>(connection: connection,
                tableName: tableName,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public static Task<TResult> AverageAllAsync<TResult>(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return AverageAllAsyncInternal<TResult>(connection: connection,
                tableName: tableName,
                field: field,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        internal static Task<TResult> AverageAllAsyncInternal<TResult>(this IDbConnection connection,
            string tableName,
            Field field,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var request = new AverageAllRequest(tableName,
                connection,
                transaction,
                field,
                hints,
                statementBuilder);

            // Return the result
            return AverageAllAsyncInternalBase<TResult>(connection: connection,
                request: request,
                param: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region AverageAllInternalBase

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="AverageAllRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The average value of the target field.</returns>
        internal static TResult AverageAllInternalBase<TResult>(this IDbConnection connection,
            AverageAllRequest request,
            object param,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetAverageAllText(request);

            // Actual Execution
            var result = ExecuteScalarInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: null,
				trace: trace,
                entityType: request.Type,
                dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
                skipCommandArrayParametersCheck: true);

            // Result
            return result;
        }

        #endregion

        #region AverageAllAsyncInternalBase

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="AverageAllRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        internal static async Task<TResult> AverageAllAsyncInternalBase<TResult>(this IDbConnection connection,
            AverageAllRequest request,
            object param,
            int? commandTimeout = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetAverageAllText(request);

            // Actual Execution
            var result = await ExecuteScalarAsyncInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: null,
				trace: trace,
                cancellationToken: cancellationToken,
                entityType: request.Type,
                dbFields: await DbFieldCache.GetAsync(connection, request.Name, transaction, true, cancellationToken),
                skipCommandArrayParametersCheck: true);

            // Result
            return result;
        }

        #endregion
    }
}
