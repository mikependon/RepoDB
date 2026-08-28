namespace RepoDb.Vertica.BulkOperations
{
    /// <summary>
    /// Shared constants used across the Vertica bulk operations.
    /// </summary>
    internal static class VerticaConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.Vertica.VerticaBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.Vertica.VerticaBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.Vertica.VerticaBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
