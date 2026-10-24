#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using RepoDb.Data.Models;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Writes the rows of a table as a JSON array of objects (one object per row, with a property per column). The values are written so that they can be read back by
    /// <c>ImportDataFromJson</c>: numbers as JSON numbers, dates in ISO 8601, a binary as base64, a <see cref="Guid"/> and a <see cref="TimeSpan"/> as text.
    /// </summary>
    internal static class JsonRowWriter
    {
        #region Public Methods

        /// <summary>
        /// Gets the options of the writer of the file: indented, and without escaping the characters that JSON does not require to escape.
        /// </summary>
        public static JsonWriterOptions Options { get; } = new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// Writes the current row of the reader as a JSON object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="record">The reader (or the record) that is on the row.</param>
        /// <param name="names">The names of the columns of the reader.</param>
        public static void WriteRow(Utf8JsonWriter writer,
            IDataRecord record,
            string[] names)
        {
            writer.WriteStartObject();
            for (var i = 0; i < names.Length; i++)
            {
                writer.WritePropertyName(names[i]);
                WriteValue(writer, record.IsDBNull(i) ? null : record.GetValue(i));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a row as a JSON object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">The row.</param>
        public static void WriteRow(Utf8JsonWriter writer,
            Row row)
        {
            writer.WriteStartObject();
            for (var i = 0; i < row.Count; i++)
            {
                writer.WritePropertyName(row.Columns[i].Name);
                WriteValue(writer, row[i]);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a value as a JSON value.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value (<c>null</c> or a database null is a JSON null).</param>
        public static void WriteValue(Utf8JsonWriter writer,
            object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    writer.WriteNullValue();
                    break;
                case string text:
                    writer.WriteStringValue(text);
                    break;
                case bool flag:
                    writer.WriteBooleanValue(flag);
                    break;
                case byte number:
                    writer.WriteNumberValue(number);
                    break;
                case sbyte number:
                    writer.WriteNumberValue(number);
                    break;
                case short number:
                    writer.WriteNumberValue(number);
                    break;
                case ushort number:
                    writer.WriteNumberValue(number);
                    break;
                case int number:
                    writer.WriteNumberValue(number);
                    break;
                case uint number:
                    writer.WriteNumberValue(number);
                    break;
                case long number:
                    writer.WriteNumberValue(number);
                    break;
                case ulong number:
                    writer.WriteNumberValue(number);
                    break;
                case decimal number:
                    writer.WriteNumberValue(number);
                    break;
                case double number:
                    WriteReal(writer, number);
                    break;
                case float number:
                    if (float.IsNaN(number) || float.IsInfinity(number))
                    {
                        writer.WriteStringValue(number.ToString("R", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.WriteNumberValue(number);
                    }
                    break;
                case DateTime date:
                    writer.WriteStringValue(date);
                    break;
                case DateTimeOffset date:
                    writer.WriteStringValue(date);
                    break;
                case Guid guid:
                    writer.WriteStringValue(guid);
                    break;
                case TimeSpan time:
                    writer.WriteStringValue(time.ToString("c", CultureInfo.InvariantCulture));
                    break;
                case byte[] bytes:
                    writer.WriteBase64StringValue(bytes);
                    break;
                case Stream stream:
                    // Some providers give a binary value as a stream (i.e.: DuckDB)
                    writer.WriteBase64StringValue(ReadAll(stream));
                    break;
                case char character:
                    writer.WriteStringValue(character.ToString());
                    break;
                case Enum enumeration:
                    writer.WriteNumberValue(Convert.ToInt64(enumeration, CultureInfo.InvariantCulture));
                    break;
                default:
                    writer.WriteStringValue(Convert.ToString(value, CultureInfo.InvariantCulture));
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the bytes of a stream from where it is.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The bytes.</returns>
        private static byte[] ReadAll(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Writes a real number. JSON has no <c>NaN</c> or infinity, so they are written as text.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="number">The number.</param>
        private static void WriteReal(Utf8JsonWriter writer,
            double number)
        {
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                writer.WriteStringValue(number.ToString("R", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNumberValue(number);
            }
        }

        #endregion
    }
}
