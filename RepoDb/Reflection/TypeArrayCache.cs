using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for an Array of Types.
    /// </summary>
    public static class TypeArrayCache
    {
        private static readonly IDictionary<TypeTypes, Type[]> _cache = new Dictionary<TypeTypes, Type[]>();

        /// <summary>
        /// Gets the cached array of Types based on type.
        /// </summary>
        /// <param name="type">The type of Types being cached.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] Get(TypeTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateTypes(type));
            }
            return _cache[type];
        }

    }
}
