using System.Collections.Generic;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a <i>System.Reflection.FieldInfoCache</i>.
    /// </summary>
    public static class FieldInfoCache
    {
        private static readonly IDictionary<FieldInfoTypes, FieldInfo> _cache = new Dictionary<FieldInfoTypes, FieldInfo>();

        /// <summary>
        /// Gets the <i>System.Reflection.FieldInfo</i> based on type.
        /// </summary>
        /// <param name="type">The type of <i>System.Reflection.FieldInfo</i> to be cached.</param>
        /// <returns>A <i>System.Reflection.FieldInfo</i> object.</returns>
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
