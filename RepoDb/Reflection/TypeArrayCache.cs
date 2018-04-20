using System;
using System.Collections.Generic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for an Array of Types.
    /// </summary>
    public static class TypeArrayCache
    {
        private static readonly IDictionary<TypeArrayCacheTypes, Type[]> _cache = new Dictionary<TypeArrayCacheTypes, Type[]>();

        /// <summary>
        /// Gets the cached array of Types based on type.
        /// </summary>
        /// <param name="type">The type of Types being cached.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] Get(TypeArrayCacheTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, Create(type));
            }
            return _cache[type];
        }

        /// <summary>
        /// Creates an array of Types based on the type of passed.
        /// </summary>
        /// <param name="type">The type of Type array to be created.</param>
        /// <returns>An array of Types.</returns>
        private static Type[] Create(TypeArrayCacheTypes type)
        {
            switch (type)
            {
                case TypeArrayCacheTypes.DataReaderTypes:
                    return new[] { TypeCache.Get(TypeCacheTypes.DataReaderType) };
                case TypeArrayCacheTypes.ObjectTypes:
                    return new[] { TypeCache.Get(TypeCacheTypes.ObjectType) };
                case TypeArrayCacheTypes.StringTypes:
                    return new[] { TypeCache.Get(TypeCacheTypes.StringType) };
                default:
                    return null;
            }
        }
    }
}
