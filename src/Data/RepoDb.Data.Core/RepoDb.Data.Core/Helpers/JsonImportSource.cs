#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// The JSON file as a source of rows (see <see cref="JsonHelper"/>).
    /// </summary>
    internal sealed class JsonImportSource : IImportSource
    {
        #region Private Variables

        private readonly string _filename;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="JsonImportSource"/> class.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
        public JsonImportSource(string filename)
        {
            _filename = filename;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the discovered columns cover the entire file; always false because only initial rows are sampled.
        /// </summary>
        public bool HasAllColumns => false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates source options; this format has no additional options to validate.
        /// </summary>
        public void Validate()
        {
        }

        /// <summary>
        /// Reads the file to find its columns: the names without repeating a name (the case is ignored), in the order that they were found.
        /// </summary>
        /// <returns>The names of the columns.</returns>
        public IList<string> ReadColumns() =>
            JsonHelper.ReadColumns(_filename);

        /// <summary>
        /// Reads the file to find its columns in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns.</returns>
        public Task<IList<string>> ReadColumnsAsync(CancellationToken cancellationToken) =>
            JsonHelper.ReadColumnsAsync(_filename,
                cancellationToken);

        /// <summary>
        /// Counts the rows of the file.
        /// </summary>
        /// <returns>The number of rows.</returns>
        public long CountRows() =>
            JsonHelper.CountRows(_filename);

        /// <summary>
        /// Counts the rows of the file in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows.</returns>
        public Task<long> CountRowsAsync(CancellationToken cancellationToken) =>
            JsonHelper.CountRowsAsync(_filename,
                cancellationToken);

        /// <summary>
        /// Opens the file to read its rows.
        /// </summary>
        /// <returns>The reader of the rows, to be disposed by the caller.</returns>
        public IImportRowReader Open() =>
            new RowReader(JsonHelper.Open(_filename));

        #endregion

        #region Private Types

        /// <summary>
        /// Adapts the JSON stream reader to the shared import row interface.
        /// </summary>
        private sealed class RowReader : IImportRowReader
        {
            #region Private Variables

            private readonly JsonRowReader _reader;

            #endregion

            #region Constructors

            /// <summary>
            /// Creates a new instance of <see cref="RowReader"/>.
            /// </summary>
            /// <param name="reader">The forward-only JSON reader owned by this row reader.</param>
            public RowReader(JsonRowReader reader)
            {
                _reader = reader;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the number of the row that was read last (the rows are counted from 1).
            /// </summary>
            public long RowNumber => _reader.RowNumber;

            #endregion

            #region Public Methods

            /// <summary>
            /// Reads the next row.
            /// </summary>
            /// <param name="index">The position of each column (by its name, without the case) in the array of the values. The columns that are not in it are left out.</param>
            /// <param name="width">The number of columns in the array of the values.</param>
            /// <returns>The values by the position of the columns (<c>null</c> for a missing value), or <c>null</c> if there is no other row.</returns>
            /// <exception cref="FormatException">The file is not valid, or a row is not valid.</exception>
            public object[] Next(IDictionary<string, int> index,
                int width)
            {
                using (var row = _reader.Next())
                {
                    return row == null ? null : JsonHelper.GetValues(row,
                        index,
                        width);
                }
            }

            /// <summary>
            /// Reads the next row in an asynchronous way.
            /// </summary>
            /// <param name="index">The position of each column (by its name, without the case) in the array of the values. The columns that are not in it are left out.</param>
            /// <param name="width">The number of columns in the array of the values.</param>
            /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
            /// <returns>A task that represents the asynchronous operation. The task result is the values by the position of the columns, or <c>null</c> if there is no other row.</returns>
            /// <exception cref="FormatException">The file is not valid, or a row is not valid.</exception>
            public async Task<object[]> NextAsync(IDictionary<string, int> index,
                int width,
                CancellationToken cancellationToken)
            {
                using (var row = await _reader.NextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return row == null ? null : JsonHelper.GetValues(row,
                        index,
                        width);
                }
            }

            /// <summary>
            /// Releases the row reader and its file resources.
            /// </summary>
            public void Dispose() =>
                _reader.Dispose();

            #endregion
        }

        #endregion
    }
}
