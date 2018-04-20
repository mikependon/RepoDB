using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a System.Reflection.MethodInfo.
    /// </summary>
    public static class MethodInfoCache
    {
        private static readonly IDictionary<MethodInfoCacheTypes, MethodInfo> _cache = new Dictionary<MethodInfoCacheTypes, MethodInfo>();
        private static readonly IDictionary<Type, MethodInfo> _convertToTypeMethodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Gets the System.Reflection.MethodInfo based on type.
        /// </summary>
        /// <param name="type">The type of System.Reflection.MethodInfo to be cached.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo Get(MethodInfoCacheTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, Create(type));
            }
            return _cache[type];
        }

        /// <summary>
        /// Gets a System.Convert.To<Type> method.
        /// </summary>
        /// <param name="type">A type of Method to get.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo GetConvertTo(Type type)
        {
            if (!_convertToTypeMethodCache.ContainsKey(type))
            {
                var methodInfo = TypeCache.Get(TypeCacheTypes.ConvertType)
                        .GetMethod($"To{type.Name}", TypeArrayCache.Get(TypeArrayCacheTypes.ObjectTypes));
                _convertToTypeMethodCache.Add(type, methodInfo);
            }
            return _convertToTypeMethodCache[type];
        }

        /// <summary>
        /// Creates a System.Reflection.MethodInfo object based on type.
        /// </summary>
        /// <param name="type">A type of System.Reflection.MethodInfo object.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo Create(MethodInfoCacheTypes type)
        {
            switch (type)
            {
                case MethodInfoCacheTypes.ConvertToStringMethod:
                    return TypeCache.Get(TypeCacheTypes.ConvertType)
                        .GetMethod("ToString", TypeArrayCache.Get(TypeArrayCacheTypes.ObjectTypes));
                case MethodInfoCacheTypes.DataReaderGetItemMethod:
                    return TypeCache.Get(TypeCacheTypes.DataReaderType)
                        .GetProperty("Item", TypeArrayCache.Get(TypeArrayCacheTypes.StringTypes)).GetMethod;
                default:
                    return null;
            }
        }
    }
}
