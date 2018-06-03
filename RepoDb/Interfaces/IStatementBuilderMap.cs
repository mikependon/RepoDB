using System;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a statement builder map item. A statement builder map-item is a class used when mapping a statement builder 
    /// (typeof <i>RepoDb.Interfaces.IStatementBuilder</i>) to be used for every connection type (typeof <i>System.Data.DbConnection</i>). 
    /// </summary>
    public interface IStatementBuilderMap
    {
        /// <summary>
        /// Gets the type of the database connection used for mapping.
        /// </summary>
        Type DbConnectionType { get; }

        /// <summary>
        /// Gets the statement builder used for mapping.
        /// </summary>
        IStatementBuilder StatementBuilder { get;  }
    }
}