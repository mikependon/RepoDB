using System;
using RepoDb.Interfaces;
using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class used as an map-item when mapping a statement builder (typeof <i>RepoDb.Interfaces.IStatementBuilder</i>)
    /// to be used for every connection type (typeof <i>System.Data.DbConnection</i>).
    /// </summary>
    public class StatementBuilderMap
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.StatementBuilderMap</i> object.
        /// </summary>
        /// <param name="dbConnectionType">
        /// The target type of the database connection to be used for mapping. This must be of type <i>System.Data.DbConnection</i>, or else,
        /// an argument exception will be thrown.
        /// </param>
        /// <param name="statementBuilder">The statement builder to be used for mapping.</param>
        public StatementBuilderMap(Type dbConnectionType, IStatementBuilder statementBuilder)
        {
            if (!dbConnectionType.IsSubclassOf(typeof(IDbConnection)) && !dbConnectionType.IsSubclassOf(typeof(DbConnection)))
            {
                throw new ArgumentException($"Argument 'dbConnectionType' must be a sub class of '{typeof(DbConnection).FullName}'.");
            }
            DbConnectionType = dbConnectionType;
            StatementBuilder = statementBuilder;
        }

        /// <summary>
        /// Gets the type of the database connection object that is being used for mapping.
        /// </summary>
        public Type DbConnectionType { get; }

        /// <summary>
        /// Gets the statement builder object that is being used for mapping.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; internal set; }
    }
}
