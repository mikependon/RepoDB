using RepoDb.MySqlConnector.BulkOperations;

namespace RepoDb.Enumerations.MySqlConnector
{
    /// <summary>
    /// Specifies what kind of staging table backs a <c>BulkMerge</c>, <c>BulkUpdate</c>, or <c>BulkDelete</c>
    /// operation against MySQL via MySqlConnector.
    /// </summary>
    public enum MySqlConnectorBulkImportPseudoTableType : short
    {
        /// <summary>
        /// Automatically selects <see cref="Physical"/> when the entity/row count being bulk-written is at
        /// least <see cref="MySqlConnectorConstants.RowCountThresholdForPhysicalTable"/>, otherwise selects
        /// <see cref="Memory"/>. This is the default.
        /// </summary>
        Auto,

        /// <summary>
        /// Backs the operation with a temporary table. Rows are session-private, making the execution isolated
        /// to any concurrent executions from different connections.
        /// </summary>
        Memory,

        /// <summary>
        /// Backs the operation with an ordinary heap table. Rows are not session-isolated, so concurrent
        /// callers can see and interfere with each other's staged rows, but is best for performing fast and
        /// more efficient bulk operations.
        /// </summary>
        Physical
    }
}
