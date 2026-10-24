#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Helpers;
using RepoDb.Data.Models;
using RepoDb.Interfaces;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for loading the rows of a CSV file into a table.
    /// </summary>
    public static class ImportDataFromCsvExtension
    {
        #region Public Methods

        #region ImportDataFromCsv (Sync)

        /// <summary>
        /// Loads the rows of a CSV file into a table without loading the file in memory. The rows are buffered, and every <paramref name="bufferSize"/> rows they are inserted with the bulk insert of the provider (in batches of plain inserts if it has none). The first line is the header (the names of the columns), and the fields of a line are separated by the <paramref name="separator"/> and can be quoted (as RFC 4180): an empty field is <c>null</c>, and an empty quoted field is an empty text. The columns that the table does not have are ignored, and the values are converted into the types of the columns of the table.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields of a line. It must not be a quote or a line break. The default is <c>,</c>.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once with the bulk insert. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the table as the qualifiers (its identity column if it has no primary key), which must be columns of the file. An <see cref="InvalidOperationException"/> is thrown if the table has neither, or the provider has no bulk merge. The default is <c>false</c>.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the file are kept, or ignored so the database generates them. The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="LoadProgress"/> after the rows are counted, after every batch is inserted, and when the load has ended. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ImportDataFromCsv"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <returns>The <see cref="LoadProgress"/> of the load, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The buffer size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="ArgumentException">The separator is a quote or a line break.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FormatException">The file is not valid CSV, a row has more fields than the header, or a value cannot be converted into the type of its column.</exception>
        /// <exception cref="InvalidOperationException">The file has no header (it is empty, or its first line has no name), no column of the file matches a column of the table, or the existing rows are bypassed and the table has no qualifiers (a primary key or an identity column) among the columns of the file, or the provider has no bulk merge.</exception>
        public static LoadProgress ImportDataFromCsv(this IDbConnection connection,
            string tableName,
            string filename,
            char separator = ',',
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            LoadColumnMappingBehavior columnMappingBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            Action<LoadProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ImportDataFromCsv,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            return ImportPipeline.Run(connection, tableName, filename, new CsvImportSource(filename, separator), bufferSize, bypassExistingRows, columnMappingBehavior, identityBehavior, progressCallback, commandTimeout, traceKey, trace, transaction);
        }

        #endregion

        #region ImportDataFromCsv (Async)

        /// <summary>
        /// Loads the rows of a CSV file into a table in an asynchronous way. See <see cref="ImportDataFromCsv"/> for how the rows are read, buffered and inserted.
        /// </summary>
        /// <param name="connection">The connection of the database that has the table.</param>
        /// <param name="tableName">The name of the table that the rows are loaded into.</param>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields of a line. It must not be a quote or a line break. The default is <c>,</c>.</param>
        /// <param name="bufferSize">The number of rows that are buffered and inserted at once with the bulk insert. The default is 10000.</param>
        /// <param name="bypassExistingRows">Defines whether the rows that already exist in the table are bypassed: the rows are bulk merged instead of bulk inserted, with the primary key columns of the table as the qualifiers (its identity column if it has no primary key), which must be columns of the file. An <see cref="InvalidOperationException"/> is thrown if the table has neither, or the provider has no bulk merge. The default is <c>false</c>.</param>
        /// <param name="columnMappingBehavior">Defines how the columns of the file are aligned with the columns of the table by the bulk insert of the provider (ignored if it has none). The default is <see cref="LoadColumnMappingBehavior.Automatic"/>.</param>
        /// <param name="identityBehavior">Defines whether the identity values of the file are kept, or ignored so the database generates them. The default is <see cref="LoadIdentityBehavior.KeepIdentity"/>.</param>
        /// <param name="progressCallback">The callback that receives the <see cref="LoadProgress"/> after the rows are counted, after every batch is inserted, and when the load has ended. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.ImportDataFromCsv"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="LoadProgress"/> of the load, which is the same instance that the callback receives.</returns>
        /// <exception cref="ArgumentNullException">The connection, the table name or the file name is <c>null</c> (or white space).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The buffer size is less than 1, or a behavior is not defined.</exception>
        /// <exception cref="ArgumentException">The separator is a quote or a line break.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="FormatException">The file is not valid CSV, a row has more fields than the header, or a value cannot be converted into the type of its column.</exception>
        /// <exception cref="InvalidOperationException">The file has no header (it is empty, or its first line has no name), no column of the file matches a column of the table, or the existing rows are bypassed and the table has no qualifiers (a primary key or an identity column) among the columns of the file, or the provider has no bulk merge.</exception>
        public static Task<LoadProgress> ImportDataFromCsvAsync(this IDbConnection connection,
            string tableName,
            string filename,
            char separator = ',',
            int bufferSize = 10000,
            bool bypassExistingRows = false,
            LoadColumnMappingBehavior columnMappingBehavior = LoadColumnMappingBehavior.Automatic,
            LoadIdentityBehavior identityBehavior = LoadIdentityBehavior.KeepIdentity,
            Action<LoadProgress> progressCallback = null,
            int? commandTimeout = null,
            string traceKey = DataTraceKeys.ImportDataFromCsv,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return ImportPipeline.RunAsync(connection, tableName, filename, new CsvImportSource(filename, separator), bufferSize, bypassExistingRows, columnMappingBehavior, identityBehavior, progressCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);
        }

        #endregion

        #endregion

    }
}
