#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.MariaDb.BulkOperations
{
    /// <summary>
    /// Shared constants used across the MariaDb bulk operations.
    /// </summary>
    internal static class MariaDbConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.MariaDb.MariaDbBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.MariaDb.MariaDbBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.MariaDb.MariaDbBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
