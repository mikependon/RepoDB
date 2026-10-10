#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb;
using RepoDb.Enumerations.Oracle;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class OracleConnectionExtension
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
            where TEntity : class
        {
            connection.EnsureOpen();
            using var reader = new DataEntityDataReader<TEntity>(entities);
            using var bulkCopy = CreateOracleBulkCopy(connection, tableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
            bulkCopy.WriteToServer(reader);
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null)
        {
            connection.EnsureOpen();
            using var bulkCopy = CreateOracleBulkCopy(connection, tableName, table, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
            var rows = GetDataRows(table, rowState)?.ToArray();
            bulkCopy.WriteToServer(rows);
            return rows != null ? rows.Length : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        internal static int WriteToServerInternal(OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool keepIdentity = false)
        {
            connection.EnsureOpen();
            var countingReader = new CountingDataReader(reader);

            // The bulk copy of Oracle loads the tables of the user of the connection only, so the tables of the other schemas are loaded by binding arrays
            if (IsTableOfAnotherSchema(connection, tableName, connection.GetDbSetting()))
            {
                using var arrayBinder = CreateOracleBulkArrayBinder(connection, tableName, countingReader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, keepIdentity);
                return arrayBinder.WriteToServerAsync(countingReader, CancellationToken.None).GetAwaiter().GetResult();
            }

            using var bulkCopy = CreateOracleBulkCopy(connection, tableName, countingReader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
            bulkCopy.WriteToServer(countingReader);
            return countingReader.Count;
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new DataEntityDataReader<TEntity>(entities);
            using var arrayBinder = CreateOracleBulkArrayBinder(connection, tableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
            return await arrayBinder.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(OracleConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            using var arrayBinder = CreateOracleBulkArrayBinder(connection, tableName, table, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);
            return await arrayBinder.WriteToServerAsync(table, rowState, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> WriteToServerAsyncInternal(OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CancellationToken cancellationToken = default,
            bool keepIdentity = false)
        {
            await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            var countingReader = new CountingDataReader(reader);
            using var bulkCopy = CreateOracleBulkArrayBinder(connection, tableName, countingReader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, keepIdentity);
            return await bulkCopy.WriteToServerAsync(countingReader, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the (quoted) name of the destination table of the bulk copy. The bulk copy of Oracle loads the tables of the user of the connection only, and it does not
        /// accept the name of the table to be qualified with the schema, so the schema is removed when it is the one of the user of the connection.
        /// </summary>
        private static string GetDestinationTableName(OracleConnection connection,
            string tableName,
            IDbSetting dbSetting)
        {
            var schema = DataEntityExtension.GetSchema(tableName, dbSetting)?.AsUnquoted(true, dbSetting);
            if (!string.IsNullOrEmpty(schema) && IsUserOfConnection(connection, schema))
            {
                return DataEntityExtension.GetTableName(tableName, dbSetting).AsQuoted(true, dbSetting);
            }
            return tableName.AsQuoted(true, dbSetting);
        }

        /// <summary>
        /// Checks whether the table is qualified with a schema that is not the one of the user of the connection.
        /// </summary>
        private static bool IsTableOfAnotherSchema(OracleConnection connection,
            string tableName,
            IDbSetting dbSetting)
        {
            var schema = DataEntityExtension.GetSchema(tableName, dbSetting)?.AsUnquoted(true, dbSetting);
            return !string.IsNullOrEmpty(schema) && !IsUserOfConnection(connection, schema);
        }

        private static bool IsUserOfConnection(OracleConnection connection,
            string schema) =>
            string.Equals(schema, new OracleConnectionStringBuilder(connection.ConnectionString).UserID, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableType"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static OracleBulkImportPseudoTableType ResolvePseudoTableType(OracleBulkImportPseudoTableType pseudoTableType,
            int? rowCount) =>
            OracleBulkImportPseudoTableType.Physical; // TODO: Revisit the Memory
            // pseudoTableType == OracleBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= OracleConstants.RowCountThresholdForPhysicalTable ?
            //     OracleBulkImportPseudoTableType.Physical :
            //         OracleBulkImportPseudoTableType.Physical; // pseudoTableType; // TODO: ODP.NET Limitation, force to Physical for now

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
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
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkCopy CreateOracleBulkCopy(OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            OracleBulkCopyOptions bulkCopyOptions,
            int? bulkCopyTimeout,
            int? batchSize = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new OracleBulkCopy(connection, bulkCopyOptions)
            {
                DestinationTableName = GetDestinationTableName(connection, tableName, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName.AsQuoted(true, dbSetting));
                }
            }
            return bulkCopy;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkCopy CreateOracleBulkCopy(OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            OracleBulkCopyOptions bulkCopyOptions,
            int? bulkCopyTimeout,
            int? batchSize = null)
        {
            var dbSetting = connection.GetDbSetting();
            var bulkCopy = new OracleBulkCopy(connection, bulkCopyOptions)
            {
                DestinationTableName = GetDestinationTableName(connection, tableName, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                bulkCopy.BatchSize = batchSize.Value;
            }
            foreach (var mapping in mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader))
            {
                bulkCopy.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
            }
            return bulkCopy;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkArrayBinder CreateOracleBulkArrayBinder(OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            OracleBulkCopyOptions bulkCopyOptions,
            int? bulkCopyTimeout,
            int? batchSize = null)
        {
            var dbSetting = connection.GetDbSetting();
            var arrayBinder = new OracleBulkArrayBinder(connection)
            {
                DestinationTableName = GetDestinationTableName(connection, tableName, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                arrayBinder.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                arrayBinder.BatchSize = batchSize.Value;
            }
            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    arrayBinder.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    arrayBinder.ColumnMappings.Add(column.ColumnName, column.ColumnName.AsQuoted(true, dbSetting));
                }
            }
            return arrayBinder;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private static OracleBulkArrayBinder CreateOracleBulkArrayBinder(OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            OracleBulkCopyOptions bulkCopyOptions,
            int? bulkCopyTimeout,
            int? batchSize = null,
            bool keepIdentity = false)
        {
            var dbSetting = connection.GetDbSetting();
            var arrayBinder = new OracleBulkArrayBinder(connection)
            {
                KeepIdentity = keepIdentity,
                DestinationTableName = GetDestinationTableName(connection, tableName, dbSetting)
            };
            if (bulkCopyTimeout.HasValue)
            {
                arrayBinder.BulkCopyTimeout = bulkCopyTimeout.Value;
            }
            if (batchSize.HasValue)
            {
                arrayBinder.BatchSize = batchSize.Value;
            }
            foreach (var mapping in mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader))
            {
                arrayBinder.ColumnMappings.Add(mapping.SourceColumn, mapping.DestinationColumn.AsQuoted(true, dbSetting));
            }
            return arrayBinder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<OracleBulkInsertMapItem> GetDefaultMappingsForDataReader(OracleConnection connection,
            string tableName,
            IDataReader reader)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, null);
            var dbSetting = connection.GetDbSetting();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbField = dbFields.GetByUnquotedName(columnName.AsUnquoted(true, dbSetting));
                if (dbField != null)
                {
                    yield return new OracleBulkInsertMapItem(columnName, dbField.Name);
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
        private static DbCommand CreateTraceCommand(OracleConnection connection,
            string commandText,
            int? commandTimeout = null,
            OracleTransaction transaction = null) =>
            (DbCommand)connection.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction);

        #endregion
    }
}
