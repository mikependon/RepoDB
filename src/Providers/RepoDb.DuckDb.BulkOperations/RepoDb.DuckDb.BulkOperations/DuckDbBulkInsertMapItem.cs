#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.DuckDb.BulkOperations
{
    /// <summary>
    /// Defines a source-to-destination column mapping for the DuckDB bulk operations.
    /// </summary>
    public class DuckDbBulkInsertMapItem : BulkInsertMapItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuckDbBulkInsertMapItem"/>.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination column name.</param>
        public DuckDbBulkInsertMapItem(string sourceColumn,
            string destinationColumn) :
            base(sourceColumn, destinationColumn)
        { }
    }
}
