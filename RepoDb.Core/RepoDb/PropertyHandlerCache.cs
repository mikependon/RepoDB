using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the property handlers.
    /// </summary>
    internal static class PropertyHandlerCache
    {
        private static readonly ConcurrentDictionary<int, object> m_cache = new ConcurrentDictionary<int, object>();

        #region Methods

        /// <summary>
        /// Gets the cached handler for the type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the property handler.</typeparam>
        /// <returns>The cached property handler.</returns>
        public static object Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        public static object Get(Type type)
        {
            var key = type.FullName.GetHashCode();
            var result = (object)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = Activator.CreateInstance(type);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached handlers.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
