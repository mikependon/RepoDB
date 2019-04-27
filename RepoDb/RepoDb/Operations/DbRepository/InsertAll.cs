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
        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public void InsertAll<TEntity>(IEnumerable<TEntity> entities,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                connection.InsertAll<TEntity>(entities: entities,
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

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task InsertAllAsync<TEntity>(IEnumerable<TEntity> entities,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InsertAllAsync<TEntity>(entities: entities,
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

        #region InsertAll(TableName)

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used. Defaulted to database table fields.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        public void InsertAll(string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                connection.InsertAll(tableName: tableName,
                    entities: entities,
                    fields: fields,
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

        #region InsertAllAsync(TableName)

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used. Defaulted to database table fields.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        public Task InsertAllAsync(string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InsertAllAsync(tableName: tableName,
                    entities: entities,
                    fields: fields,
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
