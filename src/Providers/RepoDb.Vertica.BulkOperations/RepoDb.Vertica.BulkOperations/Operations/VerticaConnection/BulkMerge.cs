#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Interfaces;
using RepoDb.Vertica.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class VerticaConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Upserts a list of entities into the database in bulk - inserts rows that do not yet exist and
        /// updates rows matched by <paramref name="qualifiers"/> (or the primary/identity key when not
        /// specified). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression defining the properties used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this VerticaConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts a list of entities into the database in bulk - inserts rows that do not yet exist and
        /// updates rows matched by <paramref name="qualifiers"/> (or the primary/identity key when not
        /// specified). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts the rows of a <see cref="DataTable"/> into the database in bulk - inserts rows that do
        /// not yet exist and updates rows matched by <paramref name="qualifiers"/> (or the primary/identity
        /// key when not specified). Uses the <see cref="DataTable.TableName"/> property as the target
        /// table. Returns the number of affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge(this VerticaConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null) =>
            BulkMerge(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts the rows of a <see cref="DataTable"/> into the database in bulk - inserts rows that do
        /// not yet exist and updates rows matched by <paramref name="qualifiers"/> (or the primary/identity
        /// key when not specified). Returns the number of affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null) =>
            BulkMergeBase(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts rows into the database in bulk by streaming from a <see cref="IDataReader"/> - inserts
        /// rows that do not yet exist and updates rows matched by <paramref name="qualifiers"/> (or the
        /// primary/identity key when not specified). Returns the number of affected rows.
        /// </summary>
        /// <remarks>
        /// There is no <c>identityBehavior</c> parameter - a forward-only, single-pass reader cannot be
        /// rewound to retry/reconcile identity values, so returning generated identity values back onto a
        /// source row is not supported for this overload.
        /// </remarks>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="reader">The source <see cref="IDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the merge operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null) =>
            BulkMergeBase(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Upserts a list of entities into the database in bulk in an asynchronous way - inserts rows that
        /// do not yet exist and updates rows matched by <paramref name="qualifiers"/> (or the
        /// primary/identity key when not specified). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression defining the properties used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this VerticaConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Upserts a list of entities into the database in bulk in an asynchronous way - inserts rows that
        /// do not yet exist and updates rows matched by <paramref name="qualifiers"/> (or the
        /// primary/identity key when not specified). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Upserts the rows of a <see cref="DataTable"/> into the database in bulk in an asynchronous way -
        /// inserts rows that do not yet exist and updates rows matched by <paramref name="qualifiers"/> (or
        /// the primary/identity key when not specified). Uses the <see cref="DataTable.TableName"/>
        /// property as the target table. Returns the number of affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync(this VerticaConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeAsync(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Upserts the rows of a <see cref="DataTable"/> into the database in bulk in an asynchronous way -
        /// inserts rows that do not yet exist and updates rows matched by <paramref name="qualifiers"/> (or
        /// the primary/identity key when not specified). Returns the number of affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create when <paramref name="identityBehavior"/> is <see cref="VerticaBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeBaseAsync(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Upserts rows into the database in bulk in an asynchronous way by streaming from a
        /// <see cref="IDataReader"/> - inserts rows that do not yet exist and updates rows matched by
        /// <paramref name="qualifiers"/> (or the primary/identity key when not specified). Returns the
        /// number of affected rows.
        /// </summary>
        /// <remarks>
        /// There is no <c>identityBehavior</c> parameter - see the remarks on the synchronous overload.
        /// </remarks>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="reader">The source <see cref="IDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update; defaults to the primary/identity key when null.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the merge operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeBaseAsync(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
