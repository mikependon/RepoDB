namespace RepoDb.MariaDbConnector.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the MariaDb bulk
    /// operations (<see cref="RepoDb.MariaDbConnector.BulkOperations"/>).
    /// </summary>
    public static partial class MariaDbTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string MariaDbBulkDelete = "MariaDbBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string MariaDbBulkDeleteByKey = "MariaDbBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string MariaDbBulkInsert = "MariaDbBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string MariaDbBulkMerge = "MariaDbBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string MariaDbBulkUpdate = "MariaDbBulkUpdate";
    }
}
