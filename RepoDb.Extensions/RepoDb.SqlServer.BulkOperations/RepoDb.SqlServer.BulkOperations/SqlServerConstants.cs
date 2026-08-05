namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// Shared constants used across the SqlServer bulk operations.
    /// </summary>
    internal static class SqlServerConstants
    {
        /// <summary>
        /// The row/entity count at (and above) which <see cref="RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Auto"/>
        /// resolves to <see cref="RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Physical"/> instead of
        /// <see cref="RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType.Memory"/>.
        /// </summary>
        public const int RowCountThresholdForPhysicalTable = 5000;
    }
}
