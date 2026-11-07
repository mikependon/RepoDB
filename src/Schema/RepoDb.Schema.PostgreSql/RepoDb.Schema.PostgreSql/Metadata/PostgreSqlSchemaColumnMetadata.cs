#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Applies the metadata of PostgreSQL that the generic column metadata does not know: the encoding <c>SQL_ASCII</c> says nothing about the characters that a column holds.
    /// </summary>
    public static class PostgreSqlSchemaColumnMetadata
    {
        #region Public Methods

        /// <summary>
        /// Leaves the encoding of the column unknown if its character set is <c>SQL_ASCII</c>.
        /// </summary>
        /// <param name="column">The column whose metadata is updated.</param>
        /// <returns>The column with its metadata.</returns>
        public static ColumnInfo Apply(ColumnInfo column)
        {
            if (string.Equals(column.CharacterSet?.Replace("-",
                string.Empty),
                "sql_ascii",
                StringComparison.OrdinalIgnoreCase))
            {
                column.IsUnicode = null;
            }

            return column;
        }

        #endregion
    }
}
