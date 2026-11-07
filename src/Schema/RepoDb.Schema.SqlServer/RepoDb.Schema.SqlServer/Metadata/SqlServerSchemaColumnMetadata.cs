#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Applies the metadata of SQL Server that the generic column metadata does not know: the character encoding of the text columns and the precision of the temporal ones.
    /// </summary>
    public static class SqlServerSchemaColumnMetadata
    {
        #region Private Variables

        private static readonly string[] AnsiTypes = { "char", "text", "varchar" };
        private static readonly string[] TemporalTypes = { "datetime2", "datetimeoffset", "time" };
        private static readonly string[] UnicodeTypes = { "nchar", "ntext", "nvarchar" };

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the encoding of a text that is not Unicode: its code page, or its collation if the catalog gives no code page.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <param name="codePage">The code page reported by the catalog, or null.</param>
        private static void ApplyAnsi(ColumnInfo column,
            object codePage)
        {
            column.LengthUnit = "bytes";
            if (codePage != null && Convert.ToInt32(codePage,
                CultureInfo.InvariantCulture) > 0)
            {
                ApplyCodePage(column,
                    Convert.ToInt32(codePage,
                        CultureInfo.InvariantCulture));
            }
            else if (column.Collation != null)
            {
                column.IsUnicode = column.Collation.EndsWith("_UTF8",
                    StringComparison.OrdinalIgnoreCase);
                column.CharacterSet = column.IsUnicode == true ? "UTF-8" : column.CharacterSet;
            }
        }

        /// <summary>
        /// Applies the encoding of a code page: 65001 is UTF-8, and any other is a legacy code page.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <param name="number">The number of the code page.</param>
        private static void ApplyCodePage(ColumnInfo column,
            int number)
        {
            column.IsUnicode = number == 65001;
            column.CharacterSet = number == 65001 ? "UTF-8" : "CP" + number.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Applies the encoding of the text columns.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <param name="codePage">The code page reported by the catalog, or null.</param>
        private static void ApplyText(ColumnInfo column,
            object codePage)
        {
            var field = column.Field;
            var native = GetBaseType(field);
            if (!(field.Type == typeof(string) || field.Type == typeof(char)))
            {
                return;
            }

            if (UnicodeTypes.Contains(native))
            {
                column.IsUnicode = true;
                column.CharacterSet = "UTF-16";
                column.LengthUnit = "utf16CodeUnits";
            }
            else if (AnsiTypes.Contains(native))
            {
                ApplyAnsi(column,
                    codePage);
            }
        }

        /// <summary>
        /// Gets the name of the native type of the field, without its size.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The name of the type, in lower case.</returns>
        private static string GetBaseType(DbField field) =>
            field.DatabaseType?.ToLowerInvariant().Split('(',
                ' ')[0];

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the metadata of SQL Server to a column that was read from its catalog.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <param name="record">The catalog record of the column.</param>
        /// <returns>The column with its metadata.</returns>
        public static ColumnInfo Apply(ColumnInfo column,
            IDataRecord record)
        {
            ApplyText(column,
                SchemaColumnMetadata.Read(record,
                    "CodePage"));
            if (TemporalTypes.Contains(GetBaseType(column.Field)))
            {
                column.TemporalPrecision = column.Field.Scale;
            }

            return column;
        }

        #endregion
    }
}
