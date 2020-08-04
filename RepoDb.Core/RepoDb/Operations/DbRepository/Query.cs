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
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection
    {
        #region Query<TEntity>(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(tableName,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(string tableName,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(string tableName,
            QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(string tableName,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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

        #region Query<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null, IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TEntity>(QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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

        #region QueryAsync<TEntity>(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(tableName: tableName,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string tableName,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(tableName: tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string tableName,
            QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(tableName: tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(tableName: tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string tableName,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(tableName: tableName,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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

        #region QueryAsync<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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

        #region Query(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> Query(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query(tableName: tableName,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> Query(string tableName,
            QueryField where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null, IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> Query(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public IEnumerable<dynamic> Query(string tableName,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Query(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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

        #region QueryAsync(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> QueryAsync(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync(tableName: tableName,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> QueryAsync(string tableName,
            QueryField where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> QueryAsync(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public async Task<IEnumerable<dynamic>> QueryAsync(string tableName,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.QueryAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
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
