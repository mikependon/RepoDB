#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Interfaces;
using RepoDb.Vertica.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrappers for the Vertica
    /// bulk-delete-by-key operation - thin pass-throughs onto <see cref="DbRepository{TDbConnection}"/>'s
    /// own wrapper (see <c>Operations/DbRepository/BulkDeleteByKey.cs</c>).
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by a bare list of primary (or identity)
        /// key values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this BaseRepository<TEntity, VerticaConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKey(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by a bare list of
        /// primary (or identity) key values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this BaseRepository<TEntity, VerticaConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
