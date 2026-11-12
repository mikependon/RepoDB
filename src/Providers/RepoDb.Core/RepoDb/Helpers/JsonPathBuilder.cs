#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright Bert Huijben and the contributors of the AmpScm RepoDb fork.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text;

namespace RepoDb.Helpers
{
    /// <summary>
    /// Splits and formats the JSON paths (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).
    /// </summary>
    public static class JsonPathBuilder
    {
        #region Private Methods

        private static void AddIndex(List<string> segments,
            string path,
            ref int position)
        {
            var end = path.IndexOf(']', position);
            if (end < 0)
            {
                end = path.Length - 1;
            }
            segments.Add(path.Substring(position, end - position + 1));
            position = end + 1;
        }

        private static void AddName(List<string> segments,
            string path,
            ref int position)
        {
            var start = position;
            while (position < path.Length && path[position] != '.' && path[position] != '[')
            {
                position++;
            }
            if (position > start)
            {
                segments.Add(path.Substring(start, position - start));
            }
        }

        private static void AppendSegment(StringBuilder builder,
            string segment)
        {
            if (IsIndex(segment))
            {
                builder.Append(segment);
            }
            else if (IsSimpleName(segment))
            {
                builder.Append('.').Append(segment);
            }
            else
            {
                builder.Append(".\"").Append(segment.Replace("\"", "\\\"")).Append('"');
            }
        }

        private static bool IsSimpleName(string segment)
        {
            foreach (var character in segment)
            {
                if (!char.IsLetterOrDigit(character) && character != '_')
                {
                    return false;
                }
            }
            return segment.Length > 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether the segment is an array index (i.e.: <c>[0]</c>).
        /// </summary>
        /// <param name="segment">The segment to check.</param>
        /// <returns><c>true</c> if the segment is an array index.</returns>
        public static bool IsIndex(string segment) =>
            segment.Length > 2 && segment[0] == '[' && segment[segment.Length - 1] == ']';

        /// <summary>
        /// Joins the segments into a path (i.e.: <c>Address.City</c> or <c>Tags[0]</c>), the reverse of <see cref="Split(string)"/>.
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The path.</returns>
        public static string Join(IReadOnlyList<string> segments)
        {
            var builder = new StringBuilder();
            foreach (var segment in segments)
            {
                builder.Append(builder.Length > 0 && !IsIndex(segment) ? "." : string.Empty).Append(segment);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Splits a JSON path into its property and array index segments.
        /// </summary>
        /// <param name="path">The JSON path (i.e.: <c>Address.City</c> or <c>Tags[0]</c>).</param>
        /// <returns>The segments, where the array indexes are kept in brackets.</returns>
        public static IReadOnlyList<string> Split(string path)
        {
            var segments = new List<string>();
            var position = 0;
            while (!string.IsNullOrEmpty(path) && position < path.Length)
            {
                if (path[position] == '.')
                {
                    position++;
                }
                else if (path[position] == '[')
                {
                    AddIndex(segments, path, ref position);
                }
                else
                {
                    AddName(segments, path, ref position);
                }
            }
            return segments;
        }

        /// <summary>
        /// Formats the segments as a JSON path (i.e.: <c>$.Address.City</c> or <c>$.Tags[0]</c>).
        /// </summary>
        /// <param name="segments">The property and array index segments.</param>
        /// <returns>The JSON path.</returns>
        public static string ToJsonPath(IReadOnlyList<string> segments)
        {
            var builder = new StringBuilder("$");
            foreach (var segment in segments)
            {
                AppendSegment(builder, segment);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Quotes the text as a SQL string literal.
        /// </summary>
        /// <param name="text">The text to quote.</param>
        /// <returns>The single-quoted text, where the single quotes are escaped.</returns>
        public static string ToSqlLiteral(string text) =>
            string.Concat("'", text.Replace("'", "''"), "'");

        #endregion
    }
}
