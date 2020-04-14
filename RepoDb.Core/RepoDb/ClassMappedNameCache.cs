using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name for the entity.
    /// </summary>
    public static class ClassMappedNameCache
    {
        private static readonly ConcurrentDictionary<int, string> m_cache = new ConcurrentDictionary<int, string>();

        #region Methods

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached mapped name of the entity.</returns>
        public static string Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        public static string Get(Type entityType)
        {
            // Validate
            ThrowNullReferenceException(entityType, "EntityType");

            // Variables
            var key = GenerateHashCode(entityType);
            var result = (string)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = DataEntityExtension.GetMappedName(entityType);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached class mapped names.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type)
        {
            return TypeExtension.GenerateHashCode(type);
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion
    }
}
