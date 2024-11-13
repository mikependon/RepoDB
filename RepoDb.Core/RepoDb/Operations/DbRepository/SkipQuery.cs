using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection, new()
    {
        #region SkipQuery<TEntity>

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {

            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            object where = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region SkipQueryAsync<TEntity>

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {

            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> SkipQueryAsync<TEntity>(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync<TEntity>(skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region SkipQuery(TableName)

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.SkipQuery(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region SkipQueryAsync(TableName)

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.SkipQueryAsync(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
			string traceKey = TraceKeys.SkipQuery,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.SkipQueryAsync(tableName: tableName,
                    skip: skip,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    where: where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
					transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion
    }
}
