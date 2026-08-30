#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Firebird.BulkOperations
{
    /// <summary>
    /// Shared constants used across the Firebird bulk operations.
    /// </summary>
    internal static class FirebirdConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.Firebird.FirebirdBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.Firebird.FirebirdBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.Firebird.FirebirdBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
