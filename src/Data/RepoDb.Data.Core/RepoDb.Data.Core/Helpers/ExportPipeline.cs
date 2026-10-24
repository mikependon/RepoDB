#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using static RepoDb.Data.DbConnection;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// The export of the rows of a table into a file, which every <c>ExportDataAs*</c> operation shares: the rows are counted (with the filter), then they are selected (with the
    /// mappings and the filter) and read forward only, and every row goes through the interceptors and is written by the <see cref="IExportWriter"/> of the format as soon as it is
    /// read. The rows are written into a temporary file that replaces the file only when the export has succeeded.
    /// </summary>
    internal static class ExportPipeline
    {
        #region Constants

        /// <summary>
        /// The number of rows after which what the writer holds is flushed into the file, so only a few rows are held in memory.
        /// </summary>
        private const int FlushRows = 1000;

        #endregion

        #region Methods

        /// <summary>
        /// Exports the rows of the table into the file.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="mappings">The mappings of the columns of the table to the names of the columns of the file, or <c>null</c>.</param>
        /// <param name="where">The filter of the rows, or <c>null</c>.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through, or <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="createWriter">Creates the writer of the format over the stream of the file.</param>
        /// <returns>The progress of the export.</returns>
        public static ExportProgress Run(IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings,
            QueryGroup where,
            IList<IDataInterceptor<CopyRow>> dataInterceptors,
            Action<ExportProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            Func<Stream, IExportWriter> createWriter)
        {
            Validate(connection, tableName, filename, mappings);
            var interceptors = RowInterception.Normalize(dataInterceptors);
            var watch = Stopwatch.StartNew();
            var progress = new ExportProgress { Start = DateTime.UtcNow };
            progress.TotalRowCount = connection.Count(tableName, where, commandTimeout: commandTimeout, trace: trace);
            Report(progress, watch, progressCallback);

            var temporary = CreateTemporaryName(filename);
            try
            {
                var (commandText, param) = CreateSelect(connection, tableName, where, mappings);
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                using (var writer = createWriter(stream))
                using (var reader = connection.ExecuteReader(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace))
                {
                    var names = GetNames(reader);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    writer.Begin(names);
                    while (reader.Read())
                    {
                        progress.Position++;
                        WriteRow(writer, reader, names, columns, interceptors);
                        progress.CurrentRowCount++;
                        if (progress.CurrentRowCount % FlushRows == 0)
                        {
                            writer.Flush();
                        }
                        Report(progress, watch, progressCallback);
                    }
                    writer.End();
                    writer.Flush();
                }
                Replace(temporary, filename);
            }
            catch
            {
                Discard(temporary);
                throw;
            }
            progress.End = DateTime.UtcNow;
            Report(progress, watch, progressCallback);
            return progress;
        }

        /// <summary>
        /// Exports the rows of the table into the file in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="mappings">The mappings of the columns of the table to the names of the columns of the file, or <c>null</c>.</param>
        /// <param name="where">The filter of the rows, or <c>null</c>.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through, or <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="createWriter">Creates the writer of the format over the stream of the file.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the progress of the export.</returns>
        public static async Task<ExportProgress> RunAsync(IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings,
            QueryGroup where,
            IList<IDataInterceptor<CopyRow>> dataInterceptors,
            Action<ExportProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            Func<Stream, IExportWriter> createWriter,
            CancellationToken cancellationToken)
        {
            Validate(connection, tableName, filename, mappings);
            var interceptors = RowInterception.Normalize(dataInterceptors);
            var watch = Stopwatch.StartNew();
            var progress = new ExportProgress { Start = DateTime.UtcNow };
            progress.TotalRowCount = await connection.CountAsync(tableName, where, commandTimeout: commandTimeout, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false);
            Report(progress, watch, progressCallback);

            var temporary = CreateTemporaryName(filename);
            try
            {
                var (commandText, param) = CreateSelect(connection, tableName, where, mappings);
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                using (var writer = createWriter(stream))
                using (var reader = await connection.ExecuteReaderAsync(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    var names = GetNames(reader);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    writer.Begin(names);
                    while (await ReadAsync(reader, cancellationToken).ConfigureAwait(false))
                    {
                        progress.Position++;
                        WriteRow(writer, reader, names, columns, interceptors);
                        progress.CurrentRowCount++;
                        if (progress.CurrentRowCount % FlushRows == 0)
                        {
                            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                        }
                        Report(progress, watch, progressCallback);
                    }
                    writer.End();
                    await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                Replace(temporary, filename);
            }
            catch
            {
                Discard(temporary);
                throw;
            }
            progress.End = DateTime.UtcNow;
            Report(progress, watch, progressCallback);
            return progress;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the arguments of the export.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="mappings">The mappings of the columns, or <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A mapping is not valid.</exception>
        internal static void Validate(IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            if (mappings == null || mappings.Count == 0)
            {
                return;
            }
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, null);
            var destinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in mappings)
            {
                if (mapping == null)
                {
                    throw new ArgumentException("The column mappings must not contain a null item.", nameof(mappings));
                }
                if (string.IsNullOrWhiteSpace(mapping.SourceColumn) || string.IsNullOrWhiteSpace(mapping.DestinationColumn))
                {
                    throw new ArgumentException("The source and the destination columns of a column mapping must not be empty.", nameof(mappings));
                }
                if (dbFields?.GetByUnquotedName(mapping.SourceColumn.AsUnquoted(true, dbSetting)) == null)
                {
                    throw new ArgumentException($"The source column '{mapping.SourceColumn}' does not exist on the table '{tableName}'.", nameof(mappings));
                }
                if (!destinations.Add(mapping.DestinationColumn))
                {
                    throw new ArgumentException($"The destination '{mapping.DestinationColumn}' is mapped more than once.", nameof(mappings));
                }
            }
        }

        /// <summary>
        /// Gets the names of the columns of the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The names, in order.</returns>
        private static string[] GetNames(IDataRecord reader)
        {
            var names = new string[reader.FieldCount];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = reader.GetName(i);
            }
            return names;
        }

        /// <summary>
        /// Writes the current row of the reader: as it is, or as the row that the interceptors return.
        /// </summary>
        /// <param name="writer">The writer of the file.</param>
        /// <param name="reader">The reader that is on the row.</param>
        /// <param name="names">The names of the columns of the reader.</param>
        /// <param name="columns">The columns of the reader (see <see cref="RowInterception.GetColumns"/>) when there are interceptors; otherwise, <c>null</c>.</param>
        /// <param name="interceptors">The interceptors, or <c>null</c>.</param>
        private static void WriteRow(IExportWriter writer,
            IDataRecord reader,
            string[] names,
            List<CopyColumn> columns,
            IList<IDataInterceptor<CopyRow>> interceptors)
        {
            if (interceptors == null)
            {
                writer.WriteRow(reader, names);
                return;
            }
            writer.WriteRow(RowInterception.Apply(RowInterception.Read(reader, columns), interceptors));
        }

        /// <summary>
        /// Updates the progress with the time that has passed, then gives it to the callback.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <param name="watch">The stopwatch that was started with the export.</param>
        /// <param name="progressCallback">The callback, or <c>null</c>.</param>
        private static void Report(ExportProgress progress,
            Stopwatch watch,
            Action<ExportProgress> progressCallback)
        {
            progress.Elapsed = watch.Elapsed;
            progressCallback?.Invoke(progress);
        }

        /// <summary>
        /// Gets the name of the temporary file that the rows are written into, which is next to the file so it can replace it.
        /// </summary>
        /// <param name="filename">The path of the file.</param>
        /// <returns>The path of the temporary file.</returns>
        private static string CreateTemporaryName(string filename) =>
            filename + "." + Guid.NewGuid().ToString("N") + ".tmp";

        /// <summary>
        /// Replaces the file with the temporary file.
        /// </summary>
        /// <param name="temporary">The path of the temporary file.</param>
        /// <param name="filename">The path of the file.</param>
        private static void Replace(string temporary,
            string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            File.Move(temporary, filename);
        }

        /// <summary>
        /// Deletes the temporary file, if it exists (a failure to delete it does not hide the error that made the export fail).
        /// </summary>
        /// <param name="temporary">The path of the temporary file.</param>
        private static void Discard(string temporary)
        {
            try
            {
                if (File.Exists(temporary))
                {
                    File.Delete(temporary);
                }
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
            }
        }

        #endregion
    }
}
