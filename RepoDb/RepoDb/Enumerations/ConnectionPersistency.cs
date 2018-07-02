namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that defines the persistency of the <i>System.Data.Common.DbConnection</i> object used by the repository.
    /// </summary>
    public enum ConnectionPersistency
    {
        /// <summary>
        /// A new connection is being created on every call of the repository operation.
        /// </summary>
        PerCall,
        /// <summary>
        /// A single connection is being used until the lifetime of the repository.
        /// </summary>
        Instance,
        ///// <summary>
        ///// A single connection is being used by all repositories.
        ///// </summary>
        //Singleton
    }
}
