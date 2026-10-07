#region Copyright Attributions

// Copyright (c) 2022 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the type.
    /// </summary>
    public static class TypeCache
    {
        private static readonly ConcurrentDictionary<Type, CachedType> cache = new ConcurrentDictionary<Type, CachedType>();
        private static readonly CachedType nullCachedType = new CachedType(type: null);
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> propertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Gets the cached <see cref="CachedType"/> object that is being mapped on a type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The mapped <see cref="CachedType"/> object of the target type.</returns>
        public static CachedType Get(Type type)
        {
            if (type is null)
            {
                return nullCachedType;
            }

            if (cache.TryGetValue(type, out var result))
            {
                return result;
            }

            result = new CachedType(type);
            cache.TryAdd(type, result);

            return result;
        }

        /// <summary>
        /// Gets the cached public properties of the type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The list of the public properties of the type.</returns>
        public static PropertyInfo[] GetProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type)
        {
            if (propertiesCache.TryGetValue(type, out var result))
            {
                return result;
            }

            result = type.GetProperties();
            propertiesCache.TryAdd(type, result);

            return result;
        }
    }
}
