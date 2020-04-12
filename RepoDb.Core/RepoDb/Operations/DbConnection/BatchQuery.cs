using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region BatchQuery<TEntity>

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                where: ToQueryGroup(where),
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryInternal<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> BatchQueryInternal<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryInternalBase<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region BatchQueryAsync<TEntity>

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsyncInternal<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static Task<IEnumerable<TEntity>> BatchQueryAsyncInternal<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsyncInternalBase<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region BatchQuery(TableName)

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> BatchQuery(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQuery(connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> BatchQuery(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQuery(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> BatchQuery(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQuery(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> BatchQuery(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQuery(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> BatchQuery(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryInternal(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static IEnumerable<dynamic> BatchQueryInternal(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryInternalBase(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region BatchQueryAsync(TableName)

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> BatchQueryAsync(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsync(connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> BatchQueryAsync(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsync(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> BatchQueryAsync(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsync(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> BatchQueryAsync(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsync(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> BatchQueryAsync(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsyncInternal(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static Task<IEnumerable<dynamic>> BatchQueryAsyncInternal(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return BatchQueryAsyncInternalBase(connection: connection,
                tableName: tableName,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region BatchQueryInternalBase<TEntity>

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> BatchQueryInternalBase<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                transaction,
                FieldCache.Get<TEntity>(),
                page,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BatchQueryAsyncInternalBase<TEntity>

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> BatchQueryAsyncInternalBase<TEntity>(this IDbConnection connection,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                transaction,
                FieldCache.Get<TEntity>(),
                page,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BatchQueryInternalBase(TableName)

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static IEnumerable<dynamic> BatchQueryInternalBase(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Check the fields
            if (fields?.Any() != true)
            {
                fields = DbFieldCache.Get(connection, tableName, transaction)?.AsFields();
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(tableName,
                connection,
                transaction,
                fields,
                page,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BatchQueryAsyncInternalBase(TableName)

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static async Task<IEnumerable<dynamic>> BatchQueryAsyncInternalBase(this IDbConnection connection,
            string tableName,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Check the fields
            if (fields?.Any() != true)
            {
                fields = (await DbFieldCache.GetAsync(connection, tableName, transaction))?.AsFields();
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(tableName,
                connection,
                transaction,
                fields,
                page,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo(null) });
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion
    }
}
