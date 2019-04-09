using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A base object for all <b>Shared-Based Repositories</b>. This object is usually being inheritted if
    /// the derived class is meant for shared-based operations when it comes to data manipulations.
    /// This object is used by <see cref="BaseRepository{TEntity, TDbConnection}"/> as an underlying repository for all its
    /// operations.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    public class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
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
            : this(connectionString, null, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public DbRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        public DbRepository(string connectionString, ICache cache)
            : this(connectionString, null, cache, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString, ITrace trace)
            : this(connectionString, null, null, trace, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString, IStatementBuilder statementBuilder)
            : this(connectionString, null, null, null, statementBuilder, ConnectionPersistency.PerCall)
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
        public DbRepository(string connectionString, ConnectionPersistency connectionPersistency)
            : this(connectionString, null, null, null, null, connectionPersistency)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
            : this(connectionString, commandTimeout, cache, trace, statementBuilder, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbRepository{TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder,
            ConnectionPersistency connectionPersistency)
        {
            // Properties
            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
            Cache = (cache ?? new MemoryCache());
            Trace = trace;
            StatementBuilder = statementBuilder;
            ConnectionPersistency = connectionPersistency;
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
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void DisposeConnectionForPerCall(IDbConnection connection, IDbTransaction transaction = null)
        {
            if (ConnectionPersistency == ConnectionPersistency.PerCall || transaction == null)
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Wraps the result into an instance of <see cref="AsyncResultExtractor{T}"/> object with parameter values based on the given <see cref="IDbConnection"/> and <see cref="IDbTransaction"/> object,
        /// only if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the result.</typeparam>
        /// <param name="result">The result to be placed inside the <see cref="AsyncResultExtractor{T}"/> object.</param>
        /// <param name="connection">The connection object that is being used on the operation.</param>
        /// <param name="transaction">The transaction object that is being used on the operation.</param>
        /// <returns></returns>
        private Task<AsyncResultExtractor<TEntity>> ConvertToAsyncResultExtractorForPerCall<TEntity>(Task<TEntity> result, IDbConnection connection, IDbTransaction transaction = null)
        {
            var wrappable = ConnectionPersistency == ConnectionPersistency.PerCall || transaction == null;
            return Task.FromResult(new AsyncResultExtractor<TEntity>(result, wrappable ? connection : null));
        }

        #endregion

        #region BatchQuery

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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
        /// Queries a data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
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

        #region BatchQueryAsync

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync<TEntity>(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.BatchQueryAsync<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    hints: hints,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region BulkInsert

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
            where TEntity : class
        {
            // Create a connection
            using (var connection = CreateConnection())
            {
                // Call the method
                return connection.BulkInsert<TEntity>(entities: entities,
                    mappings: mappings,
                    commandTimeout: CommandTimeout,
                    trace: Trace);
            }
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert<TEntity>(DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null)
            where TEntity : class
        {
            // Create a connection
            using (var connection = CreateConnection())
            {

                // Call the method
                return connection.BulkInsert<TEntity>(reader: reader,
                    mappings: mappings,
                    commandTimeout: CommandTimeout,
                    trace: Trace);
            }
        }

        #endregion

        #region BulkInsertAsync

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            // Call the method
            var result = connection.BulkInsertAsync<TEntity>(entities: entities,
                mappings: mappings,
                commandTimeout: CommandTimeout,
                trace: Trace);

            // Return the result
            return ConvertToAsyncResultExtractorForPerCall(result, connection);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> BulkInsertAsync<TEntity>(DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            // Call the method
            var result = connection.BulkInsertAsync<TEntity>(reader: reader,
                mappings: mappings,
                commandTimeout: CommandTimeout,
                trace: Trace);

            // Return the result
            return ConvertToAsyncResultExtractorForPerCall(result, connection);
        }

        #endregion

        #region Count

        /// <summary>
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database.</returns>
        public long Count<TEntity>(string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count<TEntity>(object where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count<TEntity>(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count<TEntity>(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count<TEntity>(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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
        /// Counts the number of table data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count<TEntity>(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Count<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
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

        #region CountAsync

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The dynamic expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(object where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync<TEntity>(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.CountAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    hints: hints,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(Expression<Func<TEntity, bool>> where,
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
        /// <param name="entity">The actual instance of the data entity.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(QueryField where,
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
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(IEnumerable<QueryField> where,
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
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(object whereOrPrimaryKey,
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
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete<TEntity>(QueryGroup where,
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

        #region DeleteAsync

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The actual instance of the data entity.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(entity: entity,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(QueryField where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(object whereOrPrimaryKey,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync<TEntity>(QueryGroup where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAsync<TEntity>(where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region DeleteAll

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int DeleteAll<TEntity>(IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.DeleteAll<TEntity>(commandTimeout: CommandTimeout,
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

        #region DeleteAllAsync

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAllAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.DeleteAllAsync<TEntity>(commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region InlineInsert

        /// <summary>
        /// Inserts a new data into the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public object InlineInsert<TEntity>(object entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineInsert<TEntity>(entity: entity,
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

        #region InlineInsertAsync

        /// <summary>
        /// Inserts a new data into the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public Task<AsyncResultExtractor<object>> InlineInsertAsync<TEntity>(object entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineInsertAsync<TEntity>(entity: entity,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region InlineMerge

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge<TEntity>(object entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineMerge<TEntity>(entity: entity,
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
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge<TEntity>(object entity,
            Field qualifier,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineMerge<TEntity>(entity: entity,
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
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge<TEntity>(object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineMerge<TEntity>(entity: entity,
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

        #region InlineMergeAsync

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync<TEntity>(object entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineMergeAsync<TEntity>(entity: entity,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync<TEntity>(object entity,
            Field qualifier,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineMergeAsync<TEntity>(entity: entity,
                    qualifier: qualifier,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync<TEntity>(object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineMergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                commandTimeout: CommandTimeout,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region InlineUpdate

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineUpdate<TEntity>(entity: entity,
                    whereOrPrimaryKey: whereOrPrimaryKey,
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
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineUpdate<TEntity>(entity: entity,
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
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity,
            QueryField where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineUpdate<TEntity>(entity: entity,
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
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineUpdate<TEntity>(entity: entity,
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
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity,
            QueryGroup where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.InlineUpdate<TEntity>(entity: entity,
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

        #region InlineUpdateAsync

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync<TEntity>(object entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineUpdateAsync<TEntity>(entity: entity,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync<TEntity>(object entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineUpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync<TEntity>(object entity,
            QueryField where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineUpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync<TEntity>(object entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineUpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync<TEntity>(object entity,
            QueryGroup where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                commandTimeout: CommandTimeout,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public object Insert<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Insert<TEntity>(entity: entity,
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

        #region InsertAsync

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task<AsyncResultExtractor<object>> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.InsertAsync<TEntity>(entity: entity,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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

        #region MergeAsync

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.MergeAsync<TEntity>(entity: entity,
                    qualifiers: null,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync<TEntity>(TEntity entity,
            Field qualifier,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.MergeAsync<TEntity>(entity: entity,
                    qualifier: qualifier,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync<TEntity>(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                commandTimeout: CommandTimeout,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region Query

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<OrderField> orderBy = null,
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
                return connection.Query<TEntity>(orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
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
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> where,
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
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(QueryField where,
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
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<QueryField> where,
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
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(object whereOrPrimaryKey,
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
                    hints: hints,
                    cacheKey: cacheKey,
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
        /// Queries a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(QueryGroup where,
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

        #region QueryAsync

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(object whereOrPrimaryKey,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync<TEntity>(QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryAsync<TEntity>(where: where,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region QueryMultiple

        #region T1, T2

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2>(where1: where1,
                    where2: where2,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    top2: top2,
                    orderBy2: orderBy2,
                    hints2: hints2,
                    commandTimeout: commandTimeout,
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

        #region T1, T2, T3

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3>(where1: where1,
                    where2: where2,
                    where3: where3,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    commandTimeout: commandTimeout,
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

        #region T1, T2, T3, T4

        /// <summary>
        /// Query a multiple resultset from the database based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    commandTimeout: commandTimeout,
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

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    commandTimeout: commandTimeout,
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

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="where6">The query expression to be used (at T6) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6) by this operation.</param>
        /// <param name="top6">The top number of data to be used (at T6) by this operation.</param>
        /// <param name="hints6">The table hints to be used (at T6) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5, T6>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    commandTimeout: commandTimeout,
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

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="where6">The query expression to be used (at T6) by this operation.</param>
        /// <param name="where7">The query expression to be used (at T7) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6) by this operation.</param>
        /// <param name="top6">The top number of data to be used (at T6) by this operation.</param>
        /// <param name="hints6">The table hints to be used (at T6) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used (at T7) by this operation.</param>
        /// <param name="top7">The top number of data to be used (at T7) by this operation.</param>
        /// <param name="hints7">The table hints to be used (at T7) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    where7: where7,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    orderBy7: orderBy7,
                    top7: top7,
                    hints7: hints7,
                    commandTimeout: commandTimeout,
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

        #endregion

        #region QueryMultipleAsync

        #region T1, T2

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>>>> QueryMultipleAsync<T1, T2>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2>(where1: where1,
                    where2: where2,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    top2: top2,
                    orderBy2: orderBy2,
                    hints2: hints2,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>>> QueryMultipleAsync<T1, T2, T3>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2, T3>(where1: where1,
                    where2: where2,
                    where3: where3,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query a multiple resultset from the database based on the given 4 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>>
            QueryMultipleAsync<T1, T2, T3, T4>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2, T3, T4>(where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: Trace,
                statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2, T3, T4, T5>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="where6">The query expression to be used (at T6) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6) by this operation.</param>
        /// <param name="top6">The top number of data to be used (at T6) by this operation.</param>
        /// <param name="hints6">The table hints to be used (at T6) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="where1">The query expression to be used (at T1) by this operation.</param>
        /// <param name="where2">The query expression to be used (at T2) by this operation.</param>
        /// <param name="where3">The query expression to be used (at T3) by this operation.</param>
        /// <param name="where4">The query expression to be used (at T4) by this operation.</param>
        /// <param name="where5">The query expression to be used (at T5) by this operation.</param>
        /// <param name="where6">The query expression to be used (at T6) by this operation.</param>
        /// <param name="where7">The query expression to be used (at T7) by this operation.</param>
        /// <param name="orderBy1">The order definition of the fields to be used (at T1) by this operation.</param>
        /// <param name="top1">The top number of data to be used (at T1) by this operation.</param>
        /// <param name="hints1">The table hints to be used (at T1) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used (at T2) by this operation.</param>
        /// <param name="top2">The top number of data to be used (at T2) by this operation.</param>
        /// <param name="hints2">The table hints to be used (at T2) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used (at T3) by this operation.</param>
        /// <param name="top3">The top number of data to be used (at T3) by this operation.</param>
        /// <param name="hints3">The table hints to be used (at T3) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used (at T4) by this operation.</param>
        /// <param name="top4">The top number of data to be used (at T4) by this operation.</param>
        /// <param name="hints4">The table hints to be used (at T4) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used (at T5) by this operation.</param>
        /// <param name="top5">The top number of data to be used (at T5) by this operation.</param>
        /// <param name="hints5">The table hints to be used (at T5) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used (at T6) by this operation.</param>
        /// <param name="top6">The top number of data to be used (at T6) by this operation.</param>
        /// <param name="hints6">The table hints to be used (at T6) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used (at T7) by this operation.</param>
        /// <param name="top7">The top number of data to be used (at T7) by this operation.</param>
        /// <param name="hints7">The table hints to be used (at T7) by this operation. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public Task<AsyncResultExtractor<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(where1: where1,
                    where2: where2,
                    where3: where3,
                    where4: where4,
                    where5: where5,
                    where6: where6,
                    where7: where7,
                    orderBy1: orderBy1,
                    top1: top1,
                    hints1: hints1,
                    orderBy2: orderBy2,
                    top2: top2,
                    hints2: hints2,
                    orderBy3: orderBy3,
                    top3: top3,
                    hints3: hints3,
                    orderBy4: orderBy4,
                    top4: top4,
                    hints4: hints4,
                    orderBy5: orderBy5,
                    top5: top5,
                    hints5: hints5,
                    orderBy6: orderBy6,
                    top6: top6,
                    hints6: hints6,
                    orderBy7: orderBy7,
                    top7: top7,
                    hints7: hints7,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #endregion

        #region Truncate

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <returns>The number of rows affected by this operation.</returns>
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

        #region TruncateAsync

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <returns>The number of rows affected by this operation.</returns>
        public Task<AsyncResultExtractor<int>> TruncateAsync<TEntity>()
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            // Call the method
            var result = connection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                trace: Trace,
                statementBuilder: StatementBuilder);

            // Return the result
            return ConvertToAsyncResultExtractorForPerCall(result, connection);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
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
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
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
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
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
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
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
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
                    whereOrPrimaryKey: whereOrPrimaryKey,
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
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity,
            QueryGroup where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return connection.Update<TEntity>(entity: entity,
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

        #region UpdateAsync

        /// <summary>
        /// Update an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    whereOrPrimaryKey: whereOrPrimaryKey,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync<TEntity>(TEntity entity,
            QueryGroup where,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.UpdateAsync<TEntity>(entity: entity,
                    where: where,
                    commandTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace,
                    statementBuilder: StatementBuilder);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> ExecuteQueryAsync<TEntity>(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.ExecuteQueryAsync<TEntity>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> ExecuteNonQueryAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
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
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public Task<AsyncResultExtractor<object>> ExecuteScalarAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var connection = (transaction?.Connection ?? CreateConnection());
            var hasError = false;

            try
            {
                // Call the method
                var result = connection.ExecuteScalarAsync(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: CommandTimeout,
                    transaction: transaction);

                // Return the result
                return ConvertToAsyncResultExtractorForPerCall(result, connection, transaction);
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                // Dispose the connection
                if (hasError)
                {
                    DisposeConnectionForPerCall(connection, transaction);
                }
            }
        }

        #endregion
    }
}
