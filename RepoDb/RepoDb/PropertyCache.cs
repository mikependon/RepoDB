using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the properties of the entity.
    /// </summary>
    public static class PropertyCache
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<ClassProperty>> m_cache = new ConcurrentDictionary<string, IEnumerable<ClassProperty>>();

        /// <summary>
        /// Gets the cached primary key property for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached properties of the entity.</returns>
        public static IEnumerable<ClassProperty> Get<TEntity>()
            where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            var properties = (IEnumerable<ClassProperty>)null;
            if (m_cache.TryGetValue(key, out properties) == false)
            {
                properties = ClassExpression.GetProperties<TEntity>();
                m_cache.TryAdd(key, properties);
            }
            return properties;
        }
    }
}
