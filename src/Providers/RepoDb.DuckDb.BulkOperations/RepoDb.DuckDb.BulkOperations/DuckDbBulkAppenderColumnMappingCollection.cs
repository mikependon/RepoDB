#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.ObjectModel;

namespace RepoDb.DuckDb.BulkOperations
{
    /// <summary>
    /// The source-to-destination column mappings of a <see cref="DuckDbBulkAppender"/>.
    /// </summary>
    public sealed class DuckDbBulkAppenderColumnMappingCollection : Collection<DuckDbBulkInsertMapItem>
    {
        /// <summary>
        /// Adds a source-to-destination column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination column name.</param>
        /// <returns>The added mapping.</returns>
        public DuckDbBulkInsertMapItem Add(string sourceColumn,
            string destinationColumn)
        {
            var mapping = new DuckDbBulkInsertMapItem(sourceColumn, destinationColumn);
            Add(mapping);
            return mapping;
        }
    }
}
