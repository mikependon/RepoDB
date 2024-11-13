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
        #region SkipQuery<TEntity>

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The skip of the batch to be used. This is a zero-based index (the first skip is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> SkipQuery<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return SkipQueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> SkipQueryInternal<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Ensure the fields
            fields = GetQualifiedFields<TEntity>(fields) ??
                DbFieldCache.Get(connection, tableName, transaction)?.GetAsFields();

            // Return
            return SkipQueryInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region SkipQueryAsync<TEntity>

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: (QueryGroup)null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(this IDbConnection connection,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return SkipQueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                where: where,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> SkipQueryAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Ensure the fields
            fields = GetQualifiedFields<TEntity>(fields) ??
                (await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken))?.GetAsFields();

            // Return
            return await SkipQueryAsyncInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region SkipQuery(TableName)

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return SkipQueryInternal<dynamic>(connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return SkipQueryInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return SkipQueryInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return SkipQueryInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> SkipQuery(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return SkipQueryInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region SkipQueryAsync(TableName)

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return SkipQueryAsyncInternal<dynamic>(connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                where: (QueryGroup)null,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return SkipQueryAsyncInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return SkipQueryAsyncInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return SkipQueryAsyncInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> SkipQueryAsync(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return SkipQueryAsyncInternal<dynamic>(connection: connection,
                tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
				traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region SkipQueryInternalBase<TEntity>

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> SkipQueryInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new SkipQueryRequest(tableName,
                connection,
                transaction,
                fields,
                skip,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetSkipQueryText(request);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Actual Execution
            var result = ExecuteQueryInternal<TEntity>(connection: connection,
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
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // Result
            return result;
        }

        #endregion

        #region SkipQueryAsyncInternalBase<TEntity>

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The skip of the batch to be used. This is a zero-based index (the first skip is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> SkipQueryAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new SkipQueryRequest(tableName,
                connection,
                transaction,
                fields,
                skip,
                rowsPerBatch,
                orderBy,
                where,
                hints,
                statementBuilder);
            var commandText = await CommandTextCache.GetSkipQueryTextAsync(request, cancellationToken);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
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
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // Result
            return result;
        }

        #endregion
    }
}
