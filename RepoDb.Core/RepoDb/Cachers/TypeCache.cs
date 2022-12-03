using System;
using System.Collections.Concurrent;

namespace RepoDb;

public static class TypeCache
{
    private static readonly ConcurrentDictionary<Type, CachedType> cache = new();
    private static readonly CachedType nullCachedType = new(null);

    public static CachedType Get(Type type)
    {
        return type is null 
            ? nullCachedType 
            : cache.GetOrAdd(type, new CachedType(type));
    }
}