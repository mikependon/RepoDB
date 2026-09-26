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
using RepoDb.Connector.CockroachDb;
using RepoDb.Connector.CockroachDb.Bulk;
using RepoDb;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.CockroachDb.BulkOperations;
using RepoDb.CockroachDb.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class CockroachDbConnectionExtension
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
        internal static int WriteToServerInternal<TEntity>(CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, reader, transaction);
            return bulkCopy.WriteToServer(coercingReader);
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
        internal static int WriteToServerInternal(CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, excludeField);
            var rows = GetDataRows(table, rowState)?.ToArray();
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, rows);
            return bulkCopy.WriteToServer(coercingReader);
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
        internal static int WriteToServerInternal(CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbTransaction transaction = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, reader, transaction);
            return bulkCopy.WriteToServer(coercingReader);
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
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(CockroachDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            CockroachDbTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new DataEntityDataReader<TEntity>(entities);
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, reader, transaction);
            return await bulkCopy.WriteToServerAsync(coercingReader, cancellationToken).ConfigureAwait(false);
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
        internal static async Task<int> WriteToServerAsyncInternal(CockroachDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            var bulkCopy = CreateBulkCopyForDataTable(connection, tableName, table, mappings, bulkCopyTimeout, excludeField);
            var rows = GetDataRows(table, rowState)?.ToArray();
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, rows);
            return await bulkCopy.WriteToServerAsync(coercingReader, cancellationToken).ConfigureAwait(false);
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
        internal static async Task<int> WriteToServerAsyncInternal(CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            CockroachDbTransaction transaction = null,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            var bulkCopy = CreateBulkCopyForDataReader(connection, tableName, reader, mappings, bulkCopyTimeout, transaction, excludeField);
            using var coercingReader = CockroachDbTypeCoercingDataReader.Create(connection, tableName, bulkCopy, reader, transaction);
            return await bulkCopy.WriteToServerAsync(coercingReader, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static CockroachDbBulkImportPseudoTableType ResolvePseudoTableType(CockroachDbBulkImportPseudoTableType pseudoTableType,
            int? rowCount)
        {
            if (pseudoTableType != CockroachDbBulkImportPseudoTableType.Auto)
            {
                return pseudoTableType;
            }

            return rowCount.GetValueOrDefault() >= CockroachDbConstants.RowCountThresholdForPhysicalTable
                ? CockroachDbBulkImportPseudoTableType.Physical
                : CockroachDbBulkImportPseudoTableType.Memory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
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
                throw new System.InvalidOperationException($"No rows found from data table where the state is '{rowState.ToString()}'.");
            }
            foreach (var row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static CockroachDbBulkCopy CreateBulkCopyForDataTable(CockroachDbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            Field excludeField = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new CockroachDbBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (mappings != null)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, null);
                var columnMappings = mappings.AsList();
                foreach (var mapping in columnMappings)
                {
                    var sourceOrdinal = table.Columns.IndexOf(mapping.SourceColumn);
                    if (sourceOrdinal < 0)
                    {
                        throw new InvalidTypeException($"The source column '{mapping.SourceColumn}' defined in the mappings was not found in the given data table.");
                    }
                    var destinationField = dbFields?.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                    var sourceType = table.Columns[sourceOrdinal].DataType;
                    if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{mapping.SourceColumn}' ({sourceType}) does not match the type of the destination column '{mapping.DestinationColumn}' ({destinationField.Type}).");
                    }
                    bulkCopy.ColumnMappings.Add(
                        new CockroachDbBulkColumnMapping(sourceOrdinal, mapping.DestinationColumn));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (excludeField != null && string.Equals(column.ColumnName, excludeField.Name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    bulkCopy.ColumnMappings.Add(
                        new CockroachDbBulkColumnMapping(table.Columns.IndexOf(column), column.ColumnName));
                }
            }
            return bulkCopy;
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

            static bool IsIntegral(Type type)
            {
                var code = Type.GetTypeCode(type);
                return code is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                    or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
            }

            return IsIntegral(sourceType) && IsIntegral(destinationType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static CockroachDbBulkCopy CreateBulkCopyForDataReader(CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<CockroachDbBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            CockroachDbTransaction transaction,
            Field excludeField = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new CockroachDbBulkCopy(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            var columnMappings = mappings?.AsList() ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, excludeField).AsList();
            var dbFields = mappings != null ? DbFieldCache.Get(connection, tableName, transaction) : null;
            foreach (var mapping in columnMappings)
            {
                var sourceOrdinal = reader.GetOrdinal(mapping.SourceColumn);
                if (dbFields != null)
                {
                    var destinationField = dbFields.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                    var sourceType = reader.GetFieldType(sourceOrdinal);
                    if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{mapping.SourceColumn}' ({sourceType}) does not match the type of the destination column '{mapping.DestinationColumn}' ({destinationField.Type}).");
                    }
                }
                bulkCopy.ColumnMappings.Add(new CockroachDbBulkColumnMapping(sourceOrdinal, mapping.DestinationColumn));
            }

            return bulkCopy;
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
        private static IEnumerable<CockroachDbBulkInsertMapItem> GetDefaultMappingsForDataReader(CockroachDbConnection connection,
            string tableName,
            IDataReader reader,
            CockroachDbTransaction transaction,
            Field excludeField = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbField = dbFields.GetByUnquotedName(columnName.AsUnquoted(true, dbSetting));
                if (dbField != null && !string.Equals(dbField.Name, excludeField?.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    yield return new CockroachDbBulkInsertMapItem(columnName, dbField.Name);
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
        private static DbCommand CreateTraceCommand(CockroachDbConnection connection,
            string commandText,
            int? commandTimeout = null,
            CockroachDbTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
