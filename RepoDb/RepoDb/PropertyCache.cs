using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the properties of the entity.
    /// </summary>
    public static class PropertyCache
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<ClassProperty>> m_cache = new ConcurrentDictionary<string, IEnumerable<ClassProperty>>();

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
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

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> Get(Type type)
        {
            var key = type.FullName;
            var properties = (IEnumerable<ClassProperty>)null;
            if (m_cache.TryGetValue(key, out properties) == false)
            {
                properties = type.GetProperties().Select(p => new ClassProperty(p));
                m_cache.TryAdd(key, properties);
            }
            return properties;
        }
    }
}
