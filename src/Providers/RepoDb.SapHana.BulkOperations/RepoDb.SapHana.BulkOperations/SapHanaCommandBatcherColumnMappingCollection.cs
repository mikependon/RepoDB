#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.ObjectModel;

namespace RepoDb.SapHana.BulkOperations
{
    /// <summary>
    /// A collection used to define the source-to-destination column mappings of a <see cref="SapHanaCommandBatcher"/>.
    /// Reuses <see cref="SapHanaBulkInsertMapItem"/> (the same mapping item <see cref="Sap.Data.Hana.HanaBulkCopy"/>
    /// usage in this package binds against) rather than a duplicate type of its own.
    /// </summary>
    public sealed class SapHanaCommandBatcherColumnMappingCollection : Collection<SapHanaBulkInsertMapItem>
    {
        /// <summary>
        /// Adds a source-to-destination column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column or property name.</param>
        /// <param name="destinationColumn">The destination table's column name.</param>
        /// <returns>The added mapping.</returns>
        public SapHanaBulkInsertMapItem Add(
            string sourceColumn,
            string destinationColumn)
        {
            var mapping = new SapHanaBulkInsertMapItem(sourceColumn, destinationColumn);
            Add(mapping);
            return mapping;
        }
    }
}
