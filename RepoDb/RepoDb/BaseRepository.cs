using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using System;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// An inherritable base object for all entity-based repositories. This object is usually being inheritted if the 
    /// derived class is meant for entity-based operations with corresponding data entity object for data manipulations.
    /// </summary>
    /// <typeparam name="TEntity">The type of data entity object to be mapped on this repository.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    public abstract class BaseRepository<TEntity, TDbConnection> : IDisposable
        where TDbConnection : DbConnection
        where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public BaseRepository(string connectionString)
            : this(connectionString, null, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public BaseRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        public BaseRepository(string connectionString, ICache cache)
            : this(connectionString, null, cache, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public BaseRepository(string connectionString, ITrace trace)
            : this(connectionString, null, null, trace, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public BaseRepository(string connectionString, IStatementBuilder statementBuilder)
            : this(connectionString, null, null, null, statementBuilder, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public BaseRepository(string connectionString, ConnectionPersistency connectionPersistency)
            : this(connectionString, null, null, null, null, connectionPersistency)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="IStatementBuilder"/> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
            : this(connectionString, commandTimeout, cache, trace, statementBuilder, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <see cref="ICache"/> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <see cref="ITrace"/> interface.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <see cref="ConnectionPersistency.Instance"/> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <see cref="ConnectionPersistency.PerCall"/> will create a new connection object on every repository call.
        /// </param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder,
            ConnectionPersistency connectionPersistency)
        {
            DbRepository = new DbRepository<TDbConnection>(connectionString,
                commandTimeout,
                cache,
                trace,
                statementBuilder,
                connectionPersistency);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying repository used by this repository.
        /// </summary>
        private DbRepository<TDbConnection> DbRepository { get; set; }

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
        public TDbConnection CreateConnection()
        {
            return DbRepository.CreateConnection();
        }

        /// <summary>
        /// Creates a new instance of the database connection. If the value <see cref="ConnectionPersistency"/> property is <see cref="ConnectionPersistency.Instance"/>, then this will return
        /// the <see cref="DbConnection"/> that is being used by the current repository instance. However, if the value of the <see cref="ConnectionPersistency"/> property
        /// is <see cref="ConnectionPersistency.PerCall"/>, then this will return a new instance of the <see cref="DbConnection"/> object.
        /// </summary>
        /// <param name="force">Set to true to forcely create a new instance of <see cref="DbConnection"/> object regardless of the persistency.</param>
        /// <returns>An instance of the <see cref="DbConnection"/> object.</returns>
        public TDbConnection CreateConnection(bool force)
        {
            return DbRepository.CreateConnection(force);
        }

        /// <summary>
        /// Dispose the current repository instance. It is not necessary to call this method if the value of the <see cref="ConnectionPersistency"/>
        /// property is equals to <see cref="ConnectionPersistency.PerCall"/>. This method only manages the connection persistency for the repositories where the value
        /// of the <see cref="ConnectionPersistency"/> property is equals to <see cref="ConnectionPersistency.Instance"/>.
        /// </summary>
        public void Dispose()
        {
            DbRepository.Dispose();
        }

        #endregion

        #region BatchQuery

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(IEnumerable<QueryField> where,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(QueryGroup where,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        #endregion

        #region BatchQueryAsync

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> BatchQueryAsync(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        #endregion

        #region BulkInsert

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
        {
            return DbRepository.BulkInsert<TEntity>(entities: entities, mappings: mappings);
        }

        #endregion

        #region BulkInsertAsync

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> BulkInsertAsync(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
        {
            return DbRepository.BulkInsertAsync<TEntity>(entities: entities, mappings: mappings);
        }

        #endregion

        #region Count

        /// <summary>
        /// Counts the number of table data from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database.</returns>
        public long Count(IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count(IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public long Count(QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion

        #region CountAsync

        /// <summary>
        /// Counts the number of table data from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync(Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync(QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync(IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of table data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of data counted from the database based on the given query expression.</returns>
        public Task<AsyncResultExtractor<long>> CountAsync(QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion

        #region DeleteAsync

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="primaryKey">The primary key value to be used by this operation. When is set to null, it deletes all the data from the database.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync(object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync(Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync(IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAsync(QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion

        #region DeleteAll

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int DeleteAll(IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(transaction: transaction);
        }

        #endregion

        #region DeleteAllAsync

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> DeleteAllAsync(IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>();
        }

        #endregion

        #region InlineInsert

        /// <summary>
        /// Inserts a new data into the database.
        /// </summary>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be inserted.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public object InlineInsert(object entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineInsert<TEntity>(entity: entity,
                transaction: transaction);
        }

        #endregion

        #region InlineInsertAsync

        /// <summary>
        /// Inserts a new data into the database.
        /// </summary>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be inserted.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public Task<AsyncResultExtractor<object>> InlineInsertAsync(object entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineInsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        #endregion

        #region InlineMerge

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge(object entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge(object entity,
            Expression<Func<TEntity, object>> qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge(object entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineMerge(object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        #endregion

        #region InlineMergeAsync

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync(object entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMergeAsync<TEntity>(entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync(object entity,
            Expression<Func<TEntity, object>> qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifier field to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync(object entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an object into an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be merged by this operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineMergeAsync(object entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineMergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        #endregion

        #region InlineUpdate

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate(object entity,
            object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate(object entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate(object entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate(object entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only).
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int InlineUpdate(object entity,
            QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region InlineUpdateAsync

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync(object entity,
            object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync(object entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync(object entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync(object entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database (certain fields only) in an asynchronous way.
        /// </summary>
        /// <param name="entity">The key-value pair object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> InlineUpdateAsync(object entity,
            QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public object Insert(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                transaction: transaction);
        }

        #endregion

        #region InsertAsync

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task<AsyncResultExtractor<object>> InsertAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(TEntity entity,
            Expression<Func<TEntity, object>> qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(TEntity entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Merge(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        #endregion

        #region MergeAsync

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync(TEntity entity,
            Expression<Func<TEntity, object>> qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync(TEntity entity,
            Field qualifier,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged by this operation.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> MergeAsync(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        #endregion

        #region Query

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(object primaryKey,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(primaryKey: primaryKey,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> Query(QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAsync

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(object primaryKey,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(primaryKey: primaryKey,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of data to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of data entity object.</returns>
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> QueryAsync(QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region Truncate

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        public void Truncate()
        {
            DbRepository.Truncate<TEntity>();
        }

        #endregion

        #region TruncateAsync

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        public Task TruncateAsync()
        {
            return DbRepository.TruncateAsync<TEntity>();
        }

        #endregion

        #region Update

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity, QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region UpdateAsync

        /// <summary>
        /// Update an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            object primaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                primaryKey: primaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be used for update by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<AsyncResultExtractor<int>> UpdateAsync(TEntity entity,
            QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region ExecuteQuery

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <param name="commandText">The command text to be used by this operation.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the express of <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        #endregion

        #region ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and 
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
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
        public Task<AsyncResultExtractor<IEnumerable<TEntity>>> ExecuteQueryAsync(string commandText,
            object param = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQueryAsync<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
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
            return DbRepository.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        #endregion

        #region ExecuteNonQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/>
        /// and returns the number of affected data during the execution.
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
            return DbRepository.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
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
            return DbRepository.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
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
            return DbRepository.ExecuteScalarAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        #endregion
    }
}