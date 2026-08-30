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
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Extensions;
using RepoDb.Resolvers;


namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// Array-bind based alternative to <see cref="OracleBulkCopy"/> for bulk insert huge amount of data with true asynchronous capability.
    /// </summary>
    public class OracleBulkArrayBinder : IDisposable
    {
        private const int MaxBindableParametersCount = 65_535;
        private static readonly TypeToOracleDbTypeResolver dbTypeResolver = new();
        private readonly OracleConnection connection;

        /// <summary>
        /// Creates a new instance bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to bind array inserts against.</param>
        public OracleBulkArrayBinder(OracleConnection connection)
        {
            this.connection = connection;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the target table name.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the command timeout, in seconds. Zero uses the provider default.
        /// </summary>
        public int BulkCopyTimeout { get; set; }

        /// <summary>
        /// Gets or sets the array-bind batch size. Zero auto-sizes it from the column count.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction each batch is executed under.
        /// </summary>
        public OracleTransaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps every source column to itself.
        /// </summary>
        public OracleBulkArrayBinderColumnMappingCollection ColumnMappings { get; } = new();

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        private int GetEffectiveBatchSize(
            int columnCount)
        {
            return BatchSize > 0 ? BatchSize : Math.Min(1000, MaxBindableParametersCount / columnCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DataTable> ToDataTableAsync(
            IDataReader reader,
            CancellationToken cancellationToken)
        {
            var dataTable = new DataTable();
            var fieldCount = reader.FieldCount;

            for (var i = 0; i < fieldCount; i++)
            {
                var fieldType = reader.GetFieldType(i) ?? typeof(object);
                dataTable.Columns.Add(reader.GetName(i), Nullable.GetUnderlyingType(fieldType) ?? fieldType);
            }

            var dbReader = reader as DbDataReader;

            while (dbReader != null ? await dbReader.ReadAsync(cancellationToken) : reader.Read())
            {
                var values = new object[fieldCount];
                for (var i = 0; i < fieldCount; i++)
                {
                    values[i] = reader.GetValue(i) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columnName"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static object[] GetBoundValues(
            IReadOnlyList<DataRow> rows,
            string columnName,
            int offset,
            int count)
        {
            var values = new object[count];
            for (var rowIndex = 0; rowIndex < count; rowIndex++)
            {
                values[rowIndex] = rows[offset + rowIndex][columnName] ?? DBNull.Value;
            }
            return values;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IReadOnlyList<OracleBulkInsertMapItem> ResolveMappings(
            DataTable dataTable)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings.ToArray();
            }

            return dataTable.Columns
                .OfType<DataColumn>()
                .Select(column => new OracleBulkInsertMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IReadOnlyList<OracleBulkInsertMapItem> ResolveMappings(
            IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                var identityMappings = new OracleBulkInsertMapItem[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    identityMappings[i] = new OracleBulkInsertMapItem(name, name);
                }
                return identityMappings;
            }

            var mappings = new OracleBulkInsertMapItem[ColumnMappings.Count];
            for (var i = 0; i < ColumnMappings.Count; i++)
            {
                var mapping = ColumnMappings[i];
                var ordinal = reader.GetOrdinal(mapping.SourceColumn);
                var canonicalSourceColumn = reader.GetName(ordinal);
                mappings[i] = new OracleBulkInsertMapItem(canonicalSourceColumn, mapping.DestinationColumn, mapping.OracleDbType);
            }
            return mappings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<OracleBulkInsertMapItem>> ExcludeIdentityColumnAsync(
            IReadOnlyList<OracleBulkInsertMapItem> mappings,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(DestinationTableName))
            {
                return mappings;
            }

            var dbSetting = connection.GetDbSetting();
            var unquotedTableName = DestinationTableName.AsUnquoted(true, dbSetting);
            var dbFieldList = await connection.GetDbHelper().GetFieldsAsync(connection, unquotedTableName, Transaction, cancellationToken);
            var dbFields = new DbFieldCollection(dbFieldList, dbSetting);
            var identity = dbFields.GetIdentity();

            if (identity == null)
            {
                return mappings;
            }

            return mappings
                .Where(mapping => !string.Equals(mapping.DestinationColumn.AsUnquoted(true, dbSetting), identity.Name, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private string BuildInsertStatement(
            IReadOnlyList<OracleBulkInsertMapItem> mappings)
        {
            var columnList = string.Join(", ", mappings.Select(mapping => mapping.DestinationColumn));
            var parameterList = string.Join(", ", Enumerable.Range(0, mappings.Count).Select(i => ":p" + i));
            return $"INSERT INTO {DestinationTableName} ({columnList}) VALUES ({parameterList})";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rows"></param>
        /// <param name="mappings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<int> BindArrayCoreAsync(
            DataTable dataTable,
            IReadOnlyList<DataRow> rows,
            IReadOnlyList<OracleBulkInsertMapItem> mappings,
            CancellationToken cancellationToken)
        {
            if (dataTable.Columns.Count == 0 || rows.Count == 0 || mappings.Count == 0)
            {
                return 0;
            }

            mappings = await ExcludeIdentityColumnAsync(mappings, cancellationToken);
            if (mappings.Count == 0)
            {
                return 0;
            }
            var batchSize = GetEffectiveBatchSize(mappings.Count);
            var commandText = BuildInsertStatement(mappings);
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Count; offset += batchSize)
            {
                var count = Math.Min(batchSize, rows.Count - offset);

                await using var command = connection.CreateCommand();

                command.Transaction = Transaction;
                command.BindByName = true;
                command.ArrayBindCount = count;
                command.CommandText = commandText;
                if (BulkCopyTimeout > 0)
                {
                    command.CommandTimeout = BulkCopyTimeout;
                }

                for (var i = 0; i < mappings.Count; i++)
                {
                    var mapping = mappings[i];
                    var parameter = new OracleParameter
                    {
                        ParameterName = "p" + i,
                        OracleDbType = mapping.OracleDbType ?? dbTypeResolver.Resolve(dataTable.Columns[mapping.SourceColumn].DataType),
                        Direction = ParameterDirection.Input,
                        Value = GetBoundValues(rows, mapping.SourceColumn, offset, count)
                    };
                    command.Parameters.Add(parameter);
                }

                affectedRows += await command.ExecuteNonQueryAsync(
                    cancellationToken);
            }

            return affectedRows;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Array-binds and inserts every row of <paramref name="reader"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> WriteToServerAsync(
            IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            var mappings = ResolveMappings(reader);
            var dataTable = await ToDataTableAsync(reader, cancellationToken);
            var rows = dataTable.Rows.OfType<DataRow>().ToArray();

            return await BindArrayCoreAsync(dataTable, rows, mappings, cancellationToken);
        }

        /// <summary>
        /// Array-binds and inserts the rows of <paramref name="dataTable"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="dataTable">The source rows to insert.</param>
        /// <param name="rowState">When specified, only rows in this state are inserted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> WriteToServerAsync(
            DataTable dataTable,
            DataRowState? rowState = null,
            CancellationToken cancellationToken = default)
        {
            var mappings = ResolveMappings(dataTable);
            var rowsQuery = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue)
            {
                rowsQuery = rowsQuery.Where(row => row.RowState == rowState);
            }

            return BindArrayCoreAsync(dataTable, rowsQuery.ToArray(), mappings, cancellationToken);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the underlying resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            // Does nothing for now; this instance owns no unmanaged resources.
        }

        #endregion
    }
}
