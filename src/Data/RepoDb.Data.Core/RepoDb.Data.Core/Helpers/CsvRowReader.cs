#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Reads the rows of a CSV file forward only, without loading the file in memory (only a block of characters is held). The first record is the header: the names of the
    /// columns. The fields follow RFC 4180: a field can be quoted, and a quoted field can have the separator, line breaks, and quotes (doubled). An unquoted empty field is a
    /// <c>null</c> value, and a quoted empty field (<c>""</c>) is an empty text. The blank lines are ignored. The encoding is UTF-8 unless the file has a byte order mark.
    /// </summary>
    internal sealed class CsvRowReader : IImportRowReader
    {
        #region Private Types

        /// <summary>
        /// Where the parser is in a field.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// At the start of a field.
            /// </summary>
            Start,

            /// <summary>
            /// In a field that is not quoted.
            /// </summary>
            Plain,

            /// <summary>
            /// In a quoted field.
            /// </summary>
            Quoted,

            /// <summary>
            /// Right after a quote in a quoted field: it closes the field, or it is the first of two quotes that stand for one.
            /// </summary>
            QuoteSeen,

            /// <summary>
            /// After the quote that closed a quoted field: only the separator or the end of the record can follow.
            /// </summary>
            Closed
        }

        #endregion

        #region Private Variables

        private readonly FileStream _stream;
        private readonly StreamReader _reader;
        private readonly char _separator;
        private readonly char[] _chars = new char[4096];
        private readonly StringBuilder _field = new StringBuilder();
        private readonly List<string> _fields = new List<string>();
        private int _position;
        private int _length;
        private bool _eof;
        private State _state = State.Start;
        private bool _quoted;
        private bool _skipLineFeed;
        private string[] _header;
        private long _rowNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CsvRowReader"/> class and opens the file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="separator">The character that separates the fields.</param>
        public CsvRowReader(string path,
            char separator)
        {
            _separator = separator;
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            _reader = new StreamReader(_stream, new UTF8Encoding(false), true, 4096);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public long RowNumber => _rowNumber;

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the header: the names of the columns (the name of a column without a name is <c>null</c>).
        /// </summary>
        /// <returns>The names of the columns.</returns>
        /// <exception cref="InvalidOperationException">The file has no header: it is empty, or its first line has no name.</exception>
        /// <exception cref="FormatException">The file is not valid CSV.</exception>
        public string[] ReadHeader()
        {
            if (_header == null)
            {
                _header = Validated(ReadRecord());
            }
            return _header;
        }

        /// <summary>
        /// Reads the header in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns.</returns>
        /// <exception cref="InvalidOperationException">The file has no header: it is empty, or its first line has no name.</exception>
        /// <exception cref="FormatException">The file is not valid CSV.</exception>
        public async Task<string[]> ReadHeaderAsync(CancellationToken cancellationToken)
        {
            if (_header == null)
            {
                _header = Validated(await ReadRecordAsync(cancellationToken).ConfigureAwait(false));
            }
            return _header;
        }

        /// <inheritdoc />
        public object[] Next(IDictionary<string, int> index,
            int width)
        {
            var header = ReadHeader();
            var record = ReadRecord();
            return record == null ? null : ToValues(record, header, index, width);
        }

        /// <inheritdoc />
        public async Task<object[]> NextAsync(IDictionary<string, int> index,
            int width,
            CancellationToken cancellationToken)
        {
            var header = await ReadHeaderAsync(cancellationToken).ConfigureAwait(false);
            var record = await ReadRecordAsync(cancellationToken).ConfigureAwait(false);
            return record == null ? null : ToValues(record, header, index, width);
        }

        /// <summary>
        /// Moves over the next row (the header is read first).
        /// </summary>
        /// <returns><c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        public bool Skip()
        {
            ReadHeader();
            if (ReadRecord() == null)
            {
                return false;
            }
            _rowNumber++;
            return true;
        }

        /// <summary>
        /// Moves over the next row in an asynchronous way (the header is read first).
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        public async Task<bool> SkipAsync(CancellationToken cancellationToken)
        {
            await ReadHeaderAsync(cancellationToken).ConfigureAwait(false);
            if (await ReadRecordAsync(cancellationToken).ConfigureAwait(false) == null)
            {
                return false;
            }
            _rowNumber++;
            return true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _reader.Dispose();
            _stream.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks that the first record is a header.
        /// </summary>
        /// <param name="record">The first record of the file, or <c>null</c> if the file is empty.</param>
        /// <returns>The header.</returns>
        /// <exception cref="InvalidOperationException">The file has no header.</exception>
        private static string[] Validated(string[] record)
        {
            if (record == null || record.All(name => string.IsNullOrWhiteSpace(name)))
            {
                throw new InvalidOperationException("The CSV file must have a header: its first line must have the names of the columns.");
            }
            return record;
        }

        /// <summary>
        /// Gets the values of the columns of a row.
        /// </summary>
        /// <param name="record">The fields of the row.</param>
        /// <param name="header">The names of the columns.</param>
        /// <param name="index">The position of each column (by its name, without the case) in the array of the values.</param>
        /// <param name="width">The number of columns.</param>
        /// <returns>The values by the position of the columns.</returns>
        /// <exception cref="FormatException">The row has more fields than the header has.</exception>
        private object[] ToValues(string[] record,
            string[] header,
            IDictionary<string, int> index,
            int width)
        {
            _rowNumber++;
            if (record.Length > header.Length)
            {
                throw new FormatException($"Row {_rowNumber} has {record.Length} fields, which are more than the {header.Length} of the header.");
            }
            var values = new object[width];
            for (var i = 0; i < record.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(header[i]) && index.TryGetValue(header[i], out var position))
                {
                    values[position] = record[i];
                }
            }
            return values;
        }

        /// <summary>
        /// Reads the next record (a line, or more if a quoted field has line breaks).
        /// </summary>
        /// <returns>The fields, or <c>null</c> if there is no other record.</returns>
        private string[] ReadRecord()
        {
            while (true)
            {
                if (_position == _length)
                {
                    if (_eof)
                    {
                        break;
                    }
                    _length = _reader.Read(_chars, 0, _chars.Length);
                    _position = 0;
                    if (_length == 0)
                    {
                        _eof = true;
                        break;
                    }
                }
                if (Push(_chars[_position++]))
                {
                    return TakeRecord();
                }
            }
            return Finish() ? TakeRecord() : null;
        }

        /// <summary>
        /// Reads the next record in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the fields, or <c>null</c> if there is no other record.</returns>
        private async Task<string[]> ReadRecordAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_position == _length)
                {
                    if (_eof)
                    {
                        break;
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    _length = await _reader.ReadAsync(_chars, 0, _chars.Length).ConfigureAwait(false);
                    _position = 0;
                    if (_length == 0)
                    {
                        _eof = true;
                        break;
                    }
                }
                if (Push(_chars[_position++]))
                {
                    return TakeRecord();
                }
            }
            return Finish() ? TakeRecord() : null;
        }

        /// <summary>
        /// Takes the fields of the record that ended.
        /// </summary>
        /// <returns>The fields.</returns>
        private string[] TakeRecord()
        {
            var record = _fields.ToArray();
            _fields.Clear();
            return record;
        }

        /// <summary>
        /// Feeds the parser with the next character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><c>true</c> if the character ended a record.</returns>
        /// <exception cref="FormatException">The character is not valid where it is.</exception>
        private bool Push(char c)
        {
            if (_skipLineFeed)
            {
                _skipLineFeed = false;
                if (c == '\n')
                {
                    return false;
                }
            }
            if (_state == State.QuoteSeen)
            {
                if (c == '"')
                {
                    _field.Append('"');
                    _state = State.Quoted;
                    return false;
                }
                _state = State.Closed;
            }
            switch (_state)
            {
                case State.Quoted:
                    if (c == '"')
                    {
                        _state = State.QuoteSeen;
                    }
                    else
                    {
                        _field.Append(c);
                    }
                    return false;
                case State.Closed:
                    if (c == _separator)
                    {
                        EndField();
                        return false;
                    }
                    if (c == '\r' || c == '\n')
                    {
                        return EndRecord(c);
                    }
                    throw new FormatException($"The file is not valid CSV: the quoted field of row {_rowNumber + 1} is followed by '{c}'.");
                default:
                    if (c == _separator)
                    {
                        EndField();
                        return false;
                    }
                    if (c == '\r' || c == '\n')
                    {
                        return EndRecord(c);
                    }
                    if (c == '"' && _state == State.Start)
                    {
                        _state = State.Quoted;
                        _quoted = true;
                        return false;
                    }
                    _field.Append(c);
                    _state = State.Plain;
                    return false;
            }
        }

        /// <summary>
        /// Ends the field in hand: an unquoted empty field is <c>null</c>.
        /// </summary>
        private void EndField()
        {
            _fields.Add(_quoted || _field.Length > 0 ? _field.ToString() : null);
            _field.Clear();
            _quoted = false;
            _state = State.Start;
        }

        /// <summary>
        /// Ends the record at a line break (a blank line is not a record).
        /// </summary>
        /// <param name="c">The character of the line break.</param>
        /// <returns><c>true</c> if a record ended.</returns>
        private bool EndRecord(char c)
        {
            _skipLineFeed = c == '\r';
            if (_fields.Count == 0 && _state == State.Start)
            {
                return false;
            }
            EndField();
            return true;
        }

        /// <summary>
        /// Ends the last record at the end of the file.
        /// </summary>
        /// <returns><c>true</c> if there is a last record.</returns>
        /// <exception cref="FormatException">The file ends inside a quoted field.</exception>
        private bool Finish()
        {
            if (_state == State.Quoted)
            {
                throw new FormatException($"The file is not valid CSV: the quoted field of row {_rowNumber + 1} is not closed.");
            }
            if (_fields.Count == 0 && _state == State.Start)
            {
                return false;
            }
            EndField();
            return true;
        }

        #endregion
    }
}
