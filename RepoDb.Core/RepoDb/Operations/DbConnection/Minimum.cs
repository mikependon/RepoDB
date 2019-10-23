using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Minimum<TEntity>

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Field field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Field field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumInternal<TEntity>(connection: connection,
                field: field,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Minimum<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumInternal<TEntity>(connection: connection,
                field: Field.Parse<TEntity>(field),
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static long MinimumInternal<TEntity>(this IDbConnection connection,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new MinimumRequest(typeof(TEntity),
                connection,
                transaction,
                field,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Return the result
            return MinimumInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region MinimumAsync<TEntity>

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Field field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Field field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsyncInternal<TEntity>(connection: connection,
                field: field,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsync<TEntity>(connection: connection,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MinimumAsyncInternal<TEntity>(connection: connection,
                field: Field.Parse<TEntity>(field),
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static Task<long> MinimumAsyncInternal<TEntity>(this IDbConnection connection,
            Field field,
            QueryGroup where = null,
            int? commandTimeout = null,
            string hints = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new MinimumRequest(typeof(TEntity),
                connection,
                transaction,
                field,
                where,
                hints,
                statementBuilder);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Return the result
            return MinimumInternalAsyncBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region Minimum(TableName)

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum(this IDbConnection connection,
            string tableName,
            Field field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Minimum(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum(this IDbConnection connection,
            string tableName,
            Field field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Minimum(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum(this IDbConnection connection,
            string tableName,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Minimum(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static long Minimum(this IDbConnection connection,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MinimumInternal(connection: connection,
                tableName: tableName,
                field: field,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static long MinimumInternal(this IDbConnection connection,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new MinimumRequest(tableName,
                connection,
                transaction,
                field,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetMinimumText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Return the result
            return MinimumInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region MinimumAsync(TableName)

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync(this IDbConnection connection,
            string tableName,
            Field field,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MinimumAsync(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync(this IDbConnection connection,
            string tableName,
            Field field,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MinimumAsync(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync(this IDbConnection connection,
            string tableName,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MinimumAsync(connection: connection,
                tableName: tableName,
                field: field,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public static Task<long> MinimumAsync(this IDbConnection connection,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MinimumAsyncInternal(connection: connection,
                tableName: tableName,
                field: field,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static Task<long> MinimumAsyncInternal(this IDbConnection connection,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new MinimumRequest(tableName,
                connection,
                transaction,
                field,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetMinimumText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Return the result
            return MinimumInternalAsyncBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region MinimumerInternalBase

        /// <summary>
        /// Extracts the minimum value of the target field from the database table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="MinimumRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static long MinimumInternalBase(this IDbConnection connection,
            MinimumRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Validate
            InvokeValidatorValidateMinimum(connection);

            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetMinimumText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeMinimum(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return default(int);
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternal<long>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterMinimum(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region MinimumAsyncInternalBase

        /// <summary>
        /// Minimums the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="MinimumRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        internal static async Task<long> MinimumInternalAsyncBase(this IDbConnection connection,
            MinimumRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Validate
            InvokeValidatorValidateMinimumAsync(connection);

            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetMinimumText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeMinimum(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return default(int);
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteScalarAsyncInternal<long>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterMinimum(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Invokes the <see cref="IDbValidator.ValidateMinimum"/> method.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        private static void InvokeValidatorValidateMinimum(IDbConnection connection)
        {
            connection.GetDbValidator()?.ValidateMinimum();
        }

        /// <summary>
        /// Invokes the <see cref="IDbValidator.ValidateMinimumAsync"/> method.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        private static void InvokeValidatorValidateMinimumAsync(IDbConnection connection)
        {
            connection.GetDbValidator()?.ValidateMinimumAsync();
        }

        #endregion
    }
}
