using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region Truncate<TEntity>

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <returns>The number of rows affected.</returns>
        public int Truncate<TEntity>()
            where TEntity : class
        {
            // Create a connection
            using (var connection = CreateConnection())
            {
                // Call the method
                return connection.Truncate<TEntity>(commandTimeout: CommandTimeout,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
        }

        #endregion

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>()
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
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
                DisposeConnectionForPerCall(connection);
            }
        }

        #endregion

        #region Truncate(TableName)

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string tableName)
        {
            // Create a connection
            using (var connection = CreateConnection())
            {
                // Call the method
                return connection.Truncate(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
        }

        #endregion

        #region TruncateAsync(TableName)

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName)
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
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
                DisposeConnectionForPerCall(connection);
            }
        }

        #endregion
    }
}
