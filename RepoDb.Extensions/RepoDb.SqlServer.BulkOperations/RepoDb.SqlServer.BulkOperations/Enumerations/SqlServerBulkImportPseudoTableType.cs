using RepoDb.SqlServer.BulkOperations;

namespace RepoDb.Enumerations.SqlServer
{
    /// <summary>
    /// An enumeration that is being used to define the type of the pseudo (staging) table that will be created
    /// and used towards the underlying target table during a bulk-import (bulk-insert, bulk-update, bulk-merge or
    /// bulk-delete) operation.
    /// </summary>
    public enum SqlServerBulkImportPseudoTableType : short
    {
        /// <summary>
        /// A value that indicates that the type of the pseudo (staging) table will be automatically determined
        /// based on the number of rows/entities being processed. A <see cref="Memory"/> table will be used unless
        /// the row/entity count reaches the <see cref="RepoDb.SqlServer.BulkOperations.SqlServerConstants.RowCountThresholdForPhysicalTable"/>
        /// threshold, in which case a <see cref="Physical"/> table will be used instead. This is the default behavior.
        /// </summary>
        Auto,

        /// <summary>
        /// A value that indicates that a local (session-scoped) SQL Server temporary table (i.e., a table prefixed
        /// with a single '#') will be used as the pseudo (staging) table.
        /// </summary>
        Memory,

        /// <summary>
        /// A value that indicates that a real, permanent table on the target database will be used as the pseudo
        /// (staging) table.
        /// </summary>
        Physical
    }
}
