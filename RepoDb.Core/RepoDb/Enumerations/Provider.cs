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
        /// A data provider for SqLite.
        /// </summary>
        SqLite = 3,
        /// <summary>
        /// A data provider for PostgreSql.
        /// </summary>
        PostgreSql = 4,
        /// <summary>
        /// A data provider for MySql.
        /// </summary>
        MySql = 5,
        /// <summary>
        /// A data provider for Ole.
        /// </summary>
        Ole = 6
    }
}
