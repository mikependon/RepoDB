namespace RepoDb.ClickHouse.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the ClickHouse bulk
    /// operations (<see cref="RepoDb.ClickHouse.BulkOperations"/>).
    /// </summary>
    public static partial class ClickHouseTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string ClickHouseBulkDelete = "ClickHouseBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string ClickHouseBulkDeleteByKey = "ClickHouseBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string ClickHouseBulkInsert = "ClickHouseBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string ClickHouseBulkMerge = "ClickHouseBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string ClickHouseBulkUpdate = "ClickHouseBulkUpdate";
    }
}
