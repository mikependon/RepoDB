using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to map a type of <see cref="DbConnection"/> into an instance of <see cref="IDbHelper"/> object.
    /// </summary>
    public static class DbHelperMapper
    {
        private static readonly ConcurrentDictionary<int, IDbHelper> m_maps = new ConcurrentDictionary<int, IDbHelper>();
        private static Type m_type = typeof(DbConnection);

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Database helper type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type is not a sublcass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            if (type.GetTypeInfo().IsSubclassOf(m_type) == false)
            {
                throw new InvalidTypeException($"Type must be a subclass of '{m_type.FullName}'.");
            }
        }

        /// <summary>
        /// Gets an existing <see cref="IDbHelper"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbHelper"/></returns>
        public static IDbHelper Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets an existing <see cref="IDbHelper"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <returns>An instance of mapped <see cref="IDbHelper"/></returns>
        public static IDbHelper Get(Type type)
        {
            // Guard the type
            Guard(type);

            // Variables for the cache
            var value = (IDbHelper)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbHelper"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="dbHelper">The instance of <see cref="IDbHelper"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            IDbHelper dbHelper,
            bool @override)
        {
            // Guard the type
            Guard(type);

            // Variables
            var key = type.FullName.GetHashCode();
            var existing = (IDbHelper)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, dbHelper, existing);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The database helper mapping to provider '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, dbHelper);
            }
        }

        /// <summary>
        /// Removes an mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbHelper"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="throwException">If true, it throws an exception if the mapping is not present.</param>
        /// <returns>True if the removal is successful, otherwise false.</returns>
        public static bool Remove(Type type,
            bool throwException = true)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (IDbHelper)null;
            var result = m_maps.TryRemove(key, out existing);

            // Throws an exception if necessary
            if (result == false && throwException == true)
            {
                throw new MissingMappingException($"There is no mapping defined for '{type.FullName}'.");
            }

            // Return false
            return result;
        }
    }
}
