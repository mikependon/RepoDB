using System;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb;

public class CachedType
{
    private readonly Lazy<PropertyInfo[]> lazyGetProperties;
    private readonly Lazy<Type> lazyGetUnderlyingType;
    private readonly Lazy<bool> lazyIsAnonymousType;
    private readonly Lazy<bool> lazyIsClassType;
    private readonly Lazy<bool> lazyIsDictionaryStringObject;
    private readonly Lazy<bool> lazyIsNullable;

    public CachedType(Type type)
    {
        lazyGetUnderlyingType = new Lazy<Type>(() => type?.GetUnderlyingType());
        
        if (type is null) return;

        lazyGetProperties = new Lazy<PropertyInfo[]>(type.GetProperties);
        lazyIsAnonymousType = new Lazy<bool>(type.IsAnonymousType);
        lazyIsClassType = new Lazy<bool>(type.IsClassType);
        lazyIsDictionaryStringObject = new Lazy<bool>(type.IsDictionaryStringObject);
        lazyIsNullable = new Lazy<bool>(type.IsNullable);
    }

    public PropertyInfo[] GetProperties() => lazyGetProperties.Value;

    public Type GetUnderlyingType() => lazyGetUnderlyingType.Value;

    public bool IsAnonymousType() => lazyIsAnonymousType.Value;

    public bool IsClassType() => lazyIsClassType.Value;

    public bool IsDictionaryStringObject() => lazyIsDictionaryStringObject.Value;

    public bool IsNullable() => lazyIsNullable.Value;
}