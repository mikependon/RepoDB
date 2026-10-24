#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Reads a CSV file that holds the rows of a table without loading it in memory: it finds the columns (the header), counts the rows, and opens the reader of the rows
    /// (see <see cref="CsvRowReader"/> for the shape of the file that is supported).
    /// </summary>
    internal static class CsvHelper
    {
        #region Public Methods

        /// <summary>
        /// Opens the file to read its rows.
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        /// <returns>The reader of the rows, to be disposed by the caller.</returns>
        public static CsvRowReader Open(string filename,
            char separator) =>
            new CsvRowReader(filename, separator);

        /// <summary>
        /// Reads the header of the file: the names of the columns, without repeating a name (the case is ignored) or the names that are empty.
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        /// <returns>The names of the columns, in the order that they are in the header.</returns>
        /// <exception cref="InvalidOperationException">The file has no header.</exception>
        public static IList<string> ReadColumns(string filename,
            char separator)
        {
            using (var reader = Open(filename, separator))
            {
                return Distinct(reader.ReadHeader());
            }
        }

        /// <summary>
        /// Reads the header of the file in an asynchronous way.
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns.</returns>
        /// <exception cref="InvalidOperationException">The file has no header.</exception>
        public static async Task<IList<string>> ReadColumnsAsync(string filename,
            char separator,
            CancellationToken cancellationToken = default)
        {
            using (var reader = Open(filename, separator))
            {
                return Distinct(await reader.ReadHeaderAsync(cancellationToken).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Counts the rows of the file (the header is not a row).
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        /// <returns>The number of rows.</returns>
        public static long CountRows(string filename,
            char separator)
        {
            var count = 0L;
            using (var reader = Open(filename, separator))
            {
                while (reader.Skip())
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Counts the rows of the file in an asynchronous way (the header is not a row).
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows.</returns>
        public static async Task<long> CountRowsAsync(string filename,
            char separator,
            CancellationToken cancellationToken = default)
        {
            var count = 0L;
            using (var reader = Open(filename, separator))
            {
                while (await reader.SkipAsync(cancellationToken).ConfigureAwait(false))
                {
                    count++;
                }
            }
            return count;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the names of the header without repeating a name (the case is ignored) and without the ones that are empty.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>The names.</returns>
        private static IList<string> Distinct(string[] header)
        {
            var columns = new List<string>();
            foreach (var name in header)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                var found = false;
                for (var i = 0; i < columns.Count && !found; i++)
                {
                    found = string.Equals(columns[i], name, StringComparison.OrdinalIgnoreCase);
                }
                if (!found)
                {
                    columns.Add(name);
                }
            }
            return columns;
        }

        #endregion
    }

    /// <summary>
    /// The CSV file as a source of rows (see <see cref="CsvHelper"/>).
    /// </summary>
    internal sealed class CsvImportSource : IImportSource
    {
        private readonly string _filename;
        private readonly char _separator;

        /// <summary>
        /// Creates a new instance of <see cref="CsvImportSource"/> class.
        /// </summary>
        /// <param name="filename">The path of the CSV file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        public CsvImportSource(string filename,
            char separator)
        {
            _filename = filename;
            _separator = separator;
        }

        /// <inheritdoc />
        public bool HasAllColumns => true;

        /// <inheritdoc />
        public void Validate()
        {
            if (_separator == '"' || _separator == '\r' || _separator == '\n')
            {
                throw new ArgumentException("The separator must not be a quote or a line break.", "separator");
            }
        }

        /// <inheritdoc />
        public IList<string> ReadColumns() =>
            CsvHelper.ReadColumns(_filename, _separator);

        /// <inheritdoc />
        public Task<IList<string>> ReadColumnsAsync(CancellationToken cancellationToken) =>
            CsvHelper.ReadColumnsAsync(_filename, _separator, cancellationToken);

        /// <inheritdoc />
        public long CountRows() =>
            CsvHelper.CountRows(_filename, _separator);

        /// <inheritdoc />
        public Task<long> CountRowsAsync(CancellationToken cancellationToken) =>
            CsvHelper.CountRowsAsync(_filename, _separator, cancellationToken);

        /// <inheritdoc />
        public IImportRowReader Open() =>
            CsvHelper.Open(_filename, _separator);
    }
}
