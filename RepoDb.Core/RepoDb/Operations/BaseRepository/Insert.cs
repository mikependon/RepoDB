using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Insert<TEntity>

        /// <summary>
        /// Inserts a new row in the table.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public object Insert(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new row in the table.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public TResult Insert<TResult>(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity, TResult>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region InsertAsync<TEntity>

        /// <summary>
        /// Inserts a new row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<object> InsertAsync(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public Task<TResult> InsertAsync<TResult>(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.InsertAsync<TEntity, TResult>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
