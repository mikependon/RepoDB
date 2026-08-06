namespace RepoDb.MySqlConnector.BulkOperations
{
    /// <summary>
    /// Shared constants used across the MySqlConnector bulk operations.
    /// </summary>
    internal static class MySqlConnectorConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.MySqlConnector.MySqlConnectorBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.MySqlConnector.MySqlConnectorBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.MySqlConnector.MySqlConnectorBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
