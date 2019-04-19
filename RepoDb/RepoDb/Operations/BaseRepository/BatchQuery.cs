using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region BatchQuery<TEntity>

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(Expression<Func<TEntity, bool>> where,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public IEnumerable<TEntity> BatchQuery(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region BatchQueryAsync<TEntity>

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(object where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(Expression<Func<TEntity, bool>> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Queries a data from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="page">The page of the batch to be used. This is a zero-based index (the first page is 0).</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
