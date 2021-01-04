using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using System;
using System.Dynamic;
using System.Threading;

namespace RepoDb
{
    /// <summary>
    /// A base class for all entity-based repositories. It is designed to only allow the given operations work with single data entity object.
    /// </summary>
    /// <typeparam name="TEntity">The type of data entity object to be mapped on this repository.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
        where TDbConnection : DbConnection
        where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public BaseRepository(string connectionString)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public BaseRepository(string connectionString,
            int? commandTimeout)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        public BaseRepository(string connectionString,
            ICache cache)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  cache,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public BaseRepository(string connectionString,
            ITrace trace)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  trace,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public BaseRepository(string connectionString,
            IStatementBuilder statementBuilder)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  statementBuilder)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public BaseRepository(string connectionString,
            ConnectionPersistency connectionPersistency)
            : this(connectionString,
                  null,
                  connectionPersistency,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        public BaseRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public BaseRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null)
            : this(connectionString,
                  commandTimeout,
                  cache,
                  cacheItemExpiration,
                  trace,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public BaseRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  trace,
                  statementBuilder)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public BaseRepository(string connectionString,
            int? commandTimeout,
            ConnectionPersistency connectionPersistency,
            ICache cache,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            DbRepository = new DbRepository<TDbConnection>(connectionString,
                commandTimeout,
                connectionPersistency,
                cache,
                cacheItemExpiration,
                trace,
                statementBuilder);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying repository used by this repository.
        /// </summary>
        public DbRepository<TDbConnection> DbRepository { get; }

        /// <summary>
        /// Gets the connection used by this repository.
        /// </summary>
        public string ConnectionString => DbRepository.ConnectionString;

        /// <summary>
        /// Gets the command timeout value in seconds that is being used by this repository on every operation.
        /// </summary>
        public int? CommandTimeout => DbRepository.CommandTimeout;

        /// <summary>
        /// Gets the cache object that is being used by this repository.
        /// </summary>
        public ICache Cache => DbRepository.Cache;

        /// <summary>
        /// Gets the expiration in minutes of the cache item.
        /// </summary>
        public int? CacheItemExpiration => DbRepository.CacheItemExpiration;

        /// <summary>
        /// Gets the trace object that is being used by this repository.
        /// </summary>
        public ITrace Trace => DbRepository.Trace;

        /// <summary>
        /// Gets the statement builder object that is being used by this repository.
        /// </summary>
        public IStatementBuilder StatementBuilder => DbRepository.StatementBuilder;

        /// <summary>
        /// Gets the database connection persistency used by this repository. The default value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        public ConnectionPersistency ConnectionPersistency => DbRepository.ConnectionPersistency;

        #endregion

        #region Other Methods

        // CreateConnection

        /// <summary>
        /// Creates a new instance of the database connection object. If the value of <see cref="ConnectionPersistency"/> property is <see cref="ConnectionPersistency.Instance"/>, then this will return
        /// the <see cref="DbConnection"/> that is being used by the current repository instance. However, if the value of the <see cref="ConnectionPersistency"/> property
        /// is <see cref="ConnectionPersistency.PerCall"/>, then this will return a new instance of the <see cref="DbConnection"/> object.
        /// </summary>
        /// <returns>An instance of the <see cref="DbConnection"/> object.</returns>
        public TDbConnection CreateConnection() =>
            DbRepository.CreateConnection();

        /// <summary>
        /// Creates a new instance of the database connection. If the value <see cref="ConnectionPersistency"/> property is <see cref="ConnectionPersistency.Instance"/>, then this will return
        /// the <see cref="DbConnection"/> that is being used by the current repository instance. However, if the value of the <see cref="ConnectionPersistency"/> property
        /// is <see cref="ConnectionPersistency.PerCall"/>, then this will return a new instance of the <see cref="DbConnection"/> object.
        /// </summary>
        /// <param name="force">Set to true to forcely create a new instance of <see cref="DbConnection"/> object regardless of the persistency.</param>
        /// <returns>An instance of the <see cref="DbConnection"/> object.</returns>
        public TDbConnection CreateConnection(bool force) =>
            DbRepository.CreateConnection(force);

        /// <summary>
        /// Dispose the current repository instance. It is not necessary to call this method if the value of the <see cref="ConnectionPersistency"/>
        /// property is equals to <see cref="ConnectionPersistency.PerCall"/>. This method only manages the connection persistency for the repositories where the value
        /// of the <see cref="ConnectionPersistency"/> property is equals to <see cref="ConnectionPersistency.Instance"/>.
        /// </summary>
        public void Dispose() =>
            DbRepository.Dispose();

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of data entity objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                transaction: transaction);
        }

        #endregion

        #region ExecuteQueryAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and 
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>
        /// An enumerable list of data entity objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExecuteQueryAsync<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int ExecuteNonQuery(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        #endregion

        #region ExecuteNonQueryAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/>
        /// and returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> ExecuteNonQueryAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the <see cref="Cache"/> property is set.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
        public object ExecuteScalar(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region ExecuteScalarAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the <see cref="Cache"/> property is set.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
        public Task<object> ExecuteScalarAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExecuteScalarAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region ExecuteScalar<TResult>

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the <see cref="Cache"/> property is set.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
        public TResult ExecuteScalar<TResult>(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalar<TResult>(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region ExecuteScalarAsync<TResult>

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the <see cref="Cache"/> property is set.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
        public Task<TResult> ExecuteScalarAsync<TResult>(string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExecuteScalarAsync<TResult>(commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}