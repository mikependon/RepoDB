using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a Type.
    /// </summary>
    public static class TypeCache
    {
        private static readonly IDictionary<TypeCacheTypes, Type> _cache = new Dictionary<TypeCacheTypes, Type>();

        /// <summary>
        /// Gets the cached Type based on type.
        /// </summary>
        /// <param name="type">The type of Type being cached.</param>
        /// <returns>An Type object.</returns>
        public static Type Get(TypeCacheTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, Create(type));
            }
            return _cache[type];
        }

        /// <summary>
        /// Creates a Type based on type.
        /// </summary>
        /// <param name="type">The Type to be created.</param>
        /// <returns>A type object.</returns>
        public static Type Create(TypeCacheTypes type)
        {
            switch (type)
            {
                case TypeCacheTypes.ConvertType:
                    return typeof(Convert);
                case TypeCacheTypes.DataReaderType:
                    return typeof(DbDataReader);
                case TypeCacheTypes.ExecutingAssemblyType:
                    return Assembly.GetExecutingAssembly().GetType();
                case TypeCacheTypes.ObjectType:
                    return typeof(object);
                case TypeCacheTypes.StringType:
                    return typeof(string);
                default:
                    return null;
            }
        }
    }
}
