namespace RepoDb.Db2.BulkOperations
{
    /// <summary>
    /// Shared constants used across the Db2 bulk operations.
    /// </summary>
    internal static class Db2Constants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.Db2.Db2BulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.Db2.Db2BulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.Db2.Db2BulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
