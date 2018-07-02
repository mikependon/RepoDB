using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A static class used to map a statement builder (typeof <i>RepoDb.Interfaces.IStatementBuilder</i>) object into a database connection type
    /// (typeof <i>System.Data.DbConnection</i>) object. The mapping defines by this class will bypass all the mappings made on the repository
    /// level for the specified database connection type.
    /// </summary>
    public static class StatementBuilderMapper
    {
        private static readonly object _syncLock;
        private static readonly IList<StatementBuilderMap> _maps;

        static StatementBuilderMapper()
        {
            // Properties
            _syncLock = new object();
            _maps = new List<StatementBuilderMap>();

            // Default for SqlDbConnection
            Map(typeof(SqlConnection), new SqlDbStatementBuilder());
        }

        /// <summary>
        /// Gets an instance of mapping defined for the target type.
        /// </summary>
        /// <param name="dbConnectionType">
        /// The target type of the database connection to be used for mapping. This must be of type <i>System.Data.DbConnection</i>, or else,
        /// an argument exception will be thrown.
        /// </param>
        /// <returns>An instance of <i>RepoDb.Interfaces.StatementBuilderMap</i> defined on the mapping.</returns>
        public static StatementBuilderMap Get(Type dbConnectionType)
        {
            var info = dbConnectionType.GetTypeInfo();
            if (!info.IsSubclassOf(typeof(IDbConnection)) && !info.IsSubclassOf(typeof(DbConnection)))
            {
                throw new ArgumentException($"Argument 'dbConnectionType' must be a sub class of '{typeof(DbConnection).FullName}'.");
            }
            return _maps.FirstOrDefault(m => m.DbConnectionType == dbConnectionType);
        }

        /// <summary>
        /// Creates a mapping between the statement builder (typeof <i>RepoDb.Interfaces.IStatementBuilder</i>) object and database connection type
        /// (typeof <i>System.Data.DbConnection</i>) object.
        /// </summary>
        /// <param name="dbConnectionType">
        /// The target type of the database connection to be used for mapping. This must be of type <i>System.Data.DbConnection</i>, or else,
        /// an argument exception will be thrown.
        /// </param>
        /// <param name="statementBuilder">
        /// The statement builder to be mapped (typeof <i>RepoDb.Interfaces.IStatementBuilder</i>).
        /// </param>
        public static void Map(Type dbConnectionType, IStatementBuilder statementBuilder)
        {
            lock (_syncLock)
            {
                var map = Get(dbConnectionType);
                if (map != null)
                {
                    throw new InvalidOperationException($"An existing mapping for type {dbConnectionType.Name} is already exists.");
                }
                else
                {
                    _maps.Add(new StatementBuilderMap(dbConnectionType, statementBuilder));
                }
            }
        }
    }
}
