using System;
using System.Collections.Generic;
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
        #region Merge<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge<TEntity>(entity: entity,
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
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge<TEntity>(TEntity entity,
            Field qualifier,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge<TEntity>(entity: entity,
                    qualifier: qualifier,
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
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge<TEntity>(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge<TEntity>(entity: entity,
                    qualifiers: qualifiers,
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

        #region MergeAsync<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
                    where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync<TEntity>(entity: entity,
                    qualifiers: null,
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
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync<TEntity>(TEntity entity,
            Field qualifier,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync<TEntity>(entity: entity,
                    qualifier: qualifier,
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
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync<TEntity>(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
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

        #region Merge(TableName)

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(string tableName,
            object entity,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge(tableName: tableName,
                    entity: entity,
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
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(string tableName,
            object entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge(tableName: tableName,
                    entity: entity,
                    qualifier: qualifier,
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
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Merge(tableName: tableName,
                    entity: entity,
                    qualifiers: qualifiers,
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

        #region MergeAsync(TableName)

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync(string tableName,
            object entity,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync(tableName: tableName,
                    entity: entity,
                    qualifiers: null,
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
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync(string tableName,
            object entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync(tableName: tableName,
                    entity: entity,
                    qualifier: qualifier,
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
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> MergeAsync(string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.MergeAsync(tableName: tableName,
                    entity: entity,
                    qualifiers: qualifiers,
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
