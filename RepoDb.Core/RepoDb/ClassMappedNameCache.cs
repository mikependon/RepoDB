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
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached mapped name of the entity.</returns>
        public static string Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        internal static string Get(Type type)
        {
            var key = type.FullName.GetHashCode();
            var result = (string)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = DataEntityExtension.GetMappedName(type);
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

        #endregion
    }
}
