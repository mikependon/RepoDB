#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that represents the mapping of a column of the source table to the column that is created in the destination table, used by the <c>CopyTableSchemaTo</c> operations
    /// that create the tables with only the mapped columns.
    /// </summary>
    public class ColumnMapping
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ColumnMapping"/> class.
        /// </summary>
        /// <param name="source">The name of the column of the source table.</param>
        /// <param name="target">The name of the column in the destination table. The default is <c>null</c>, which keeps the name of the source column.</param>
        public ColumnMapping(string source,
            string target = null)
        {
            Source = source;
            Target = target;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the column of the source table.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the name of the column in the destination table, or <c>null</c> (or white space) if the column keeps the name of the source column.
        /// </summary>
        public string Target { get; }

        #endregion
    }
}
