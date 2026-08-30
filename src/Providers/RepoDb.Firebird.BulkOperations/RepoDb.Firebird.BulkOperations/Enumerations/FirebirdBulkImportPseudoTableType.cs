#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Firebird.BulkOperations;

namespace RepoDb.Enumerations.Firebird
{
    /// <summary>
    /// Specifies what kind of staging table backs a <c>BulkInsert</c> (with <see cref="FirebirdBulkImportIdentityBehavior.ReturnIdentity"/>),
    /// <c>BulkMerge</c>, <c>BulkUpdate</c>, or <c>BulkDelete</c> operation against Firebird.
    /// </summary>
    /// <remarks>
    /// Every pseudo table is created with a per-call unique name (see <see cref="FirebirdText"/>), so unlike
    /// some other providers' bulk-operations packages, <see cref="Physical"/> and <see cref="Memory"/> are
    /// both safe for concurrent callers writing against the same target table - there is no shared,
    /// deterministic staging-table name for them to race on.
    /// </remarks>
    public enum FirebirdBulkImportPseudoTableType : short
    {
        /// <summary>
        /// Automatically selects <see cref="Physical"/> when the entity/row count being bulk-written is at
        /// least <see cref="FirebirdConstants.RowCountThresholdForPhysicalTable"/>, otherwise selects
        /// <see cref="Memory"/>. This is the default.
        /// </summary>
        Auto,

        /// <summary>
        /// Backs the operation with a Firebird <c>GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS</c>.
        /// Rows are private to the connection that wrote them.
        /// </summary>
        Memory,

        /// <summary>
        /// Backs the operation with an ordinary heap table. Faster to create for very large row counts than
        /// a global temporary table's per-connection storage, at the cost of the rows briefly existing as an
        /// ordinary (if uniquely-named) table.
        /// </summary>
        Physical
    }
}
