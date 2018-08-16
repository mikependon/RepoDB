using RepoDb.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name of the property.
    /// </summary>
    public static class PropertyMappedNameCache
    {
        private static readonly ConcurrentDictionary<PropertyInfo, string> m_cache = new ConcurrentDictionary<PropertyInfo, string>();

        /// <summary>
        /// Gets the cached mapped-name of the property.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(PropertyInfo property)
        {
            if (property==null)
            {
                return null;
            }
            var result = (string)null;
            var key = property;
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = ClassExpression.GetPropertyMappedName(property);
                m_cache.TryAdd(key, result);
            }
            return result;
        }
    }
}
