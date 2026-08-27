using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Extensions;

namespace RepoDb.Firebird.BulkOperations
{
    /// <summary>
    /// A <see cref="FbBatchCommand"/>-based bulk-copy class for bulk-inserting huge datasets.
    /// </summary>
    public class FirebirdCommandBatcher : IDisposable
    {
        private readonly FbConnection connection;

        /// <summary>
        /// Creates a new instance bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection to batch inserts against.</param>
        public FirebirdCommandBatcher(
            FbConnection connection)
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
        /// Gets or sets the number of rows submitted per <see cref="FbBatchCommand"/> round trip. Zero (the
        /// default) submits every row in a single round trip.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction each batch is executed under.
        /// </summary>
        public FbTransaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps every source column to itself.
        /// </summary>
        public FirebirdCommandBatcherColumnMappingCollection ColumnMappings { get; } = new();

        #endregion

        #region Helpers

        private IReadOnlyList<FirebirdCommandBatcherMapItem> ResolveMappings(DataTable dataTable)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings.ToArray();
            }

            return dataTable.Columns
                .OfType<DataColumn>()
                .Select(column => new FirebirdCommandBatcherMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        private IReadOnlyList<FirebirdCommandBatcherMapItem> ResolveMappings(IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                var identityMappings = new FirebirdCommandBatcherMapItem[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    identityMappings[i] = new FirebirdCommandBatcherMapItem(name, name);
                }
                return identityMappings;
            }

            var mappings = new FirebirdCommandBatcherMapItem[ColumnMappings.Count];
            for (var i = 0; i < ColumnMappings.Count; i++)
            {
                var mapping = ColumnMappings[i];
                var ordinal = reader.GetOrdinal(mapping.SourceColumn);
                mappings[i] = new FirebirdCommandBatcherMapItem(reader.GetName(ordinal), mapping.DestinationColumn, mapping.FbDbType);
            }
            return mappings;
        }

        private static string ParameterName(int index) => "p" + index;

        private string BuildInsertStatement(IReadOnlyList<FirebirdCommandBatcherMapItem> mappings)
        {
            var dbSetting = connection.GetDbSetting();
            var columnList = string.Join(", ", mappings.Select(m => m.DestinationColumn.AsQuoted(true, dbSetting)));
            var parameterList = string.Join(", ", Enumerable.Range(0, mappings.Count).Select(i => "@" + ParameterName(i)));
            return $"INSERT INTO {DestinationTableName} ({columnList}) VALUES ({parameterList})";
        }

        // FbBatchCommand has no CommandTimeout-equivalent property - BulkCopyTimeout is accepted for
        // signature symmetry with the other bulk-operations packages but has nothing to apply to here.
        private FbBatchCommand CreateBatch(string commandText) =>
            new(commandText, connection, Transaction);

        private static void AddRow(FbBatchCommand batch, object[] values)
        {
            var parameters = batch.AddBatchParameters();
            for (var i = 0; i < values.Length; i++)
            {
                parameters.Add(new FbParameter { ParameterName = ParameterName(i), Value = values[i] ?? DBNull.Value });
            }
        }

        private static int EnsureSuccessAndCount(FbBatchNonQueryResult result)
        {
            result.EnsureSuccess();
            return result.Count;
        }

        #endregion

        #region Sync

        /// <summary>
        /// Batches and inserts every row of <paramref name="reader"/> into <see cref="DestinationTableName"/>,
        /// streaming rows to the destination as they are read rather than materializing them into memory first.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int WriteToServer(IDataReader reader)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            var mappings = ResolveMappings(reader);
            var sourceOrdinals = mappings.Select(m => reader.GetOrdinal(m.SourceColumn)).ToArray();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : int.MaxValue;
            var affectedRows = 0;

            // A fresh FbBatchCommand per chunk (rather than reusing one across multiple ExecuteNonQuery
            // calls) sidesteps needing to know whether BatchParameters resets itself after execution.
            var batch = CreateBatch(commandText);
            var pendingRows = 0;

            try
            {
                while (reader.Read())
                {
                    var values = new object[sourceOrdinals.Length];
                    for (var i = 0; i < sourceOrdinals.Length; i++)
                    {
                        values[i] = reader.GetValue(sourceOrdinals[i]);
                    }
                    AddRow(batch, values);
                    pendingRows++;

                    if (pendingRows >= effectiveBatchSize)
                    {
                        affectedRows += EnsureSuccessAndCount(batch.ExecuteNonQuery());
                        batch.Dispose();
                        batch = CreateBatch(commandText);
                        pendingRows = 0;
                    }
                }

                if (pendingRows > 0)
                {
                    affectedRows += EnsureSuccessAndCount(batch.ExecuteNonQuery());
                }
            }
            finally
            {
                batch.Dispose();
            }

