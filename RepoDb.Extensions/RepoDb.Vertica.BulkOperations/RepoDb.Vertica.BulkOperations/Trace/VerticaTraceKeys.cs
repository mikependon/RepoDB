namespace RepoDb.Vertica.BulkOperations
{
    /// <summary>
    /// A class that holds the constant values of the operation tracking keys used by the Vertica bulk
    /// operations (<see cref="RepoDb.Vertica.BulkOperations"/>).
    /// </summary>
    public static partial class VerticaTraceKeys
    {
        /// <summary>
        /// The trace key for the <c>BulkDelete</c> operation.
        /// </summary>
        public const string VerticaBulkDelete = "VerticaBulkDelete";

        /// <summary>
        /// The trace key for the <c>BulkDeleteByKey</c> operation.
        /// </summary>
        public const string VerticaBulkDeleteByKey = "VerticaBulkDeleteByKey";

        /// <summary>
        /// The trace key for the <c>BulkInsert</c> operation.
        /// </summary>
        public const string VerticaBulkInsert = "VerticaBulkInsert";

        /// <summary>
        /// The trace key for the <c>BulkMerge</c> operation.
        /// </summary>
        public const string VerticaBulkMerge = "VerticaBulkMerge";

        /// <summary>
        /// The trace key for the <c>BulkUpdate</c> operation.
        /// </summary>
        public const string VerticaBulkUpdate = "VerticaBulkUpdate";
    }
}
