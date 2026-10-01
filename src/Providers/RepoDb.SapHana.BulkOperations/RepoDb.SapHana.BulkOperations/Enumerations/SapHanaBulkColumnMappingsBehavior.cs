#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations.SapHana
{
    /// <summary>
    /// An enumeration that is being used to define how a bulk operation (bulk-insert, bulk-merge, bulk-update or
    /// bulk-delete) aligns the columns of the source (entities, <see cref="System.Data.DataTable"/> or
    /// <see cref="System.Data.Common.DbDataReader"/>) with the columns of the destination table. The behavior is
    /// only applied when no explicit mappings were passed to the operation; explicit mappings always take precedence.
    /// </summary>
    public enum SapHanaBulkColumnMappingsBehavior : short
    {
        /// <summary>
        /// A value that indicates that only the columns that exist on both the source and the destination table
        /// (matched by name) will be mapped. All other columns are ignored on both sides, and the order of the
        /// columns does not matter. An exception is still thrown if no column matched at all. This is the default
        /// behavior, and it is the behavior of the earlier versions of the library.
        /// </summary>
        Automatic,

        /// <summary>
        /// A value that indicates that the columns of the source must be identical to the columns of the destination
        /// table in both name and order. Any difference (a missing column on either side, or a different order) throws
        /// a <see cref="RepoDb.Exceptions.SapHanaBulkColumnMappingsException"/> before any data is written.
        /// </summary>
        Strict,

        /// <summary>
        /// A value that indicates that every column of the source must exist on the destination table (matched by name),
        /// but the order of the columns does not matter. The columns of the destination table that are not supplied by
        /// the source are bypassed (left to their default, identity or computed values). A source column that does not
        /// exist on the destination table throws a <see cref="RepoDb.Exceptions.SapHanaBulkColumnMappingsException"/>
        /// before any data is written, so no source data is ever silently dropped.
        /// </summary>
        StrictBypass
    }
}
