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
        private static readonly ConcurrentDictionary<int, string> m_cache = new ConcurrentDictionary<int, string>();

        #region Methods

        /// <summary>
        /// Gets the cached mapped-name of the property.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(PropertyInfo property, bool quoted = true)
        {
            var key = property.DeclaringType.FullName.GetHashCode()
                    ^ property.Name.GetHashCode()
                    ^ quoted.GetHashCode();
            var result = (string)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = PropertyInfoExtension.GetMappedName(property, quoted);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property mapped names.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
