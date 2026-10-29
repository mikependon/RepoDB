#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Reads a JSON file that holds the rows of a table without loading it in memory: it finds the columns of the rows, counts the rows, and reads the values of a row
    /// (see <see cref="JsonRowReader"/> for the shapes of the file that are supported). It also converts the values of the JSON into the types of the columns of a table.
    /// </summary>
    internal static class JsonHelper
    {
        #region Constants

        /// <summary>
        /// The number of rows from the start of the file that are looked at to find the columns (a row may not have a property that the other rows have).
        /// </summary>
        public const int ProbeRows = 100;

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the file to read its rows.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
        /// <returns>The reader of the rows, to be disposed by the caller.</returns>
        public static JsonRowReader Open(string filename) =>
            new JsonRowReader(filename);

        /// <summary>
        /// Reads the first rows of the file to find the columns: the names of the properties of the rows, without repeating a name (the case is ignored).
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
        /// <returns>The names of the columns, in the order that they were found.</returns>
        public static IList<string> ReadColumns(string filename)
        {
            var columns = new List<string>();
            using (var reader = Open(filename))
            {
                for (var i = 0; i < ProbeRows; i++)
                {
                    using (var document = reader.Next())
                    {
                        if (document == null)
                        {
                            break;
                        }

                        AddColumns(columns,
                            document);
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Reads the first rows of the file to find the columns in an asynchronous way.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns, in the order that they were found.</returns>
        public static async Task<IList<string>> ReadColumnsAsync(string filename,
            CancellationToken cancellationToken = default)
        {
            var columns = new List<string>();
            using (var reader = Open(filename))
            {
                for (var i = 0; i < ProbeRows; i++)
                {
                    using (var document = await reader.NextAsync(cancellationToken).ConfigureAwait(false))
                    {
                        if (document == null)
                        {
                            break;
                        }

                        AddColumns(columns,
                            document);
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Counts the rows of the file, by moving over them without parsing them.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
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
        /// Counts the rows of the file in an asynchronous way, by moving over them without parsing them.
        /// </summary>
        /// <param name="filename">The path of the JSON file.</param>
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

        /// <summary>
        /// Gets the values of the columns of a row.
        /// </summary>
        /// <param name="row">The row: a JSON object.</param>
        /// <param name="index">The position of each column (by its name, without the case) in the array of the values.</param>
        /// <param name="width">The number of columns.</param>
        /// <returns>The values by the position of the columns: <c>null</c> for the property that the row does not have, or has as <c>null</c>.</returns>
        public static object[] GetValues(JsonDocument row,
            IDictionary<string, int> index,
            int width)
        {
            var values = new object[width];
            foreach (var property in row.RootElement.EnumerateObject())
            {
                if (index.TryGetValue(property.Name,
                    out var position))
                {
                    values[position] = GetValue(property.Value);
                }
            }

            return values;
        }

        /// <summary>
        /// Gets the value that a JSON value holds: a string, a number (as a <see cref="decimal "/>, or a <see cref="double "/> if it does not fit), a boolean or <c>null</c>.
        /// A value with its type (<c>{ "type": "Int32", "value": 1 }</c>) gives the value that it has, and any other JSON value gives its text.
        /// </summary>
        /// <param name="cell">The JSON value.</param>
        /// <returns>The value.</returns>
        public static object GetValue(JsonElement cell)
        {
            if (cell.ValueKind == JsonValueKind.Object && cell.TryGetProperty("value",
                out var typed))
            {
                cell = typed;
            }

            switch (cell.ValueKind)
            {
                case JsonValueKind.String:
                    return cell.GetString();
                case JsonValueKind.Number:
                    return cell.TryGetDecimal(out var number) ? number : cell.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                default:
                    return cell.GetRawText();
            }
        }

        /// <summary>
        /// Converts a value that was read from the JSON into the type of a column of the table.
        /// </summary>
        /// <param name="value">The value that was read from the JSON.</param>
        /// <param name="type">The type of the column, or <c>null</c> if it is not known (the value is kept as it is).</param>
        /// <returns>The value as the type, or <c>null</c> if the value is <c>null</c>.</returns>
        /// <exception cref="FormatException">The value cannot be converted into the type.</exception>
        public static object ConvertValue(object value,
            Type type) =>
            ValueConverter.Convert(value,
                type);

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the names of the properties of a row that are not in the columns yet.
        /// </summary>
        /// <param name="columns">The columns that were found so far.</param>
        /// <param name="row">The row: a JSON object.</param>
        private static void AddColumns(IList<string> columns,
            JsonDocument row)
        {
            foreach (var property in row.RootElement.EnumerateObject())
            {
                var found = false;
                for (var i = 0; i < columns.Count && !found; i++)
                {
                    found = string.Equals(columns[i],
                        property.Name,
                        StringComparison.OrdinalIgnoreCase);
                }

                if (!found)
                {
                    columns.Add(property.Name);
                }
            }
        }

        #endregion
    }
}
