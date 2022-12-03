using System;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb;

public class CachedType
{
    private static Type value;

    private readonly Lazy<PropertyInfo[]> lazyGetProperties = new(() => value.GetProperties());
    private readonly Lazy<Type> lazyGetUnderlyingType = new(() => value.GetUnderlyingType());
    private readonly Lazy<bool> lazyIsAnonymousType = new(() => value.IsAnonymousType());
    private readonly Lazy<bool> lazyIsClassType = new(() => value.IsClassType());
    private readonly Lazy<bool> lazyIsDictionaryStringObject = new(() => value.IsDictionaryStringObject());
    private readonly Lazy<bool> lazyIsNullable = new(() => value.IsNullable());

    public CachedType(Type type) => value = type;

    public PropertyInfo[] GetProperties() => lazyGetProperties.Value;
    
    public Type GetUnderlyingType() => lazyGetUnderlyingType.Value;
    
    public bool IsAnonymousType() => lazyIsAnonymousType.Value;

    public bool IsClassType() => lazyIsClassType.Value;
    
    public bool IsDictionaryStringObject() => lazyIsDictionaryStringObject.Value;
    
    public bool IsNullable() => lazyIsNullable.Value;
}