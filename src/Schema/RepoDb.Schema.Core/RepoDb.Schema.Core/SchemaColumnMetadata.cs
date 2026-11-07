#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Reads additional column metadata from optional provider catalog fields.
    /// </summary>
    public static class SchemaColumnMetadata
    {
        #region Public Methods

        /// <summary>
        /// Applies optional catalog metadata to a column while retaining compatibility with older catalog projections.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <param name="record">The provider catalog record.</param>
        /// <returns>The column with its additional metadata.</returns>
        public static ColumnInfo Apply(ColumnInfo column,
            IDataRecord record)
        {
            var field = column.Field;
            column.NativeType = Read(record,
                "NativeType") as string ?? Read(record,
                    "ColumnType") as string;
            var precision = Read(record,
                "Precision");
            var scale = Read(record,
                "Scale");
            if (precision != null)
                column.NumericPrecision = Convert.ToInt32(precision,
                    CultureInfo.InvariantCulture);
            if (scale != null)
                column.NumericScale = Convert.ToInt32(scale,
                    CultureInfo.InvariantCulture);
            ReadNativeNumericType(column);
            var text = field.Type == typeof(string) || field.Type == typeof(char);
            var charset = Read(record,
                "CharacterSet") as string;
            column.Collation = column.Collation ?? Read(record,
                "Collation") as string;
            column.CharacterSet = text ? charset : null;
            if (text && charset != null)
            {
                var encoding = charset.ToLowerInvariant().Replace("-",
                    "");
                if (encoding.StartsWith("utf",
                    StringComparison.Ordinal) || string.Equals(encoding,
                        "ucs2",
                        StringComparison.Ordinal))
                    column.IsUnicode = true;
                else
                    column.IsUnicode = false;
                column.LengthUnit = "characters";
            }

            var temporal = Read(record,
                "TemporalPrecision");
            if (temporal != null)
                column.TemporalPrecision = Convert.ToInt32(temporal,
                    CultureInfo.InvariantCulture);
            return column;
        }

        /// <summary>
        /// Reads an optional catalog field without requiring every provider to project it.
        /// </summary>
        /// <param name="record">The catalog record.</param>
        /// <param name="name">The name of the optional field.</param>
        /// <returns>The field value, or null if the field is absent or contains a database null.</returns>
        public static object Read(IDataRecord record,
            string name)
        {
            for (var i = 0; i < record.FieldCount; i++)
                if (string.Equals(record.GetName(i),
                    name,
                    StringComparison.OrdinalIgnoreCase))
                    return record.IsDBNull(i) ? null : record.GetValue(i);
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the full numeric precision and scale from a native type declaration.
        /// </summary>
        /// <param name="column">The column whose numeric metadata is updated.</param>
        private static void ReadNativeNumericType(ColumnInfo column)
        {
            if (column.NativeType == null)
                return;
            var numeric = Regex.Match(column.NativeType,
                @"^(?:numeric|decimal)\s*\(\s*(?<precision>\d+)\s*,\s*(?<scale>-?\d+)\s*\)",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                TimeSpan.FromSeconds(1));
            if (!numeric.Success)
                return;
            column.NumericPrecision = int.Parse(numeric.Groups["precision"].Value,
                CultureInfo.InvariantCulture);
            column.NumericScale = int.Parse(numeric.Groups["scale"].Value,
                CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
