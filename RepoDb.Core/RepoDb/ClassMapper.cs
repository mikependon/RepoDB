using RepoDb.Exceptions;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to map a class into its equivalent database object (ie: Table, View).
    /// </summary>
    public static class ClassMapper
    {
        private static readonly ConcurrentDictionary<int, string> m_maps = new ConcurrentDictionary<int, string>();

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a class and the database object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR Type to be mapped.</typeparam>
        /// <param name="objectName">The name of the database object (ie: Table, View).</param>
        public static void Add<T>(string objectName)
            where T : class => Add(typeof(T), objectName);

        /// <summary>
        /// Adds a mapping between a class and the database object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR Type to be mapped.</typeparam>
        /// <param name="objectName">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(string objectName,
            bool force)
            where T : class => Add(typeof(T), objectName, force);

        /// <summary>
        /// Adds a mapping between a class and the database object.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="objectName">The name of the database object (ie: Table, View).</param>
        public static void Add(Type type,
            string objectName) => Add(type, objectName, false);

        /// <summary>
        /// Adds a mapping between a class and the database object.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="objectName">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            string objectName,
            bool force)
        {
            // Validate
            Validate(objectName);

            // Variables
            var key = type.FullName.GetHashCode();
            var value = (string)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, objectName, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"Mapping to '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, objectName);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped name of the class.
        /// </summary>
        /// <typeparam name="T">The dynamic .NET CLR Type used for mapping.</typeparam>
        /// <returns>The mapped name of the class.</returns>
        public static string Get<T>()
            where T : class => Get(typeof(T));

        /// <summary>
        /// Gets the mapped name of the class.
        /// </summary>
        /// <param name="type">The .NET CLR Type used for mapping.</param>
        /// <returns>The mapped name of the class.</returns>
        public static string Get(Type type)
        {
            var value = (string)null;
            var key = type.FullName.GetHashCode();

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes a mapping of targetted .NET CLR Type from the collection.
        /// </summary>
        /// <typeparam name="T">The .NET CLR Type mapping to be removed.</typeparam>
        public static void Remove<T>()
            where T : class => Remove(typeof(T));

        /// <summary>
        /// Removes a mapping of targetted .NET CLR Type from the collection.
        /// </summary>
        /// <param name="type">The .NET CLR Type mapping to be removed.</param>
        public static void Remove(Type type)
        {
            var key = type.FullName.GetHashCode();
            var value = (string)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached class mapped names.
        /// </summary>
        public static void Flush()
        {
            m_maps.Clear();
        }

        /// <summary>
        /// Validates the database object name that is being passed.
        /// </summary>
        /// <param name="objectName">The target database object name.</param>
        private static void Validate(string objectName)
        {
            if (string.IsNullOrEmpty(objectName?.Trim()))
            {
                throw new NullReferenceException("The database object name cannot be null.");
            }
        }

        #endregion
    }
}
