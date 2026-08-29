namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// Shared constants used across the ClickHouse bulk operations.
    /// </summary>
    internal static class ClickHouseConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.ClickHouse.ClickHouseBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.ClickHouse.ClickHouseBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.ClickHouse.ClickHouseBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
