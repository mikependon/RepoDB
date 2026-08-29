#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Extensions;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.BulkOperations
{
    /// <summary>
    /// An official Bulk loader class used by RepoDB for its bulk operations. It is built on top of
    /// <c>Vertica.Data</c>'s native <see cref="VerticaCopyStream"/>.
    /// </summary>
    internal sealed class VerticaBulkCopy
    {
        private const char FieldDelimiter = '\t';
        private const char RecordTerminator = '\n';
        private const string NullMarker = "\\N";
        private readonly VerticaConnection connection;

        /// <summary>
        /// Creates a new instance of <see cref="VerticaBulkCopy"/> bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The (already-open) connection <see cref="VerticaCopyStream"/> will stream through.</param>
        public VerticaBulkCopy(VerticaConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Gets or sets the destination table name.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the bulk-load timeout, in seconds.
        /// </summary>
        public int BulkCopyTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of rows submitted per round trip.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the transaction the copy runs under.
        /// </summary>
        public VerticaTransaction Transaction { get; set; }

        /// <summary>
        /// Gets the source-to-destination column mappings. Empty maps every source column to itself.
        /// </summary>
        public List<VerticaBulkInsertMapItem> ColumnMappings { get; } = new();

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IReadOnlyList<VerticaBulkInsertMapItem> ResolveMappings(DataTable dataTable)
        {
            if (ColumnMappings.Count > 0)
            {
                return ColumnMappings;
            }

            return dataTable.Columns
                .OfType<DataColumn>()
                .Select(column => new VerticaBulkInsertMapItem(column.ColumnName, column.ColumnName))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IReadOnlyList<VerticaBulkInsertMapItem> ResolveMappings(IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                var identityMappings = new VerticaBulkInsertMapItem[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    identityMappings[i] = new VerticaBulkInsertMapItem(name, name);
                }
                return identityMappings;
            }

            return ColumnMappings;
        }

        /// <summary>
        /// Builds the <c>COPY ... FROM STDIN</c> statement text that <see cref="VerticaCopyStream"/> parses
        /// the fed stream against - column order here must match the field order written by <see cref="WriteRow"/>.
        /// </summary>
        private string BuildCopyStatement(IReadOnlyList<VerticaBulkInsertMapItem> mappings)
        {
            var dbSetting = connection.GetDbSetting();
            var columnList = string.Join(", ", mappings.Select(m => m.DestinationColumn.AsQuoted(true, dbSetting)));
            return $"COPY {DestinationTableName} ({columnList}) FROM STDIN " +
                $"DELIMITER E'\\t' RECORD TERMINATOR E'\\n' NULL E'{NullMarker}' ENFORCELENGTH ABORT ON ERROR";
        }

        /// <summary>
        /// Writes one record to <paramref name="writer"/> - one delimited field per entry in
        /// <paramref name="sourceOrdinals"/>, in that order, terminated by <see cref="RecordTerminator"/>.
        /// </summary>
        /// <param name="writer">The in-memory stream writer.</param>
        /// <param name="sourceOrdinals">The source ordinal for each mapping, in write order.</param>
        /// <param name="getValue">Resolves a source ordinal to its raw CLR value (or <see langword="null"/> for a database <c>NULL</c>).</param>
        private static void WriteRow(TextWriter writer,
            int[] sourceOrdinals,
            Func<int, object> getValue)
        {
            for (var i = 0; i < sourceOrdinals.Length; i++)
            {
                if (i > 0)
                {
                    writer.Write(FieldDelimiter);
                }
                writer.Write(FormatValue(getValue(sourceOrdinals[i])));
            }
            writer.Write(RecordTerminator);
        }

        /// <summary>
        /// Formats a single CLR value for the COPY stream. Dates/times use Vertica's default TIMESTAMP
        /// text-input format; booleans use Vertica's (Postgres-lineage) <c>t</c>/<c>f</c> literals.
        /// </summary>
        private static string FormatValue(object value)
        {
            if (value == null || value is DBNull)
            {
                return NullMarker;
            }

            return value switch
            {
                Guid guid => guid.ToString("D", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
                bool boolean => boolean ? "t" : "f",
                byte[] bytes => Convert.ToHexString(bytes),
                IFormattable formattable => Escape(formattable.ToString(null, CultureInfo.InvariantCulture)),
                _ => Escape(value.ToString()),
            };
        }

        /// <summary>
        /// Backslash-escapes the handful of characters the COPY stream would otherwise misinterpret as
        /// record structure: a literal backslash, tab (the field delimiter), and CR/LF (the record terminator).
        /// </summary>
        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value ?? string.Empty;
            }

            StringBuilder builder = null;
            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];
                var escaped = ch switch
                {
                    '\\' => "\\\\",
                    '\t' => "\\t",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    _ => null,
                };
                if (escaped != null)
                {
                    builder ??= new StringBuilder(value, 0, i, value.Length + 8);
                    builder.Append(escaped);
                }
                else
                {
                    builder?.Append(ch);
                }
            }
            return builder?.ToString() ?? value;
        }

        private static StreamWriter CreateStreamWriter(Stream stream) =>
            new(stream, new UTF8Encoding(false), 1024, leaveOpen: true);

        /// <summary>
        /// Runs the configured <see cref="VerticaCopyStream"/> against the in-memory stream built by
        /// <see cref="WriteRow"/>. <see cref="VerticaCopyStream"/> exposes no async API of its own, so the
        /// synchronous Start/AddStream/Execute/Finish sequence is offloaded to a background thread.
        /// </summary>
        private Task<int> ExecuteAsync(IReadOnlyList<VerticaBulkInsertMapItem> mappings,
            MemoryStream stream,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = 0;

            return Task.Run(() =>
            {
                var copyStatement = BuildCopyStatement(mappings);
                var copyStream = new VerticaCopyStream(connection, copyStatement);
                copyStream.Start();
                copyStream.AddStream(stream, false);
                copyStream.Execute();
                return checked((int)copyStream.Finish());
            }, cancellationToken);
        }

        #endregion

        #region WriteToServer

        /// <summary>
        /// Streams every remaining row of <paramref name="reader"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="reader">The source rows to insert.</param>
        /// <returns>The number of rows <see cref="VerticaCopyStream"/> reports having loaded.</returns>
        public int WriteToServer(IDataReader reader) =>
            WriteToServerAsync(reader, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(IDataReader)"/>.
        /// </summary>
        public async Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            if (reader.FieldCount == 0)
            {
                return 0;
            }

            var mappings = ResolveMappings(reader);
            var sourceOrdinals = mappings.Select(m => reader.GetOrdinal(m.SourceColumn)).ToArray();

            using var stream = new MemoryStream();
            using (var writer = CreateStreamWriter(stream))
            {
                while (reader.Read())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    WriteRow(writer, sourceOrdinals, ordinal => reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal));
                }
            }

            return await ExecuteAsync(mappings, stream, cancellationToken);
        }

        /// <summary>
        /// Streams the rows of <paramref name="dataTable"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="dataTable">The source rows to insert.</param>
        /// <param name="rowState">When specified, only rows in this state are inserted.</param>
        /// <returns>The number of rows <see cref="VerticaCopyStream"/> reports having loaded.</returns>
        public int WriteToServer(DataTable dataTable, DataRowState? rowState = null) =>
            WriteToServerAsync(dataTable, rowState, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(DataTable, DataRowState?)"/>.
        /// </summary>
        public async Task<int> WriteToServerAsync(DataTable dataTable,
            DataRowState? rowState = null,
            CancellationToken cancellationToken = default)
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

            var sourceOrdinals = mappings.Select(m => dataTable.Columns.IndexOf(m.SourceColumn)).ToArray();

            using var stream = new MemoryStream();
            using (var writer = CreateStreamWriter(stream))
            {
                foreach (var row in rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    WriteRow(writer, sourceOrdinals, ordinal => row.IsNull(ordinal) ? null : row[ordinal]);
                }
            }

            return await ExecuteAsync(mappings, stream, cancellationToken);
        }

        #endregion
    }
}
