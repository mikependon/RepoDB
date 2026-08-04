using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;

using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region BulkDelete<TEntity>

        /// <summary>
        /// Bulk delete a list of data entity objects from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-delete operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkDelete<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDelete,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkDelete(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk delete a list of data entity objects from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-delete operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkDelete<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDelete,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkDelete(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkDeleteAsync<TEntity>

        /// <summary>
        /// Bulk delete a list of data entity objects from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-delete operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDelete,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkDeleteAsync(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk delete a list of data entity objects from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-delete operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDelete,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkDeleteAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
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
