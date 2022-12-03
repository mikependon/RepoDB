using System;
using System.Collections.Concurrent;

namespace RepoDb;

public static class TypeCache
{
    private static readonly ConcurrentDictionary<Type, CachedType> cache = new();
    private static readonly CachedType nullCachedType = new(null);

    public static CachedType Get(Type type)
    {
        if (type is null)
        {
            return nullCachedType;
        }

        if (cache.TryGetValue(type, out var result) == false)
        {
            result = new CachedType(type);
            cache.TryAdd(type, result);
        }
            
        return result;
    }
}