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
        private static readonly ConcurrentDictionary<string, ClassProperty> m_cache = new ConcurrentDictionary<string, ClassProperty>();

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
            var key = type.FullName;
            var property = (ClassProperty)null;
            if (m_cache.TryGetValue(key, out property) == false)
            {
                property = PropertyCache.Get(type).FirstOrDefault(p => p.IsIdentity() == true);
                m_cache.TryAdd(key, property);
            }
            return property;
        }
    }
}
