namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the PostgreSql bulk
    /// operations (<see cref="RepoDb.PostgreSql.BulkOperations"/>). Extends the core <see cref="TraceKeys"/>
    /// (defined in <c>RepoDb.Core</c>) rather than replacing it.
    /// </summary>
    public static partial class PostgreSqlTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string PostgreSqlBulkDelete = "PostgreSqlBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string PostgreSqlBulkDeleteByKey = "PostgreSqlBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string PostgreSqlBulkInsert = "PostgreSqlBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string PostgreSqlBulkMerge = "PostgreSqlBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string PostgreSqlBulkUpdate = "PostgreSqlBulkUpdate";
    }
}
