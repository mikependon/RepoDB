#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Enumerations.EnterpriseDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.EnterpriseDb.BulkOperations;
using RepoDb.EnterpriseDb.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class EDBConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == EDBBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    entities,
                    qualifiers,
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
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
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
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseForReturnIdentity<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = EDBExecution.MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, entityList, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseNoReturnIdentity<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = EDBExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
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
        private static int BulkMergeBase(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == EDBBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    table,
                    qualifiers,
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
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
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
        private static int BulkMergeBaseForReturnIdentity(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = EDBExecution.MergeFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, rows, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseNoReturnIdentity(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = EDBExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this EDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = EDBExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeBaseAsync<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == EDBBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await EDBExecution.MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, entityList, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            IEnumerable<EDBBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            EDBBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            EDBTransaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await EDBExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeBaseAsync(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == EDBBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportIdentityBehavior identityBehavior = default,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await EDBExecution.MergeFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, rows, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await EDBExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this EDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkMerge,
            EDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = EDBText.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await EDBExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

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
        /// <param name="qualifierFields"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldsException"></exception>
        private static IEnumerable<Field> GetMergeFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<EDBBulkInsertMapItem> mappings,
            IEnumerable<Field> qualifierFields)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)) ||
                    qualifierFields.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)));
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
