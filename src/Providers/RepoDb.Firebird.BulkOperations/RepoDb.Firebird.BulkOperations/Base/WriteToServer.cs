#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Options;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FirebirdConnectionExtension
    {
        internal const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        #region WriteToServerInternal

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
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(FbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FbTransaction transaction = null)
        {
            connection.EnsureOpen();
            var batcher = CreateFirebirdCommandBatcher(connection, tableName, mappings, bulkCopyTimeout, batchSize, transaction);
            return batcher.WriteToServer(table, rowState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FbTransaction transaction = null)
        {
            connection.EnsureOpen();
            var batcher = CreateFirebirdCommandBatcher(connection, tableName, mappings, bulkCopyTimeout, batchSize, transaction);
            return batcher.WriteToServer(reader);
        }

        #endregion

        #region WriteToServerAsyncInternal

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
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(FbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var batcher = CreateFirebirdCommandBatcher(connection, tableName, mappings, bulkCopyTimeout, batchSize, transaction);
            return await batcher.WriteToServerAsync(table, rowState, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var batcher = CreateFirebirdCommandBatcher(connection, tableName, mappings, bulkCopyTimeout, batchSize, transaction);
            return await batcher.WriteToServerAsync(reader, cancellationToken);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static FirebirdCommandBatcher CreateFirebirdCommandBatcher(FbConnection connection,
            string tableName,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            FbTransaction transaction)
        {
            var batcher = new FirebirdCommandBatcher(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, connection.GetDbSetting()),
                Transaction = transaction
            };
            if (bulkCopyTimeout.HasValue)
            {
                batcher.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                batcher.BatchSize = batchSize.Value;
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    batcher.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
                }
            }
            return batcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static FirebirdBulkImportPseudoTableType ResolvePseudoTableType(FirebirdBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == FirebirdBulkImportPseudoTableType.Auto
                ? (rowCount.GetValueOrDefault() >= FirebirdConstants.RowCountThresholdForPhysicalTable
                    ? FirebirdBulkImportPseudoTableType.Physical
                    : FirebirdBulkImportPseudoTableType.Memory)
                : pseudoTableType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        /// <exception cref="PrimaryFieldNotFoundException"></exception>
        private static Field GetPrimaryOrIdentityQualifier(string tableName,
            DbFieldCollection dbFields)
        {
            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return primaryOrIdentity.AsField();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers = null) =>
            qualifiers?.Any() == true ? qualifiers : new[] { GetPrimaryOrIdentityQualifier(tableName, dbFields) };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldsException"></exception>
        private static IEnumerable<Field> GetInsertFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings,
            Field identityField)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (identityField != null)
            {
                fields = fields?.Where(field => !string.Equals(field.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

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
            IEnumerable<FirebirdCommandBatcherMapItem> mappings,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static bool HasUpdateableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers) =>
            fields.Any(field => qualifiers.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static IEnumerable<DataRow> GetDataRows(DataTable dataTable,
            DataRowState? rowState = null)
        {
            var rows = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue)
            {
                rows = rows.Where(r => r.RowState == rowState);
            }
            if (!rows.Any())
            {
                throw new InvalidOperationException($"No rows found from data table where the state is '{rowState}'.");
            }
            foreach (var row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// Adds a client-assigned <see cref="RowOrderColumnName"/> column (BIGINT, 0-based) onto a copy of
        /// <paramref name="rows"/>, in array order. Only needed ahead of staging into a pseudo table whose
        /// finishing statement reads generated identities back per row (see <c>FirebirdText</c>) - the value
        /// is assigned here, client-side, rather than relying on a server-generated column, so correctness
        /// never depends on a bulk-write mechanism preserving input order.
        /// </summary>
        private static DataTable AddRowOrderColumn(DataTable source,
            IList<DataRow> rows)
        {
            var table = source.Clone();
            table.Columns.Add(RowOrderColumnName, typeof(long));

            for (var i = 0; i < rows.Count; i++)
            {
                var newRow = table.NewRow();
                for (var c = 0; c < source.Columns.Count; c++)
                {
                    newRow[c] = rows[i][c];
                }
                newRow[RowOrderColumnName] = (long)i;
                table.Rows.Add(newRow);
            }

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private static IEnumerable<FirebirdCommandBatcherMapItem> WithRowOrderMapping(IEnumerable<FirebirdCommandBatcherMapItem> mappings) =>
            mappings == null ? null : mappings.Append(new FirebirdCommandBatcherMapItem(RowOrderColumnName, RowOrderColumnName));

        /// <summary>
        ///
        /// </summary>
        private static DataTable BuildEntityDataTable<TEntity>(IList<TEntity> entities,
            IList<Field> fields = null,
            bool includeRowOrder = false)
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
            var table = new DataTable();
            foreach (var column in columns)
            {
                table.Columns.Add(column.Field.Name, typeof(object));
            }
            if (includeRowOrder)
            {
                table.Columns.Add(RowOrderColumnName, typeof(long));
            }

            for (var i = 0; i < entities.Count; i++)
            {
                var row = table.NewRow();
                var entity = entities[i];
                for (var c = 0; c < columns.Count; c++)
                {
                    row[c] = GetValueForWrite(entity, columns[c].Field, columns[c].Property, isDictionaryStringObject);
                }
                if (includeRowOrder)
                {
                    row[RowOrderColumnName] = (long)i;
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
        private static object GetValueForWrite(object entity,
            Field field,
            ClassProperty property,
            bool isDictionaryStringObject)
        {
            object rawValue;
            if (isDictionaryStringObject)
            {
                var dictionary = entity as IDictionary<string, object>;
                if (dictionary == null || !dictionary.TryGetValue(field.Name, out rawValue))
                {
                    var key = dictionary?.Keys.FirstOrDefault(k => string.Equals(k, field.Name, StringComparison.OrdinalIgnoreCase));
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
            // typeof(object), not qualifierField.Type - same reasoning as BuildEntityDataTable above.
            var table = new DataTable();
            table.Columns.Add(qualifierField.Name, typeof(object));

            foreach (var keyValue in keyValues)
            {
                table.Rows.Add(keyValue ?? DBNull.Value);
            }

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        private static IEnumerable<FirebirdCommandBatcherMapItem> GetDefaultMappingsForDataTable(DataTable table,
            Field excludeField = null) =>
            table.Columns
                .OfType<DataColumn>()
                .Where(column => excludeField == null || !string.Equals(column.ColumnName, excludeField.Name, StringComparison.OrdinalIgnoreCase))
                .Select(column => new FirebirdCommandBatcherMapItem(column.ColumnName, column.ColumnName));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        private static IEnumerable<FirebirdCommandBatcherMapItem> GetDefaultMappingsForDataReader(FbConnection connection,
            string tableName,
            IDataReader reader,
            FbTransaction transaction,
            Field excludeField = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbField = dbFields.GetByUnquotedName(columnName.AsUnquoted(true, dbSetting));
                if (dbField != null && !string.Equals(dbField.Name, excludeField?.Name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new FirebirdCommandBatcherMapItem(columnName, dbField.Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(FbConnection connection,
            string commandText,
            int? commandTimeout = null,
            FbTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
