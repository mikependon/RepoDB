using FirebirdSql.Data.FirebirdClient;
using System;
using System.Linq.Expressions;
using RepoDb.Enumerations.Firebird;
using RepoDb.Interfaces;
using RepoDb.Firebird.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Firebird bulk-update operation. See the
    /// remarks on <c>Operations/DbRepository/BulkInsert.cs</c> for the connection-lifecycle pattern shared
    /// by every wrapper in this folder.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Updates existing rows in the database in bulk. Returns the number of updated rows.
        /// </summary>
        public static int BulkUpdate<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.BulkUpdate(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkUpdate{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkUpdate<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkUpdate(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <inheritdoc cref="BulkUpdate{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkUpdateAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkUpdate{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkUpdateAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion
    }
}
