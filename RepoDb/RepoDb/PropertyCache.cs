using RepoDb.Enumerations;
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
        /// <param name="command">The target command.</param>
        /// <returns>The cached properties of the entity.</returns>
        public static IEnumerable<ClassProperty> Get<TEntity>(Command command = Command.None)
            where TEntity : class
        {
            var key = string.Concat(typeof(TEntity).FullName, ".", command);
            var properties = (IEnumerable<ClassProperty>)null;
            if (m_cache.TryGetValue(key, out properties) == false)
            {
                properties = ClassExpression.GetProperties<TEntity>(command);
                m_cache.TryAdd(key, properties);
            }
            return properties;
        }
    }
}
