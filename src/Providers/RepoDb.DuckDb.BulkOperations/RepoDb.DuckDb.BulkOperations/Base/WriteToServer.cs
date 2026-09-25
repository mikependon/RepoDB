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
using DuckDB.NET.Data;
using RepoDb.DuckDb.BulkOperations;
using RepoDb.Enumerations.DuckDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// Contains the bulk operation extension methods for <see cref="DuckDBConnection"/>.
    /// </summary>
    public static partial class DuckDbConnectionExtension
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
        internal static int WriteToServerInternal<TEntity>(DuckDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDBTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(reader), mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            return appender.WriteToServer(reader);
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
        internal static int WriteToServerInternal(DuckDBConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(table), mappings, bulkCopyTimeout, batchSize, null, excludeField);
            return appender.WriteToServer(table, rowState);
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
        internal static int WriteToServerInternal(DuckDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            DuckDBTransaction transaction = null,
            Field excludeField = null)
        {
            connection.EnsureOpen();
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(reader), mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            return appender.WriteToServer(reader);
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
        internal static async Task<int> WriteToServerAsyncInternal<TEntity>(DuckDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            DuckDBTransaction transaction = null,
            Field excludeField = null)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new DataEntityDataReader<TEntity>(entities);
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(reader), mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            return await appender.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
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
        internal static async Task<int> WriteToServerAsyncInternal(DuckDBConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(table), mappings, bulkCopyTimeout, batchSize, null, excludeField);
            return await appender.WriteToServerAsync(table, rowState, cancellationToken).ConfigureAwait(false);
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
        internal static async Task<int> WriteToServerAsyncInternal(DuckDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<DuckDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            DuckDBTransaction transaction = null,
            Field excludeField = null)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var appender = CreateDuckDbBulkAppender(connection, tableName, GetColumns(reader), mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            return await appender.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static DuckDbBulkImportPseudoTableType ResolvePseudoTableType(DuckDbBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == DuckDbBulkImportPseudoTableType.Physical ?
                DuckDbBulkImportPseudoTableType.Physical :
                DuckDbBulkImportPseudoTableType.Memory;

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
            return rows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static (string Name, Type Type)[] GetColumns(IDataReader reader) =>
            Enumerable.Range(0, reader.FieldCount)
                .Select(index => (reader.GetName(index), reader.GetFieldType(index)))
                .ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static (string Name, Type Type)[] GetColumns(DataTable table) =>
            table.Columns.OfType<DataColumn>()
                .Select(column => (column.ColumnName, column.DataType))
                .ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="sourceColumns"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="excludeField"></param>
        /// <returns></returns>
        /// <exception cref="InvalidTypeException"></exception>
        private static DuckDbBulkAppender CreateDuckDbBulkAppender(DuckDBConnection connection,
            string tableName,
            (string Name, Type Type)[] sourceColumns,
            IEnumerable<DuckDbBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            DuckDBTransaction transaction,
            Field excludeField = null)
        {
            var dbSetting = connection.GetDbSetting();
            var appender = new DuckDbBulkAppender(connection)
            {
                DestinationTableName = tableName.AsQuoted(true, dbSetting),
                BulkCopyTimeout = bulkCopyTimeout ?? 0,
                BatchSize = batchSize ?? 0,
                Transaction = transaction
            };

            // Validate the explicit mappings, otherwise map the matching columns except the excluded one
            if (mappings != null)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                foreach (var mapping in mappings)
                {
                    var source = sourceColumns.FirstOrDefault(c => string.Equals(c.Name, mapping.SourceColumn, StringComparison.OrdinalIgnoreCase));
                    if (source.Name == null)
                    {
                        throw new InvalidTypeException($"The source column '{mapping.SourceColumn}' defined in the mappings was not found.");
                    }
                    var destination = dbFields?.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                    if (destination?.Type != null && source.Type != null && !AreMappingTypesCompatible(source.Type, destination.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{mapping.SourceColumn}' ({source.Type}) does not match the type of the destination column '{mapping.DestinationColumn}' ({destination.Type}).");
                    }
                    appender.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn);
                }
            }
            else if (excludeField != null)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                foreach (var (name, _) in sourceColumns)
                {
                    var dbField = dbFields?.GetByUnquotedName(name.AsUnquoted(true, dbSetting));
                    if (dbField != null && !string.Equals(dbField.Name, excludeField?.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        appender.ColumnMappings.Add(name, dbField.Name);
                    }
                }
            }

            return appender;
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

            static bool IsIntegral(Type type) =>
                Type.GetTypeCode(type) is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                    or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;

            return sourceType == destinationType ||
                sourceType == typeof(object) ||
                (IsIntegral(sourceType) && IsIntegral(destinationType));
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
        private static DbCommand CreateTraceCommand(DuckDBConnection connection,
            string commandText,
            int? commandTimeout = null,
            DuckDBTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
