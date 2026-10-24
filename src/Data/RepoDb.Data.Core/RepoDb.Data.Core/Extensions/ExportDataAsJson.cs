#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Helpers;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using static RepoDb.Data.DbConnection;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for exporting the rows of a table into a JSON file.
    /// </summary>
    public static class ExportDataAsJsonExtension
    {
        #region Private Variables

        /// <summary>
        /// The number of rows after which what the writer holds is flushed into the file, so only a few rows are held in memory.
        /// </summary>
        private const int FlushRows = 1000;

        #endregion

        #region Public Methods

        #region ExportDataAsJson (Sync)

        /// <summary>
        /// Exports the rows of a table into a JSON file without holding them in memory: the rows are counted first (with the filter), then they are read forward only and every row
        /// is written into the file as soon as it is read. The file is an array of objects, one object per row with a property for each column, so <c>ImportDataFromJson</c> can load it back.
        /// The rows are written into a temporary file that replaces <paramref name="filename"/> only when the export has succeeded, so a failed export leaves no partial file behind.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The filter of the rows to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsJson(this IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            QueryGroup where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null)
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
                using (var writer = new Utf8JsonWriter(stream, JsonRowWriter.Options))
                using (var reader = connection.ExecuteReader(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace))
                {
                    var names = GetNames(reader);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    writer.WriteStartArray();
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
                    writer.WriteEndArray();
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

        #endregion

        #region ExportDataAsJson (Async)

        /// <summary>
        /// Exports the rows of a table into a JSON file in an asynchronous way. See <see cref="ExportDataAsJson"/> for how the rows are counted, read and written.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The filter of the rows to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static async Task<ExportProgress> ExportDataAsJsonAsync(this IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            QueryGroup where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
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
                using (var writer = new Utf8JsonWriter(stream, JsonRowWriter.Options))
                using (var reader = await connection.ExecuteReaderAsync(commandText, param, commandTimeout: commandTimeout, traceKey: traceKey ?? TraceKeys.ExecuteReader, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    var names = GetNames(reader);
                    var columns = interceptors == null ? null : RowInterception.GetColumns(reader);
                    writer.WriteStartArray();
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
                    writer.WriteEndArray();
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

        #region ExportDataAsJson (TEntity, Sync)

        /// <summary>
        /// Exports the rows of the table mapped to <typeparamref name="TEntity"/> into a JSON file. See <see cref="ExportDataAsJson(IDbConnection, string, string, IList{CopyColumnMapping}, QueryGroup, IList{IDataInterceptor{CopyRow}}, Action{ExportProgress}, int?, string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsJson<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null)
            where TEntity : class =>
            connection.ExportDataAsJson(ClassMappedNameCache.Get<TEntity>(), filename, mappings, where == null ? null : QueryGroup.Parse(where), dataInterceptors, progressCallback, commandTimeout, traceKey, trace);

        /// <summary>
        /// Exports the rows of a table into a JSON file, filtered by an expression of <typeparamref name="TEntity"/>. See <see cref="ExportDataAsJson(IDbConnection, string, string, IList{CopyColumnMapping}, QueryGroup, IList{IDataInterceptor{CopyRow}}, Action{ExportProgress}, int?, string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that the expression is of.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static ExportProgress ExportDataAsJson<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null)
            where TEntity : class =>
            connection.ExportDataAsJson(tableName, filename, mappings, where == null ? null : QueryGroup.Parse(where), dataInterceptors, progressCallback, commandTimeout, traceKey, trace);

        #endregion

        #region ExportDataAsJson (TEntity, Async)

        /// <summary>
        /// Exports the rows of the table mapped to <typeparamref name="TEntity"/> into a JSON file in an asynchronous way. See <see cref="ExportDataAsJson(IDbConnection, string, string, IList{CopyColumnMapping}, QueryGroup, IList{IDataInterceptor{CopyRow}}, Action{ExportProgress}, int?, string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static Task<ExportProgress> ExportDataAsJsonAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.ExportDataAsJsonAsync(ClassMappedNameCache.Get<TEntity>(), filename, mappings, where == null ? null : QueryGroup.Parse(where), dataInterceptors, progressCallback, commandTimeout, traceKey, trace, cancellationToken);

        /// <summary>
        /// Exports the rows of a table into a JSON file in an asynchronous way, filtered by an expression of <typeparamref name="TEntity"/>. See <see cref="ExportDataAsJson(IDbConnection, string, string, IList{CopyColumnMapping}, QueryGroup, IList{IDataInterceptor{CopyRow}}, Action{ExportProgress}, int?, string, ITrace)"/> for how the rows are counted, read and written.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that the expression is of.</typeparam>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are exported from.</param>
        /// <param name="filename">The path of the JSON file. An existing file is replaced.</param>
        /// <param name="mappings">The mappings of the columns of the table (the source) to the names of the properties of the file (the destination), as in <c>CopyDataTo</c> (see <see cref="CopyColumnMapping"/>). Only the mapped columns are selected and written. The default is <c>null</c>, which selects all the columns of the table.</param>
        /// <param name="where">The expression used to filter the rows of the table to be exported, which also filters the rows that are counted. The default is <c>null</c>, which exports all the rows.</param>
        /// <param name="dataInterceptors">The interceptors that every row goes through before it is written, in the order that they are given: each one receives the <see cref="CopyRow"/> that the previous one returned, and the row that the last one returns is the one that is written, with the columns that it has. The columns of the row are the ones that are selected (named by the <paramref name="mappings"/>, if there are any). The default is <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="ExportProgress"/> after the rows are counted, after every row is written, and when the export has ended (the same instance every time). The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ExportDataAsJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="ExportProgress"/> of the export, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentException">A data interceptor is <c>null</c>, or a mapping is not valid (it is <c>null</c>, it has an empty name, its source column is not a column of the table, or its destination is used more than once).</exception>
        /// <exception cref="InvalidOperationException">A data interceptor returned <c>null</c>.</exception>
        public static Task<ExportProgress> ExportDataAsJsonAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            string tableName,
            string filename,
            IList<CopyColumnMapping> mappings = null,
            Expression<Func<TEntity, bool>> where = null,
            IList<IDataInterceptor<CopyRow>> dataInterceptors = null,
            Action<ExportProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ExportDataAsJson,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.ExportDataAsJsonAsync(tableName, filename, mappings, where == null ? null : QueryGroup.Parse(where), dataInterceptors, progressCallback, commandTimeout, traceKey, trace, cancellationToken);

        #endregion

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
        private static void Validate(IDbConnection connection,
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
        private static void WriteRow(Utf8JsonWriter writer,
            IDataRecord reader,
            string[] names,
            List<CopyColumn> columns,
            IList<IDataInterceptor<CopyRow>> interceptors)
        {
            if (interceptors == null)
            {
                JsonRowWriter.WriteRow(writer, reader, names);
                return;
            }
            JsonRowWriter.WriteRow(writer, RowInterception.Apply(RowInterception.Read(reader, columns), interceptors));
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
