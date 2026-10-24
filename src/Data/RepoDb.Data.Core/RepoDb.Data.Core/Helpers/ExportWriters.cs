#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using RepoDb.Data.Models;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Writes the rows of a table into a file of a format (JSON, XML, CSV), one row at a time. Every <c>ExportDataAs*</c> operation reads, counts and reports the same way
    /// (see <see cref="ExportPipeline"/>), and only the way that the rows are written is of the format.
    /// </summary>
    internal interface IExportWriter : IDisposable
    {
        /// <summary>
        /// Starts the file.
        /// </summary>
        /// <param name="names">The names of the columns that are selected.</param>
        void Begin(string[] names);

        /// <summary>
        /// Writes the current row of the reader as it is.
        /// </summary>
        /// <param name="record">The reader (or the record) that is on the row.</param>
        /// <param name="names">The names of the columns of the reader.</param>
        void WriteRow(IDataRecord record,
            string[] names);

        /// <summary>
        /// Writes a row that the interceptors returned.
        /// </summary>
        /// <param name="row">The row.</param>
        void WriteRow(Row row);

        /// <summary>
        /// Ends the file.
        /// </summary>
        void End();

        /// <summary>
        /// Writes what the writer holds into the stream.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes what the writer holds into the stream in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task FlushAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Writes the rows as a JSON array of objects (see <see cref="JsonRowWriter"/>).
    /// </summary>
    internal sealed class JsonExportWriter : IExportWriter
    {
        private readonly Utf8JsonWriter _writer;

        public JsonExportWriter(Stream stream)
        {
            _writer = new Utf8JsonWriter(stream, JsonRowWriter.Options);
        }

        public void Begin(string[] names) =>
            _writer.WriteStartArray();

        public void WriteRow(IDataRecord record,
            string[] names) =>
            JsonRowWriter.WriteRow(_writer, record, names);

        public void WriteRow(Row row) =>
            JsonRowWriter.WriteRow(_writer, row);

        public void End() =>
            _writer.WriteEndArray();

        public void Flush() =>
            _writer.Flush();

        public Task FlushAsync(CancellationToken cancellationToken) =>
            _writer.FlushAsync(cancellationToken);

        public void Dispose() =>
            _writer.Dispose();
    }

    /// <summary>
    /// Writes the rows as an XML document: a <c>rows</c> root element that has a <c>row</c> element for each row, and each <c>row</c> has an element for each column (named by
    /// the column, encoded if the name is not a valid XML name). A <c>null</c> is a self-closing element and an empty text is an element with its start and end tags. The attributes are not used.
    /// </summary>
    internal sealed class XmlExportWriter : IExportWriter
    {
        /// <summary>
        /// The name of the root element.
        /// </summary>
        public const string RootName = "rows";

        /// <summary>
        /// The name of the element of a row.
        /// </summary>
        public const string RowName = "row";

        private readonly XmlWriter _writer;

        public XmlExportWriter(Stream stream)
        {
            _writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Async = true,
                Indent = true,
                Encoding = new UTF8Encoding(false),
                CloseOutput = false
            });
        }

        public void Begin(string[] names)
        {
            _writer.WriteStartDocument();
            _writer.WriteStartElement(RootName);
        }

        public void WriteRow(IDataRecord record,
            string[] names)
        {
            _writer.WriteStartElement(RowName);
            for (var i = 0; i < names.Length; i++)
            {
                WriteCell(names[i], record.IsDBNull(i) ? null : record.GetValue(i));
            }
            _writer.WriteEndElement();
        }

        public void WriteRow(Row row)
        {
            _writer.WriteStartElement(RowName);
            for (var i = 0; i < row.Count; i++)
            {
                WriteCell(row.Columns[i].Name, row[i]);
            }
            _writer.WriteEndElement();
        }

        public void End()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
        }

        public void Flush() =>
            _writer.Flush();

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _writer.FlushAsync();
        }

        public void Dispose() =>
            _writer.Dispose();

        private void WriteCell(string name,
            object value)
        {
            _writer.WriteStartElement(XmlConvert.EncodeLocalName(name));
            var text = ExportValueFormatter.ToText(value);
            if (text == null)
            {
                // A null is a self-closing element
                _writer.WriteEndElement();
                return;
            }
            if (text.Length > 0)
            {
                _writer.WriteString(text);
            }

            // An empty text keeps its start and end tags
            _writer.WriteFullEndElement();
        }
    }

    /// <summary>
    /// Writes the rows as CSV (RFC 4180): the first line is the header, a field is quoted only when it has to be, a <c>null</c> is an unquoted empty field and an empty text is
    /// a quoted empty field (<c>""</c>), so <c>ImportDataFromCsv</c> reads them back as they were. The lines end with <c>\r\n</c>, and the encoding is UTF-8 without a byte order mark.
    /// </summary>
    internal sealed class CsvExportWriter : IExportWriter
    {
        private const string LineBreak = "\r\n";

        private readonly StreamWriter _writer;
        private readonly char _separator;
        private string[] _header;
        private string[] _names;

        public CsvExportWriter(Stream stream,
            char separator)
        {
            _separator = separator;
            _writer = new StreamWriter(stream, new UTF8Encoding(false), 4096, true);
        }

        public void Begin(string[] names) =>
            _names = names;

        public void WriteRow(IDataRecord record,
            string[] names)
        {
            WriteHeader(names);
            for (var i = 0; i < names.Length; i++)
            {
                WriteField(i, record.IsDBNull(i) ? null : ExportValueFormatter.ToText(record.GetValue(i)));
            }
            _writer.Write(LineBreak);
        }

        public void WriteRow(Row row)
        {
            var names = new string[row.Count];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = row.Columns[i].Name;
            }
            WriteHeader(names);
            if (!IsHeader(names))
            {
                throw new InvalidOperationException("The data interceptors must give every row the same columns, in the same order: the CSV file has one header.");
            }
            for (var i = 0; i < row.Count; i++)
            {
                WriteField(i, ExportValueFormatter.ToText(row[i]));
            }
            _writer.Write(LineBreak);
        }

        public void End() =>
            WriteHeader(_names);

        public void Flush() =>
            _writer.Flush();

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _writer.FlushAsync();
        }

        public void Dispose() =>
            _writer.Dispose();

        private void WriteHeader(string[] names)
        {
            if (_header != null)
            {
                return;
            }
            _header = names;
            for (var i = 0; i < names.Length; i++)
            {
                WriteField(i, names[i]);
            }
            _writer.Write(LineBreak);
        }

        private bool IsHeader(string[] names)
        {
            if (names.Length != _header.Length)
            {
                return false;
            }
            for (var i = 0; i < names.Length; i++)
            {
                if (!string.Equals(names[i], _header[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        private void WriteField(int position,
            string text)
        {
            if (position > 0)
            {
                _writer.Write(_separator);
            }
            if (text == null)
            {
                return;
            }
            if (text.Length == 0 || text.IndexOf(_separator) >= 0 || text.IndexOf('"') >= 0 || text.IndexOf('\r') >= 0 || text.IndexOf('\n') >= 0)
            {
                _writer.Write('"');
                _writer.Write(text.Replace("\"", "\"\""));
                _writer.Write('"');
            }
            else
            {
                _writer.Write(text);
            }
        }
    }

    /// <summary>
    /// Gives the text that a value is written as in a text file (XML, CSV): the text that <c>ImportDataFromXml</c> and <c>ImportDataFromCsv</c> convert back into the type of the column.
    /// </summary>
    internal static class ExportValueFormatter
    {
        /// <summary>
        /// Gets the text of a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The text, or <c>null</c> if the value is <c>null</c> (or a database null).</returns>
        public static string ToText(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case string text:
                    return text;
                case bool flag:
                    return flag ? "true" : "false";
                case DateTime date:
                    return date.ToString("o", CultureInfo.InvariantCulture);
                case DateTimeOffset date:
                    return date.ToString("o", CultureInfo.InvariantCulture);
                case TimeSpan time:
                    return time.ToString("c", CultureInfo.InvariantCulture);
                case Guid guid:
                    return guid.ToString("D", CultureInfo.InvariantCulture);
                case double real:
                    return real.ToString("R", CultureInfo.InvariantCulture);
                case float real:
                    return real.ToString("R", CultureInfo.InvariantCulture);
                case byte[] bytes:
                    return Convert.ToBase64String(bytes);
                case Stream stream:
                    using (var memory = new MemoryStream())
                    {
                        stream.CopyTo(memory);
                        return Convert.ToBase64String(memory.ToArray());
                    }
                case Enum enumeration:
                    return Convert.ToInt64(enumeration, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                default:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
        }
    }
}
