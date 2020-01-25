using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Merge<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public object Merge(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public object Merge(TEntity entity,
            Field qualifier,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifier: qualifier,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public object Merge(TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifier: null,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            Field qualifier,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifier: qualifier,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public TResult Merge<TResult>(TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                hints: hints,
				transaction: transaction);
        }

        #endregion

        #region MergeAsync<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            Field qualifier,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifier: qualifier,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<object> MergeAsync(TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifier: null,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            Field qualifier,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifier: qualifier,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<TResult> MergeAsync<TResult>(TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity, TResult>(entity: entity,
                qualifiers: qualifiers,
                hints: hints,
				transaction: transaction);
        }

        #endregion
    }
}
