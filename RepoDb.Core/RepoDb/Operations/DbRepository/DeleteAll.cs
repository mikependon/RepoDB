using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region DeleteAll<TEntity>

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int DeleteAll<TEntity>(string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.DeleteAll<TEntity>(hints: hints,
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

        #region DeleteAllAsync<TEntity>

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> DeleteAllAsync<TEntity>(string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAllAsync<TEntity>(hints: hints,
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

        #region DeleteAll(TableName)

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int DeleteAll(string tableName,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.DeleteAll(tableName: tableName,
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

        #region DeleteAllAsync(TableName)

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> DeleteAllAsync(string tableName,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAllAsync(tableName: tableName,
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
