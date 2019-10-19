using RepoDb.DbOperationProviders;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map the type of <see cref="DbConnection"/> into an instance of <see cref="IDbOperation"/> object.
    /// </summary>
    public static class DbOperationMapper
    {
        private static readonly ConcurrentDictionary<int, IDbOperation> m_maps = new ConcurrentDictionary<int, IDbOperation>();
        private static Type m_type = typeof(DbConnection);

        static DbOperationMapper()
        {
            // Default for SqlConnection
            Add(typeof(SqlConnection), new SqlServerDbOperation(), true);
        }

        /// <summary>
        /// Throws an exception if the type is not a sublcass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            if (type.IsSubclassOf(m_type) == false)
            {
                throw new InvalidTypeException($"Type must be a subclass of '{m_type.FullName}'.");
            }
        }

        /// <summary>
        /// Gets the mapped <see cref="IDbOperation"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of <see cref="IDbOperation"/> defined on the mapping.</returns>
        public static IDbOperation Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets the mapped <see cref="IDbOperation"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/>.</param>
        /// <returns>An instance of <see cref="IDbOperation"/> defined on the mapping.</returns>
        public static IDbOperation Get(Type type)
        {
            Guard(type);

            var value = (IDbOperation)null;

            if (m_maps.TryGetValue(type.FullName.GetHashCode(), out value))
            {
                return value;
            }

            throw new MissingMappingException($"There is no existing database operation provider mapping for '{type.FullName}'.");
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbOperation"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="statementBuilder">The statement builder to be mapped.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            IDbOperation statementBuilder,
            bool @override)
        {
            // Guard the type
            Guard(type);

            // Variables
            var value = (IDbOperation)null;
            var key = type.FullName.GetHashCode();

            // Try get the mappings
            if (m_maps.TryGetValue(key, out value))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, statementBuilder, value);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The database operation mapping to provider '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, statementBuilder);
            }
        }
    }
}
