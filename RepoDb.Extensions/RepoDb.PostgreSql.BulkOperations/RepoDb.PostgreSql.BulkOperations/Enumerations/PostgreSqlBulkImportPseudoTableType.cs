namespace RepoDb.Enumerations.PostgreSql
{
    /// <summary>
    /// An enumeration that is being used to define the type of pseudo-temporary table to be
    /// created during the bulk-import operations.
    /// </summary>
    public enum PostgreSqlBulkImportPseudoTableType : short
    {
        /// <summary>
        /// Automatically chooses between <see cref="Physical"/> and <see cref="Memory"/> based on the
        /// number of rows/entities being bulk-written: <see cref="Physical"/> when the row count is greater
        /// than or equal to <see cref="OracleConstants.RowCountThresholdForPhysicalTable"/> (<c>5,000</c>),
        /// otherwise <see cref="Memory"/>. This is the default. <b>Currently always resolves to
        /// <see cref="Physical"/> regardless of row count</b> - see the remarks on <see cref="Memory"/>.
        /// </summary>
        Auto,

        /// <summary>
        /// A temporary pseudo-table will be created. The table is dedicated to the session of the 
        /// connection and is automatically being destroyed once the connection is closed/disposed.
        /// Use this if you are working within an asynchronous environment.
        /// </summary>
        Temporary,
        /// <summary>
        /// A physical pseudo-table will be created. The table is shared to any other connections. 
        /// Use this if you prefer performance and is not working within an asynchronous environment.
        /// </summary>
        Physical
    }
}
