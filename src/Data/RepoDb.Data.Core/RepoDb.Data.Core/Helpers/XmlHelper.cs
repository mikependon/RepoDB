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
    /// Reads an XML file that holds the rows of a table without loading it in memory: it finds the columns of the rows, counts the rows, and opens the reader of the rows
    /// (see <see cref="XmlRowReader"/> for the shape of the file that is supported).
    /// </summary>
    internal static class XmlHelper
    {
        #region Public Methods

        /// <summary>
        /// Opens the file to read its rows.
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        /// <returns>The reader of the rows, to be disposed by the caller.</returns>
        public static XmlRowReader Open(string filename) =>
            new XmlRowReader(filename);

        /// <summary>
        /// Reads the first rows of the file to find the columns: the names of the child elements of the rows, without repeating a name (the case is ignored).
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        /// <returns>The names of the columns, in the order that they were found.</returns>
        public static IList<string> ReadColumns(string filename)
        {
            var columns = new List<string>();
            using (var reader = Open(filename))
            {
                for (var i = 0; i < JsonHelper.ProbeRows; i++)
                {
                    var cells = reader.ReadCells();
                    if (cells == null)
                    {
                        break;
                    }
                    AddColumns(columns, cells);
                }
            }
            return columns;
        }

        /// <summary>
        /// Reads the first rows of the file to find the columns in an asynchronous way.
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns, in the order that they were found.</returns>
        public static async Task<IList<string>> ReadColumnsAsync(string filename,
            CancellationToken cancellationToken = default)
        {
            var columns = new List<string>();
            using (var reader = Open(filename))
            {
                for (var i = 0; i < JsonHelper.ProbeRows; i++)
                {
                    var cells = await reader.ReadCellsAsync(cancellationToken).ConfigureAwait(false);
                    if (cells == null)
                    {
                        break;
                    }
                    AddColumns(columns, cells);
                }
            }
            return columns;
        }

        /// <summary>
        /// Counts the rows of the file, by moving over them without reading their cells.
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        /// <returns>The number of rows.</returns>
        public static long CountRows(string filename)
        {
            var count = 0L;
            using (var reader = Open(filename))
            {
                while (reader.Skip())
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Counts the rows of the file in an asynchronous way, by moving over them without reading their cells.
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows.</returns>
        public static async Task<long> CountRowsAsync(string filename,
            CancellationToken cancellationToken = default)
        {
            var count = 0L;
            using (var reader = Open(filename))
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
        /// Adds the names of the cells of a row that are not in the columns yet.
        /// </summary>
        /// <param name="columns">The columns that were found so far.</param>
        /// <param name="cells">The cells of the row.</param>
        private static void AddColumns(IList<string> columns,
            IList<KeyValuePair<string, string>> cells)
        {
            foreach (var cell in cells)
            {
                var found = false;
                for (var i = 0; i < columns.Count && !found; i++)
                {
                    found = string.Equals(columns[i], cell.Key, StringComparison.OrdinalIgnoreCase);
                }
                if (!found)
                {
                    columns.Add(cell.Key);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The XML file as a source of rows (see <see cref="XmlHelper"/>).
    /// </summary>
    internal sealed class XmlImportSource : IImportSource
    {
        private readonly string _filename;

        /// <summary>
        /// Creates a new instance of <see cref="XmlImportSource"/> class.
        /// </summary>
        /// <param name="filename">The path of the XML file.</param>
        public XmlImportSource(string filename)
        {
            _filename = filename;
        }

        /// <inheritdoc />
        public bool HasAllColumns => false;

        /// <inheritdoc />
        public void Validate()
        {
        }

        /// <inheritdoc />
        public IList<string> ReadColumns() =>
            XmlHelper.ReadColumns(_filename);

        /// <inheritdoc />
        public Task<IList<string>> ReadColumnsAsync(CancellationToken cancellationToken) =>
            XmlHelper.ReadColumnsAsync(_filename, cancellationToken);

        /// <inheritdoc />
        public long CountRows() =>
            XmlHelper.CountRows(_filename);

        /// <inheritdoc />
        public Task<long> CountRowsAsync(CancellationToken cancellationToken) =>
            XmlHelper.CountRowsAsync(_filename, cancellationToken);

        /// <inheritdoc />
        public IImportRowReader Open() =>
            XmlHelper.Open(_filename);
    }
}
