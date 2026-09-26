#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.CockroachDb.BulkOperations;
using RepoDb.CockroachDb.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class CockroachDbConnectionExtension
    {
        #region Sync

        #region BulkInsertBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == CockroachDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkInsertBaseForReturnIdentity(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkInsertBaseNoReturnIdentity(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = CockroachDbExecution.InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                CockroachDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                transaction,
                identityField);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == CockroachDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkInsertBaseForReturnIdentity(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkInsertBaseNoReturnIdentity(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        /// <see cref="DataTable"/> counterpart of <see cref="BulkInsertBaseForReturnIdentity{TEntity}"/> - see
        /// its remarks for the detailed behavior (identical here), except the generated identity values are
        /// assigned back onto the matching row's identity column of <paramref name="table"/> instead of an
        /// entity property.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = CockroachDbExecution.InsertFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                CockroachDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// Bulk-writes the rows of <paramref name="table"/> directly into <paramref name="tableName"/> -
        /// see <see cref="BulkInsertBaseNoReturnIdentity{TEntity}"/> for the detailed remarks (identical
        /// here). This is the "actual base execution" that the <see cref="Tracer"/> Before/After pair wraps.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                identityField);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase(this CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize,
                transaction,
                identityField);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkInsertBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == CockroachDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkInsertBaseForReturnIdentityAsync(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await connection.BulkInsertBaseNoReturnIdentityAsync(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await CockroachDbExecution.InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await CockroachDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                transaction,
                identityField).ConfigureAwait(false);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == CockroachDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkInsertBaseForReturnIdentityAsync(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await connection.BulkInsertBaseNoReturnIdentityAsync(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportIdentityBehavior identityBehavior = default,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await CockroachDbExecution.InsertFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await CockroachDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                identityField).ConfigureAwait(false);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync(this CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkInsert,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                transaction,
                identityField).ConfigureAwait(false);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldsException"></exception>
        private static IEnumerable<Field> GetInsertFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

        #endregion
    }
}
