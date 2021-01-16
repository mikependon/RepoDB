using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Exists<TEntity>

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity>(this IDbConnection connection,
            object what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: WhatToQueryGroup<TEntity>(connection, what, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity, TWhat>(this IDbConnection connection,
            TWhat what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: WhatToQueryGroup<TEntity>(connection, what, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity>(this IDbConnection connection,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TEntity>(this IDbConnection connection,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return ExistsInternal<TEntity>(connection: connection,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static bool ExistsInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new ExistsRequest(typeof(TEntity),
                connection,
                transaction,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Return the result
            return ExistsInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region ExistsAsync<TEntity>

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static async Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
            object what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await ExistsAsyncInternal<TEntity>(connection: connection,
                where: await WhatToQueryGroupAsync<TEntity>(connection, what, transaction, cancellationToken),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        public static async Task<bool> ExistsAsync<TEntity, TWhat>(this IDbConnection connection,
            TWhat what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await ExistsAsyncInternal<TEntity>(connection: connection,
                where: await WhatToQueryGroupAsync<TEntity>(connection, what, transaction, cancellationToken),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return ExistsAsyncInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return ExistsAsyncInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return ExistsAsyncInternal<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return ExistsAsyncInternal<TEntity>(connection: connection,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static Task<bool> ExistsAsyncInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            string hints = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables
            var request = new ExistsRequest(typeof(TEntity),
                connection,
                transaction,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Return the result
            return ExistsAsyncInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region Exists(TableName)

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists<TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return ExistsInternal(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists(this IDbConnection connection,
            string tableName,
            object what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return ExistsInternal(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists(this IDbConnection connection,
            string tableName,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return ExistsInternal(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return ExistsInternal(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static bool Exists(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return ExistsInternal(connection: connection,
                tableName: tableName,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static bool ExistsInternal(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new ExistsRequest(tableName,
                connection,
                transaction,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Return the result
            return ExistsInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region ExistsAsync(TableName)

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static async Task<bool> ExistsAsync<TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return await ExistsAsyncInternal(connection: connection,
                tableName: tableName,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static async Task<bool> ExistsAsync(this IDbConnection connection,
            string tableName,
            object what,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return await ExistsAsyncInternal(connection: connection,
                tableName: tableName,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync(this IDbConnection connection,
            string tableName,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return ExistsAsyncInternal(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return ExistsAsyncInternal(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public static Task<bool> ExistsAsync(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return ExistsAsyncInternal(connection: connection,
                tableName: tableName,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static Task<bool> ExistsAsyncInternal(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var request = new ExistsRequest(tableName,
                connection,
                transaction,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Return the result
            return ExistsAsyncInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region ExistsInternalBase

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="ExistsRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static bool ExistsInternalBase(this IDbConnection connection,
            ExistsRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetExistsText(request);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeExists(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return false;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternal<object>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: null,
                entityType: request.Type,
                dbFields: DbFieldCache.Get(connection, request.Name, transaction, true),
                skipCommandArrayParametersCheck: true);

            // After Execution
            trace?.AfterExists(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return Converter.DbNullToNull(result) != null;
        }

        #endregion

        #region ExistsAsyncInternalBase

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="ExistsRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        internal static async Task<bool> ExistsAsyncInternalBase(this IDbConnection connection,
            ExistsRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetExistsText(request);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeExists(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return false;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteScalarAsyncInternal<object>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: null,
                cancellationToken: cancellationToken,
                entityType: request.Type,
                dbFields: await DbFieldCache.GetAsync(connection, request.Name, transaction, true, cancellationToken),
                skipCommandArrayParametersCheck: true);

            // After Execution
            trace?.AfterExists(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return Converter.DbNullToNull(result) != null;
        }

        #endregion
    }
}
