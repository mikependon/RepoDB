using RepoDb.MySqlConnector.BulkOperations;

namespace RepoDb.Enumerations.MySqlConnector
{
    /// <summary>
    /// An enumeration that is being used to define the type of staging (pseudo) table to be created
    /// during the <c>BulkMerge</c>, <c>BulkUpdate</c>, and <c>BulkDelete</c> operations.
    /// </summary>
    /// <remarks>
    /// <b>Currently, every value resolves to <see cref="Physical"/> at runtime</b> - see the remarks on
    /// <see cref="Memory"/> below. This is a temporary, driver-level limitation, not a decision to remove
    /// <see cref="Auto"/>'s row-count threshold behavior or <see cref="Memory"/> permanently; both are kept
    /// here so existing code/signatures don't need to change once a working strategy for <see cref="Memory"/>
    /// is implemented.
    /// </remarks>
    public enum MySqlConnectorBulkImportPseudoTableType : short
    {
        /// <summary>
        /// Automatically chooses between <see cref="Physical"/> and <see cref="Memory"/> based on the
        /// number of rows/entities being bulk-written: <see cref="Physical"/> when the row count is greater
        /// than or equal to <see cref="MySqlConnectorConstants.RowCountThresholdForPhysicalTable"/> (<c>5,000</c>),
        /// otherwise <see cref="Memory"/>. This is the default. <b>Currently always resolves to
        /// <see cref="Physical"/> regardless of row count</b> - see the remarks on <see cref="Memory"/>.
        /// </summary>
        Auto,

        /// <summary>
        /// A Global Temporary Table (<c>CREATE GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS</c>) is
        /// used. Its rows are private to each session - concurrent sessions bulk-writing to the same table
        /// never see or interfere with each other's staged data, even though they share one table
        /// definition. The safe choice for concurrent/multi-connection workloads, and what <see cref="Auto"/>
        /// picks for smaller batches (fewer than <c>5,000</c> rows/entities).
        /// </summary>
        /// <remarks>
        /// <b>Not currently usable - always resolves to <see cref="Physical"/> instead.</b> Every bulk
        /// operation writes staged rows via <see cref="MySqlConnector.ManagedDataAccess.Client.MySqlBulkCopy"/>,
        /// which always performs a direct-path load internally (ODP.NET has no conventional-path
        /// alternative) - and MySqlConnector's direct-path engine cannot write into a Global Temporary Table at
        /// all, failing with <c>ORA-39826: Direct path load of view or synonym (...) could not be
        /// resolved</c> (confirmed live). This will be revisited once a working strategy exists (e.g.
        /// writing to the GTT via array-bound <c>INSERT</c>s instead of <c>MySqlBulkCopy</c>) or is fully
        /// supported by the ODP.NET library.
        /// </remarks>
        Memory,

        /// <summary>
        /// An ordinary heap table (<c>CREATE TABLE ... AS SELECT ...</c>) is used instead. It carries no
        /// per-session data isolation - every session/connection reads and writes the <em>same</em> rows,
        /// so two connections bulk-writing to the same target table concurrently with this option will
        /// corrupt or race each other's staged data. Only use this for workloads where calls against the
        /// same table are known to be sequential (e.g. a single-threaded batch job), in exchange for
        /// avoiding whatever session-temporary-object overhead your MySqlConnector environment attaches to GTTs.
        /// What <see cref="Auto"/> picks for larger batches (<c>5,000</c> rows/entities or more).
        /// </summary>
        Physical
    }
}
