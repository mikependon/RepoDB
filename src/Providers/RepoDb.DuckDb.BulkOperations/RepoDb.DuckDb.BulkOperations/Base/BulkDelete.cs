#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using RepoDb.Enumerations.DuckDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.DuckDb.BulkOperations;
using RepoDb.DuckDb.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class DuckDbConnectionExtension
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
        private static int BulkDeleteBase<TEntity>(this DuckDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                DuckDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);

                if (qualifierFieldsForPseudoTable != null)
                {
                    using var qualifierTable = BuildEntityDataTable(entityList, qualifierFieldsForPseudoTable);
                    WriteToServerInternal(connection, pseudoTableName, qualifierTable, null, mappings, bulkCopyTimeout, batchSize);
                }
                else
                {
                    WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);
                }

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = DuckDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                DuckDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <exception cref="ArgumentNullException"></exception>
        private static int BulkDeleteBase(this DuckDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                DuckDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = DuckDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                DuckDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static int BulkDeleteBase(this DuckDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                DuckDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = DuckDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                DuckDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this DuckDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                await DuckDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                if (qualifierFieldsForPseudoTable != null)
                {
                    using var qualifierTable = BuildEntityDataTable(entityList, qualifierFieldsForPseudoTable);
                    await WriteToServerAsyncInternal(connection, pseudoTableName, qualifierTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);
                }

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await DuckDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await DuckDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="ArgumentNullException"></exception>
        private static async Task<int> BulkDeleteBaseAsync(this DuckDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                await DuckDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await DuckDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await DuckDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
        private static async Task<int> BulkDeleteBaseAsync(this DuckDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = DuckDbTraceKeys.DuckDbBulkDelete,
            DuckDBTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = DuckDbText.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process
                await DuckDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await DuckDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await DuckDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static IList<Field> TryGetQualifierFieldsForPseudoTable(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers)
        {
            return GetQualifierFields(tableName, dbFields, qualifiers).AsList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifierFields"></param>
        /// <returns></returns>
        private static IList<DuckDbBulkInsertMapItem> ToPseudoTableMappings(IList<Field> qualifierFields) =>
            qualifierFields?.Select(f => new DuckDbBulkInsertMapItem(f.Name, f.Name)).AsList();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static DataTable BuildEntityDataTable<TEntity>(IList<TEntity> entities,
            IList<Field> fields = null)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "The entities could not be null.");
            }

            var entityType = typeof(TEntity) == typeof(object)
                ? (entities.FirstOrDefault()?.GetType() ?? typeof(TEntity))
                : typeof(TEntity);
            var isDictionaryStringObject = TypeCache.Get(entityType).IsDictionaryStringObject();
            fields ??= GetEntityFieldsForWrite(entityType, entities, isDictionaryStringObject);

            var columns = fields
                .Select(f => (
                    Field: f,
                    Property: isDictionaryStringObject ? null : PropertyCache.Get(entityType, f, includeMappings: true)))
                .AsList();

            var rows = entities
                .Select(entity => columns
                    .Select(column => GetQualifierValueForWrite(entity, column.Field, column.Property, isDictionaryStringObject))
                    .AsList())
                .AsList();

            var table = new DataTable();
            for (var i = 0; i < columns.Count; i++)
            {
                var columnType = rows
                    .Select(row => row[i])
                    .FirstOrDefault(value => value != null && value != DBNull.Value)?
                    .GetType() ?? typeof(object);
                table.Columns.Add(columns[i].Field.Name, columnType);
            }

            foreach (var rowValues in rows)
            {
                var row = table.NewRow();
                for (var i = 0; i < columns.Count; i++)
                {
                    row[i] = rowValues[i];
                }
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entities"></param>
        /// <param name="isDictionaryStringObject"></param>
        /// <returns></returns>
        private static IList<Field> GetEntityFieldsForWrite(Type entityType,
            IEnumerable<object> entities,
            bool isDictionaryStringObject)
        {
            if (isDictionaryStringObject)
            {
                var dictionary = entities?.FirstOrDefault() as IDictionary<string, object>;
                return dictionary?.Keys.Select(k => new Field(k)).AsList() ?? new List<Field>();
            }

            return PropertyCache.Get(entityType)?.Select(p => new Field(p.GetMappedName())).AsList() ?? new List<Field>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="field"></param>
        /// <param name="property"></param>
        /// <param name="isDictionaryStringObject"></param>
        /// <returns></returns>
        private static object GetQualifierValueForWrite(object entity,
            Field field,
            ClassProperty property,
            bool isDictionaryStringObject)
        {
            object rawValue = null;
            if (isDictionaryStringObject)
            {
                var dictionary = entity as IDictionary<string, object>;
                if (dictionary != null && !dictionary.TryGetValue(field.Name, out rawValue))
                {
                    var key = dictionary.Keys.FirstOrDefault(k => string.Equals(k, field.Name, StringComparison.OrdinalIgnoreCase));
                    rawValue = key != null ? dictionary[key] : null;
                }
            }
            else
            {
                rawValue = property?.PropertyInfo.GetValue(entity);
            }

            if (rawValue == null)
            {
                return DBNull.Value;
            }

            var handler = property?.GetPropertyHandler();
            if (handler != null)
            {
                var options = PropertyHandlerSetOptions.Create(null, property);
                return ((dynamic)handler).Set((dynamic)rawValue, options) ?? (object)DBNull.Value;
            }

            return rawValue;
        }

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
