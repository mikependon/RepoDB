namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// Shared constants used across the Oracle bulk operations.
    /// </summary>
    internal static class OracleConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.Oracle.OracleBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.Oracle.OracleBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.Oracle.OracleBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
