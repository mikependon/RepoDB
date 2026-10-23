#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Helpers;
using RepoDb.Data.Models;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for loading the rows of a JSON file into a table.
    /// </summary>
    public static class LoadDataFromJsonExtension
    {
        #region Private Types

        /// <summary>
        /// Maps the columns of the JSON file to the columns of the table: the columns of the file that the table does not have are left out.
        /// </summary>
        private sealed class ColumnMap
        {
            /// <summary>
            /// Gets the columns of the table that the file has, by their position in the values of a row.
            /// </summary>
            public IList<DbField> Fields { get; } = new List<DbField>();

            /// <summary>
            /// Gets the position of each column of the file (by its name, without the case) in the values of a row.
            /// </summary>
            public IDictionary<string, int> Index { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        #region LoadDataFromJson (Sync)

        /// <summary>
        /// Loads the rows of a JSON file into a table. The rows of the file are counted first, then they are read one by one without loading the file in memory,
        /// and buffered. Every time the buffer reaches <paramref name="batchSize"/> rows, they are inserted with the bulk insert of the provider of the connection, and the buffer is cleared
        /// (the rows are inserted in batches of the same size if the provider has no bulk insert). The file is an array of objects (one object per row),
        /// an object whose first array property holds them, or a sequence of objects (one object per line). The columns of the file that the table does not have are ignored,
        /// and the values are converted into the types of the columns of the table.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the JSON file.</param>
        /// <param name="batchSize">The number of rows that are buffered in memory, and inserted at once with the bulk insert, before the buffer is cleared and the progress is reported. The default is 10000.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert of the provider. The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns of the file are kept, or generated again by the database. The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="LoadProgress"/> right after the rows of the file are counted, right after every batch has been inserted, and when the load has ended. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.LoadDataFromJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="LoadProgress"/> of the load, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The batch size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FormatException">The file is not valid JSON, a row is not an object, or a value cannot be converted into the type of its column.</exception>
        /// <exception cref="InvalidOperationException">No column of the file matches a column of the table.</exception>
        public static LoadProgress LoadDataFromJson(this IDbConnection connection,
            string tableName,
            string filename,
            int batchSize = 10000,
            LoadColumnMappingBehavior columnMappingBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            Action<LoadProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.LoadDataFromJson,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, filename, batchSize, columnMappingBehavior, identityBehavior);
            var map = CreateMap(connection, tableName, transaction, identityBehavior, JsonHelper.ReadColumns(filename));
            var progress = new LoadProgress
            {
                Start = DateTime.UtcNow,
                TotalRowCount = JsonHelper.CountRows(filename)
            };
            progressCallback?.Invoke(progress);

            var bulk = IsBulk(connection, map, false);
            var buffer = new List<object[]>(Math.Min(batchSize, 1024));
            using (var reader = JsonHelper.Open(filename))
            {
                while (true)
                {
                    using (var row = reader.Next())
                    {
                        if (row == null)
                        {
                            break;
                        }
                        buffer.Add(ToValues(row, reader.RowNumber, map));
                    }
                    if (buffer.Count == batchSize)
                    {
                        Flush(connection, tableName, map, buffer, bulk, batchSize, columnMappingBehavior, identityBehavior, commandTimeout, traceKey, trace, transaction, progress, progressCallback);
                    }
                }
            }
            if (buffer.Count > 0)
            {
                Flush(connection, tableName, map, buffer, bulk, batchSize, columnMappingBehavior, identityBehavior, commandTimeout, traceKey, trace, transaction, progress, progressCallback);
            }
            progressCallback?.Invoke(progress);
            return progress;
        }

        #endregion

        #region LoadDataFromJson (Async)

        /// <summary>
        /// Loads the rows of a JSON file into a table in an asynchronous way. See <see cref="LoadDataFromJson"/> for how the rows are read, buffered and inserted.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the JSON file.</param>
        /// <param name="batchSize">The number of rows that are buffered in memory, and inserted at once with the bulk insert, before the buffer is cleared and the progress is reported. The default is 10000.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert of the provider. The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns of the file are kept, or generated again by the database. The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="LoadProgress"/> right after the rows of the file are counted, right after every batch has been inserted, and when the load has ended. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.LoadDataFromJson"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="LoadProgress"/> of the load, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The batch size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FormatException">The file is not valid JSON, a row is not an object, or a value cannot be converted into the type of its column.</exception>
        /// <exception cref="InvalidOperationException">No column of the file matches a column of the table.</exception>
        public static async Task<LoadProgress> LoadDataFromJsonAsync(this IDbConnection connection,
            string tableName,
            string filename,
            int batchSize = 10000,
            LoadColumnMappingBehavior columnMappingBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            Action<LoadProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.LoadDataFromJson,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, filename, batchSize, columnMappingBehavior, identityBehavior);
            var map = CreateMap(connection, tableName, transaction, identityBehavior, await JsonHelper.ReadColumnsAsync(filename, cancellationToken).ConfigureAwait(false));
            var progress = new LoadProgress
            {
                Start = DateTime.UtcNow,
                TotalRowCount = await JsonHelper.CountRowsAsync(filename, cancellationToken).ConfigureAwait(false)
            };
            progressCallback?.Invoke(progress);

            var bulk = IsBulk(connection, map, true);
            var buffer = new List<object[]>(Math.Min(batchSize, 1024));
            using (var reader = JsonHelper.Open(filename))
            {
                while (true)
                {
                    using (var row = await reader.NextAsync(cancellationToken).ConfigureAwait(false))
                    {
                        if (row == null)
                        {
                            break;
                        }
                        buffer.Add(ToValues(row, reader.RowNumber, map));
                    }
                    if (buffer.Count == batchSize)
                    {
                        await FlushAsync(connection, tableName, map, buffer, bulk, batchSize, columnMappingBehavior, identityBehavior, commandTimeout, traceKey, trace, transaction, progress, progressCallback, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            if (buffer.Count > 0)
            {
                await FlushAsync(connection, tableName, map, buffer, bulk, batchSize, columnMappingBehavior, identityBehavior, commandTimeout, traceKey, trace, transaction, progress, progressCallback, cancellationToken).ConfigureAwait(false);
            }
            progressCallback?.Invoke(progress);
            return progress;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the arguments of the load.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the JSON file.</param>
        /// <param name="batchSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns of the file are kept.</param>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The batch size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The column mapping behavior or the identity behavior is not defined.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        private static void Validate(IDbConnection connection,
            string tableName,
            string filename,
            int batchSize,
            LoadColumnMappingBehavior columnMappingBehavior,
            LoadIdentityBehavior identityBehavior)
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
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be at least 1.");
            }
            if (!Enum.IsDefined(typeof(LoadColumnMappingBehavior), columnMappingBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(columnMappingBehavior), "The column mapping behavior is not defined.");
            }
            if (!Enum.IsDefined(typeof(LoadIdentityBehavior), identityBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(identityBehavior), "The identity behavior is not defined.");
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"The file '{filename}' does not exist.", filename);
            }
        }

        /// <summary>
        /// Maps the columns of the file to the columns of the table.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="identityBehavior">Defines whether the identity columns are mapped (kept) or left to the database.</param>
        /// <param name="columns">The names of the columns of the file.</param>
        /// <returns>The map of the columns.</returns>
        /// <exception cref="InvalidOperationException">The file has columns, and no column of the file matches a column of the table.</exception>
        private static ColumnMap CreateMap(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            LoadIdentityBehavior identityBehavior,
            IList<string> columns)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var map = new ColumnMap();
            foreach (var column in columns)
            {
                var field = dbFields.GetByUnquotedName(column.AsUnquoted(true, dbSetting));
                if (field == null ||
                    map.Index.ContainsKey(column) ||
                    (identityBehavior == LoadIdentityBehavior.ReturnIdentity && field.IsIdentity))
                {
                    continue;
                }
                map.Index[column] = map.Fields.Count;
                map.Fields.Add(field);
            }
            if (map.Fields.Count == 0 && columns.Count > 0)
            {
                throw new InvalidOperationException($"No column of the file matches a column of the table '{tableName}'.");
            }
            return map;
        }

        /// <summary>
        /// Checks whether the rows are loaded with the bulk insert, which is when the provider of the connection has one.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="isAsync">A value that indicates whether the asynchronous bulk insert is searched.</param>
        /// <returns><c>true</c> if the rows are loaded with the bulk insert; otherwise, <c>false</c>.</returns>
        private static bool IsBulk(IDbConnection connection,
            ColumnMap map,
            bool isAsync)
        {
            using (var table = CreateTable(map))
            using (var reader = table.CreateDataReader())
            {
                return BulkInsertInvoker.IsAvailable(connection, reader, isAsync);
            }
        }

        /// <summary>
        /// Gets the values of a row of the file, converted into the types of the columns of the table.
        /// </summary>
        /// <param name="row">The row: a JSON object.</param>
        /// <param name="rowNumber">The number of the row in the file, starting at 1.</param>
        /// <param name="map">The map of the columns.</param>
        /// <returns>The values by the position of the columns of the map.</returns>
        /// <exception cref="FormatException">A value cannot be converted into the type of its column.</exception>
        private static object[] ToValues(JsonDocument row,
            long rowNumber,
            ColumnMap map)
        {
            var values = JsonHelper.GetValues(row, map.Index, map.Fields.Count);
            for (var i = 0; i < values.Length; i++)
            {
                try
                {
                    values[i] = JsonHelper.ConvertValue(values[i], map.Fields[i].Type);
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Row {rowNumber}, column '{map.Fields[i].Name}': {e.Message}", e);
                }
            }
            return values;
        }

        /// <summary>
        /// Creates the table that holds the rows to be bulk inserted: it has the columns of the map, with the names and the types of the columns of the table.
        /// </summary>
        /// <param name="map">The map of the columns.</param>
        /// <returns>The table, without rows.</returns>
        private static DataTable CreateTable(ColumnMap map)
        {
            var table = new DataTable();
            foreach (var field in map.Fields)
            {
                table.Columns.Add(field.Name, field.Type == null ? typeof(object) : Nullable.GetUnderlyingType(field.Type) ?? field.Type);
            }
            return table;
        }

        /// <summary>
        /// Creates the table with the buffered rows, to be bulk inserted.
        /// </summary>
        /// <param name="map">The map of the columns.</param>
        /// <param name="buffer">The buffered rows: the values by the position of the columns of the map.</param>
        /// <returns>The table with the rows.</returns>
        private static DataTable ToTable(ColumnMap map,
            IList<object[]> buffer)
        {
            var table = CreateTable(map);
            foreach (var values in buffer)
            {
                var row = table.NewRow();
                for (var i = 0; i < values.Length; i++)
                {
                    row[i] = values[i] ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Converts the buffered rows into the rows that the batch insert takes: the dictionaries of the column names and the values.
        /// </summary>
        /// <param name="map">The map of the columns.</param>
        /// <param name="buffer">The buffered rows: the values by the position of the columns of the map.</param>
        /// <returns>The rows.</returns>
        private static IList<object> ToRows(ColumnMap map,
            IList<object[]> buffer)
        {
            var rows = new List<object>(buffer.Count);
            foreach (var values in buffer)
            {
                var row = new Dictionary<string, object>(StringComparer.Ordinal);
                for (var i = 0; i < values.Length; i++)
                {
                    row[map.Fields[i].Name] = values[i];
                }
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// Inserts the buffered rows, clears the buffer, and reports the progress.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="buffer">The buffered rows (cleared when they are inserted).</param>
        /// <param name="bulk">A value that indicates whether the rows are inserted with the bulk insert.</param>
        /// <param name="size">The number of rows of the buffer.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns are kept by the bulk insert.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="progress">The progress of the load.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        private static void Flush(IDbConnection connection,
            string tableName,
            ColumnMap map,
            IList<object[]> buffer,
            bool bulk,
            int size,
            LoadColumnMappingBehavior columnMappingBehavior,
            LoadIdentityBehavior identityBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            LoadProgress progress,
            Action<LoadProgress> progressCallback)
        {
            int inserted;
            if (bulk)
            {
                using (var table = ToTable(map, buffer))
                using (var reader = table.CreateDataReader())
                {
                    inserted = BulkInsertInvoker.BulkInsert(connection, tableName, reader, size, columnMappingBehavior, identityBehavior == LoadIdentityBehavior.KeepIdentity, commandTimeout, traceKey, trace, transaction);
                }
            }
            else
            {
                inserted = RowInserter.Insert(connection, tableName, ToRows(map, buffer), commandTimeout, traceKey, trace, transaction);
            }
            Succeeded(buffer, inserted, progress, progressCallback);
        }

        /// <summary>
        /// Inserts the buffered rows in an asynchronous way, clears the buffer, and reports the progress.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="buffer">The buffered rows (cleared when they are inserted).</param>
        /// <param name="bulk">A value that indicates whether the rows are inserted with the bulk insert.</param>
        /// <param name="size">The number of rows of the buffer.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns are kept by the bulk insert.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="progress">The progress of the load.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task FlushAsync(IDbConnection connection,
            string tableName,
            ColumnMap map,
            IList<object[]> buffer,
            bool bulk,
            int size,
            LoadColumnMappingBehavior columnMappingBehavior,
            LoadIdentityBehavior identityBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            LoadProgress progress,
            Action<LoadProgress> progressCallback,
            CancellationToken cancellationToken)
        {
            int inserted;
            if (bulk)
            {
                using (var table = ToTable(map, buffer))
                using (var reader = table.CreateDataReader())
                {
                    inserted = await BulkInsertInvoker.BulkInsertAsync(connection, tableName, reader, size, columnMappingBehavior, identityBehavior == LoadIdentityBehavior.KeepIdentity, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                inserted = await RowInserter.InsertAsync(connection, tableName, ToRows(map, buffer), commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            }
            Succeeded(buffer, inserted, progress, progressCallback);
        }

        /// <summary>
        /// Takes in the result of a batch (or bulk) operation that has succeeded: clears the buffer, updates the progress and calls the callback.
        /// </summary>
        /// <param name="buffer">The buffered rows (cleared here).</param>
        /// <param name="inserted">The number of rows that were inserted.</param>
        /// <param name="progress">The progress of the load.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        private static void Succeeded(IList<object[]> buffer,
            int inserted,
            LoadProgress progress,
            Action<LoadProgress> progressCallback)
        {
            buffer.Clear();
            progress.BatchNumber++;
            progress.LoadedRowCount += inserted;
            progressCallback?.Invoke(progress);
        }

        #endregion
    }
}
