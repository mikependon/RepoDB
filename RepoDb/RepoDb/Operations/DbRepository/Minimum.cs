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
        #region Minimum<TEntity>

        /// <summary>
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum<TEntity>(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum<TEntity>(field: field,
                    where: where,
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

        #region MinimumAsync<TEntity>

        /// <summary>
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync<TEntity>(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync<TEntity>(field: field,
                    where: where,
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

        #region Minimum(TableName)

        /// <summary>
        /// Minimums the target field from the database table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum(string tableName,
            Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum(string tableName,
            Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum(string tableName,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object Minimum(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Minimum(tableName: tableName,
                    field: field,
                    hints: hints,
                    where: where,
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

        #region MinimumAsync(TableName)

        /// <summary>
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync(string tableName,
            Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync(string tableName,
            Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync(string tableName,
            Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync(tableName: tableName,
                    field: field,
                    where: where,
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
        /// Minimums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public async Task<object> MinimumAsync(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MinimumAsync(tableName: tableName,
                    field: field,
                    where: where,
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
