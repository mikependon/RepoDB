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
        private static readonly ConcurrentDictionary<string, string> m_cache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the cached mapped-name of the property.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(PropertyInfo property, bool quoted = true)
        {
            var key = string.Concat(property.DeclaringType.FullName, ".", property.Name, ".", quoted.ToString());
            var result = (string)null;
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = PropertyInfoExtension.GetMappedName(property, quoted);
                m_cache.TryAdd(key, result);
            }
            return result;
        }
    }
}
