using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDb;
using RepoDb.Connector.MariaDb.Bulk;

namespace RepoDb.MariaDb.BulkOperations
{
    /// <summary>
    /// An official Bulk loader class used by RepoDB for its bulk operations. It is built on top of <c>RepoDb.Connector.MariaDb</c>'s <see cref="MariaDbBulkLoader"/>.
    /// </summary>
    internal sealed class MariaDbBulkCopy
    {
        private readonly MariaDbConnection connection;

        /// <summary>
        /// Creates a new instance of <see cref="MariaDbBulkCopy"/> bound to <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">The connection <see cref="MariaDbBulkLoader"/> will load through.</param>
        public MariaDbBulkCopy(MariaDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Gets or sets the destination table name. Expected to already be correctly quoted by the caller
        /// (every call site in <c>WriteToServer.cs</c> sets this from <c>tableName.AsQuoted(true, dbSetting)</c>)
        /// - passed straight through to <see cref="MariaDbBulkLoader.TableName"/> unmodified.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets or sets the bulk-load timeout, in seconds. Mapped directly to <see cref="MariaDbBulkLoader.Timeout"/>.
        /// </summary>
        public int BulkCopyTimeout { get; set; } = 30;

        /// <summary>
        /// Gets the source-ordinal-to-destination-column mappings that determine both which columns get
        /// written and the column order of the temp file built for <see cref="MariaDbBulkLoader"/>.
        /// </summary>
        public List<MariaDbBulkCopyColumnMapping> ColumnMappings { get; } = new();

        /// <summary>
        /// The character used to separate fields in the temp file. Kept out of CSV-comma territory
        /// deliberately - see the remarks on <see cref="MariaDbBulkCopy"/> for why.
        /// </summary>
        private const string FieldTerminator = "\t";

        /// <summary>
        /// The character used to separate rows (lines) in the temp file. Deliberately a bare <c>\n</c>
        /// (rather than <see cref="Environment.NewLine"/>) so the file's actual bytes are identical
        /// regardless of the OS this code happens to run on.
        /// </summary>
        private const string LineTerminator = "\n";

        #region WriteToServer

        /// <summary>
        /// Writes every remaining row of <paramref name="reader"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="reader">
        /// The source reader. Read positionally via each mapping's <see cref="MariaDbBulkCopyColumnMapping.SourceOrdinal"/> -
        /// see the remarks on <see cref="MariaDbBulkCopy"/> for why no other ordinal of this reader is ever touched.
        /// </param>
        /// <returns>The number of rows <see cref="MariaDbBulkLoader"/> reports having loaded.</returns>
        public int WriteToServer(IDataReader reader) =>
            WriteToServerAsync(reader, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(IDataReader)"/>.
        /// </summary>
        public async Task<int> WriteToServerAsync(IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            var filePath = GetTempFilePath();
            try
            {
                using (var writer = CreateFileWriter(filePath))
                {
                    while (reader.Read())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        WriteRow(writer, ordinal => reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal));
                    }
                }
                return await LoadAsync(filePath, cancellationToken);
            }
            finally
            {
                DeleteTempFile(filePath);
            }
        }

