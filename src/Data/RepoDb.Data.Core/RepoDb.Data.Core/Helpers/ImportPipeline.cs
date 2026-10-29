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
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// The load of the rows of a file into a table, which every <c>ImportDataFrom*</c> operation shares: the file is read forward only by its <see cref="IImportSource"/>, the rows are
    /// buffered, and every time that the buffer is full it is inserted (or merged) with the bulk operations of the provider (in batches of plain inserts if it has no bulk insert).
    /// </summary>
    internal static class ImportPipeline
    {
        #region Public Methods

        /// <summary>
        /// Loads the rows of the file into the table.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="source">The reader of the file.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the rows are merged (with the primary key, or the identity, as the qualifiers) instead of inserted.</param>
        /// <param name="qualifierNames">The names of the columns that identify the existing rows, or <c>null</c> to use the primary key (or the identity) of the table. Used only if the existing rows are bypassed.</param>
        /// <param name="mappings">The mappings of the columns of the file to the columns of the table, or <c>null</c> to load the columns that have the same name on both.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the file are kept.</param>
        /// <param name="dataInterceptors">The interceptors that every row of the file goes through before it is loaded, in order, or <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The progress of the load.</returns>
        public static ImportProgress Run(IDbConnection connection,
            string tableName,
            string filename,
            IImportSource source,
            int bufferSize,
            bool bypassExistingRows,
            IList<string> qualifierNames,
            IList<ColumnMapping> mappings,
            ColumnMappingBehavior columnMappingBehavior,
            IdentityBehavior identityBehavior,
            IList<IDataInterceptor<Row>> dataInterceptors,
            Action<ImportProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            Validate(connection,
                tableName,
                filename,
                bufferSize,
                columnMappingBehavior,
                identityBehavior);
            source.Validate();
            var map = CreateMap(connection,
                tableName,
                transaction,
                dataInterceptors != null && dataInterceptors.Count > 0 ? IdentityBehavior.KeepIdentity : identityBehavior,
                source.ReadColumns(),
                mappings,
                source.HasAllColumns);
            var (qualifiers, interception) = PrepareInterception(connection,
                tableName,
                transaction,
                identityBehavior,
                map,
                qualifierNames,
                bypassExistingRows,
                dataInterceptors);
            var progress = new ImportProgress
            {
                Start = DateTime.UtcNow,
                TotalRowCount = source.CountRows()
            };
            progressCallback?.Invoke(progress);
            var bulk = IsBulk(connection,
                map,
                false,
                bypassExistingRows);
            var buffer = new List<object[]>(Math.Min(bufferSize,
                1024));
            ReadImportRows(source,
                map,
                interception,
                buffer,
                bufferSize,
                () => Flush(connection,
                    tableName,
                    interception?.Map ?? map,
                    buffer,
                    bulk,
                    qualifiers,
                    bufferSize,
                    columnMappingBehavior,
                    identityBehavior,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction,
                    progress,
                    progressCallback));
            progressCallback?.Invoke(progress);
            return progress;
        }

        /// <summary>
        /// Loads the rows of the file into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="source">The reader of the file.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the rows are merged (with the primary key, or the identity, as the qualifiers) instead of inserted.</param>
        /// <param name="qualifierNames">The names of the columns that identify the existing rows, or <c>null</c> to use the primary key (or the identity) of the table. Used only if the existing rows are bypassed.</param>
        /// <param name="mappings">The mappings of the columns of the file to the columns of the table, or <c>null</c> to load the columns that have the same name on both.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the file are kept.</param>
        /// <param name="dataInterceptors">The interceptors that every row of the file goes through before it is loaded, in order, or <c>null</c>.</param>
        /// <param name="progressCallback">The callback that receives the progress, or <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the progress of the load.</returns>
        public static async Task<ImportProgress> RunAsync(IDbConnection connection,
            string tableName,
            string filename,
            IImportSource source,
            int bufferSize,
            bool bypassExistingRows,
            IList<string> qualifierNames,
            IList<ColumnMapping> mappings,
            ColumnMappingBehavior columnMappingBehavior,
            IdentityBehavior identityBehavior,
            IList<IDataInterceptor<Row>> dataInterceptors,
            Action<ImportProgress> progressCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            Validate(connection,
                tableName,
                filename,
                bufferSize,
                columnMappingBehavior,
                identityBehavior);
            source.Validate();
            var map = CreateMap(connection,
                tableName,
                transaction,
                dataInterceptors != null && dataInterceptors.Count > 0 ? IdentityBehavior.KeepIdentity : identityBehavior,
                await source.ReadColumnsAsync(cancellationToken).ConfigureAwait(false),
                mappings,
                source.HasAllColumns);
            var (qualifiers, interception) = PrepareInterception(connection,
                tableName,
                transaction,
                identityBehavior,
                map,
                qualifierNames,
                bypassExistingRows,
                dataInterceptors);
            var progress = new ImportProgress
            {
                Start = DateTime.UtcNow,
                TotalRowCount = await source.CountRowsAsync(cancellationToken).ConfigureAwait(false)
            };
            progressCallback?.Invoke(progress);
            var bulk = IsBulk(connection,
                map,
                true,
                bypassExistingRows);
            var buffer = new List<object[]>(Math.Min(bufferSize,
                1024));
            await ReadImportRowsAsync(source,
                map,
                interception,
                buffer,
                bufferSize,
                () => FlushAsync(connection,
                    tableName,
                    interception?.Map ?? map,
                    buffer,
                    bulk,
                    qualifiers,
                    bufferSize,
                    columnMappingBehavior,
                    identityBehavior,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction,
                    progress,
                    progressCallback,
                    cancellationToken),
                cancellationToken).ConfigureAwait(false);
            progressCallback?.Invoke(progress);
            return progress;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the arguments of the load.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the file.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table.</param>
        /// <param name="identityBehavior">Defines whether the values of the identity columns of the file are kept.</param>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The buffer size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The column mapping behavior or the identity behavior is not defined.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        private static void Validate(IDbConnection connection,
            string tableName,
            string filename,
            int bufferSize,
            ColumnMappingBehavior columnMappingBehavior,
            IdentityBehavior identityBehavior)
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

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize),
                    "The buffer size must be at least 1.");
            }

            if (!Enum.IsDefined(typeof(ColumnMappingBehavior),
                columnMappingBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(columnMappingBehavior),
                    "The column mapping behavior is not defined.");
            }

            if (!Enum.IsDefined(typeof(IdentityBehavior),
                identityBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(identityBehavior),
                    "The identity behavior is not defined.");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"The file '{filename}' does not exist.",
                    filename);
            }
        }

        /// <summary>
        /// Maps the columns of the file to the columns of the table: by the mappings if there are any (only the mapped columns are loaded), otherwise by their names.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="identityBehavior">Defines whether the identity columns are mapped (kept) or left to the database.</param>
        /// <param name="columns">The names of the columns of the file.</param>
        /// <param name="mappings">The mappings of the columns of the file to the columns of the table, or <c>null</c>.</param>
        /// <param name="hasAllColumns">A value that indicates whether <paramref name="columns"/> has every column of the file, so a mapped column that is not in it does not exist.</param>
        /// <returns>The map of the columns.</returns>
        /// <exception cref="ArgumentException">A mapping is not valid: it is <c>null</c>, it has an empty name, its destination column is not a column of the table (or is mapped more than once), or its source column is not a column of the file.</exception>
        /// <exception cref="InvalidOperationException">The file has columns, and no column of the file matches a column of the table.</exception>
        private static ColumnMap CreateMap(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            IdentityBehavior identityBehavior,
            IList<string> columns,
            IList<ColumnMapping> mappings,
            bool hasAllColumns)
        {
            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection,
                tableName,
                transaction);
            var map = new ColumnMap();
            if (mappings != null && mappings.Count > 0)
            {
                return MapExplicitColumns(tableName,
                    identityBehavior,
                    columns,
                    mappings,
                    hasAllColumns,
                    name => dbFields.GetByUnquotedName(name.AsUnquoted(true,
                        dbSetting)),
                    name => name.AsUnquoted(true,
                        dbSetting));
            }

            foreach (var column in columns)
            {
                var field = dbFields.GetByUnquotedName(column.AsUnquoted(true,
                    dbSetting));
                if (field == null || map.Index.ContainsKey(column) || (identityBehavior == IdentityBehavior.Ignore && field.IsIdentity))
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
        /// Validates explicit source and destination mappings and records duplicate source copies.
        /// </summary>
        /// <param name="tableName">The destination table name used in validation errors.</param>
        /// <param name="identityBehavior">Whether identity values are mapped.</param>
        /// <param name="columns">The known source column names.</param>
        /// <param name="mappings">The explicit column mappings.</param>
        /// <param name="hasAllColumns">Whether the known columns describe the entire file.</param>
        /// <param name="find">Resolves an unquoted destination name to its field.</param>
        /// <param name="unquote">Removes provider quoting from a field name.</param>
        /// <returns>The validated map and positions of copied values.</returns>
        private static ColumnMap MapExplicitColumns(string tableName,
            IdentityBehavior identityBehavior,
            IList<string> columns,
            IList<ColumnMapping> mappings,
            bool hasAllColumns,
            Func<string, DbField> find,
            Func<string, string> unquote)
        {
            var map = new ColumnMap();
            var known = new HashSet<string>(columns,
                StringComparer.OrdinalIgnoreCase);
            var destinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in mappings)
            {
                ValidateMappingNames(mapping);
                var field = find(mapping.DestinationColumn);
                if (field == null)
                {
                    throw new ArgumentException($"The destination column '{mapping.DestinationColumn}' does not exist on the table '{tableName}'.",
                        "mappings");
                }

                if (!destinations.Add(unquote(field.Name)))
                {
                    throw new ArgumentException($"The destination column '{mapping.DestinationColumn}' is mapped more than once.",
                        "mappings");
                }

                if (hasAllColumns && !known.Contains(mapping.SourceColumn))
                {
                    throw new ArgumentException($"The source column '{mapping.SourceColumn}' does not exist on the file.",
                        "mappings");
                }

                if (identityBehavior == IdentityBehavior.Ignore && field.IsIdentity)
                {
                    continue;
                }

                var position = map.Fields.Count;
                map.Fields.Add(field);
                if (map.Index.TryGetValue(mapping.SourceColumn,
                    out var first))
                {
                    map.Copies.Add(new KeyValuePair<int, int>(first,
                        position));
                }
                else
                {
                    map.Index[mapping.SourceColumn] = position;
                }
            }

            if (map.Fields.Count == 0)
            {
                throw new InvalidOperationException($"No column of the file is mapped to a column of the table '{tableName}' that is loaded.");
            }

            return map;
        }

        /// <summary>
        /// Creates the interception of the rows, if the user gave interceptors.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="identityBehavior">Defines whether the identity values are kept.</param>
        /// <param name="map">The map of the columns of the file.</param>
        /// <param name="qualifiers">The qualifiers of the merge, or <c>null</c>.</param>
        /// <param name="interceptors">The interceptors that the user gave, or <c>null</c>.</param>
        /// <returns>The interception, or <c>null</c> if there are no interceptors.</returns>
        /// <exception cref="ArgumentException">An interceptor is <c>null</c>.</exception>
        private static Interception CreateInterception(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            IdentityBehavior identityBehavior,
            ColumnMap map,
            IList<Field> qualifiers,
            IList<IDataInterceptor<Row>> interceptors)
        {
            interceptors = RowInterception.Normalize(interceptors);
            if (interceptors == null)
            {
                return null;
            }

            var dbSetting = connection.GetDbSetting();
            var dbFields = DbFieldCache.Get(connection,
                tableName,
                transaction);
            return new Interception(interceptors,
                map,
                name => dbFields.GetByUnquotedName(name.AsUnquoted(true,
                    dbSetting)),
                name => name.AsUnquoted(true,
                    dbSetting),
                identityBehavior,
                qualifiers,
                tableName);
        }

        /// <summary>
        /// Gives the value of a column of the file to every other column of the table that it is mapped to.
        /// </summary>
        /// <param name="values">The values of a row by the position of the columns of the map.</param>
        /// <param name="map">The map of the columns.</param>
        /// <returns>The values.</returns>
        private static object[] Copy(object[] values,
            ColumnMap map)
        {
            foreach (var copy in map.Copies)
            {
                values[copy.Value] = values[copy.Key];
            }

            return values;
        }

        /// <summary>
        /// Checks whether the rows are loaded with the bulk insert, which is when the provider of the connection has one (the bulk merge if the existing rows are bypassed).
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="isAsync">A value that indicates whether the asynchronous bulk insert is searched.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the bulk merge is used instead of the bulk insert.</param>
        /// <returns><c>true</c> if the rows are loaded with the bulk insert (or the bulk merge); otherwise, <c>false</c>.</returns>
        private static bool IsBulk(IDbConnection connection,
            ColumnMap map,
            bool isAsync,
            bool bypassExistingRows)
        {
            using (var table = CreateTable(map))
            using (var reader = table.CreateDataReader())
            {
                return bypassExistingRows ? BulkInsertInvoker.IsMergeAvailable(connection,
                    reader,
                    isAsync) : BulkInsertInvoker.IsAvailable(connection,
                        reader,
                        isAsync);
            }
        }

        /// <summary>
        /// Gets the qualifiers of the merge: the columns that were given, or the primary key columns of the table (its identity column if it has no primary key).
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="names">The names of the columns that were given, or <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The qualifiers.</returns>
        /// <exception cref="ArgumentException">A given qualifier is empty, or is not a column of the table.</exception>
        /// <exception cref="InvalidOperationException">No qualifier was given and the table has no primary key and no identity column, or a qualifier is not a column that is loaded.</exception>
        private static IList<Field> GetQualifiers(IDbConnection connection,
            string tableName,
            ColumnMap map,
            IList<string> names,
            IDbTransaction transaction)
        {
            var dbSetting = connection.GetDbSetting();
            var qualifiers = RepoDb.Data.DbConnection.GetQualifiers(connection,
                tableName,
                names,
                null,
                transaction);
            var loaded = new HashSet<string>(map.Fields.Select(field => field.Name.AsUnquoted(true,
                dbSetting)),
                StringComparer.OrdinalIgnoreCase);
            var missing = qualifiers.FirstOrDefault(qualifier => !loaded.Contains(qualifier.Name));
            if (missing != null)
            {
                throw new InvalidOperationException($"The existing rows cannot be bypassed as the qualifier '{missing.Name}' of the table '{tableName}' is not a column of the file that is loaded.");
            }

            return qualifiers;
        }

        /// <summary>
        /// Converts the values of a row of the file into the types of the columns of the table.
        /// </summary>
        /// <param name="values">The values of the row by the position of the columns of the map (converted in place).</param>
        /// <param name="rowNumber">The number of the row in the file, starting at 1.</param>
        /// <param name="map">The map of the columns.</param>
        /// <returns>The values.</returns>
        /// <exception cref="FormatException">A value cannot be converted into the type of its column.</exception>
        private static object[] ConvertValues(object[] values,
            long rowNumber,
            ColumnMap map)
        {
            for (var i = 0; i < values.Length; i++)
            {
                try
                {
                    values[i] = ValueConverter.Convert(values[i],
                        map.Fields[i].Type);
                }
                catch (FormatException e)
                {
                    throw new FormatException($"Row {rowNumber}, column '{map.Fields[i].Name}': {e.Message}",
                        e);
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
                table.Columns.Add(field.Name,
                    field.Type == null ? typeof(object) : Nullable.GetUnderlyingType(field.Type) ?? field.Type);
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
        /// <param name="qualifiers">The qualifiers of the merge, or <c>null</c> to insert the rows.</param>
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
            IList<Field> qualifiers,
            int size,
            ColumnMappingBehavior columnMappingBehavior,
            IdentityBehavior identityBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            ImportProgress progress,
            Action<ImportProgress> progressCallback)
        {
            int inserted;
            if (bulk)
            {
                using (var table = ToTable(map,
                    buffer))
                using (var reader = table.CreateDataReader())
                {
                    inserted = qualifiers != null ? BulkInsertInvoker.BulkMerge(connection,
                        tableName,
                        reader,
                        qualifiers,
                        size,
                        columnMappingBehavior,
                        identityBehavior == IdentityBehavior.KeepIdentity,
                        commandTimeout,
                        traceKey,
                        trace,
                        transaction) : BulkInsertInvoker.BulkInsert(connection,
                            tableName,
                            reader,
                            size,
                            columnMappingBehavior,
                            identityBehavior == IdentityBehavior.KeepIdentity,
                            commandTimeout,
                            traceKey,
                            trace,
                            transaction);
                }
            }
            else
            {
                inserted = qualifiers != null ? RowInserter.Merge(connection,
                    tableName,
                    ToRows(map,
                        buffer),
                    qualifiers,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction) : RowInserter.Insert(connection,
                        tableName,
                        ToRows(map,
                            buffer),
                        commandTimeout,
                        traceKey,
                        trace,
                        transaction);
            }

            Succeeded(buffer,
                inserted,
                progress,
                progressCallback);
        }

        /// <summary>
        /// Inserts the buffered rows in an asynchronous way, clears the buffer, and reports the progress.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="map">The map of the columns.</param>
        /// <param name="buffer">The buffered rows (cleared when they are inserted).</param>
        /// <param name="bulk">A value that indicates whether the rows are inserted with the bulk insert.</param>
        /// <param name="qualifiers">The qualifiers of the merge, or <c>null</c> to insert the rows.</param>
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
            IList<Field> qualifiers,
            int size,
            ColumnMappingBehavior columnMappingBehavior,
            IdentityBehavior identityBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            ImportProgress progress,
            Action<ImportProgress> progressCallback,
            CancellationToken cancellationToken)
        {
            int inserted;
            if (bulk)
            {
                using (var table = ToTable(map,
                    buffer))
                using (var reader = table.CreateDataReader())
                {
                    inserted = qualifiers != null ? await BulkInsertInvoker.BulkMergeAsync(connection,
                        tableName,
                        reader,
                        qualifiers,
                        size,
                        columnMappingBehavior,
                        identityBehavior == IdentityBehavior.KeepIdentity,
                        commandTimeout,
                        traceKey,
                        trace,
                        transaction,
                        cancellationToken).ConfigureAwait(false) : await BulkInsertInvoker.BulkInsertAsync(connection,
                            tableName,
                            reader,
                            size,
                            columnMappingBehavior,
                            identityBehavior == IdentityBehavior.KeepIdentity,
                            commandTimeout,
                            traceKey,
                            trace,
                            transaction,
                            cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                inserted = qualifiers != null ? await RowInserter.MergeAsync(connection,
                    tableName,
                    ToRows(map,
                        buffer),
                    qualifiers,
                    commandTimeout,
                    traceKey,
                    trace,
                    transaction,
                    cancellationToken).ConfigureAwait(false) : await RowInserter.InsertAsync(connection,
                        tableName,
                        ToRows(map,
                            buffer),
                        commandTimeout,
                        traceKey,
                        trace,
                        transaction,
                        cancellationToken).ConfigureAwait(false);
            }

            Succeeded(buffer,
                inserted,
                progress,
                progressCallback);
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
            ImportProgress progress,
            Action<ImportProgress> progressCallback)
        {
            buffer.Clear();
            progress.BatchNumber++;
            progress.LoadedRowCount += inserted;
            progressCallback?.Invoke(progress);
        }

        /// <summary>
        /// Reads, converts, intercepts, and flushes imported rows in bounded batches.
        /// </summary>
        /// <param name="source">The reader of the file.</param>
        /// <param name="map">The mapping of the file columns.</param>
        /// <param name="interception">The row interception state, or null.</param>
        /// <param name="buffer">The buffer shared with the flush operation.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="flush">Writes and clears a completed batch using the current interception mapping.</param>
        private static void ReadImportRows(IImportSource source,
            ColumnMap map,
            Interception interception,
            List<object[]> buffer,
            int bufferSize,
            Action flush)
        {
            using (var reader = source.Open())
            {
                while (true)
                {
                    var values = reader.Next(map.Index,
                        map.Fields.Count);
                    if (values == null)
                    {
                        break;
                    }

                    buffer.Add(interception == null ? ConvertValues(Copy(values,
                        map),
                        reader.RowNumber,
                        map) : interception.Apply(ConvertValues(Copy(values,
                            map),
                            reader.RowNumber,
                            map),
                            reader.RowNumber));
                    if (buffer.Count == bufferSize)
                    {
                        flush();
                    }
                }
            }

            if (buffer.Count > 0)
            {
                flush();
            }
        }

        /// <summary>
        /// Reads, converts, intercepts, and flushes imported rows in bounded batches.
        /// </summary>
        /// <param name="source">The reader of the file.</param>
        /// <param name="map">The mapping of the file columns.</param>
        /// <param name="interception">The row interception state, or null.</param>
        /// <param name="buffer">The buffer shared with the flush operation.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once.</param>
        /// <param name="flush">Writes and clears a completed batch using the current interception mapping.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing completion of the imported row batches.</returns>
        private static async Task ReadImportRowsAsync(IImportSource source,
            ColumnMap map,
            Interception interception,
            List<object[]> buffer,
            int bufferSize,
            Func<Task> flush,
            CancellationToken cancellationToken)
        {
            using (var reader = source.Open())
            {
                while (true)
                {
                    var values = await reader.NextAsync(map.Index,
                        map.Fields.Count,
                        cancellationToken).ConfigureAwait(false);
                    if (values == null)
                    {
                        break;
                    }

                    buffer.Add(interception == null ? ConvertValues(Copy(values,
                        map),
                        reader.RowNumber,
                        map) : interception.Apply(ConvertValues(Copy(values,
                            map),
                            reader.RowNumber,
                            map),
                            reader.RowNumber));
                    if (buffer.Count == bufferSize)
                    {
                        await flush().ConfigureAwait(false);
                    }
                }
            }

            if (buffer.Count > 0)
            {
                await flush().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Rejects null mappings and blank source or destination column names.
        /// </summary>
        /// <param name="mapping">The explicit column mapping to validate.</param>
        private static void ValidateMappingNames(ColumnMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentException("The column mappings must not contain a null item.",
                    "mappings");
            }

            if (string.IsNullOrWhiteSpace(mapping.SourceColumn) || string.IsNullOrWhiteSpace(mapping.DestinationColumn))
            {
                throw new ArgumentException("The source and the destination columns of a column mapping must not be empty.",
                    "mappings");
            }
        }

        /// <summary>
        /// Resolves merge qualifiers and prepares row interception before counting or importing rows.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the file are kept.</param>
        /// <param name="map">The source column mapping.</param>
        /// <param name="qualifierNames">The names of the columns that identify the existing rows, or <c>null</c> to use the primary key (or the identity) of the table. Used only if the existing rows are bypassed.</param>
        /// <param name="bypassExistingRows">A value that indicates whether the rows are merged (with the primary key, or the identity, as the qualifiers) instead of inserted.</param>
        /// <param name="dataInterceptors">The interceptors that every row of the file goes through before it is loaded, in order, or <c>null</c>.</param>
        /// <returns>The resolved merge qualifiers and interception state.</returns>
        private static (IList<Field> Qualifiers, Interception Interception) PrepareInterception(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            IdentityBehavior identityBehavior,
            ColumnMap map,
            IList<string> qualifierNames,
            bool bypassExistingRows,
            IList<IDataInterceptor<Row>> dataInterceptors)
        {
            var qualifiers = bypassExistingRows ? GetQualifiers(connection,
                tableName,
                map,
                qualifierNames,
                transaction) : null;
            var interception = CreateInterception(connection,
                tableName,
                transaction,
                identityBehavior,
                map,
                qualifiers,
                dataInterceptors);
            return (qualifiers, interception);
        }

        #endregion

        #region Private Types

        /// <summary>
        /// Maps the columns of the file to the columns of the table: the columns of the file that the table does not have are left out.
        /// </summary>
        private sealed class ColumnMap
        {
            #region Properties

            /// <summary>
            /// Gets the columns of the table that get the value of a column of the file that is mapped to more than one column: the position of the value, and the position of its copy.
            /// </summary>
            public IList<KeyValuePair<int, int>> Copies { get; } = new List<KeyValuePair<int, int>>();

            /// <summary>
            /// Gets the columns of the table that the file has, by their position in the values of a row.
            /// </summary>
            public IList<DbField> Fields { get; } = new List<DbField>();

            /// <summary>
            /// Gets the position of each column of the file (by its name, without the case) in the values of a row.
            /// </summary>
            public IDictionary<string, int> Index { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            #endregion
        }

        /// <summary>
        /// Runs the interceptors over the rows of the file: every row is given to them as a <see cref="Row"/> with the columns of the table that it is loaded into (named by them, typed
        /// by them), then the row that the last interceptor returns is what is loaded. The columns of that row are the ones that are loaded (the ones that the table does not have are
        /// left out, and so are the identity columns if the database generates them), and they must be the same for every row.
        /// </summary>
        private sealed class Interception
        {
            #region Private Variables

            private readonly IList<IDataInterceptor<Row>> _interceptors;
            private readonly List<Column> _columns;
            private readonly ColumnMap _source;
            private readonly Func<string, DbField> _find;
            private readonly IdentityBehavior _identityBehavior;
            private readonly IList<Field> _qualifiers;
            private readonly string _tableName;
            private ColumnMap _map;
            private string[] _names;
            private int[] _ordinals;

            #endregion

            #region Constructors

            /// <summary>
            /// Creates a new instance of <see cref="Interception"/>.
            /// </summary>
            /// <param name="interceptors">The ordered row interceptors.</param>
            /// <param name="source">The mapping of the source file columns.</param>
            /// <param name="find">Resolves a destination column name to its field metadata.</param>
            /// <param name="unquote">Removes provider quoting from a column name.</param>
            /// <param name="identityBehavior">Whether identity column values are included in the imported rows.</param>
            /// <param name="qualifiers">The columns identifying rows for a merge, or null for an insert.</param>
            /// <param name="tableName">The destination table name used in validation errors.</param>
            public Interception(IList<IDataInterceptor<Row>> interceptors,
                ColumnMap source,
                Func<string, DbField> find,
                Func<string, string> unquote,
                IdentityBehavior identityBehavior,
                IList<Field> qualifiers,
                string tableName)
            {
                _interceptors = interceptors;
                _source = source;
                _find = find;
                _identityBehavior = identityBehavior;
                _qualifiers = qualifiers;
                _tableName = tableName;
                _columns = source.Fields.Select(field => new Column(unquote(field.Name),
                    field.Type)).ToList();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the map of the columns that are loaded: the one of the file until the first row goes through the interceptors, then the one of the rows that they return.
            /// </summary>
            public ColumnMap Map => _map ?? _source;

            #endregion

            #region Public Methods

            /// <summary>
            /// Runs the interceptors over a row.
            /// </summary>
            /// <param name="values">The values of the row by the position of the columns of the map of the file, already converted into the types of the columns.</param>
            /// <param name="rowNumber">The number of the row in the file, starting at 1.</param>
            /// <returns>The values of the row that the last interceptor returned, by the position of the columns of <see cref="Map"/>.</returns>
            /// <exception cref="InvalidOperationException">An interceptor returned <c>null</c>, the rows do not have the same columns, or no column of the rows matches a column of the table.</exception>
            /// <exception cref="FormatException">A value cannot be converted into the type of its column.</exception>
            public object[] Apply(object[] values,
                long rowNumber)
            {
                var row = RowInterception.Apply(new Row(_columns,
                    values),
                    _interceptors);
                if (_names == null)
                {
                    Shape(row);
                }
                else if (!IsShape(row))
                {
                    throw new InvalidOperationException("The data interceptors must give every row the same columns, in the same order.");
                }

                var result = new object[_map.Fields.Count];
                for (var i = 0; i < result.Length; i++)
                {
                    try
                    {
                        result[i] = ValueConverter.Convert(row[_ordinals[i]],
                            _map.Fields[i].Type);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException($"Row {rowNumber}, column '{_map.Fields[i].Name}': {e.Message}",
                            e);
                    }
                }

                return result;
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Builds the destination mapping from the first intercepted row and validates merge qualifiers.
            /// </summary>
            /// <param name="row">The intercepted row to inspect.</param>
            private void Shape(Row row)
            {
                var map = new ColumnMap();
                var ordinals = new List<int>();
                for (var i = 0; i < row.Count; i++)
                {
                    var field = _find(row.Columns[i].Name);
                    if (field == null || (_identityBehavior == IdentityBehavior.Ignore && field.IsIdentity))
                    {
                        continue;
                    }

                    map.Fields.Add(field);
                    ordinals.Add(i);
                }

                if (map.Fields.Count == 0)
                {
                    throw new InvalidOperationException($"No column of the rows that the data interceptors give matches a column of the table '{_tableName}'.");
                }

                if (_qualifiers != null)
                {
                    var missing = _qualifiers.FirstOrDefault(qualifier => !map.Fields.Any(field => string.Equals(_find(qualifier.Name)?.Name,
                        field.Name,
                        StringComparison.OrdinalIgnoreCase)));
                    if (missing != null)
                    {
                        throw new InvalidOperationException($"The existing rows cannot be bypassed as the qualifier '{missing.Name}' of the table '{_tableName}' is not a column of the rows that the data interceptors give.");
                    }
                }

                _names = row.Columns.Select(column => column.Name).ToArray();
                _ordinals = ordinals.ToArray();
                _map = map;
            }

            /// <summary>
            /// Checks whether an intercepted row has the established columns in the same order.
            /// </summary>
            /// <param name="row">The intercepted row to inspect.</param>
            /// <returns>Whether the row matches the established import shape.</returns>
            private bool IsShape(Row row)
            {
                if (row.Count != _names.Length)
                {
                    return false;
                }

                for (var i = 0; i < _names.Length; i++)
                {
                    if (!string.Equals(row.Columns[i].Name,
                        _names[i],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                return true;
            }

            #endregion
        }

        #endregion
    }
}
