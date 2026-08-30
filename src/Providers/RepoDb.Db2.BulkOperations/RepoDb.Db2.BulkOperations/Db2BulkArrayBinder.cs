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
using IBM.Data.Db2;
using RepoDb.Extensions;
using RepoDb.Resolvers;

namespace RepoDb.Db2.BulkOperations
{
    /// <summary>
    /// Array-bind based alternative to <see cref="Db2BulkCopy"/> for bulk insert huge amount of data with true asynchronous capability.
    /// </summary>
    public class Db2BulkArrayBinder : IDisposable
    {
        private const int MaxBindableParametersCount = 32_767;
        private static readonly TypeToDb2TypeResolver dbTypeResolver = new();
        private static readonly Db2DbTypeNameToDb2TypeResolver nativeTypeResolver = new();
        private readonly DB2Connection connection;

        /// <summary>
        /// Creates a new instance bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to bind array inserts against.</param>
        public Db2BulkArrayBinder(DB2Connection connection)
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
        /// Gets or sets the number of rows inserted per multi-row <c>VALUES</c> batch. Zero auto-sizes it from the column count.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction each batch is executed under.
        /// </summary>
        public DB2Transaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps every source column to itself.
        /// </summary>
        public Db2BulkArrayBinderColumnMappingCollection ColumnMappings { get; } = new();

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
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IReadOnlyList<Db2BulkInsertMapItem> ResolveMappings(
            DataTable dataTable)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings.ToArray();
            }

            return dataTable.Columns
                .OfType<DataColumn>()
                .Select(column => new Db2BulkInsertMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IReadOnlyList<Db2BulkInsertMapItem> ResolveMappings(
            IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                var identityMappings = new Db2BulkInsertMapItem[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    identityMappings[i] = new Db2BulkInsertMapItem(name, name);
                }
                return identityMappings;
            }

            var mappings = new Db2BulkInsertMapItem[ColumnMappings.Count];
            for (var i = 0; i < ColumnMappings.Count; i++)
            {
                var mapping = ColumnMappings[i];
                var ordinal = reader.GetOrdinal(mapping.SourceColumn);
                var canonicalSourceColumn = reader.GetName(ordinal);
                mappings[i] = new Db2BulkInsertMapItem(canonicalSourceColumn, mapping.DestinationColumn, mapping.DB2Type);
            }
            return mappings;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DbFieldCollection> GetDbFieldsAsync(
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(DestinationTableName))
            {
                return null;
            }

            var dbSetting = connection.GetDbSetting();
            var unquotedTableName = DestinationTableName.AsUnquoted(true, dbSetting);
            var dbFieldList = await connection.GetDbHelper().GetFieldsAsync(connection, unquotedTableName, Transaction, cancellationToken);
            return new DbFieldCollection(dbFieldList, dbSetting);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private IReadOnlyList<Db2BulkInsertMapItem> ExcludeIdentityColumn(
            IReadOnlyList<Db2BulkInsertMapItem> mappings,
            DbFieldCollection dbFields)
        {
            var identity = dbFields?.GetIdentity();

            if (identity == null)
            {
                return mappings;
            }

            var dbSetting = connection.GetDbSetting();
            return mappings
                .Where(mapping => !string.Equals(mapping.DestinationColumn.AsUnquoted(true, dbSetting), identity.Name, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <param name="destinationColumn"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private DB2Type ResolveDb2Type(
            DbFieldCollection dbFields,
            RepoDb.Interfaces.IDbSetting dbSetting,
            string destinationColumn,
            Type sourceType)
        {
            if (sourceType == typeof(byte[]))
            {
                var destinationField = dbFields?.GetByUnquotedName(destinationColumn.AsUnquoted(true, dbSetting));
                if (destinationField?.DatabaseType != null)
                {
                    return nativeTypeResolver.Resolve(destinationField.DatabaseType);
                }
            }

            return dbTypeResolver.Resolve(sourceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private string BuildInsertStatement(
            IReadOnlyList<Db2BulkInsertMapItem> mappings,
            int rowCount)
        {
            var columnList = string.Join(", ", mappings.Select(mapping => mapping.DestinationColumn));
            var parameterGroup = "(" + string.Join(", ", Enumerable.Repeat("?", mappings.Count)) + ")";
            var valuesList = string.Join(", ", Enumerable.Repeat(parameterGroup, rowCount));
            return $"INSERT INTO {DestinationTableName} ({columnList}) VALUES {valuesList}";
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
            IReadOnlyList<Db2BulkInsertMapItem> mappings,
            CancellationToken cancellationToken)
        {
            if (dataTable.Columns.Count == 0 || rows.Count == 0 || mappings.Count == 0)
            {
                return 0;
            }

            var dbFields = await GetDbFieldsAsync(cancellationToken);
            mappings = ExcludeIdentityColumn(mappings, dbFields);
            if (mappings.Count == 0)
            {
                return 0;
            }
            var dbSetting = connection.GetDbSetting();
            var batchSize = GetEffectiveBatchSize(mappings.Count);
            var db2Types = mappings
                .Select(mapping => mapping.DB2Type ?? ResolveDb2Type(dbFields, dbSetting, mapping.DestinationColumn, dataTable.Columns[mapping.SourceColumn].DataType))
                .ToArray();
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Count; offset += batchSize)
            {
                var count = Math.Min(batchSize, rows.Count - offset);

                await using var command = connection.CreateCommand();

                command.Transaction = Transaction;
                command.CommandText = BuildInsertStatement(mappings, count);
                if (BulkCopyTimeout > 0)
                {
                    command.CommandTimeout = BulkCopyTimeout;
                }

                for (var rowIndex = 0; rowIndex < count; rowIndex++)
                {
                    var row = rows[offset + rowIndex];
                    for (var i = 0; i < mappings.Count; i++)
                    {
                        var parameter = new DB2Parameter
                        {
                            DB2Type = db2Types[i],
                            Direction = ParameterDirection.Input,
                            Value = row[mappings[i].SourceColumn] ?? DBNull.Value
                        };
                        command.Parameters.Add(parameter);
                    }
                }

                affectedRows += await command.ExecuteNonQueryAsync(
                    cancellationToken);
            }

            return affectedRows;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Batches and inserts every row of <paramref name="reader"/> into <see cref="DestinationTableName"/>.
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
        /// Batches and inserts the rows of <paramref name="dataTable"/> into <see cref="DestinationTableName"/>.
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
