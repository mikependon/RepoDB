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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sap.Data.Hana;
using RepoDb;
using RepoDb.Enumerations.SapHana;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SapHana.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// Writes rows into SAP HANA for the bulk operations in this package.
    /// </summary>
    /// <remarks>
    /// SAP HANA's ADO.NET provider (<c>Sap.Data.Hana</c>) has no documented equivalent of
    /// <c>SqlBulkCopy</c>/<c>MySqlBulkCopy</c>, so rather than guess at an undocumented array-bind API this
    /// writes chunked, multi-row parameterized <c>INSERT</c> statements instead (the same technique the
    /// non-bulk provider's own <c>InsertAll</c> uses at a smaller scale) - slower than a true bulk-copy
    /// protocol, but built entirely from standard <see cref="IDbCommand"/>/<see cref="IDataParameter"/> members
    /// that are guaranteed to exist on any ADO.NET provider, so it compiles and runs regardless of what
    /// provider-specific bulk APIs this driver version does or doesn't expose. If a real HANA bulk-load API is
    /// confirmed to exist later, swapping it in here only touches this one file.
    /// </remarks>
    public static partial class SapHanaConnectionExtension
    {
        /// <summary>
        /// HANA's own bind-variable limit is high; this just keeps a single batch's parameter count sane.
        /// </summary>
        private const int MaxBoundParametersPerBatch = 32_000;

        private const int DefaultBatchSize = 500;

        #region WriteToServerInternal

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
            return WriteReaderToServer(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
        }

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
            var filteredTable = table.Clone();
            foreach (var row in GetDataRows(table, rowState))
            {
                filteredTable.ImportRow(row);
            }
            using var reader = new DataTableReader(filteredTable);
            return WriteReaderToServer(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize, null, excludeField);
        }

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
            var countingReader = new CountingDataReader(reader);
            WriteReaderToServer(connection, tableName, countingReader, mappings, bulkCopyTimeout, batchSize, transaction, excludeField);
            return countingReader.Count;
        }

        #endregion

        #region WriteToServerAsyncInternal

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
            using var reader = new DataEntityDataReader<TEntity>(entities);
            return await WriteReaderToServerAsync(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize, transaction, excludeField, cancellationToken);
        }

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
            var filteredTable = table.Clone();
            foreach (var row in GetDataRows(table, rowState))
            {
                filteredTable.ImportRow(row);
            }
            using var reader = new DataTableReader(filteredTable);
            return await WriteReaderToServerAsync(connection, tableName, reader, mappings, bulkCopyTimeout, batchSize, null, excludeField, cancellationToken);
        }

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
            var countingReader = new CountingDataReader(reader);
            await WriteReaderToServerAsync(connection, tableName, countingReader, mappings, bulkCopyTimeout, batchSize, transaction, excludeField, cancellationToken);
            return countingReader.Count;
        }

        #endregion

        #region Core

        /// <summary>
        ///
        /// </summary>
        private static (int[] SourceOrdinals, string[] DestinationColumns) ResolveColumnPlan(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings,
            HanaTransaction transaction,
            Field excludeField,
            IDbSetting dbSetting)
        {
            var columnMappings = mappings?.AsList()
                ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction, excludeField).AsList();

            if (columnMappings.Count == 0)
            {
                return (Array.Empty<int>(), Array.Empty<string>());
            }

            var dbFields = mappings != null ? DbFieldCache.Get(connection, tableName, transaction) : null;
            var sourceOrdinals = new int[columnMappings.Count];
            var destinationColumns = new string[columnMappings.Count];

            for (var i = 0; i < columnMappings.Count; i++)
            {
                var sourceOrdinal = reader.GetOrdinal(columnMappings[i].SourceColumn);
                if (dbFields != null)
                {
                    var destinationField = dbFields.GetByUnquotedName(columnMappings[i].DestinationColumn.AsUnquoted(true, dbSetting));
                    var sourceType = reader.GetFieldType(sourceOrdinal);
                    if (destinationField?.Type != null && sourceType != null && !AreMappingTypesCompatible(sourceType, destinationField.Type))
                    {
                        throw new InvalidTypeException($"The type of the source column '{columnMappings[i].SourceColumn}' ({sourceType}) does not match the type of the destination column '{columnMappings[i].DestinationColumn}' ({destinationField.Type}).");
                    }
                }
                sourceOrdinals[i] = sourceOrdinal;
                destinationColumns[i] = columnMappings[i].DestinationColumn.AsQuoted(true, dbSetting);
            }

            return (sourceOrdinals, destinationColumns);
        }

        /// <summary>
        ///
        /// </summary>
        private static int GetEffectiveBatchSize(int? batchSize,
            int columnCount)
        {
            if (batchSize.GetValueOrDefault() > 0)
            {
                return batchSize.Value;
            }
            return Math.Max(1, Math.Min(DefaultBatchSize, MaxBoundParametersPerBatch / Math.Max(1, columnCount)));
        }

        /// <summary>
        ///
        /// </summary>
        private static string BuildBatchInsertText(string quotedTableName,
            string[] destinationColumns,
            int rowCount)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO ").Append(quotedTableName)
                .Append(" (").Append(string.Join(", ", destinationColumns)).Append(") VALUES ");

            var paramIndex = 0;
            for (var r = 0; r < rowCount; r++)
            {
                if (r > 0)
                {
                    builder.Append(", ");
                }
                builder.Append('(');
                for (var c = 0; c < destinationColumns.Length; c++)
                {
                    if (c > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(':').Append('p').Append(paramIndex);
                    paramIndex++;
                }
                builder.Append(')');
            }

            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        private static int FlushBatch(HanaConnection connection,
            string quotedTableName,
            string[] destinationColumns,
            List<object[]> buffer,
            HanaTransaction transaction,
            int? commandTimeout)
        {
            if (buffer.Count == 0)
            {
                return 0;
            }

            var commandText = BuildBatchInsertText(quotedTableName, destinationColumns, buffer.Count);
            using var command = (HanaCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

            var paramIndex = 0;
            for (var r = 0; r < buffer.Count; r++)
            {
                for (var c = 0; c < destinationColumns.Length; c++)
                {
                    command.Parameters.Add(new HanaParameter("p" + paramIndex, buffer[r][c] ?? DBNull.Value));
                    paramIndex++;
                }
            }

            var affected = command.ExecuteNonQuery();
            buffer.Clear();
            return affected;
        }

        /// <summary>
        ///
        /// </summary>
        private static async Task<int> FlushBatchAsync(HanaConnection connection,
            string quotedTableName,
            string[] destinationColumns,
            List<object[]> buffer,
            HanaTransaction transaction,
            int? commandTimeout,
            CancellationToken cancellationToken)
        {
            if (buffer.Count == 0)
            {
                return 0;
            }

            var commandText = BuildBatchInsertText(quotedTableName, destinationColumns, buffer.Count);
            await using var command = (HanaCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

            var paramIndex = 0;
            for (var r = 0; r < buffer.Count; r++)
            {
                for (var c = 0; c < destinationColumns.Length; c++)
                {
                    command.Parameters.Add(new HanaParameter("p" + paramIndex, buffer[r][c] ?? DBNull.Value));
                    paramIndex++;
                }
            }

            var affected = await command.ExecuteNonQueryAsync(cancellationToken);
            buffer.Clear();
            return affected;
        }

        /// <summary>
        ///
        /// </summary>
        private static int WriteReaderToServer(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            HanaTransaction transaction,
            Field excludeField)
        {
            var dbSetting = connection.GetDbSetting();
            var (sourceOrdinals, destinationColumns) = ResolveColumnPlan(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (destinationColumns.Length == 0)
            {
                return 0;
            }

            using var filteredReader = new ColumnFilteredDataReader(reader, sourceOrdinals);
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var effectiveBatchSize = GetEffectiveBatchSize(batchSize, destinationColumns.Length);
            var buffer = new List<object[]>(effectiveBatchSize);
            var affected = 0;

            while (filteredReader.Read())
            {
                var values = new object[destinationColumns.Length];
                filteredReader.GetValues(values);
                buffer.Add(values);
                if (buffer.Count >= effectiveBatchSize)
                {
                    affected += FlushBatch(connection, quotedTableName, destinationColumns, buffer, transaction, bulkCopyTimeout);
                }
            }
            affected += FlushBatch(connection, quotedTableName, destinationColumns, buffer, transaction, bulkCopyTimeout);

            return affected;
        }

        /// <summary>
        ///
        /// </summary>
        private static async Task<int> WriteReaderToServerAsync(HanaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<SapHanaBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            HanaTransaction transaction,
            Field excludeField,
            CancellationToken cancellationToken)
        {
            var dbSetting = connection.GetDbSetting();
            var (sourceOrdinals, destinationColumns) = ResolveColumnPlan(connection, tableName, reader, mappings, transaction, excludeField, dbSetting);
            if (destinationColumns.Length == 0)
            {
                return 0;
            }

            using var filteredReader = new ColumnFilteredDataReader(reader, sourceOrdinals);
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var effectiveBatchSize = GetEffectiveBatchSize(batchSize, destinationColumns.Length);
            var buffer = new List<object[]>(effectiveBatchSize);
            var affected = 0;

            while (filteredReader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var values = new object[destinationColumns.Length];
                filteredReader.GetValues(values);
                buffer.Add(values);
                if (buffer.Count >= effectiveBatchSize)
                {
                    affected += await FlushBatchAsync(connection, quotedTableName, destinationColumns, buffer, transaction, bulkCopyTimeout, cancellationToken);
                }
            }
            affected += await FlushBatchAsync(connection, quotedTableName, destinationColumns, buffer, transaction, bulkCopyTimeout, cancellationToken);

            return affected;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Both branches currently resolve to <see cref="SapHanaBulkImportPseudoTableType.Physical"/> - kept
        /// as a distinct method (rather than inlining) purely for call-site/signature compatibility with the
        /// other pseudo-table-based providers in this codebase, and as the one place a future
        /// <see cref="SapHanaBulkImportPseudoTableType.Memory"/> (HANA "LOCAL TEMPORARY") path would be wired
        /// in based on row count.
        /// </summary>
        private static SapHanaBulkImportPseudoTableType ResolvePseudoTableType(SapHanaBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            pseudoTableType == SapHanaBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= SapHanaConstants.RowCountThresholdForPhysicalTable ?
                SapHanaBulkImportPseudoTableType.Physical :
                    SapHanaBulkImportPseudoTableType.Physical;

        /// <summary>
        ///
        /// </summary>
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
        private static DbCommand CreateTraceCommand(HanaConnection connection,
            string commandText,
            int? commandTimeout = null,
            HanaTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
