using System;
using RepoDb.Extensions;

public class CachedType
{
    private static Type value;
    
    private readonly Lazy<Type> lazyGetUnderlyingType = new(() => value.GetUnderlyingType());
    private readonly Lazy<bool> lazyIsDictionaryStringObject = new(() => value.IsDictionaryStringObject());

    public CachedType(Type type)
    {
        value = type;
    }

    public bool IsDictionaryStringObject() => lazyIsDictionaryStringObject.Value;

    public Type GetUnderlyingType() => lazyGetUnderlyingType.Value;
}