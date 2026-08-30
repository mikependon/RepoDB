#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Extensions;
using RepoDb.Vertica.BulkOperations;
using RepoDb.Vertica.BulkOperations.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class VerticaConnectionExtension
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
        private static int BulkInsertBase<TEntity>(this VerticaConnection connection,
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
            var entityList = entities.AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            return returnIdentity
                ? connection.BulkInsertBaseForReturnIdentity(tableName, entityList, mappings, bulkCopyTimeout, batchSize,
                    ResolvePseudoTableType(pseudoTableType, entityList?.Count), trace, traceKey, transaction)
                : connection.BulkInsertBaseNoReturnIdentity(tableName, entityList, mappings, bulkCopyTimeout, batchSize, trace, traceKey, transaction);
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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity<TEntity>(this VerticaConnection connection,
            string tableName,
            IList<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction)
            where TEntity : class
        {
            var pseudoTableName = VerticaText.CreatePseudoTableName("I");

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();

                VerticaExecution.CreatePseudoTable(connection, pseudoTableName, insertFields, dbFields, pseudoTableType, trace, traceKey, transaction);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : insertFields;
                using var entityTable = BuildEntityDataTable(entities, entityFields, includeRowOrder: true);
                WriteToServerInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = VerticaExecution.InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, insertFields, identityField, entities, trace, traceKey, transaction);
            }
            finally
            {
                VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
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
        private static int BulkInsertBaseNoReturnIdentity<TEntity>(this VerticaConnection connection,
            string tableName,
            IList<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity()?.AsField();
            var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();
            var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : insertFields;
            using var entityTable = BuildEntityDataTable(entities, entityFields);
            var writeMappings = mappings ?? insertFields.Select(f => new VerticaBulkInsertMapItem(f.Name, f.Name)).AsList();
            var result = WriteToServerInternal(connection, tableName, entityTable, mappings: writeMappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

            Tracer.InvokeAfterExecution(traceResult, trace, result);
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
        private static int BulkInsertBase(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            return returnIdentity
                ? connection.BulkInsertBaseForReturnIdentity(tableName, table, rowState, mappings, bulkCopyTimeout, batchSize,
                    ResolvePseudoTableType(pseudoTableType, table?.Rows.Count), trace, traceKey, transaction)
                : connection.BulkInsertBaseNoReturnIdentity(tableName, table, rowState, mappings, bulkCopyTimeout, batchSize, trace, traceKey, transaction);
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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var pseudoTableName = VerticaText.CreatePseudoTableName("I");

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();

                VerticaExecution.CreatePseudoTable(connection, pseudoTableName, insertFields, dbFields, pseudoTableType, trace, traceKey, transaction);

                using var orderedTable = AddRowOrderColumn(table, rows);
                var writeMappings = mappings != null ? WithRowOrderMapping(mappings) : GetDefaultMappingsForDataTable(orderedTable, identityField).AsList();
                WriteToServerInternal(connection, pseudoTableName, orderedTable, mappings: writeMappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = VerticaExecution.InsertFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction);
            }
            finally
            {
                VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
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
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var writeMappings = mappings ?? GetDefaultMappingsForDataTable(table, identityField).AsList();
            var result = WriteToServerInternal(connection, tableName, table, rowState, writeMappings, bulkCopyTimeout, batchSize, transaction);

            Tracer.InvokeAfterExecution(traceResult, trace, result);
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
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection, tableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, identityField).AsList(), bulkCopyTimeout, batchSize, transaction);

            Tracer.InvokeAfterExecution(traceResult, trace, result);
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
        private static async Task<int> BulkInsertBaseAsync<TEntity>(this VerticaConnection connection,
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
            var entityList = entities.AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            return returnIdentity
                ? await connection.BulkInsertBaseForReturnIdentityAsync(tableName, entityList, mappings, bulkCopyTimeout, batchSize,
                    ResolvePseudoTableType(pseudoTableType, entityList?.Count), trace, traceKey, transaction, cancellationToken)
                : await connection.BulkInsertBaseNoReturnIdentityAsync(tableName, entityList, mappings, bulkCopyTimeout, batchSize, trace, traceKey, transaction, cancellationToken);
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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync<TEntity>(this VerticaConnection connection,
            string tableName,
            IList<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var pseudoTableName = VerticaText.CreatePseudoTableName("I");

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
                var identityField = dbFields.GetIdentity().AsField();
                var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();

                await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, insertFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : insertFields;
                using var entityTable = BuildEntityDataTable(entities, entityFields, includeRowOrder: true);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = await VerticaExecution.InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, insertFields, identityField, entities, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
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
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync<TEntity>(this VerticaConnection connection,
            string tableName,
            IList<TEntity> entities,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity()?.AsField();
            var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();
            var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : insertFields;
            using var entityTable = BuildEntityDataTable(entities, entityFields);
            var writeMappings = mappings ?? insertFields.Select(f => new VerticaBulkInsertMapItem(f.Name, f.Name)).AsList();
            var result = await WriteToServerAsyncInternal(connection, tableName, entityTable, mappings: writeMappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
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
        private static async Task<int> BulkInsertBaseAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            return returnIdentity
                ? await connection.BulkInsertBaseForReturnIdentityAsync(tableName, table, rowState, mappings, bulkCopyTimeout, batchSize,
                    ResolvePseudoTableType(pseudoTableType, table?.Rows.Count), trace, traceKey, transaction, cancellationToken)
                : await connection.BulkInsertBaseNoReturnIdentityAsync(tableName, table, rowState, mappings, bulkCopyTimeout, batchSize, trace, traceKey, transaction, cancellationToken);
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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            VerticaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction,
            CancellationToken cancellationToken)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var pseudoTableName = VerticaText.CreatePseudoTableName("I");

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
                var identityField = dbFields.GetIdentity().AsField();
                var insertFields = GetInsertFields(tableName, dbFields, mappings, identityField).AsList();

                await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, insertFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);

                using var orderedTable = AddRowOrderColumn(table, rows);
                var writeMappings = mappings != null ? WithRowOrderMapping(mappings) : GetDefaultMappingsForDataTable(orderedTable, identityField).AsList();
                await WriteToServerAsyncInternal(connection, pseudoTableName, orderedTable, mappings: writeMappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = await VerticaExecution.InsertFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
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
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState,
            IEnumerable<VerticaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            ITrace trace,
            string traceKey,
            VerticaTransaction transaction,
            CancellationToken cancellationToken)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetIdentity()?.AsField();
            var writeMappings = mappings ?? GetDefaultMappingsForDataTable(table, identityField).AsList();
            var result = await WriteToServerAsyncInternal(connection, tableName, table, rowState, writeMappings, bulkCopyTimeout, batchSize, transaction, cancellationToken);

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
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
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkInsert,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection, tableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, identityField).AsList(), bulkCopyTimeout, batchSize, transaction, cancellationToken);

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #endregion
    }
}
