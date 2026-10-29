#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Reads the rows of an XML file forward only, without loading the file in memory: each child element of the root element is a row, and each child element of a row is a
    /// column (its name is the name of the column and its text is the value). The attributes are not columns. A self-closing element (<c>&lt;Name /&gt;</c>) is a <c>null</c> value, an element that has no content between its start and end tags (<c>&lt;Name&gt; &lt;/Name&gt;</c>) is an empty text (the value is converted to <c>null</c> when the column is not a text), and an element that has
    /// child elements has the XML of its content as the value.
    /// </summary>
    internal sealed class XmlRowReader : IImportRowReader
    {
        #region Private Variables

        private readonly FileStream _stream;
        private readonly XmlReader _reader;

        /// <summary>
        /// True when the reader needs to read the next node before it looks for the next row (it is not true right after a row was skipped).
        /// </summary>
        private bool _pendingRead = true;

        /// <summary>
        /// True once the root element was found.
        /// </summary>
        private bool _started;

        /// <summary>
        /// True once there is no other row.
        /// </summary>
        private bool _done;
        private long _rowNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="XmlRowReader"/> class and opens the file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public XmlRowReader(string path)
        {
            _stream = new FileStream(path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            _reader = XmlReader.Create(_stream,
                new XmlReaderSettings { Async = true, IgnoreComments = true, IgnoreProcessingInstructions = true, IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Prohibit });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of the row that was read last (the rows are counted from 1).
        /// </summary>
        public long RowNumber => _rowNumber;

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
            var cells = ReadCells();
            return cells == null ? null : ToValues(cells,
                index,
                width);
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
            var cells = await ReadCellsAsync(cancellationToken).ConfigureAwait(false);
            return cells == null ? null : ToValues(cells,
                index,
                width);
        }

        /// <summary>
        /// Reads the next row as its cells: the name of each column with its text.
        /// </summary>
        /// <returns>The cells, or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid XML.</exception>
        public IList<KeyValuePair<string, string>> ReadCells()
        {
            try
            {
                if (!MoveToRow())
                {
                    return null;
                }

                _rowNumber++;
                var cells = new List<KeyValuePair<string, string>>();
                using (var row = _reader.ReadSubtree())
                {
                    row.Read();
                    if (!row.IsEmptyElement)
                    {
                        while (row.Read())
                        {
                            if (row.NodeType == XmlNodeType.Element && row.Depth == 1)
                            {
                                var name = XmlConvert.DecodeName(row.LocalName);
                                cells.Add(new KeyValuePair<string, string>(name,
                                    ReadCell(row)));
                            }
                        }
                    }
                }

                return cells;
            }
            catch (XmlException e)
            {
                throw Invalid(e);
            }
        }

        /// <summary>
        /// Reads the next row as its cells in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the cells, or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid XML.</exception>
        public async Task<IList<KeyValuePair<string, string>>> ReadCellsAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!await MoveToRowAsync(cancellationToken).ConfigureAwait(false))
                {
                    return null;
                }

                _rowNumber++;
                var cells = new List<KeyValuePair<string, string>>();
                using (var row = _reader.ReadSubtree())
                {
                    await row.ReadAsync().ConfigureAwait(false);
                    if (!row.IsEmptyElement)
                    {
                        while (await row.ReadAsync().ConfigureAwait(false))
                        {
                            if (row.NodeType == XmlNodeType.Element && row.Depth == 1)
                            {
                                var name = XmlConvert.DecodeName(row.LocalName);
                                cells.Add(new KeyValuePair<string, string>(name,
                                    await ReadCellAsync(row,
                                        cancellationToken).ConfigureAwait(false)));
                            }
                        }
                    }
                }

                return cells;
            }
            catch (XmlException e)
            {
                throw Invalid(e);
            }
        }

        /// <summary>
        /// Moves over the next row without reading its cells.
        /// </summary>
        /// <returns><c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid XML.</exception>
        public bool Skip()
        {
            try
            {
                if (!MoveToRow())
                {
                    return false;
                }

                _rowNumber++;
                _reader.Skip();
                _pendingRead = false;
                return true;
            }
            catch (XmlException e)
            {
                throw Invalid(e);
            }
        }

        /// <summary>
        /// Moves over the next row without reading its cells in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid XML.</exception>
        public async Task<bool> SkipAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!await MoveToRowAsync(cancellationToken).ConfigureAwait(false))
                {
                    return false;
                }

                _rowNumber++;
                await _reader.SkipAsync().ConfigureAwait(false);
                _pendingRead = false;
                return true;
            }
            catch (XmlException e)
            {
                throw Invalid(e);
            }
        }

        /// <summary>
        /// Releases the row reader and its file resources.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
            _stream.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Moves to the start of the next row (a child element of the root element).
        /// </summary>
        /// <returns><c>true</c> if the reader is on the next row; <c>false</c> if there is no other row.</returns>
        private bool MoveToRow()
        {
            while (!_done)
            {
                if (_pendingRead && !_reader.Read())
                {
                    return Finish();
                }

                _pendingRead = true;
                if (Examine())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Moves to the start of the next row in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the reader is on the next row; <c>false</c> if there is no other row.</returns>
        private async Task<bool> MoveToRowAsync(CancellationToken cancellationToken)
        {
            while (!_done)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_pendingRead && !await _reader.ReadAsync().ConfigureAwait(false))
                {
                    return Finish();
                }

                _pendingRead = true;
                if (Examine())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Looks at the node that the reader is on: finds the root element, then the rows.
        /// </summary>
        /// <returns><c>true</c> if the node is the start of a row.</returns>
        private bool Examine()
        {
            if (!_started)
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    _started = true;
                    if (_reader.IsEmptyElement)
                    {
                        _done = true;
                    }
                }

                return false;
            }

            if (_reader.NodeType == XmlNodeType.Element && _reader.Depth == 1)
            {
                return true;
            }

            if (_reader.NodeType == XmlNodeType.EndElement && _reader.Depth == 0)
            {
                _done = true;
            }

            return false;
        }

        /// <summary>
        /// Ends the rows because the file has no other node.
        /// </summary>
        /// <returns>Always <c>false</c>.</returns>
        /// <exception cref="FormatException">The file has no root element.</exception>
        private bool Finish()
        {
            _done = true;
            if (!_started)
            {
                throw new FormatException("The file is not valid XML: it has no root element.");
            }

            return false;
        }

        /// <summary>
        /// Reads the text of a cell: the reader of the row is left on the end of the cell.
        /// </summary>
        /// <param name="row">The reader of the row, on the start of the cell.</param>
        /// <returns>The text (empty if the cell has no content between its start and end tags), or <c>null</c> if the cell is self-closing.</returns>
        private static string ReadCell(XmlReader row)
        {
            if (row.IsEmptyElement)
            {
                return null;
            }

            var depth = row.Depth;
            var text = new StringBuilder();
            var more = row.Read();
            while (more)
            {
                if (row.NodeType == XmlNodeType.EndElement && row.Depth == depth)
                {
                    break;
                }

                if (row.NodeType == XmlNodeType.Element)
                {
                    text.Append(row.ReadOuterXml());
                    continue;
                }

                if (row.NodeType == XmlNodeType.Text || row.NodeType == XmlNodeType.CDATA || row.NodeType == XmlNodeType.SignificantWhitespace || row.NodeType == XmlNodeType.Whitespace)
                {
                    text.Append(row.Value);
                }

                more = row.Read();
            }

            return text.ToString();
        }

        /// <summary>
        /// Reads the text of a cell in an asynchronous way: the reader of the row is left on the end of the cell.
        /// </summary>
        /// <param name="row">The reader of the row, on the start of the cell.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the text (empty if the cell has no content between its start and end tags), or <c>null</c> if the cell is self-closing.</returns>
        private static async Task<string> ReadCellAsync(XmlReader row,
            CancellationToken cancellationToken)
        {
            if (row.IsEmptyElement)
            {
                return null;
            }

            var depth = row.Depth;
            var text = new StringBuilder();
            var more = await row.ReadAsync().ConfigureAwait(false);
            while (more)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (row.NodeType == XmlNodeType.EndElement && row.Depth == depth)
                {
                    break;
                }

                if (row.NodeType == XmlNodeType.Element)
                {
                    text.Append(await row.ReadOuterXmlAsync().ConfigureAwait(false));
                    continue;
                }

                if (row.NodeType == XmlNodeType.Text || row.NodeType == XmlNodeType.CDATA || row.NodeType == XmlNodeType.SignificantWhitespace || row.NodeType == XmlNodeType.Whitespace)
                {
                    text.Append(row.Value);
                }

                more = await row.ReadAsync().ConfigureAwait(false);
            }

            return text.ToString();
        }

        /// <summary>
        /// Gets the values of the columns of a row.
        /// </summary>
        /// <param name="cells">The cells of the row.</param>
        /// <param name="index">The position of each column (by its name, without the case) in the array of the values.</param>
        /// <param name="width">The number of columns.</param>
        /// <returns>The values by the position of the columns.</returns>
        private static object[] ToValues(IList<KeyValuePair<string, string>> cells,
            IDictionary<string, int> index,
            int width)
        {
            var values = new object[width];
            foreach (var cell in cells)
            {
                if (index.TryGetValue(cell.Key,
                    out var position))
                {
                    values[position] = cell.Value;
                }
            }

            return values;
        }

        /// <summary>
        /// Creates an error describing invalid XML row content.
        /// </summary>
        /// <param name="e">The XML parser exception that caused the invalid input error.</param>
        /// <returns>An exception describing the invalid content.</returns>
        private static FormatException Invalid(XmlException e) =>
            new FormatException("The file is not valid XML: " + e.Message,
                e);

        #endregion
    }
}
