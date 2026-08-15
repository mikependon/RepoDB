using IBM.Data.Db2;
using RepoDb.Enumerations.Db2;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.Db2.BulkOperations;
using RepoDb.Db2.BulkOperations.Extensions;
using RepoDb.Db2.PropertyHandlers;
using System;
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
    public static partial class Db2ConnectionExtension
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
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
                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                if (qualifierFieldsForPseudoTable != null)
                {
                    using var qualifierTable = BuildEntityDataTable(entityList, qualifierFieldsForPseudoTable);
                    WriteToServerInternal(connection, pseudoTableName, qualifierTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
                }
                else
                {
                    WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
                }

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = Db2Execution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
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
                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = Db2Execution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBase(this DB2Connection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
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
                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = Db2Execution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);

                if (qualifierFieldsForPseudoTable != null)
                {
                    using var qualifierTable = BuildEntityDataTable(entityList, qualifierFieldsForPseudoTable);
                    await WriteToServerAsyncInternal(connection, pseudoTableName, qualifierTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken: cancellationToken);
                }
                else
                {
                    await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);
                }

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await Db2Execution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await Db2Execution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseAsync(this DB2Connection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkDelete,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = Db2Text.GetPseudoTableNameForDelete(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFieldsForPseudoTable = TryGetQualifierFieldsForPseudoTable(tableName, dbFields, qualifiers);
            var mappings = ToPseudoTableMappings(qualifierFieldsForPseudoTable);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFieldsForPseudoTable, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await Db2Execution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        private static IList<Db2BulkInsertMapItem> ToPseudoTableMappings(IList<Field> qualifierFields) =>
            qualifierFields?.Select(f => new Db2BulkInsertMapItem(f.Name, f.Name)).AsList();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private static DataTable BuildEntityDataTable<TEntity>(IList<TEntity> entities,
            IList<Field> fields = null)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities could not be null.");
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

            return rawValue switch
            {
                Guid guid => new Db2GuidToByteArrayPropertyHandler().Set(guid, null),
                byte b => new Db2ByteToInt16PropertyHandler().Set(b, null),
                _ => rawValue
            };
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
