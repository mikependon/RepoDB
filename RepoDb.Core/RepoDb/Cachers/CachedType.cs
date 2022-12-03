using System;
using RepoDb.Extensions;

namespace RepoDb;

public class CachedType
{
    private static Type value;

    private readonly Lazy<Type> lazyGetUnderlyingType = new(() => value.GetUnderlyingType());
    private readonly Lazy<bool> lazyIsClassType = new(() => value.IsClassType());
    private readonly Lazy<bool> lazyIsDictionaryStringObject = new(() => value.IsDictionaryStringObject());

    public CachedType(Type type) => value = type;

    public bool IsClassType() => lazyIsClassType.Value;

    public bool IsDictionaryStringObject() => lazyIsDictionaryStringObject.Value;

    public Type GetUnderlyingType() => lazyGetUnderlyingType.Value;
}