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
        /// <param name="statementBuilder">The statement builder to be mapped.</param>
        /// <param name="force">True if to overwrite existing mapping if present.</param>
        public static void Map(Provider provider, IStatementBuilder statementBuilder, bool force = false)
        {
            Map(new StatementBuilderMapItem(provider, statementBuilder));
        }

        /// <summary>
        /// Creates a mapping between the <see cref="Provider"/> and <see cref="IStatementBuilder"/>) object.
        /// </summary>
        /// <param name="mapItem">The map item object.</param>
        /// <param name="force">True if to overwrite existing mapping if present.</param>
        public static void Map(StatementBuilderMapItem mapItem, bool force = false)
        {
            var mapping = Get(mapItem.Provider);
            if (mapping == null)
            {
                m_maps.Add(mapItem);
            }
            else
            {
                if (force)
                {
                    mapping.StatementBuilder = mapItem.StatementBuilder;
                }
                else
                {
                    throw new InvalidOperationException($"Mapping is already exists for provider {mapItem.Provider.ToString()}.");
                }
            }
        }
    }
}
