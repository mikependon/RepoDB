namespace RepoDb.Enumerations.PostgreSql
{
    /// <summary>
    /// An enumeration that is being used to define the type of pseudo-temporary table to be
    /// created during the bulk-import operations.
    /// </summary>
    public enum BulkImportPseudoTableType : short
    {
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
