using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class used to map a type of <see cref="DbConnection"/> into an instance of <see cref="IDbSetting"/> object.
    /// </summary>
    public static class DbSettingMapper
    {
        private static readonly ConcurrentDictionary<int, IDbSetting> m_maps = new ConcurrentDictionary<int, IDbSetting>();
        private static Type m_type = typeof(DbConnection);

        static DbSettingMapper()
        {
            // By default, map the Sql
            Add(typeof(SqlConnection), new SqlServerDbSetting(), true);
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
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get(Type type)
        {
            // Guard the type
            Guard(type);

            // Variables for the cache
            var value = (IDbSetting)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            IDbSetting dbSetting,
            bool @override)
        {
            // Guard the type
            Guard(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (IDbSetting)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, dbSetting, existing);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The database setting mapping to provider '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, dbSetting);
            }
        }
    }
}