        /// <summary>
        /// Writes <paramref name="rows"/> to <see cref="DestinationTableName"/>.
        /// </summary>
        /// <param name="rows">The source rows, read positionally via each mapping's <see cref="MariaDbBulkCopyColumnMapping.SourceOrdinal"/> - i.e. the real <see cref="DataTable"/> column index, not the mapping's position in <see cref="ColumnMappings"/> (those can legitimately diverge - see <c>WriteToServer.cs</c>'s remarks on why the ordinal must come from <c>DataTable.Columns.IndexOf</c>).</param>
        /// <param name="columnCount">Unused beyond a defensive bounds check - <see cref="ColumnMappings"/> alone already determines exactly which columns get written.</param>
        /// <returns>The number of rows <see cref="MariaDbBulkLoader"/> reports having loaded.</returns>
        public int WriteToServer(DataRow[] rows,
            int columnCount) =>
            WriteToServerAsync(rows, columnCount, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous counterpart of <see cref="WriteToServer(DataRow[], int)"/>.
        /// </summary>
        public async Task<int> WriteToServerAsync(DataRow[] rows,
            int columnCount,
            CancellationToken cancellationToken = default)
        {
            var filePath = GetTempFilePath();
            try
            {
                using (var writer = CreateFileWriter(filePath))
                {
                    foreach (var row in rows ?? Array.Empty<DataRow>())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        WriteRow(writer, ordinal => ordinal < columnCount && ordinal < row.Table.Columns.Count && !row.IsNull(ordinal) ? row[ordinal] : null);
                    }
                }
                return await LoadAsync(filePath, cancellationToken);
            }
            finally
            {
                DeleteTempFile(filePath);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Writes one row to <paramref name="writer"/> - one tab-separated field per entry in
        /// <see cref="ColumnMappings"/>, in that order, terminated by <see cref="LineTerminator"/>.
        /// </summary>
        /// <param name="writer">The temp file writer.</param>
        /// <param name="getValue">Resolves a source ordinal to its raw CLR value (or <see langword="null"/> for a database <c>NULL</c>).</param>
        private void WriteRow(StreamWriter writer,
            Func<int, object> getValue)
        {
            for (var i = 0; i < ColumnMappings.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write(FieldTerminator);
                }
                writer.Write(FormatValue(getValue(ColumnMappings[i].SourceOrdinal)));
            }
            writer.Write(LineTerminator);
        }

        /// <summary>
        /// Formats a single CLR value for the temp file, using <c>LOAD DATA</c>'s own documented escape
        /// sequences (<c>\N</c> for <c>NULL</c>, <c>\\</c>/<c>\t</c>/<c>\n</c>/<c>\r</c> for the literal
        /// characters that would otherwise be misread as field/line structure).
        /// </summary>
        private static string FormatValue(object value)
        {
            if (value == null || value is DBNull)
            {
                return "\\N";
            }

            return value switch
            {
                Guid guid => guid.ToString("D", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
                bool boolean => boolean ? "1" : "0",
                byte[] bytes => Convert.ToHexString(bytes),
                IFormattable formattable => Escape(formattable.ToString(null, CultureInfo.InvariantCulture)),
                _ => Escape(value.ToString()),
            };
        }

        /// <summary>
        /// Backslash-escapes the handful of characters <c>LOAD DATA</c> would otherwise misinterpret as field
        /// structure: a literal backslash, tab (the field terminator), and CR/LF (the line terminator).
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

        /// <summary>
        /// Runs the configured <see cref="MariaDbBulkLoader"/> against the temp file built by <see cref="WriteRow"/>, back-tick-quoting each mapped destination column along the way - the
        /// counterpart to <see cref="DestinationTableName"/> already arriving pre-quoted (see its remarks).
        /// </summary>
        private async Task<int> LoadAsync(string filePath,
            CancellationToken cancellationToken)
        {
            var bulkLoader = new MariaDbBulkLoader(connection)
            {
                TableName = DestinationTableName,
                FileName = filePath,
                Local = true,
                FieldTerminator = FieldTerminator,
                LineTerminator = LineTerminator,
                NumberOfLinesToSkip = 0,
                CharacterSet = "utf8mb4",
                Timeout = BulkCopyTimeout,
            };
            foreach (var mapping in ColumnMappings)
            {
                bulkLoader.Columns.Add(QuoteIdentifier(mapping.DestinationColumn));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await bulkLoader.LoadAsync();
        }

        /// <summary>
        /// Back-tick-quotes a raw identifier, doubling any embedded back-tick per MariaDB's identifier-quoting
        /// escape rule.
        /// </summary>
        private static string QuoteIdentifier(string identifier) =>
            $"`{identifier?.Replace("`", "``")}`";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetTempFilePath() => Path.GetTempFileName();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static StreamWriter CreateFileWriter(string filePath) =>
            new(filePath, false, new UTF8Encoding(false));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private static void DeleteTempFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException)
            {
                // Best-effort cleanup - a lingering temp file is a minor annoyance, not worth failing an
                // otherwise-successful (or already-failed) bulk load over.
            }
        }

        #endregion
    }
}
