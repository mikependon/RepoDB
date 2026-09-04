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
using Sap.Data.Hana;
using RepoDb;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SapHana;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SapHana.BulkOperations;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Writes rows into SAP HANA for the bulk operations in this package.
    /// </summary>
    public static partial class SapHanaConnectionExtension
    {
        #region WriteToServerInternal

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
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal<TEntity>(HanaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            HanaTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var dbSetting = connection.GetDbSetting();
            var resolvedMappings = ResolveMappings(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var table = BuildDataTableFromReader(connection, tableName, reader, resolvedMappings, transaction, dbSetting);
            using var bulkCopy = CreateHanaBulkCopy(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, transaction, dbSetting);
            bulkCopy.WriteToServer(table);
            return entities != null ? entities.Count() : 0;
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
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(HanaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var rows = GetDataRows(table, rowState)?.ToArray();
            var dbSetting = connection.GetDbSetting();
            using var tableReader = new DataTableReader(table);
            var resolvedMappings = ResolveMappings(connection, tableName, tableReader, mappings, null, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var typedTable = BuildDataTableFromRows(connection, tableName, rows, resolvedMappings, dbSetting);
            using var bulkCopy = CreateHanaBulkCopy(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, null, dbSetting);
            bulkCopy.WriteToServer(typedTable);
            return rows != null ? rows.Length : 0;
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
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            HanaTransaction transaction = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var dbSetting = connection.GetDbSetting();
            var resolvedMappings = ResolveMappings(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var table = BuildDataTableFromReader(connection, tableName, reader, resolvedMappings, transaction, dbSetting);
            using var bulkCopy = CreateHanaBulkCopy(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, transaction, dbSetting);
            bulkCopy.WriteToServer(table);
            return table.Rows.Count;
        }

        #endregion

        #region WriteToServerAsyncInternal

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
        /// <param name="cancellationToken"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(HanaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            HanaTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var dbSetting = connection.GetDbSetting();
            if (dbSetting is SapHanaBulkDbSetting bulkDbSetting && bulkDbSetting.WriteToServerExecution == SapHanaWriteToServerExecution.AsyncOverSync)
            {
                return WriteToServerInternal(connection, tableName, entities, mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            }
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var resolvedMappings = ResolveMappings(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var batcher = CreateSapHanaCommandBatcher(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, transaction, dbSetting);
            await batcher.WriteToServerAsync(reader, cancellationToken);
            return entities != null ? entities.Count() : 0;
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
        /// <param name="cancellationToken"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(HanaConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var dbSetting = connection.GetDbSetting();
            if (dbSetting is SapHanaBulkDbSetting bulkDbSetting && bulkDbSetting.WriteToServerExecution == SapHanaWriteToServerExecution.AsyncOverSync)
            {
                return WriteToServerInternal(connection, tableName, table, rowState, mappings, bulkCopyTimeout, batchSize, excludeField);
            }
            var rows = GetDataRows(table, rowState)?.ToArray();
            using var tableReader = new DataTableReader(table);
            var resolvedMappings = ResolveMappings(connection, tableName, tableReader, mappings, null, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var batcher = CreateSapHanaCommandBatcher(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, null, dbSetting);
            await batcher.WriteToServerAsync(rows, cancellationToken);
            return rows != null ? rows.Length : 0;
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
        /// <param name="cancellationToken"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            HanaTransaction transaction = null,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken);
            var dbSetting = connection.GetDbSetting();
            if (dbSetting is SapHanaBulkDbSetting bulkDbSetting && bulkDbSetting.WriteToServerExecution == SapHanaWriteToServerExecution.AsyncOverSync)
            {
                return WriteToServerInternal(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            }
            var resolvedMappings = ResolveMappings(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (resolvedMappings.Count == 0)
            {
                return 0;
            }
            using var batcher = CreateSapHanaCommandBatcher(connection, tableName, resolvedMappings, bulkCopyTimeout, batchSize, transaction, dbSetting);
            return await batcher.WriteToServerAsync(reader, cancellationToken);
        }

        #endregion

        #region Core

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static List<SapHanaBulkInsertMapItem> ResolveMappings(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings,
            HanaTransaction transaction,
            Field excludeField,
            IDbSetting dbSetting)
        {
            var columnMappings = mappings?.AsList()
                ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, excludeField).AsList();

            if (columnMappings.Count == 0 || mappings == null)
            {
                return columnMappings;
            }

            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            foreach (var mapping in columnMappings)
            {
                var sourceOrdinal = reader.GetOrdinal(mapping.SourceColumn);
                var destinationField = dbFields.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                var sourceType = reader.GetFieldType(sourceOrdinal);
                if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                {
                    throw new InvalidTypeException($"The type of the source column '{mapping.SourceColumn}' ({sourceType}) does not match the type of the destination column '{mapping.DestinationColumn}' ({destinationField.Type}).");
                }
            }

            return columnMappings;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static HanaBulkCopy CreateHanaBulkCopy(HanaConnection connection,
            string tableName,
            List<SapHanaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            HanaTransaction transaction,
            IDbSetting dbSetting)
        {
            var bulkCopy = new HanaBulkCopy(connection, HanaBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            foreach (var mapping in mappings)
            {
                bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
            }
            return bulkCopy;
        }

        /// <summary>
        /// Builds a <see cref="SapHanaCommandBatcher"/> from already-resolved mappings, mirroring
        /// <see cref="CreateHanaBulkCopy"/>. Used only by the async WriteToServerAsyncInternal overloads -
        /// see the remarks on <see cref="SapHanaCommandBatcher"/> for why they use this instead of
        /// <see cref="HanaBulkCopy"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static SapHanaCommandBatcher CreateSapHanaCommandBatcher(HanaConnection connection,
            string tableName,
            List<SapHanaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            HanaTransaction transaction,
            IDbSetting dbSetting)
        {
            var batcher = new SapHanaCommandBatcher(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting),
                TableName = tableName,
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
            foreach (var mapping in mappings)
            {
                batcher.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
            }
            return batcher;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="transaction"></param>
        /// <param name="dbSetting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static DataTable BuildDataTableFromReader(HanaConnection connection,
            string tableName,
            IDataReader reader,
            List<SapHanaBulkInsertMapItem> mappings,
            HanaTransaction transaction,
            IDbSetting dbSetting,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var table = new DataTable();
            var ordinals = new int[mappings.Count];
            var columnTypes = new Type[mappings.Count];

            for (var i = 0; i < mappings.Count; i++)
            {
                ordinals[i] = reader.GetOrdinal(mappings[i].SourceColumn);
                var destinationField = dbFields.GetByUnquotedName(mappings[i].DestinationColumn.AsUnquoted(true, dbSetting));
                var fieldType = destinationField?.Type ?? reader.GetFieldType(ordinals[i]) ?? typeof(object);
                columnTypes[i] = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
                table.Columns.Add(mappings[i].SourceColumn, columnTypes[i]);
            }

            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var row = table.NewRow();
                for (var i = 0; i < mappings.Count; i++)
                {
                    row[i] = ConvertValueForColumn(reader.GetValue(ordinals[i]), columnTypes[i]);
                }
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static DataTable BuildDataTableFromRows(HanaConnection connection,
            string tableName,
            DataRow[] rows,
            List<SapHanaBulkInsertMapItem> mappings,
            IDbSetting dbSetting)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, null);
            var table = new DataTable();
            var columnTypes = new Type[mappings.Count];

            for (var i = 0; i < mappings.Count; i++)
            {
                var destinationField = dbFields.GetByUnquotedName(mappings[i].DestinationColumn.AsUnquoted(true, dbSetting));
                var sourceColumnType = rows?.Length > 0 ? rows[0].Table.Columns[mappings[i].SourceColumn]?.DataType : null;
                var fieldType = destinationField?.Type ?? sourceColumnType ?? typeof(object);
                columnTypes[i] = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
                table.Columns.Add(mappings[i].SourceColumn, columnTypes[i]);
            }

            if (rows != null)
            {
                foreach (var sourceRow in rows)
                {
                    var row = table.NewRow();
                    for (var i = 0; i < mappings.Count; i++)
                    {
                        row[i] = ConvertValueForColumn(sourceRow[mappings[i].SourceColumn], columnTypes[i]);
                    }
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static SapHanaBulkImportPseudoTableType ResolvePseudoTableType(SapHanaBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == SapHanaBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= SapHanaConstants.RowCountThresholdForPhysicalTable ?
                SapHanaBulkImportPseudoTableType.Physical :
                    SapHanaBulkImportPseudoTableType.Physical;

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
            if (rowState.HasValue == true)
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
        ///
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        private static bool AreMappingTypesCompatible(Type sourceType,
            Type destinationType)
        {
            sourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            destinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

            if (sourceType == destinationType)
            {
                return true;
            }
            if ((sourceType == typeof(Guid) && destinationType == typeof(string)) ||
                (sourceType == typeof(string) && destinationType == typeof(Guid)))
            {
                return true;
            }

            return IsIntegral(sourceType) && IsIntegral(destinationType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsIntegral(Type type)
        {
            var code = Type.GetTypeCode(type);
            return code is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static object ConvertValueForColumn(object value,
            Type columnType)
        {
            if (value == null || value is DBNull)
            {
                return DBNull.Value;
            }
            if (columnType.IsInstanceOfType(value))
            {
                return value;
            }
            if (value is Guid guidValue && columnType == typeof(string))
            {
                return guidValue.ToString();
            }
            if (value is string stringValue && columnType == typeof(Guid))
            {
                return Guid.Parse(stringValue);
            }
            if (IsIntegral(value.GetType()) && IsIntegral(columnType))
            {
                return Convert.ChangeType(value, columnType);
            }
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        private static IEnumerable<SapHanaBulkInsertMapItem> GetDefaultMappingsForDataReader(HanaConnection connection,
            string tableName,
            IDataReader reader,
            HanaTransaction transaction,
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
                    yield return new SapHanaBulkInsertMapItem(columnName, dbField.Name);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        /// <exception cref="PrimaryFieldNotFoundException"></exception>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers = null)
        {
            if (qualifiers?.Any() == true)
            {
                return qualifiers;
            }

            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return new[] { primaryOrIdentity.AsField() };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static DbCommand CreateTraceCommand(HanaConnection connection,
            string commandText,
            int? commandTimeout = null,
            HanaTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
