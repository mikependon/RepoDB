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
        #region UpdateAll<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll<TEntity>(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.UpdateAll<TEntity>(entities: entities,
                    batchSize: batchSize,
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
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.UpdateAll<TEntity>(entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
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

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> UpdateAllAsync<TEntity>(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.UpdateAllAsync<TEntity>(entities: entities,
                    batchSize: batchSize,
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
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> UpdateAllAsync<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.UpdateAllAsync<TEntity>(entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
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

        #region UpdateAll(TableName)

        /// <summary>
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll(string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.UpdateAll(tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: fields,
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
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll(string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.UpdateAll(tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields,
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

        #region UpdateAllAsync(TableName)

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> UpdateAllAsync(string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.UpdateAllAsync(tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: fields,
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
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> UpdateAllAsync(string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
			IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.UpdateAllAsync(tableName: tableName,
                    entities: entities,
                    qualifiers: qualifiers,
                    batchSize: batchSize,
                    fields: fields,
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
