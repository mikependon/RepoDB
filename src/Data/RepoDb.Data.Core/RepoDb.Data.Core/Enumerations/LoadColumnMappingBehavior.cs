#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define how the columns of the JSON file are aligned with the columns of the table when the rows are loaded with the bulk insert of the provider
    /// of the connection. It is passed on to the equivalent enumeration of the provider (i.e.: <c>SqlServerBulkColumnMappingsBehavior</c>), which has the same values. It is ignored
    /// when the provider has no bulk insert, as the rows are then inserted in batches by the columns that both the file and the table have.
    /// </summary>
    public enum LoadColumnMappingBehavior : short
    {
        /// <summary>
        /// Only the columns that exist on both the file and the table (matched by name) are loaded. All the other columns are ignored on both sides, and the order of the columns
        /// does not matter. An exception is still thrown if no column matched at all. This is the default behavior.
        /// </summary>
        Automatic,

        /// <summary>
        /// The columns of the file must be identical to the columns of the table in both name and order. Any difference (a missing column on either side, or a different order)
        /// throws the exception of the provider before any row is written.
        /// </summary>
        Strict,

        /// <summary>
        /// Every column of the file must exist on the table (matched by name), but the order of the columns does not matter. The columns of the table that are not supplied by the file
        /// are bypassed (left to their default, identity or computed values). A column of the file that does not exist on the table throws the exception of the provider before any
        /// row is written, so no data is silently dropped.
        /// </summary>
        StrictBypass
    }
}
