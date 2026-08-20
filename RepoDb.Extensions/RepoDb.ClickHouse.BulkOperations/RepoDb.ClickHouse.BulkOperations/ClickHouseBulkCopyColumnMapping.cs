namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ClickHouseBulkCopyColumnMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceOrdinal"></param>
        /// <param name="destinationColumn"></param>
        public ClickHouseBulkCopyColumnMapping(int sourceOrdinal,
            string destinationColumn)
        {
            SourceOrdinal = sourceOrdinal;
            DestinationColumn = destinationColumn;
        }

        /// <summary>
        /// Gets the ordinal of the source column that supplies this mapping's value.
        /// </summary>
        public int SourceOrdinal { get; }

        /// <summary>
        /// Gets the raw (unquoted) destination column name.
        /// </summary>
        public string DestinationColumn { get; }
    }
}
