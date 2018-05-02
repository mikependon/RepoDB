using System.Collections.Generic;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a System.Reflection.FieldInfoCache.
    /// </summary>
    public static class FieldInfoCache
    {
        private static readonly IDictionary<FieldInfoTypes, FieldInfo> _cache = new Dictionary<FieldInfoTypes, FieldInfo>();

        /// <summary>
        /// Gets the System.Reflection.FieldInfo based on type.
        /// </summary>
        /// <param name="type">The type of System.Reflection.FieldInfo to be cached.</param>
        /// <returns>A System.Reflection.FieldInfo object.</returns>
        public static FieldInfo Get(FieldInfoTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateField(type));
            }
            return _cache[type];
        }
    }
}
