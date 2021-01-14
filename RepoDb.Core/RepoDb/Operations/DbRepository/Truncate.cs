using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection
    {
        #region Truncate<TEntity>

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The number of rows affected.</returns>
        public int Truncate<TEntity>()
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return connection.Truncate<TEntity>(
                    commandTimeout: CommandTimeout,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate<TEntity>(IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return connection.Truncate<TEntity>(commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        #endregion

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: CancellationToken.None);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>(IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
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
            var connection = CreateConnection();

            try
            {
                // Call the method
                return connection.Truncate(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string tableName,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return connection.Truncate(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
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
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: CancellationToken.None);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken);
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
