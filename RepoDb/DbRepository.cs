using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using RepoDb.Exceptions;
using System.Reflection;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A base object for all <b>Shared-Based Repositories</b>. This object is usually being inheritted if
    /// the derived class is meant for shared-based operations when it comes to data manipulations.
    /// This object is used by <i>RepoDb.BaseRepository</i> as an underlying repository for all its
    /// operations.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public class DbRepository<TDbConnection> where TDbConnection : DbConnection
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.DbRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public DbRepository(string connectionString)
            : this(connectionString, null, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.DbRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public DbRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.DbRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.DbRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Trace</i> interface.</param>
        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
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
        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
        {
            // Properties
            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
            Cache = (cache ?? new MemoryCache());
            Trace = trace;
            StatementBuilder = (statementBuilder ??
                StatementBuilderMapper.Get(typeof(TDbConnection))?.StatementBuilder ??
                new SqlDbStatementBuilder());
        }

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

        // CreateConnection (TDbConnection)

        /// <summary>
        /// Creates a new instance of database connection.
        /// </summary>
        /// <returns>An instance of new database connection.</returns>
        public TDbConnection CreateConnection()
        {
            var connection = Activator.CreateInstance<TDbConnection>();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        // GuardPrimaryKey
        private PropertyInfo GetAndGuardPrimaryKey<TEntity>(Command command)
            where TEntity : DataEntity
        {
            var property = DataEntityExtension.GetPrimaryProperty<TEntity>();
            if (property == null)
            {
                throw new PrimaryFieldNotFoundException($"No primary key found at type '{typeof(TEntity).FullName}' for database object '{DataEntityExtension.GetMappedName<TEntity>(command)}'.");
            }
            return property;
        }

        // GuardCountable

        private void GuardCountable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsCountable<TEntity>())
            {
                throw new EntityNotCountableException(DataEntityExtension.GetMappedName<TEntity>(Command.Count));
            }
        }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public long Count<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Count<TEntity>(where: (QueryGroup)null,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long Count<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
            }
            return Count<TEntity>(where: queryGroup,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long Count<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Count<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long Count<TEntity>(QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardCountable<TEntity>();

            // Variables
            var command = Command.Count;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateCount(new QueryBuilder<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return default(int);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = Convert.ToInt64(ExecuteScalar(commandText: commandText,
                 param: param,
                 commandType: DataEntityExtension.GetCommandType<TEntity>(command),
                 commandTimeout: CommandTimeout,
                 transaction: transaction));

            // After Execution
            if (Trace != null)
            {
                Trace.AfterCount(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public Task<long> CountAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(transaction: transaction));
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountAsync<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountAsync<TEntity>(QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        // GuardBatchQueryable

        private void GuardBatchQueryable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsBatchQueryable<TEntity>())
            {
                throw new EntityNotBatchQueryableException(DataEntityExtension.GetMappedName<TEntity>(Command.BatchQuery));
            }
        }

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return BatchQuery<TEntity>(where: (QueryGroup)null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else
            {
                queryGroup = QueryGroup.Parse(where);
            }
            return BatchQuery<TEntity>(where: queryGroup,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return BatchQuery<TEntity>(where: new QueryGroup(where),
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery<TEntity>(QueryGroup where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardBatchQueryable<TEntity>();

            // Variables
            var command = Command.BatchQuery;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateBatchQuery(new QueryBuilder<TEntity>(), where, page, rowsPerBatch, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(command),
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterBatchQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(QueryGroup where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        // GuardQueryable

        private void GuardQueryable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsQueryable<TEntity>())
            {
                throw new EntityNotQueryableException(DataEntityExtension.GetMappedName<TEntity>(Command.Query));
            }
        }

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(where: (QueryGroup)null,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>(Command.Query);
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return Query<TEntity>(where: queryGroup,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public IEnumerable<TEntity> Query<TEntity>(QueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = Cache?.Get(cacheKey);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item;
                }
            }

            // Check
            GuardQueryable<TEntity>();

            // Variables
            var command = Command.Query;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateQuery(new QueryBuilder<TEntity>(), where, top, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQuery<TEntity>(commandText: commandText,
                 param: param,
                 commandType: commandType,
                 commandTimeout: CommandTimeout,
                 transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null && result != null && result.Any())
            {
                Cache?.Add(cacheKey, result);
            }

            // Result
            return result;
        }

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>DataEntity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryGroup where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<OrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        // GuardInsertable

        private void GuardInsertable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInsertable<TEntity>())
            {
                throw new EntityNotInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.Insert));
            }
        }

        // Insert

        /// <summary>
        /// Insert a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public object Insert<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInsertable<TEntity>();

            // Variables
            var command = Command.Insert;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;
            var param = entity?.AsObject();

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (StatementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = DataEntityIsPrimaryIdentityCache.Get<TEntity>(ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)StatementBuilder).CreateInsert(new QueryBuilder<TEntity>(), isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = StatementBuilder.CreateInsert(new QueryBuilder<TEntity>());
                }
            }

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InsertAsync

        /// <summary>
        /// Insert a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public Task<object> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<object>(() =>
                Insert<TEntity>(entity: entity,
                    transaction: transaction));
        }

        // GuardInlineInsertable

        private void GuardInlineInsertable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineInsertable<TEntity>())
            {
                throw new EntityNotInlineInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineInsert));
            }
        }

        // InlineInsert

        /// <summary>
        /// Inserts a data in the database targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public object InlineInsert<TEntity>(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInlineInsertable<TEntity>();

            // Variables
            var command = Command.InlineInsert;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (StatementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = DataEntityIsPrimaryIdentityCache.Get<TEntity>(ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)StatementBuilder).CreateInlineInsert(new QueryBuilder<TEntity>(), entity?.AsFields(),
                        overrideIgnore, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = StatementBuilder.CreateInlineInsert(new QueryBuilder<TEntity>(), entity?.AsFields(), overrideIgnore);
                }
            }

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                Trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalar(commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineInsertAsync

        /// <summary>
        /// Inserts a data in the database targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public Task<object> InlineInsertAsync<TEntity>(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineInsert<TEntity>(entity: entity,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        // GuardInlineMergeable

        private void GuardInlineMergeable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineMergeable<TEntity>())
            {
                throw new EntityNotInlineMergeableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineMerge));
            }
        }

        // InlineMerge

        /// <summary>
        /// Merges a data in the database targetting certain fields only. It uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineMerge<TEntity>(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return InlineMerge<TEntity>(entity,
                qualifiers: null,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data in the database targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineMerge<TEntity>(object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false,
            IDbTransaction transaction = null) where TEntity : DataEntity
        {
            // Check
            GuardInlineMergeable<TEntity>();

            // Variables
            var command = Command.InlineMerge;
            var entityProperties = entity?.GetType().GetProperties();

            // Force to use the PrimaryKey
            if (qualifiers == null)
            {
                var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException($"Merge operation could proceed. Primary field is missing at type " +
                        $"{typeof(TEntity).FullName} ({DataEntityExtension.GetMappedName<TEntity>(command)}).");
                }
            }

            // All qualifiers must be present in the dynamic entity
            var missingFields = qualifiers?.Where(qualifier => entityProperties.FirstOrDefault(property =>
                property.GetMappedName().ToLower() == qualifier.Name.ToLower()) == null);
            if (missingFields?.Count() > 0)
            {
                throw new InvalidOperationException($"All qualifier fields must be presented in the given dynamic entity object. " +
                    $"The list of missing field(s) are {missingFields.Select(f => f.AsField()).Join(", ")}.");
            }

            // Other variables
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (StatementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = DataEntityIsPrimaryIdentityCache.Get<TEntity>(ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)StatementBuilder).CreateInlineMerge(new QueryBuilder<TEntity>(), entity?.AsFields(),
                        qualifiers, overrideIgnore, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = StatementBuilder.CreateInlineMerge(new QueryBuilder<TEntity>(), entity?.AsFields(), qualifiers,
                        overrideIgnore);
                }
            }

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                Trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineMergeAsync

        /// <summary>
        /// Merges a data in the database targetting certain fields only in an asynchronous way. Uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineMergeAsync<TEntity>(object entity, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineMerge<TEntity>(entity,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        /// <summary>
        /// Merges a data in the database targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineMergeAsync<TEntity>(object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false,
            IDbTransaction transaction = null) where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineMerge<TEntity>(entity: entity,
                    qualifiers: qualifiers,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        // GuardInlineUpdateable

        private void GuardInlineUpdateable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineUpdateable<TEntity>())
            {
                throw new EntityNotInlineUpdateableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate));
            }
        }

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else
            {
                queryGroup = QueryGroup.Parse(where);
            }
            return InlineUpdate<TEntity>(entity: entity,
                where: queryGroup,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return InlineUpdate<TEntity>(entity: entity,
                where: new QueryGroup(where),
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate<TEntity>(object entity, QueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInlineUpdateable<TEntity>();

            // Append prefix to all parameters
            where.AppendParametersPrefix();

            // Variables
            var command = Command.InlineUpdate;
            var commandText = StatementBuilder.CreateInlineUpdate(new QueryBuilder<TEntity>(),
                entity.AsFields(), where, overrideIgnore);
            var param = entity?.Merge(where);

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(command),
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInlineUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        /// <summary>
        /// Updates a data in the database targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync<TEntity>(object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        /// <summary>
        /// Updates a data in the database targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync<TEntity>(object entity, QueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        // GuardUpdateable

        private void GuardUpdateable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsUpdateable<TEntity>())
            {
                throw new EntityNotUpdateableException(DataEntityExtension.GetMappedName<TEntity>(Command.Update));
            }
        }

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var property = GetAndGuardPrimaryKey<TEntity>(Command.Update);
            return Update(entity: entity,
                where: new QueryGroup(property.AsQueryField(entity, true).AsEnumerable()),
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity, IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Update(entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>(Command.Update);
                    queryGroup = new QueryGroup(new QueryField(property?.GetMappedName(), where).AsEnumerable());
                }
            }
            return Update(entity: entity,
                where: queryGroup,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update<TEntity>(TEntity entity, QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardUpdateable<TEntity>();

            // Variables
            var command = Command.Update;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            if (commandType != CommandType.StoredProcedure)
            {
                // Append prefix to all parameters for non StoredProcedure (this is mappable, that's why)
                where.AppendParametersPrefix();
            }
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateUpdate(new QueryBuilder<TEntity>(), where);
            var param = entity?.AsObject(where);

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    transaction: transaction));
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync<TEntity>(TEntity entity, QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        // GuardDeletable

        private void GuardDeletableAll<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsDeletable<TEntity>())
            {
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>(Command.Delete));
            }
        }

        // DeleteAll

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int DeleteAll<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletableAll<TEntity>();

            // Variables
            var command = Command.DeleteAll;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateDeleteAll(new QueryBuilder<TEntity>());

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, null, null);
                Trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                commandType: commandType,
                commandTimeout: CommandTimeout);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // DeleteAllAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAllAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() => DeleteAll<TEntity>(transaction: transaction));
        }

        // GuardDeletable

        private void GuardDeletable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsDeletable<TEntity>())
            {
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>(Command.Delete));
            }
        }

        // Delete

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Delete<TEntity>((QueryGroup)null, transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Delete<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((QueryField)where).AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else if (where is TEntity)
            {
                var property = GetAndGuardPrimaryKey<TEntity>(Command.Delete);
                queryGroup = new QueryGroup(property.AsQueryField(where).AsEnumerable());
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>(Command.Delete);
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return Delete<TEntity>(where: queryGroup,
                    transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete<TEntity>(QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletable<TEntity>();

            // Variables
            var command = Command.Delete;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                StatementBuilder.CreateDelete(new QueryBuilder<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // DeleteAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() => Delete<TEntity>(transaction: transaction));
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync<TEntity>(IEnumerable<QueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync<TEntity>(QueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        // GuardMergeable

        private void GuardMergeable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsMergeable<TEntity>())
            {
                throw new EntityNotMergeableException(DataEntityExtension.GetMappedName<TEntity>(Command.Merge));
            }
        }

        // Merge

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Merge<TEntity>(entity: entity,
                qualifiers: null,
                    transaction: transaction);
        }

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge<TEntity>(TEntity entity, IEnumerable<Field> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var command = Command.Merge;

            // Check
            GuardMergeable<TEntity>();
            GetAndGuardPrimaryKey<TEntity>(command);

            // Variables
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;
            var param = entity?.AsObject();

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (StatementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = DataEntityIsPrimaryIdentityCache.Get<TEntity>(ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)StatementBuilder).CreateMerge(new QueryBuilder<TEntity>(), qualifiers, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = StatementBuilder.CreateMerge(new QueryBuilder<TEntity>(), qualifiers);
                }
            }

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(command),
                commandTimeout: CommandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Merge<TEntity>(entity: entity,
                    transaction: transaction));
        }

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync<TEntity>(TEntity entity, IEnumerable<Field> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Merge<TEntity>(entity: entity,
                    qualifiers: qualifiers,
                    transaction: transaction));
        }

        // GuardBulkInsert

        private void GuardBulkInsert<TEntity>()
            where TEntity : DataEntity
        {
            if (typeof(TDbConnection) != typeof(System.Data.SqlClient.SqlConnection)
                || !DataEntityExtension.IsBulkInsertable<TEntity>())
            {
                throw new EntityNotBulkInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.BulkInsert));
            }
        }

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int BulkInsert<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardBulkInsert<TEntity>();

            // Variables
            var command = Command.BulkInsert;
            using (var connection = (transaction?.Connection ?? CreateConnection()).EnsureOpen())
            {
                // Before Execution
                if (Trace != null)
                {
                    var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), command.ToString(), entities, null);
                    Trace.BeforeBulkInsert(cancellableTraceLog);
                    if (cancellableTraceLog.IsCancelled)
                    {
                        if (cancellableTraceLog.IsThrowException)
                        {
                            throw new CancelledExecutionException(command.ToString());
                        }
                        return 0;
                    }
                }

                // Convert to table
                //var table = DataEntityConverter.ToDataTable(entities);
                var table = entities.AsDataTable(connection, Command.BulkInsert);

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Actual Execution
                var sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)connection);
                var result = entities.Count();
                sqlBulkCopy.DestinationTableName = table.TableName;
                foreach (var column in table.Columns.OfType<DataColumn>())
                {
                    sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);

                // After Execution
                if (Trace != null)
                {
                    Trace.AfterBulkInsert(new TraceLog(MethodBase.GetCurrentMethod(), command.ToString(), table, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }

                // Result
                return result;
            }
        }

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                BulkInsert(entities: entities,
                    transaction: transaction));
        }

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
        }

        // ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the 
        /// <i>System.Data.IDataReader</i> object and converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                ExecuteQuery<TEntity>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
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
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
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
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<int>(() =>
                ExecuteNonQuery(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
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
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: CommandTimeout,
                transaction: transaction);
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
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
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<object>(() =>
                ExecuteScalar(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }
    }
}