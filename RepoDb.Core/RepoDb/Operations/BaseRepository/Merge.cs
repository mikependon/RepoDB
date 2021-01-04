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
        #region Merge<TEntity>

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public object Merge(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public object Merge(TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifier: qualifier,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public object Merge(TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public object Merge(TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifier: qualifier,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MergeAsync<TEntity>

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifier: qualifier,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
