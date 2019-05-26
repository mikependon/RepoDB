using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the identity property of the entity.
    /// </summary>
    public static class IdentityCache
    {
        private static readonly ConcurrentDictionary<int, ClassProperty> m_cache = new ConcurrentDictionary<int, ClassProperty>();

        #region Methods

        /// <summary>
        /// Gets the cached identity property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached identity property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached identity property of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached identity property.</returns>
        public static ClassProperty Get(Type type)
        {
            var key = type.FullName.GetHashCode();
            var property = (ClassProperty)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out property) == false)
            {
                property = PropertyCache.Get(type).FirstOrDefault(p => p.IsIdentity() == true);
                m_cache.TryAdd(key, property);
            }

            // Return the value
            return property;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached identity <see cref="ClassProperty"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
