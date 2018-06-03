using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a base object for all <b>Entity-Based Repositories</b>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <i>Data Entity</i> object to be mapped on this repository. This object must inherit the <i>RepoDb.DataEntity</i>
    /// object in order to be qualified as a repository entity.
    /// </typeparam>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public interface IBaseRepository<TEntity, TDbConnection>
        where TEntity : DataEntity
        where TDbConnection : DbConnection
    {

        // CreateConnection

        /// <summary>
        /// Creates a new instance of database connection.
        /// </summary>
        /// <returns>An instance of new database connection.</returns>
        TDbConnection CreateConnection();

        // DbRepository

        /// <summary>
        /// Gets the underlying repository used by this repository.
        /// </summary>
        IDbRepository<TDbConnection> DbRepository { get; }

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

        // StatementBuilder

        /// <summary>
        /// Gets the statement builder object that is being used by this repository.
        /// </summary>
        IStatementBuilder StatementBuilder { get; }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        int Count(IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        int Count(IQueryGroup where, IDbTransaction transaction = null);

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        Task<int> CountAsync(IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<int> CountAsync(IQueryGroup where, IDbTransaction transaction = null);

        // CountBig

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        long CountBig(IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        long CountBig(IQueryGroup where, IDbTransaction transaction = null);

        // CountBigAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        Task<long> CountBigAsync(IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        Task<long> CountBigAsync(IQueryGroup where, IDbTransaction transaction = null);

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> BatchQuery(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> BatchQueryAsync(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null);

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        IEnumerable<TEntity> Query(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        Task<IEnumerable<TEntity>> QueryAsync(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null);

        // Insert

        /// <summary>
        /// Insert a data in the database.
        /// </summary>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        object Insert(TEntity entity, IDbTransaction transaction = null);

        // InsertAsync

        /// <summary>
        /// Insert a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        Task<object> InsertAsync(TEntity entity, IDbTransaction transaction = null);

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int InlineUpdate(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> InlineUpdateAsync(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null);

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update(TEntity entity, object where, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Update(TEntity entity, IQueryGroup where, IDbTransaction transaction = null);

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync(TEntity entity, object where, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> UpdateAsync(TEntity entity, IQueryGroup where, IDbTransaction transaction = null);

        // Delete

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Delete(IQueryGroup where, IDbTransaction transaction = null);

        // DeleteAsync

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync(object where, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null);

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> DeleteAsync(IQueryGroup where, IDbTransaction transaction = null);

        // Merge

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Merge(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int Merge(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null);

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> MergeAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> MergeAsync(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null);

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        int BulkInsert(IEnumerable<TEntity> entities, IDbTransaction transaction = null);

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null);

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
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
        IEnumerable<TEntity> ExecuteQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

        // ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the 
        /// <i>System.Data.IDataReader</i> object and converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
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
        Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null);

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
        int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null);

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
        Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null);

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