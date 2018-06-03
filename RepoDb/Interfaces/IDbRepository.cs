using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a base object for all <b>Shared-Based Repositories</b>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public interface IDbRepository<TDbConnection>
        where TDbConnection : DbConnection
    {

        // CreateConnection (TDbConnection)

        /// <summary>
        /// Creates a new instance of database connection.
        /// </summary>
        /// <returns>An instance of new database connection.</returns>
        TDbConnection CreateConnection();

        // DbCache

        /// <summary>
        /// Gets the cache object that is being used by this repository.
        /// </summary>
        ICache Cache { get; }

        // Trace

        /// <summary>
        /// Gets the trace object that is being used by this repository.
        /// </summary>
        ITrace Trace { get; }

        // StamentBuilder

        /// <summary>
        /// Gets the statement builder object that is being used by this repository.
        /// </summary>
        IStatementBuilder StatementBuilder { get; }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        int Count<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        Task<int> CountAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // CountBig

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        long CountBig<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // CountBigAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        Task<long> CountBigAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery<TEntity>(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery<TEntity>(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery<TEntity>(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query<TEntity>(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query<TEntity>(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity;

        // Insert

        /// <summary>
        /// Insert a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        object Insert<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // InsertAsync

        /// <summary>
        /// Insert a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        Task<object> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate<TEntity>(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate<TEntity>(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync<TEntity>(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync<TEntity>(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;
        
        // Delete

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // DeleteAsync

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // Merge

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Merge<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Merge<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> MergeAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> MergeAsync<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int BulkInsert<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>Data Entity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity;

        // ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the 
        /// <i>System.Data.IDataReader</i> object and converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>Data Entity</i> object.</typeparam>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>Data Entity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity;

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
        int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

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
        Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

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
        object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

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
        Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);
    }
}