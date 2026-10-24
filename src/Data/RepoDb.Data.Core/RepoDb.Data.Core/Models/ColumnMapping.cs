#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that represents the mapping of a column of the source table to a column of the target table, used by the <c>CopyDataTo</c> operations.
    /// </summary>
    public class ColumnMapping
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ColumnMapping"/> class.
        /// </summary>
        /// <param name="sourceColumn">The name of the column of the source table.</param>
        /// <param name="destinationColumn">The name of the column of the target table.</param>
        public ColumnMapping(string sourceColumn,
            string destinationColumn)
        {
            SourceColumn = sourceColumn;
            DestinationColumn = destinationColumn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the column of the source table.
        /// </summary>
        public string SourceColumn { get; }

        /// <summary>
        /// Gets the name of the column of the target table.
        /// </summary>
        public string DestinationColumn { get; }

        #endregion
    }
}
