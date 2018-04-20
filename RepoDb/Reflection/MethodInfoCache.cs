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
        private static readonly IDictionary<MethodInfoTypes, MethodInfo> _cache = new Dictionary<MethodInfoTypes, MethodInfo>();
        private static readonly IDictionary<Type, MethodInfo> _convertToTypeMethodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Gets the System.Reflection.MethodInfo based on type.
        /// </summary>
        /// <param name="type">The type of System.Reflection.MethodInfo to be cached.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo Get(MethodInfoTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateMethod(type));
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
                var methodInfo = TypeCache.Get(TypeTypes.ConvertType)
                        .GetMethod($"To{type.Name}", TypeArrayCache.Get(TypeArrayTypes.ObjectTypes));
                _convertToTypeMethodCache.Add(type, methodInfo);
            }
            return _convertToTypeMethodCache[type];
        }

    }
}
