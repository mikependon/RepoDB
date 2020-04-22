using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map a class into its equivalent database object (ie: Table, View). This is an alternative class to <see cref="MapAttribute"/> object for class mapping.
    /// </summary>
    public static class ClassMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, string> m_maps = new ConcurrentDictionary<int, string>();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a data entity and the database object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="databaseObjectName">The name of the database object (ie: Table, View).</param>
        public static void Add<TEntity>(string databaseObjectName)
            where TEntity : class =>
            Add(typeof(TEntity), databaseObjectName);

        /// <summary>
        /// Adds a mapping between a data entity and the database object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="databaseObjectName">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string databaseObjectName,
            bool force)
            where TEntity : class =>
            Add(typeof(TEntity), databaseObjectName, force);

        /// <summary>
        /// Adds a mapping between a data entity and the database object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="databaseObjectName">The name of the database object (ie: Table, View).</param>
        public static void Add(Type entityType,
            string databaseObjectName) =>
            Add(entityType, databaseObjectName, false);

        /// <summary>
        /// Adds a mapping between a data entity and the database object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="databaseObjectName">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type entityType,
            string databaseObjectName,
            bool force)
        {
            // Validate
            Validate(databaseObjectName);

            // Variables
            var key = entityType.FullName.GetHashCode();
            var value = (string)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, databaseObjectName, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"A class mapping to '{entityType.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, databaseObjectName);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped name of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The mapped name of the class.</returns>
        public static string Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the mapped name of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The mapped name of the class.</returns>
        public static string Get(Type entityType)
        {
            var value = (string)null;
            var key = entityType.FullName.GetHashCode();

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        public static void Remove<TEntity>()
            where TEntity : class =>
            Remove(typeof(TEntity));

        /// <summary>
        /// Removes the mapping of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        public static void Remove(Type entityType)
        {
            var key = entityType.FullName.GetHashCode();
            var value = (string)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached class mapped names.
        /// </summary>
        public static void Clear()
        {
            m_maps.Clear();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Validates the database object name that is being passed.
        /// </summary>
        /// <param name="databaseObjectName">The target database object name.</param>
        private static void Validate(string databaseObjectName)
        {
            if (string.IsNullOrEmpty(databaseObjectName?.Trim()))
            {
                throw new NullReferenceException("The database object name cannot be null.");
            }
        }

        #endregion
    }
}