            return affectedRows;
        }

        /// <summary>
        /// Batches and inserts the rows of <paramref name="dataTable"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="dataTable">The source rows to insert.</param>
        /// <param name="rowState">When specified, only rows in this state are inserted.</param>
        /// <returns>The number of rows affected.</returns>
        public int WriteToServer(DataTable dataTable, DataRowState? rowState = null)
        {
            var mappings = ResolveMappings(dataTable);
            var rowsQuery = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue)
            {
                rowsQuery = rowsQuery.Where(row => row.RowState == rowState);
            }
            var rows = rowsQuery.ToArray();

            if (dataTable.Columns.Count == 0 || rows.Length == 0 || mappings.Count == 0)
            {
                return 0;
            }

            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : rows.Length;
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Length; offset += effectiveBatchSize)
            {
                var count = Math.Min(effectiveBatchSize, rows.Length - offset);
                using var batch = CreateBatch(commandText);

                for (var rowIndex = 0; rowIndex < count; rowIndex++)
                {
                    var row = rows[offset + rowIndex];
                    AddRow(batch, mappings.Select(m => row[m.SourceColumn]).ToArray());
                }

                affectedRows += EnsureSuccessAndCount(batch.ExecuteNonQuery());
            }

            return affectedRows;
        }

        #endregion

        #region Async

        /// <summary>
        /// Batches and inserts every row of <paramref name="reader"/> into <see cref="DestinationTableName"/>,
        /// streaming rows to the destination as they are read rather than materializing them into memory first.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> WriteToServerAsync(IDataReader reader, CancellationToken cancellationToken = default)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            var mappings = ResolveMappings(reader);
            var sourceOrdinals = mappings.Select(m => reader.GetOrdinal(m.SourceColumn)).ToArray();
            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : int.MaxValue;
            var affectedRows = 0;
            var dbReader = reader as DbDataReader;

            // A fresh FbBatchCommand per chunk (rather than reusing one across multiple ExecuteNonQueryAsync
            // calls) sidesteps needing to know whether BatchParameters resets itself after execution.
            var batch = CreateBatch(commandText);
            var pendingRows = 0;

            try
            {
                while (dbReader != null ? await dbReader.ReadAsync(cancellationToken) : reader.Read())
                {
                    var values = new object[sourceOrdinals.Length];
                    for (var i = 0; i < sourceOrdinals.Length; i++)
                    {
                        values[i] = reader.GetValue(sourceOrdinals[i]);
                    }
                    AddRow(batch, values);
                    pendingRows++;

                    if (pendingRows >= effectiveBatchSize)
                    {
                        affectedRows += EnsureSuccessAndCount(await batch.ExecuteNonQueryAsync(cancellationToken));
                        await batch.DisposeAsync();
                        batch = CreateBatch(commandText);
                        pendingRows = 0;
                    }
                }

                if (pendingRows > 0)
                {
                    affectedRows += EnsureSuccessAndCount(await batch.ExecuteNonQueryAsync(cancellationToken));
                }
            }
            finally
            {
                await batch.DisposeAsync();
            }

            return affectedRows;
        }

        /// <summary>
        /// Batches and inserts the rows of <paramref name="dataTable"/> into <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="dataTable">The source rows to insert.</param>
        /// <param name="rowState">When specified, only rows in this state are inserted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> WriteToServerAsync(DataTable dataTable, DataRowState? rowState = null, CancellationToken cancellationToken = default)
        {
            var mappings = ResolveMappings(dataTable);
            var rowsQuery = dataTable.Rows.OfType<DataRow>();
            if (rowState.HasValue)
            {
                rowsQuery = rowsQuery.Where(row => row.RowState == rowState);
            }
            var rows = rowsQuery.ToArray();

            if (dataTable.Columns.Count == 0 || rows.Length == 0 || mappings.Count == 0)
            {
                return 0;
            }

            var commandText = BuildInsertStatement(mappings);
            var effectiveBatchSize = BatchSize > 0 ? BatchSize : rows.Length;
            var affectedRows = 0;

            for (var offset = 0; offset < rows.Length; offset += effectiveBatchSize)
            {
                var count = Math.Min(effectiveBatchSize, rows.Length - offset);
                await using var batch = CreateBatch(commandText);

                for (var rowIndex = 0; rowIndex < count; rowIndex++)
                {
                    var row = rows[offset + rowIndex];
                    AddRow(batch, mappings.Select(m => row[m.SourceColumn]).ToArray());
                }

                affectedRows += EnsureSuccessAndCount(await batch.ExecuteNonQueryAsync(cancellationToken));
            }

            return affectedRows;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the underlying resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            // Does nothing for now; this instance owns no unmanaged resources of its own - each
            // FbBatchCommand it creates is scoped to (and disposed within) a single WriteToServer call.
        }

        #endregion
    }
}
