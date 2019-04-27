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
        #region Delete<TEntity>

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(entity: entity,
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
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(object whereOrPrimaryKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
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
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(Expression<Func<TEntity, bool>> where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(where: where,
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
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(QueryField where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(where: where,
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
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(where: where,
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
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(QueryGroup where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete<TEntity>(where: where,
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

        #region DeleteAsync<TEntity>

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(entity: entity,
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
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(object whereOrPrimaryKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
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
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> where = null,
            IDbTransaction transaction = null)
                    where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(where: where,
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
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(QueryField where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(where: where,
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
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(where: where,
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
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync<TEntity>(QueryGroup where = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync<TEntity>(where: where,
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

        #region Delete(TableName)

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(string tableName,
            object where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(string tableName,
            QueryField where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(string tableName,
            IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(string tableName,
            QueryGroup where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Delete(tableName: tableName,
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

        #region DeleteAsync(TableName)

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync(string tableName,
            object where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync(string tableName,
            QueryField where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync(string tableName,
            IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync(tableName: tableName,
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

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> DeleteAsync(string tableName,
            QueryGroup where = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.DeleteAsync(tableName: tableName,
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
    }
}
