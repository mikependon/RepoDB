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
        private readonly string _filename;

        /// <summary>
        /// Creates a new instance of <see cref="JsonImportSource"/> class.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
        public JsonImportSource(string filename)
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
            JsonHelper.ReadColumns(_filename);

        /// <inheritdoc />
        public Task<IList<string>> ReadColumnsAsync(CancellationToken cancellationToken) =>
            JsonHelper.ReadColumnsAsync(_filename, cancellationToken);

        /// <inheritdoc />
        public long CountRows() =>
            JsonHelper.CountRows(_filename);

        /// <inheritdoc />
        public Task<long> CountRowsAsync(CancellationToken cancellationToken) =>
            JsonHelper.CountRowsAsync(_filename, cancellationToken);

        /// <inheritdoc />
        public IImportRowReader Open() =>
            new RowReader(JsonHelper.Open(_filename));

        private sealed class RowReader : IImportRowReader
        {
            private readonly JsonRowReader _reader;

            public RowReader(JsonRowReader reader)
            {
                _reader = reader;
            }

            public long RowNumber => _reader.RowNumber;

            public object[] Next(IDictionary<string, int> index,
                int width)
            {
                using (var row = _reader.Next())
                {
                    return row == null ? null : JsonHelper.GetValues(row, index, width);
                }
            }

            public async Task<object[]> NextAsync(IDictionary<string, int> index,
                int width,
                CancellationToken cancellationToken)
            {
                using (var row = await _reader.NextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return row == null ? null : JsonHelper.GetValues(row, index, width);
                }
            }

            public void Dispose() =>
                _reader.Dispose();
        }
    }
}
