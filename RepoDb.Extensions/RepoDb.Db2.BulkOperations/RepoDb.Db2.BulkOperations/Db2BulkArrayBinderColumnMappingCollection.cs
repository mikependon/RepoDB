#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.ObjectModel;

namespace RepoDb.Db2.BulkOperations
{
    /// <summary>
    /// A collection used to define the source-to-destination column mappings of a <see cref="Db2BulkArrayBinder"/>,
    /// mirroring the shape of <see cref="DB2BulkCopyColumnMappingCollection"/>.
    /// </summary>
    public sealed class Db2BulkArrayBinderColumnMappingCollection : Collection<Db2BulkInsertMapItem>
    {
        /// <summary>
        /// Adds a source-to-destination column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination table's column name.</param>
        /// <returns>The added mapping.</returns>
        public Db2BulkInsertMapItem Add(
            string sourceColumn,
            string destinationColumn)
        {
            var mapping = new Db2BulkInsertMapItem(sourceColumn, destinationColumn);
            Add(mapping);
            return mapping;
        }
    }
}
