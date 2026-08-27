using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Interfaces;
using RepoDb.Firebird.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Firebird bulk-delete operation. See the
    /// remarks on <c>Operations/DbRepository/BulkInsert.cs</c> for the connection-lifecycle pattern shared
    /// by every wrapper in this folder.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk. Returns the number of deleted rows.
        /// </summary>
        public static int BulkDelete<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.BulkDelete(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkDelete{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkDelete<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkDelete(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <inheritdoc cref="BulkDelete{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkDeleteAsync<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkDeleteAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDelete{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkDeleteAsync<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkDeleteAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion
    }
}
