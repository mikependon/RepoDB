using System;
using System.Collections.Concurrent;

namespace RepoDb;

/// <summary>
/// A class that is being used to cache the type.
/// </summary>
public static class TypeCache
{
    private static readonly ConcurrentDictionary<Type, CachedType> cache = new();
    private static readonly CachedType nullCachedType = new(null);

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
}