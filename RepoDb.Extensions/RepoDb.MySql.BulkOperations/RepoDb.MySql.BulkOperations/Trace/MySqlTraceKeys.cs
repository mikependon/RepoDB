namespace RepoDb.MySql.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the MySql bulk
    /// operations (<see cref="RepoDb.MySql.BulkOperations"/>).
    /// </summary>
    public static partial class MySqlTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string MySqlBulkDelete = "MySqlBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string MySqlBulkDeleteByKey = "MySqlBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string MySqlBulkInsert = "MySqlBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string MySqlBulkMerge = "MySqlBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string MySqlBulkUpdate = "MySqlBulkUpdate";
    }
}
