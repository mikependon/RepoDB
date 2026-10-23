#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Reads the rows of a JSON file forward only, without loading the file in memory: only a block of the file is held in a buffer (the buffer grows only when a single row
    /// does not fit in it), and only the object of the row in hand is parsed. The rows can be in one of these shapes:
    /// an array of objects (one object per row), an object whose first array property holds the rows, or a sequence of objects (one object per row, i.e.: one per line).
    /// The reader follows the opening and closing of the brackets, so a minified file (all on one line) reads like an indented one.
    /// </summary>
    internal sealed class JsonRowReader : IDisposable
    {
        #region Private Types

        /// <summary>
        /// What the parse of the next step found.
        /// </summary>
        private enum Step
        {
            /// <summary>
            /// A row (an object).
            /// </summary>
            Item,

            /// <summary>
            /// There is no other row.
            /// </summary>
            End,

            /// <summary>
            /// The buffer ends in the middle of what is read: it needs more of the file.
            /// </summary>
            More
        }

        /// <summary>
        /// Where the rows are, which is found when the first row is read.
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// The place of the rows was not found yet.
            /// </summary>
            Search,

            /// <summary>
            /// Inside the array that holds the rows.
            /// </summary>
            Array,

            /// <summary>
            /// In a sequence of objects (each one is a row).
            /// </summary>
            Sequence,

            /// <summary>
            /// There is no other row.
            /// </summary>
            Done
        }

        #endregion

        #region Private Variables

        /// <summary>
        /// The bytes read from the file that were not parsed yet are <c>_buffer[_start.._length]</c>.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// True once the end of the file was read.
        /// </summary>
        private bool _final;

        /// <summary>
        /// Where in the buffer the bytes that were not parsed start.
        /// </summary>
        private int _start;

        /// <summary>
        /// How many bytes the buffer holds.
        /// </summary>
        private int _length;

        /// <summary>
        /// The number of the row being read.
        /// </summary>
        private long _rowNumber;

        /// <summary>
        /// Where the rows are.
        /// </summary>
        private Phase _phase = Phase.Search;

        /// <summary>
        /// Where in the buffer the reader of the step in hand started (what it consumed is counted from there).
        /// </summary>
        private int _spanStart;

        /// <summary>
        /// The state of the JSON reader between two blocks.
        /// </summary>
        private JsonReaderState _state = new JsonReaderState(new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            AllowMultipleValues = true
        });

        /// <summary>
        /// True once the first block of the file was read (the byte order mark is skipped).
        /// </summary>
        private bool _started;

        /// <summary>
        /// The file.
        /// </summary>
        private readonly FileStream _stream;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="JsonRowReader"/> class and opens the file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="bufferSize">The size of the block of the file that is read at once, in bytes. The default is 64 KB.</param>
        public JsonRowReader(string path,
            int bufferSize = 64 * 1024)
        {
            if (bufferSize < 16)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "The buffer size must be at least 16 bytes.");
            }
            _buffer = new byte[bufferSize];
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
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
        /// <returns>The row as a JSON document (to be disposed by the caller), or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not a valid JSON, or a row is not an object.</exception>
        public JsonDocument Next() =>
            NextCore(false, out var document) ? document : null;

        /// <summary>
        /// Reads the next row in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the row as a JSON document (to be disposed by the caller), or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not a valid JSON, or a row is not an object.</exception>
        public async Task<JsonDocument> NextAsync(CancellationToken cancellationToken = default)
        {
            var (found, document) = await NextCoreAsync(false, cancellationToken).ConfigureAwait(false);
            return found ? document : null;
        }

        /// <summary>
        /// Moves over the next row without parsing it.
        /// </summary>
        /// <returns><c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not a valid JSON, or a row is not an object.</exception>
        public bool Skip() =>
            NextCore(true, out _);

        /// <summary>
        /// Moves over the next row without parsing it, in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if a row was skipped; <c>false</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not a valid JSON, or a row is not an object.</exception>
        public async Task<bool> SkipAsync(CancellationToken cancellationToken = default) =>
            (await NextCoreAsync(true, cancellationToken).ConfigureAwait(false)).Found;

        /// <summary>
        /// Closes the file.
        /// </summary>
        public void Dispose() =>
            _stream.Dispose();

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads (or skips) the next row, reading more of the file whenever the parse needs it.
        /// </summary>
        /// <param name="skip"><c>true</c> to move over the row without parsing it.</param>
        /// <param name="document">Receives the row (<c>null</c> if the row is skipped).</param>
        /// <returns><c>true</c> if there was a row; otherwise, <c>false</c>.</returns>
        private bool NextCore(bool skip,
            out JsonDocument document)
        {
            document = null;
            if (_phase == Phase.Done)
            {
                return false;
            }
            while (true)
            {
                var step = TryNextSafe(skip, out document);
                if (step == Step.End)
                {
                    return false;
                }
                if (step == Step.Item)
                {
                    return true;
                }
                if (!Fill())
                {
                    throw new FormatException("The file is not valid JSON: it ends before the rows are closed.");
                }
            }
        }

        /// <summary>
        /// Reads (or skips) the next row in an asynchronous way, reading more of the file whenever the parse needs it.
        /// </summary>
        /// <param name="skip"><c>true</c> to move over the row without parsing it.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is whether there was a row, and the row (<c>null</c> if the row is skipped).</returns>
        private async Task<(bool Found, JsonDocument Document)> NextCoreAsync(bool skip,
            CancellationToken cancellationToken)
        {
            if (_phase == Phase.Done)
            {
                return (false, null);
            }
            while (true)
            {
                var step = TryNextSafe(skip, out var document);
                if (step == Step.End)
                {
                    return (false, null);
                }
                if (step == Step.Item)
                {
                    return (true, document);
                }
                if (!await FillAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new FormatException("The file is not valid JSON: it ends before the rows are closed.");
                }
            }
        }

        /// <summary>
        /// Parses the next step, reporting the errors of the JSON as a <see cref="FormatException"/>.
        /// </summary>
        /// <param name="skip"><c>true</c> to move over the row without parsing it.</param>
        /// <param name="document">Receives the row.</param>
        /// <returns>What the parse found.</returns>
        private Step TryNextSafe(bool skip,
            out JsonDocument document)
        {
            try
            {
                return TryNext(skip, out document);
            }
            catch (JsonException e)
            {
                throw new FormatException("The file is not valid JSON: " + e.Message, e);
            }
        }

        /// <summary>
        /// Parses what the buffer holds up to the next row: finds where the rows are, then reads (or skips) the next object. What was parsed is kept only when it was
        /// complete, so a step that needs more of the file starts again from where it began.
        /// </summary>
        /// <param name="skip"><c>true</c> to move over the row without parsing it.</param>
        /// <param name="document">Receives the row.</param>
        /// <returns>What the parse found.</returns>
        private Step TryNext(bool skip,
            out JsonDocument document)
        {
            document = null;
            if (_phase == Phase.Search)
            {
                var found = Search();
                if (found == Step.More)
                {
                    return Step.More;
                }
                if (found == Step.End)
                {
                    return Step.End;
                }
            }

            var reader = NewReader();
            if (!reader.Read())
            {
                if (_phase == Phase.Sequence && _final)
                {
                    _phase = Phase.Done;
                    Keep(reader.BytesConsumed, reader.CurrentState);
                    return Step.End;
                }
                if (_final)
                {
                    throw new FormatException("The file is not valid JSON: it ends before the array of the rows is closed.");
                }
                return Step.More;
            }

            if (_phase == Phase.Array && reader.TokenType == JsonTokenType.EndArray)
            {
                _phase = Phase.Done;
                Keep(reader.BytesConsumed, reader.CurrentState);
                return Step.End;
            }
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new FormatException($"Item {_rowNumber + 1} is not an object.");
            }

            if (skip)
            {
                if (!reader.TrySkip())
                {
                    return Step.More;
                }
            }
            else if (!JsonDocument.TryParseValue(ref reader, out document))
            {
                return Step.More;
            }

            _rowNumber++;
            Keep(reader.BytesConsumed, reader.CurrentState);
            return Step.Item;
        }

        /// <summary>
        /// Finds where the rows are: an array at the root, the first array property of an object at the root, or a sequence of objects.
        /// </summary>
        /// <returns><see cref="Step.Item"/> if the place of the rows was found; <see cref="Step.More"/> if the buffer needs more of the file; <see cref="Step.End"/> if the file has no row.</returns>
        private Step Search()
        {
            var reader = NewReader();
            if (!reader.Read())
            {
                if (_final)
                {
                    throw new FormatException("The file is not valid JSON: it has no JSON.");
                }
                return Step.More;
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                _phase = Phase.Array;
                Keep(reader.BytesConsumed, reader.CurrentState);
                return Step.Item;
            }
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new FormatException("The JSON must be an array of objects (one object per row), or a sequence of objects.");
            }

            // An object: it holds the rows if one of its properties is an array, otherwise it is the first row of a sequence of objects (what was scanned is not consumed)
            var scan = reader;
            while (true)
            {
                if (!scan.Read())
                {
                    if (_final)
                    {
                        throw new FormatException("The file is not valid JSON: it ends before the rows are closed.");
                    }
                    return Step.More;
                }
                if (scan.TokenType == JsonTokenType.EndObject && scan.CurrentDepth == 0)
                {
                    _phase = Phase.Sequence;
                    return Step.Item;
                }
                if (scan.TokenType == JsonTokenType.StartArray && scan.CurrentDepth == 1)
                {
                    _phase = Phase.Array;
                    Keep(scan.BytesConsumed, scan.CurrentState);
                    return Step.Item;
                }
                if ((scan.TokenType == JsonTokenType.StartObject || scan.TokenType == JsonTokenType.StartArray) && scan.CurrentDepth > 0 && !scan.TrySkip())
                {
                    if (_final)
                    {
                        throw new FormatException("The file is not valid JSON: it ends before the rows are closed.");
                    }
                    return Step.More;
                }
            }
        }

        /// <summary>
        /// Marks the bytes that were parsed.
        /// </summary>
        /// <param name="consumed">How many bytes of the unparsed ones were parsed.</param>
        /// <param name="state">The state of the reader after them.</param>
        private void Keep(long consumed,
            JsonReaderState state)
        {
            _start = _spanStart + (int)consumed;
            _state = state;
        }

        /// <summary>
        /// Creates a reader of the bytes that were not parsed yet.
        /// </summary>
        /// <returns>The reader.</returns>
        private Utf8JsonReader NewReader()
        {
            _spanStart = _start;
            return new Utf8JsonReader(_buffer.AsSpan(_start, _length - _start), _final, _state);
        }

        /// <summary>
        /// Reads the next block of the file after the bytes that were not parsed (the buffer grows when a row does not fit in it).
        /// </summary>
        /// <returns><c>false</c> when the whole file was read already.</returns>
        private bool Fill()
        {
            if (_final)
            {
                return false;
            }
            MakeRoom();
            var read = _stream.Read(_buffer, _length, _buffer.Length - _length);
            Received(read);
            return true;
        }

        /// <summary>
        /// Reads the next block of the file after the bytes that were not parsed in an asynchronous way (the buffer grows when a row does not fit in it).
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>false</c> when the whole file was read already.</returns>
        private async Task<bool> FillAsync(CancellationToken cancellationToken)
        {
            if (_final)
            {
                return false;
            }
            MakeRoom();
            var read = await _stream.ReadAsync(_buffer, _length, _buffer.Length - _length, cancellationToken).ConfigureAwait(false);
            Received(read);
            return true;
        }

        /// <summary>
        /// Makes room in the buffer for the next block: the bytes that were parsed are dropped, and the buffer grows if it is full.
        /// </summary>
        private void MakeRoom()
        {
            if (_start > 0)
            {
                Buffer.BlockCopy(_buffer, _start, _buffer, 0, _length - _start);
                _length -= _start;
                _start = 0;
            }
            if (_length == _buffer.Length)
            {
                Array.Resize(ref _buffer, _buffer.Length * 2);
            }
        }

        /// <summary>
        /// Takes in the bytes that were read from the file (the byte order mark of the file is skipped).
        /// </summary>
        /// <param name="read">The number of bytes that were read, which is 0 at the end of the file.</param>
        private void Received(int read)
        {
            if (read == 0)
            {
                _final = true;
            }
            else
            {
                _length += read;
            }
            if (!_started && _length >= 3)
            {
                _started = true;
                if (_buffer[0] == 0xEF && _buffer[1] == 0xBB && _buffer[2] == 0xBF)
                {
                    _start = 3;
                }
            }
        }

        #endregion
    }
}
