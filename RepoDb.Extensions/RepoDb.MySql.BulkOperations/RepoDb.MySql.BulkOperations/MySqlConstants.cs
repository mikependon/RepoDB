namespace RepoDb.MySql.BulkOperations
{
    /// <summary>
    /// Shared constants used across the MySql bulk operations.
    /// </summary>
    internal static class MySqlConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.MySql.MySqlBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.MySql.MySqlBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.MySql.MySqlBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
