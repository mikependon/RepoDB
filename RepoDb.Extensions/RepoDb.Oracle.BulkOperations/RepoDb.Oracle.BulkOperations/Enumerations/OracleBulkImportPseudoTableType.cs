namespace RepoDb.Enumerations.Oracle
{
    /// <summary>
    /// An enumeration that is being used to define the type of staging (pseudo) table to be created
    /// during the <c>BulkMerge</c>, <c>BulkUpdate</c>, and <c>BulkDelete</c> operations.
    /// </summary>
    /// <remarks>
    /// Unlike the PostgreSQL bulk package, both values here are created <b>once</b> (the first time they
    /// are needed for a given table, within the process) and reused thereafter via a plain <c>DELETE</c> -
    /// never dropped and recreated per call. Oracle's <c>CREATE TABLE</c>/<c>DROP TABLE</c> are DDL and
    /// cause an implicit <c>COMMIT</c>, so recreating a staging table on every bulk call the way the
    /// PostgreSQL package does would silently commit whatever the caller's transaction had pending. See
    /// the package README for the full explanation.
    /// </remarks>
    public enum OracleBulkImportPseudoTableType : short
    {
        /// <summary>
        /// A Global Temporary Table (<c>CREATE GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS</c>) is
        /// used. Its rows are private to each session - concurrent sessions bulk-writing to the same table
        /// never see or interfere with each other's staged data, even though they share one table
        /// definition. This is the default, and the safe choice for concurrent/multi-connection workloads.
        /// </summary>
        Temporary,

        /// <summary>
        /// An ordinary heap table (<c>CREATE TABLE ... AS SELECT ...</c>) is used instead. It carries no
        /// per-session data isolation - every session/connection reads and writes the <em>same</em> rows,
        /// so two connections bulk-writing to the same target table concurrently with this option will
        /// corrupt or race each other's staged data. Only use this for workloads where calls against the
        /// same table are known to be sequential (e.g. a single-threaded batch job), in exchange for
        /// avoiding whatever session-temporary-object overhead your Oracle environment attaches to GTTs.
        /// </summary>
        Physical
    }
}
