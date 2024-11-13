using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region SkipQuery<TEntity>

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(where: where,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the rows from the database by batch.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> SkipQuery(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null)
        {
            return DbRepository.SkipQuery<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        #endregion

        #region SkipQueryAsync<TEntity>

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(string tableName,
            int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            string? hints = null,
            IEnumerable<Field> fields = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(tableName: tableName,
                skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            object where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryField? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            IEnumerable<QueryField> where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the rows from the database by batch in an asynchronous way.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="rowsPerBatch">The number of data per batch to be returned.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> SkipQueryAsync(int skip,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            QueryGroup? where = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            string traceKey = TraceKeys.SkipQuery,
			IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.SkipQueryAsync<TEntity>(skip: skip,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                where: where,
                fields: fields,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
