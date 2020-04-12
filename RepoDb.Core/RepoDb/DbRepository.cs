using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using System.Dynamic;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region Fields

        private static readonly object m_syncLock = new object();
        private TDbConnection m_connection;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public DbRepository(string connectionString)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public DbRepository(string connectionString,
            int? commandTimeout)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        public DbRepository(string connectionString,
            ICache cache,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  null,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString,
            ITrace trace)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  trace,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString,
            IStatementBuilder statementBuilder)
            : this(connectionString,
                  null,
                  ConnectionPersistency.PerCall,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  statementBuilder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public DbRepository(string connectionString,
            ConnectionPersistency connectionPersistency)
            : this(connectionString,
                  null,
                  connectionPersistency,
                  null,
                  Constant.DefaultCacheItemExpirationInMinutes,
                  null,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        public DbRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  null,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  trace,
                  null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public DbRepository(string connectionString,
            int? commandTimeout,
            ICache cache,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            : this(connectionString,
                  commandTimeout,
                  ConnectionPersistency.PerCall,
                  cache,
                  cacheItemExpiration,
                  trace,
                  statementBuilder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public DbRepository(string connectionString,
            int? commandTimeout,
            ConnectionPersistency connectionPersistency,
            ICache cache = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Properties
            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
            ConnectionPersistency = connectionPersistency;
            Cache = (cache ?? new MemoryCache());
            CacheItemExpiration = cacheItemExpiration;
            Trace = trace;
            StatementBuilder = statementBuilder;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection used by this repository.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the command timeout value in seconds that is being used by this repository on every operation.
        /// </summary>
        public int? CommandTimeout { get; }

        /// <summary>
        /// Gets the cache object that is being used by this repository.
        /// </summary>
        public ICache Cache { get; }

        /// <summary>
        /// Gets the expiration in minutes of the cache item.
        /// </summary>
        public int CacheItemExpiration { get; }

        /// <summary>
        /// Gets the trace object that is being used by this repository.
        /// </summary>
        public ITrace Trace { get; }

        /// <summary>
        /// Gets the statement builder object that is being used by this repository.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; }

        /// <summary>
        /// Gets the database connection persistency used by this repository. The default value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        public ConnectionPersistency ConnectionPersistency { get; }

        #endregion

        #region Other Methods

        // CreateConnection (TDbConnection)

        /// <summary>
        /// Creates a new instance of the database connection. If the value <see cref="ConnectionPersistency"/> property is <see cref="ConnectionPersistency.Instance"/>, then this will return
        /// the <see cref="DbConnection"/> that is being used by the current repository instance. However, if the value of the <see cref="ConnectionPersistency"/>
        /// property is <see cref="ConnectionPersistency.PerCall"/>, then this will return a new instance of the <see cref="DbConnection"/> object.
        /// </summary>
        /// <returns>An instance of the <see cref="DbConnection"/> object.</returns>
        public TDbConnection CreateConnection()
        {
            return CreateConnection(false);
        }

        /// <summary>
        /// Creates a new instance of the database connection. If the value <see cref="ConnectionPersistency"/> property is <see cref="ConnectionPersistency.Instance"/>, then this will return
        /// the <see cref="DbConnection"/> that is being used by the current repository instance. However, if the value of the <see cref="ConnectionPersistency"/>
        /// property is <see cref="ConnectionPersistency.PerCall"/>, then this will return a new instance of the <see cref="DbConnection"/> object.
        /// </summary>
        /// <param name="force">Set to true to forcely create a new instance of <see cref="DbConnection"/> object regardless of the persistency.</param>
        /// <returns>An instance of the <see cref="DbConnection"/> object.</returns>
        public TDbConnection CreateConnection(bool force)
        {
            var connection = (TDbConnection)null;
            if (force == false && ConnectionPersistency == ConnectionPersistency.Instance)
            {
                lock (m_syncLock)
                {
                    if (m_connection == null)
                    {
                        connection = Activator.CreateInstance<TDbConnection>();
                        connection.ConnectionString = ConnectionString;
                        m_connection = connection;
                    }
                    else
                    {
                        connection = m_connection;
                    }
                }
            }
            else
            {
                connection = Activator.CreateInstance<TDbConnection>();
                connection.ConnectionString = ConnectionString;
            }
            return connection;
        }

        /// <summary>
        /// Dispose the current repository instance (of type <see cref="DbRepository{TDbConnection}"/>). It is not necessary to call this method if the value of the <see cref="ConnectionPersistency"/>
        /// property is equals to <see cref="ConnectionPersistency.PerCall"/>. This method only manages the connection persistency for the repositories where the value
        /// of the <see cref="ConnectionPersistency"/> property is equals to <see cref="ConnectionPersistency.Instance"/>.
        /// </summary>
        public void Dispose()
        {
            if (ConnectionPersistency == ConnectionPersistency.Instance)
            {
                m_connection?.Dispose();
            }
        }

        /// <summary>
        /// Disposes an <see cref="IDbConnection"/> object if there is no <see cref="IDbTransaction"/> object connected
        /// and if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        /// <param name="connection">The instance of <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The instance of <see cref="IDbTransaction"/> object.</param>
        internal void DisposeConnectionForPerCall(IDbConnection connection, IDbTransaction transaction = null)
        {
            if (ConnectionPersistency == ConnectionPersistency.PerCall)
            {
                if (transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion
        
        #region ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.ExecuteQuery<TEntity>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and 
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.ExecuteQueryAsync<TEntity>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
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
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.ExecuteNonQuery(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteNonQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.ExecuteNonQueryAsync(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public object ExecuteScalar(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.ExecuteScalar(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteScalarAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public async Task<object> ExecuteScalarAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.ExecuteScalarAsync(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteScalar<TResult>

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        public TResult ExecuteScalar<TResult>(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.ExecuteScalar<TResult>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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

        #region ExecuteScalarAsync<TResult>

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        public async Task<TResult> ExecuteScalarAsync<TResult>(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await connection.ExecuteScalarAsync<TResult>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);
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
