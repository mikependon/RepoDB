using System;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb;

/// <summary>
/// A class holds the type with lazy properties.
/// </summary>
public class CachedType
{
    private readonly Lazy<PropertyInfo[]> lazyGetProperties;
    private readonly Lazy<Type> lazyGetUnderlyingType;
    private readonly Lazy<bool> lazyIsAnonymousType;
    private readonly Lazy<bool> lazyIsClassType;
    private readonly Lazy<bool> lazyIsDictionaryStringObject;
    private readonly Lazy<bool> lazyIsNullable;

    /// <summary>
    /// Creates a new instance of <see cref="CachedType" /> object.
    /// </summary>
    /// <param name="type">The target type.</param>
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

    /// <summary>
    /// Returns all the public properties of the current Type.
    /// </summary>
    /// <returns>
    /// An array of PropertyInfo objects representing all public properties of the current Type.
    /// -or- An empty array of type PropertyInfo, if the current Type does not have public properties.
    /// </returns>
    public PropertyInfo[] GetProperties() => lazyGetProperties.Value;

    /// <summary>
    /// Returns the underlying type of the current type. If there is no underlying type, this will return the current type.
    /// </summary>
    /// <returns>The underlying type or the current type.</returns>
    public Type GetUnderlyingType() => lazyGetUnderlyingType.Value;

    /// <summary>
    /// Checks whether the current type is an anonymous type.
    /// </summary>
    /// <returns>Returns true if the current type is an anonymous class.</returns>
    public bool IsAnonymousType() => lazyIsAnonymousType.Value;

    /// <summary>
    /// Checks whether the current type is a class.
    /// </summary>
    /// <returns>Returns true if the current type is a class.</returns>
    public bool IsClassType() => lazyIsClassType.Value;

    /// <summary>
    /// Checks whether the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).
    /// </summary>
    /// <returns>Returns true if the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).</returns>
    public bool IsDictionaryStringObject() => lazyIsDictionaryStringObject.Value;

    /// <summary>
    /// Checks whether the current type is wrapped within a <see cref="Nullable{T}"/> object.
    /// </summary>
    /// <returns>Returns true if the current type is wrapped within a <see cref="Nullable{T}"/> object.</returns>
    public bool IsNullable() => lazyIsNullable.Value;
}