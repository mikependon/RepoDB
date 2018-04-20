using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a Type.
    /// </summary>
    public static class TypeCache
    {
        private static readonly IDictionary<TypeTypes, Type> _cache = new Dictionary<TypeTypes, Type>();

        /// <summary>
        /// Gets the cached Type based on type.
        /// </summary>
        /// <param name="type">The type of Type being cached.</param>
        /// <returns>An Type object.</returns>
        public static Type Get(TypeTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateType(type));
            }
            return _cache[type];
        }
    }
}
