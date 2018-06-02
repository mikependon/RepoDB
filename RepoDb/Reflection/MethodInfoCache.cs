using System;
using System.Collections.Generic;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A cacher for a <i>System.Reflection.MethodInfo</i>.
    /// </summary>
    public static class MethodInfoCache
    {
        private static readonly IDictionary<MethodInfoTypes, MethodInfo> _cache = new Dictionary<MethodInfoTypes, MethodInfo>();
        private static readonly IDictionary<Type, MethodInfo> _convertToTypeMethodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Gets the <i>System.Reflection.MethodInfo</i> based on type.
        /// </summary>
        /// <param name="type">The type of <i>System.Reflection.MethodInfo</i> to be cached.</param>
        /// <returns>A <i>System.Reflection.MethodInfo</i> object.</returns>
        public static MethodInfo Get(MethodInfoTypes type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache.Add(type, ReflectionFactory.CreateMethod(type));
            }
            return _cache[type];
        }

        /// <summary>
        /// Gets a <i>System.Convert.To</i> type name method.
        /// </summary>
        /// <param name="type">A type of Method to get.</param>
        /// <returns>A <i>System.Reflection.MethodInfo</i> object.</returns>
        public static MethodInfo GetConvertTo(Type type)
        {
            if (!_convertToTypeMethodCache.ContainsKey(type))
            {
                var methodInfo = TypeCache.Get(TypeTypes.Convert)
                        .GetMethod($"To{type.Name}", TypeArrayCache.Get(TypeTypes.Object));
                _convertToTypeMethodCache.Add(type, methodInfo);
            }
            return _convertToTypeMethodCache[type];
        }

    }
}
