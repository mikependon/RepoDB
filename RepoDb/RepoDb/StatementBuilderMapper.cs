using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A static class used to map a statement builder (typeof <see cref="IStatementBuilder"/>) object into a database connection type
    /// (typeof <see cref="DbConnection"/>) object. The mapping defines by this class will bypass all the mappings made on the repository
    /// level for the specified database connection type.
    /// </summary>
    public static class StatementBuilderMapper
    {
        private static readonly IList<StatementBuilderMap> m_maps;

        static StatementBuilderMapper()
        {
            // Properties
            m_maps = new List<StatementBuilderMap>();

            // Default for SqlDbConnection
            Map(typeof(SqlConnection), new SqlDbStatementBuilder());
        }

        /// <summary>
        /// Gets an instance of mapping defined for the target type.
        /// </summary>
        /// <param name="dbConnectionType">
        /// The target type of the database connection to be used for mapping. This must be of type <see cref="DbConnection"/>, or else,
        /// an argument exception will be thrown.
        /// </param>
        /// <returns>An instance of <see cref="StatementBuilderMap"/> defined on the mapping.</returns>
        public static StatementBuilderMap Get(Type dbConnectionType)
        {
            if (!dbConnectionType.IsSubclassOf(typeof(IDbConnection)) && !dbConnectionType.IsSubclassOf(typeof(DbConnection)))
            {
                throw new ArgumentException($"Argument 'dbConnectionType' must be a sub class of '{typeof(DbConnection).FullName}'.");
            }
            return m_maps.FirstOrDefault(m => m.DbConnectionType == dbConnectionType);
        }

        /// <summary>
        /// Creates a mapping between the statement builder (typeof <see cref="IStatementBuilder"/>) object and database connection type
        /// (typeof <see cref="DbConnection"/>) object.
        /// </summary>
        /// <param name="dbConnectionType">
        /// The target type of the database connection to be used for mapping. This must be of type <see cref="DbConnection"/>, or else,
        /// an argument exception will be thrown.
        /// </param>
        /// <param name="statementBuilder">
        /// The statement builder to be mapped (typeof <see cref="IStatementBuilder"/>).
        /// </param>
        public static void Map(Type dbConnectionType, IStatementBuilder statementBuilder)
        {
            var map = Get(dbConnectionType);
            if (map != null)
            {
                throw new InvalidOperationException($"An existing mapping for type {dbConnectionType.Name} is already exists.");
            }
            else
            {
                m_maps.Add(new StatementBuilderMap(dbConnectionType, statementBuilder));
            }
        }
    }
}
