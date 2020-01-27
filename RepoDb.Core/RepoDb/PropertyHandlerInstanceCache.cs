using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the property handler instances.
    /// </summary>
    internal static class PropertyHandlerInstanceCache
    {
        private static readonly ConcurrentDictionary<int, object> m_cache = new ConcurrentDictionary<int, object>();

        #region Methods

        /// <summary>
        /// Gets the cached handler instance for the type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the property handler.</typeparam>
        /// <returns>The cached property handler instance.</returns>
        public static object Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached handler instance for the type.
        /// </summary>
        /// <param name="type">The type of the property handler.</param>
        /// <returns>The cached property handler instance.</returns>
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
    }
}
