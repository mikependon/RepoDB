#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.CockroachDb.BulkOperations;
using RepoDb.CockroachDb.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
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

        #region BulkDeleteBase<TEntity>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                CockroachDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, null, bulkCopyTimeout, batchSize);

                // Execute and return
                result = CockroachDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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

        #endregion

        #region BulkDeleteBase<DataTable>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                CockroachDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, null, bulkCopyTimeout, batchSize);

                // Execute and return
                result = CockroachDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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

        #endregion

        #region BulkDeleteBase<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                CockroachDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, null, bulkCopyTimeout, batchSize);

                // Execute and return
                result = CockroachDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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

        #endregion

        #endregion

        #region Async

        #region BulkDeleteBaseAsync<TEntity>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, null, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                result = await CockroachDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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

        #endregion

        #region BulkDeleteBaseAsync<DataTable>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this CockroachDbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, null, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                result = await CockroachDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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

        #endregion

        #region BulkDeleteBaseAsync<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDelete,
            CockroachDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = CockroachDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();

                await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, null, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                result = await CockroachDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifierField"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        private static DataTable CreateKeyValuesDataTable(Field qualifierField,
            IEnumerable<object> keyValues)
        {
            var table = new DataTable();
            table.Columns.Add(qualifierField.Name, qualifierField.Type ?? typeof(object));

            foreach (var keyValue in keyValues)
            {
                table.Rows.Add(keyValue ?? DBNull.Value);
            }

            return table;
        }

        #endregion
    }
}
