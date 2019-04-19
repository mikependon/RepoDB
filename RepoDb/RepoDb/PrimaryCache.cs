using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the primary property of the entity.
    /// </summary>
    public static class PrimaryCache
    {
        private static readonly ConcurrentDictionary<string, ClassProperty> m_cache = new ConcurrentDictionary<string, ClassProperty>();

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get(Type type)
        {
            var key = type.FullName;
            var property = (ClassProperty)null;
            if (m_cache.TryGetValue(key, out property) == false)
            {
                var properties = PropertyCache.Get(type).Where(p => p.IsPrimary() == true);

                // Check if there is forced [Primary] attribute
                property = properties.FirstOrDefault(p => p.GetPrimaryAttribute() != null);

                // Otherwise, get any if present
                if (property == null)
                {
                    property = properties?.FirstOrDefault();
                }

                // Add to the cache (whatever)
                m_cache.TryAdd(key, property);
            }
            return property;
        }
    }
}
