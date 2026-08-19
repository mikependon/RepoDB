namespace RepoDb.MariaDbConnector.BulkOperations
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
