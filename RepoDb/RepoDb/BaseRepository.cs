using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using System;

namespace RepoDb
{
    /// <summary>
    /// An inherritable base object for all <b>Entity-Based Repositories</b>. This object is usually being inheritted if the 
    /// derived class is meant for entity-based operations with corresponding <i>DataEntity</i> object for data manipulations.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <i>DataEntity</i> object to be mapped on this repository. This object must inherit the <i>RepoDb.DataEntity</i>
    /// object in order to be qualified as a repository entity.
    /// </typeparam>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public abstract class BaseRepository<TEntity, TDbConnection> : IDisposable where TEntity : DataEntity
        where TDbConnection : DbConnection
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public BaseRepository(string connectionString)
            : this(connectionString, null, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public BaseRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        public BaseRepository(string connectionString, ICache cache)
            : this(connectionString, null, cache, null, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        public BaseRepository(string connectionString, ITrace trace)
            : this(connectionString, null, null, trace, null, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        public BaseRepository(string connectionString, IStatementBuilder statementBuilder)
            : this(connectionString, null, null, null, statementBuilder, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <i>Single</i> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <i>PerCall</i> will create a new connection object on every repository call.
        /// </param>
        public BaseRepository(string connectionString, ConnectionPersistency connectionPersistency)
            : this(connectionString, null, null, null, null, connectionPersistency)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
            : this(connectionString, commandTimeout, cache, trace, statementBuilder, ConnectionPersistency.PerCall)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.DbRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        /// <param name="connectionPersistency">
        /// The database connection persistency type. Setting to <i>Single</i> will make the repository re-used a single connection all throughout its lifespan. Setting 
        /// to <i>PerCall</i> will create a new connection object on every repository call.
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
        /// Gets the database connection persistency used by this repository. The default value is <i>ConnectionPersistency.PerCall</i>.
        /// </summary>
        public ConnectionPersistency ConnectionPersistency => DbRepository.ConnectionPersistency;

        #endregion

        #region Other Methods

        // CreateConnection

        /// <summary>
        /// Creates a new instance of the database connection. If the value <i>ConnectionPersistency</i> property is <i>Instance</i>, then this will return
        /// the <i>System.Data.Common.DbConnection</i> that is being used by the current repository instance. However, if the value of the <i>ConnectionPersistency</i>
        /// property is <i>PerCall</i>, then this will return a new instance of the <i>System.Data.Common.DbConnection</i> object.
        /// </summary>
        /// <returns>An instance of the <i>System.Data.Common.DbConnection</i> object.</returns>
        public TDbConnection CreateConnection()
        {
            return DbRepository.CreateConnection();
        }

        /// <summary>
        /// Creates a new instance of the database connection. If the value <i>ConnectionPersistency</i> property is <i>Instance</i>, then this will return
        /// the <i>System.Data.Common.DbConnection</i> that is being used by the current repository instance. However, if the value of the <i>ConnectionPersistency</i>
        /// property is <i>PerCall</i>, then this will return a new instance of the <i>System.Data.Common.DbConnection</i> object.
        /// </summary>
        /// <param name="force">Set to true to forcely create a new instance of <i>System.Data.Common.DbConnection</i> object regardless of the persistency.</param>
        /// <returns>An instance of the <i>System.Data.Common.DbConnection</i> object.</returns>
        public TDbConnection CreateConnection(bool force)
        {
            return DbRepository.CreateConnection(force);
        }

        /// <summary>
        /// Dispose the current repository instance (of type <i>RepoDb.BaseRepository</i>). It is not necessary to call this method if the value of the <i>ConnectionPersistency</i>
        /// property is equals to <i>ConnectionPersistency.PerCall</i>. This method only manages the connection persistency for the repositories where the value
        /// of the <i>ConnectionPersitency</i> property is equals to <i>ConnectionPersitency.Instance</i>.
        /// </summary>
        public void Dispose()
        {
            DbRepository.Dispose();
        }

        #endregion

        #region Operational Methods

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(object where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(QueryGroup where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(object where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(QueryGroup where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int BulkInsert(IEnumerable<TEntity> entities)
        {
            return DbRepository.BulkInsert(entities: entities);
        }

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            return DbRepository.BulkInsertAsync<TEntity>(entities: entities);
        }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public long Count(IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public long Count(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public long Count(IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public long Count(QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public Task<long> CountAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public Task<long> CountAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public Task<long> CountAsync(IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public Task<long> CountAsync(QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // Delete

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        // DeleteAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation. When is set to <i>NULL</i>, it deletes all the data from the database.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // DeleteAll

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int DeleteAll(IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(transaction: transaction);
        }

        // DeleteAllAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAllAsync(IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>();
        }

        // InlineInsert

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public object InlineInsert(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineInsert<TEntity>(entity: entity,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineInsertAsync

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public Task<object> InlineInsertAsync(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineInsertAsync<TEntity>(entity: entity,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineMerge

        /// <summary>
        /// Merges a data in the database by targetting certain fields only. Uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> that contains the targetted columns to be merged.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineMerge(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineMerge(object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineMerge<TEntity>(entity,
                qualifiers: qualifiers,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineMergeAsync

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an asynchronous way. Uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> that contains the targetted columns to be merged.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineMergeAsync(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew(() =>
                DbRepository.InlineMerge<TEntity>(entity,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineMergeAsync(object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false,
            IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew(() =>
                DbRepository.InlineMerge<TEntity>(entity: entity,
                    qualifiers: qualifiers,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, QueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, QueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // Insert

        /// <summary>
        /// Inserts a data in the database.
        /// </summary>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public object Insert(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                transaction: transaction);
        }

        // InsertAsync

        /// <summary>
        /// Inserts a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public Task<object> InsertAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        // Merge

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge(TEntity entity, IEnumerable<Field> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity, IEnumerable<Field> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query(int? top = 0, IEnumerable<OrderField> orderBy = null, string cacheKey = null,
            IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.Query<TEntity>(top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query(object where, int? top = 0, IEnumerable<OrderField> orderBy = null, string cacheKey = null, 
            IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<QueryField> where, int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query(QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.QueryAsync<TEntity>(top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(object where, int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<QueryField> where, int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null,
            string cacheKey = null, IDbTransaction transaction = null, bool? recursive = false, int? recursionDepth = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                transaction: transaction,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        // Truncate

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        public void Truncate()
        {
            DbRepository.Truncate<TEntity>();
        }

        // TruncateAsync

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        public Task TruncateAsync()
        {
            return DbRepository.TruncateAsync<TEntity>();
        }

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, IEnumerable<QueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, QueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region Execute Methods

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        // ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the 
        /// <i>System.Data.IDataReader</i> object and converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQueryAsync<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        // ExecuteNonQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteNonQuery</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        // ExecuteNonQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteNonQuery</i> method of the
        /// <i>System.Data.IDataReader</i> object and returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        // ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        // ExecuteScalarAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalarAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                transaction: transaction);
        }

        #endregion
    }
}