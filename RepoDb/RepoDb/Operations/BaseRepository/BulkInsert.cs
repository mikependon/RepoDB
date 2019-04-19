using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region BulkInsert

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
        {
            return DbRepository.BulkInsert<TEntity>(entities: entities,
                mappings: mappings);
        }

        #endregion

        #region BulkInsertAsync

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> BulkInsertAsync(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null)
        {
            return DbRepository.BulkInsertAsync<TEntity>(entities: entities,
                mappings: mappings);
        }

        #endregion
    }
}
