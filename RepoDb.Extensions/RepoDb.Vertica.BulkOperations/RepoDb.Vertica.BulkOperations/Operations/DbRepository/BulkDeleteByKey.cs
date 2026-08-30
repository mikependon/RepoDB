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
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Vertica bulk-delete-by-key operation. See
    /// the remarks on <c>Operations/DbRepository/BulkInsert.cs</c> for the connection-lifecycle pattern
    /// shared by every wrapper in this folder.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, targeting the table mapped to
        /// <typeparamref name="TEntity"/>, matched by a bare list of primary (or identity) key values
        /// rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary (or identity) key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="primaryKeys">The list of primary (or identity) key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this DbRepository<VerticaConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            repository.BulkDeleteByKey(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk, targeting the given table, matched by a bare
        /// list of primary (or identity) key values rather than full entities. Returns the number of
        /// deleted rows.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary (or identity) key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary (or identity) key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TPrimaryKey>(this DbRepository<VerticaConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null)
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkDeleteByKey(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, targeting the table
        /// mapped to <typeparamref name="TEntity"/>, matched by a bare list of primary (or identity) key
        /// values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary (or identity) key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="primaryKeys">The list of primary (or identity) key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static async Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this DbRepository<VerticaConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, targeting the given
        /// table, matched by a bare list of primary (or identity) key values rather than full entities.
        /// Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary (or identity) key.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary (or identity) key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for the operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static async Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this DbRepository<VerticaConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkDeleteByKeyAsync(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion
    }
}
