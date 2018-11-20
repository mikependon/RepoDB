using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to map a <see cref="Provider"/> to a <see cref="IStatementBuilder"/> object.
    /// </summary>
    public static class StatementBuilderMapper
    {
        private static readonly IList<StatementBuilderMapItem> m_maps;

        static StatementBuilderMapper()
        {
            // Properties
            m_maps = new List<StatementBuilderMapItem>();

            // Default for SqlDbConnection
            Map(Provider.Sql, new SqlDbStatementBuilder());
        }

        /// <summary>
        /// Gets an instance of mapping defined for the target <see cref="Provider"/>.
        /// </summary>
        /// <param name="provider">The target provider.</param>
        /// <returns>An instance of <see cref="StatementBuilderMapItem"/> defined on the mapping.</returns>
        public static StatementBuilderMapItem Get(Provider provider)
        {
            return m_maps.FirstOrDefault(m => m.Provider == provider);
        }

        /// <summary>
        /// Creates a mapping between the <see cref="Provider"/> and <see cref="IStatementBuilder"/>) object.
        /// </summary>
        /// <param name="provider">The target provider.</param>
        /// <param name="statementBuilder">
        /// The statement builder to be mapped (typeof <see cref="IStatementBuilder"/>).
        /// </param>
        public static void Map(Provider provider, IStatementBuilder statementBuilder)
        {
            Map(new StatementBuilderMapItem(provider, statementBuilder));
        }

        /// <summary>
        /// Creates a mapping between the <see cref="Provider"/> and <see cref="IStatementBuilder"/>) object.
        /// </summary>
        /// <param name="mapItem">The map item object.</param>
        public static void Map(StatementBuilderMapItem mapItem)
        {
            if (Get(mapItem.Provider) != null)
            {
                throw new InvalidOperationException($"Mapping is already exists for provider {mapItem.Provider.ToString()}.");
            }
            else
            {
                m_maps.Add(mapItem);
            }
        }
    }
}
