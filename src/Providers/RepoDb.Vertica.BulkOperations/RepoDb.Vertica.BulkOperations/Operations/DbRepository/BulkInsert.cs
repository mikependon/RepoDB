#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Enumerations;
using RepoDb.Enumerations.Vertica;
using RepoDb.Interfaces;
using RepoDb.Vertica.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Vertica bulk-insert operation. Each
    /// method resolves a connection (reusing the transaction's connection when one is supplied, otherwise
    /// creating one via the repository), delegates to the matching <see cref="VerticaConnection"/> extension
    /// method, and disposes the connection afterwards only when the repository owns a per-call connection
    /// and no external transaction was supplied.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Inserts a list of entities into the database in bulk, targeting the table mapped to
        /// <typeparamref name="TEntity"/>. Returns the number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int BulkInsert<TEntity>(this DbRepository<VerticaConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            repository.BulkInsert(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Inserts a list of entities into the database in bulk, targeting the given table. Returns the
        /// number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int BulkInsert<TEntity>(this DbRepository<VerticaConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkInsert(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Inserts a list of entities into the database in bulk in an asynchronous way, targeting the table
        /// mapped to <typeparamref name="TEntity"/>. Returns the number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of inserted rows.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this DbRepository<VerticaConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkInsertAsync(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Inserts a list of entities into the database in bulk in an asynchronous way, targeting the given
        /// table. Returns the number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of inserted rows.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this DbRepository<VerticaConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkInsertAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Helpers

        private static void DisposeIfOwned(DbRepository<VerticaConnection> repository,
            VerticaTransaction transaction,
            VerticaConnection connection)
        {
            if (repository.ConnectionPersistency == ConnectionPersistency.PerCall && transaction == null)
            {
                connection.Dispose();
            }
        }

        #endregion
    }
}
