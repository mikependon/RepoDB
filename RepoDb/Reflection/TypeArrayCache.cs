using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for an array of <i>System.Type</i> objects.
    /// </summary>
    public static class TypeArrayCache
    {
        private static readonly IDictionary<TypeTypes[], Type[]> _cache = new Dictionary<TypeTypes[], Type[]>();

        /// <summary>
        /// Gets the cached array of <i>System.Type</i> objects based on type.
        /// </summary>
        /// <param name="type">The type of <i>System.Type</i> being cached.</param>
        /// <returns>An array of <i>System.Type</i> objects.</returns>
        public static Type[] Get(params TypeTypes[] type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateTypes(type));
            }
            return _cache[type];
        }
    }
}
