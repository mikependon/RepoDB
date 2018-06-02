using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a <i>System.Type</i> objects.
    /// </summary>
    public static class TypeCache
    {
        private static readonly IDictionary<TypeTypes, Type> _cache = new Dictionary<TypeTypes, Type>();

        /// <summary>
        /// Gets the cached <i>System.Type</i> based on type.
        /// </summary>
        /// <param name="type">The type of <i>System.Type</i> being cached.</param>
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
