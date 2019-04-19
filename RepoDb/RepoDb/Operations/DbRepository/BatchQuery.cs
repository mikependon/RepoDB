using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region BatchQuery<TEntity>

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        #endregion

        #region BatchQueryAsync<TEntity>

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.BatchQueryAsync<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IEnumerable<QueryField> where,
        int page,
        int rowsPerBatch,
        IEnumerable<OrderField> orderBy,
        string hints = null,
        IDbTransaction transaction = null)
        where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection, transaction);
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public async Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {

                // Call the method
                return await connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            catch
            {
                // Throw back the error
                throw;
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
