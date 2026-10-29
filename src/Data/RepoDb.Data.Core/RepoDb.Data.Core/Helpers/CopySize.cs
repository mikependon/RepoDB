#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Estimates payload bytes without serializing values or retaining completed batches.
    /// </summary>
    internal static class CopySize
    {
        #region Public Methods

        /// <summary>
        /// Estimates the payload bytes of the values of the rows.
        /// </summary>
        /// <param name="rows">The rows (each one is a dictionary of the column names and the values).</param>
        /// <returns>The estimated bytes of the values of all the rows.</returns>
        internal static long OfRows(IList<object> rows)
        {
            long bytes = 0;
            foreach (IDictionary<string, object> row in rows)
            {
                foreach (var value in row.Values)
                {
                    bytes += OfValue(value);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Estimates the payload bytes of a value. Nulls and unsupported provider-specific values count as zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The estimated bytes of the value.</returns>
        internal static long OfValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            if (value is string text)
                return (long)text.Length * 2;
            if (value is byte[] bytes)
                return bytes.LongLength;
            if (value is char[] chars)
                return chars.LongLength * 2;
            if (value is Guid || value is decimal || value is DateTimeOffset)
                return 16;
            if (value is DateTime || value is TimeSpan || value is long || value is ulong || value is double)
                return 8;
            if (value is int || value is uint || value is float)
                return 4;
            if (value is short || value is ushort || value is char)
                return 2;
            if (value is byte || value is sbyte || value is bool)
                return 1;
            return 0;
        }

        #endregion
    }
}
