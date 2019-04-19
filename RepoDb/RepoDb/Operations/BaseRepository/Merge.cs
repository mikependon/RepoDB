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
        /// <param name="transaction">The transaction to be used.</param>
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
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
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
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
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

        #region MergeAsync<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity,
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
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity,
            IEnumerable<Field> qualifiers,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        #endregion
    }
}
