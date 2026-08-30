#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrappers for the Firebird
    /// bulk-update operation - thin pass-throughs onto <see cref="DbRepository{TDbConnection}"/>'s own
    /// wrapper (see <c>Operations/DbRepository/BulkUpdate.cs</c>).
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Updates existing rows in the database in bulk, matched against the given entities. Returns the
        /// number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities carrying the values to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows to update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of updated rows.</returns>
        public static int BulkUpdate<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            repository.DbRepository.BulkUpdate(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Updates existing rows in the database in bulk, targeting an explicitly named table and matched
        /// against the given entities. Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities carrying the values to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows to update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of updated rows.</returns>
        public static int BulkUpdate<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            where TEntity : class =>
            repository.DbRepository.BulkUpdate(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Updates existing rows in the database in bulk in an asynchronous way, matched against the given
        /// entities. Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities carrying the values to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows to update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of updated rows.</returns>
        public static Task<int> BulkUpdateAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            repository.DbRepository.BulkUpdateAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Updates existing rows in the database in bulk in an asynchronous way, targeting an explicitly
        /// named table and matched against the given entities. Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities carrying the values to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows to update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of updated rows.</returns>
        public static Task<int> BulkUpdateAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            where TEntity : class =>
            repository.DbRepository.BulkUpdateAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
