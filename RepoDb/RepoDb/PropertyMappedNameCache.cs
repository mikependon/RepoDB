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
            var result = (string)null;
            if (m_cache.TryGetValue(property, out result) == false)
            {
                result = PropertyInfoExtension.GetMappedName(property);
                m_cache.TryAdd(property, result);
            }
            return result;
        }
    }
}
