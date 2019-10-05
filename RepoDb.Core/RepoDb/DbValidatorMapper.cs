using RepoDb.DbValidators.SqlServer;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to map a type of <see cref="DbConnection"/> into an instance of <see cref="IDbValidator"/> object.
    /// </summary>
    public static class DbValidatorMapper
    {
        private static readonly ConcurrentDictionary<int, IDbValidator> m_maps = new ConcurrentDictionary<int, IDbValidator>();
        private static Type m_type = typeof(DbConnection);

        static DbValidatorMapper()
        {
            // By default, map the Sql
            Add(typeof(SqlConnection), new SqlServerDbValidator());
        }

        /// <summary>
        /// Throws an exception if the type is not a sublcass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            if (type.GetTypeInfo().IsSubclassOf(m_type) == false)
            {
                throw new InvalidOperationException($"Type must be a subclass of '{m_type.FullName}'.");
            }
        }

        /// <summary>
        /// Gets an existing <see cref="IDbValidator"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbValidator"/></returns>
        public static IDbValidator Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets an existing <see cref="IDbValidator"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <returns>An instance of mapped <see cref="IDbValidator"/></returns>
        public static IDbValidator Get(Type type)
        {
            // Guard the type
            Guard(type);

            // Variables for the cache
            var value = (IDbValidator)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbValidator"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="dbValidator">The instance of <see cref="IDbValidator"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type, IDbValidator dbValidator, bool @override = false)
        {
            // Guard the type
            Guard(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (IDbValidator)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, dbValidator, existing);
                }
                else
                {
                    // Throw an exception
                    throw new InvalidOperationException($"Mapping to validator '{key}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, dbValidator);
            }
        }
    }
}
