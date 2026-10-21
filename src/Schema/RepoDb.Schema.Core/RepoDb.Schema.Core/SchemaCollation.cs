#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that decides whether the collation of a column can be written in the destination database. The names of the collations belong to a database provider
    /// (i.e.: <c>SQL_Latin1_General_CP1_CI_AS</c> is a SQL Server collation that MySQL rejects as an unknown collation), so a collation is only kept when the column is read
    /// from a database of a provider that the destination understands; otherwise it is left out and the destination uses its default collation.
    /// </summary>
    public static class SchemaCollation
    {
        #region Public Methods

        /// <summary>
        /// Gets the collation of the column that can be written in the destination database.
        /// </summary>
        /// <param name="column">The column that is composed.</param>
        /// <param name="compatibleProviders">The names of the providers (see <see cref="RepoDb.DbField.Provider"/>) whose collation names the destination database understands, including its own.</param>
        /// <returns>
        /// The collation of the column if it has one and it is read from a database of a compatible provider (or of an unknown provider, as it can not be judged);
        /// otherwise, <c>null</c>, which leaves the collation out so the destination database uses its default one.
        /// </returns>
        public static string Resolve(ColumnInfo column,
            params string[] compatibleProviders)
        {
            if (column == null || string.IsNullOrWhiteSpace(column.Collation))
            {
                return null;
            }
            var provider = column.Field?.Provider;
            if (string.IsNullOrWhiteSpace(provider))
            {
                return column.Collation;
            }
            return compatibleProviders != null && compatibleProviders.Contains(provider, StringComparer.OrdinalIgnoreCase)
                ? column.Collation
                : null;
        }

        #endregion
    }
}
