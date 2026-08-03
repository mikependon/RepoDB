using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region BulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkDeleteByKey(tableName: ClassMappedNameCache.Get<TEntity>(),
                primaryKeys: primaryKeys,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkDeleteByKeyAsync<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkDeleteByKeyAsync(tableName: ClassMappedNameCache.Get<TEntity>(),
                primaryKeys: primaryKeys,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
