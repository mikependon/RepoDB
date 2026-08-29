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
