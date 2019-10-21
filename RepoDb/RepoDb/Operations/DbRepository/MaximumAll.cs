using System;
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
        #region MaximumAll<TEntity>

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object MaximumAll<TEntity>(Field field,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.MaximumAll<TEntity>(field: field,
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
        /// Extracts the maximum value of the target field from all data of the database table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object MaximumAll<TEntity>(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.MaximumAll<TEntity>(field: field,
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

        #region MaximumAllAsync<TEntity>

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public async Task<object> MaximumAllAsync<TEntity>(Field field,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MaximumAllAsync<TEntity>(field: field,
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
        /// Extracts the maximum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public async Task<object> MaximumAllAsync<TEntity>(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MaximumAllAsync<TEntity>(field: field,
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

        #region MaximumAll(TableName)

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public object MaximumAll(string tableName,
            Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.MaximumAll(tableName: tableName,
                    field: field,
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

        #region MaximumAllAsync(TableName)

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<object> MaximumAllAsync(string tableName,
            Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MaximumAllAsync(tableName: tableName,
                    field: field,
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
