namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define the connection provider.
    /// </summary>
    public enum Provider : short
    {
        /// <summary>
        /// A data provider for Sql Server.
        /// </summary>
        Sql = 1,
        /// <summary>
        /// A data provider for Oracle.
        /// </summary>
        Oracle = 2,
        /// <summary>
        /// A data provider for Sqlite.
        /// </summary>
        Sqlite = 3,
        /// <summary>
        /// A data provider for Npgsql.
        /// </summary>
        Npgsql = 4,
        /// <summary>
        /// A data provider for MySql.
        /// </summary>
        MySql = 5,
        /// <summary>
        /// A data provider for OleDb.
        /// </summary>
        OleDb = 6
    }
}
