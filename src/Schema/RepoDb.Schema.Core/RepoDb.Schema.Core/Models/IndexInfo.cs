#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the definition of an index of a table.
    /// </summary>
    public class IndexInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the index is unique.
        /// </summary>
        public bool IsUnique { get; internal set; }

        /// <summary>
        /// Gets or sets the ordered names of the key columns of the index.
        /// </summary>
        public IList<string> Columns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the names of the non-key (included) columns of the index.
        /// </summary>
        public IList<string> IncludedColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets the names of the key columns that are sorted in descending order. The other key columns are sorted in ascending order.
        /// </summary>
        public IList<string> DescendingColumns { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value that indicates whether the index is clustered.
        /// </summary>
        public bool IsClustered { get; internal set; }

        /// <summary>
        /// Gets or sets the filter expression of the index, or <c>null</c> if the index is not filtered.
        /// </summary>
        public string Filter { get; internal set; }

        #endregion
    }
}
