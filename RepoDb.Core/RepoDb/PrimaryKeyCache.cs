using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the primary property of the entity.
    /// </summary>
    public static class PrimaryKeyCache
    {
        private static readonly ConcurrentDictionary<string, ClassProperty> m_cache = new ConcurrentDictionary<string, ClassProperty>();

        /// <summary>
        /// Gets the cached primary key property for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            var property = (ClassProperty)null;
            if (m_cache.TryGetValue(key, out property) == false)
            {
                property = PropertyCache.Get<TEntity>().FirstOrDefault(p => p.IsPrimary() == true);
                m_cache.TryAdd(key, property);
            }
            return property;
        }
    }
}
