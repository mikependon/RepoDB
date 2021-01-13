using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Reflection;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the database object name mappings of the data entity type.
    /// </summary>
    public class PropertyMappedNameResolver : IResolver<PropertyInfo, Type, string>
    {
        /// <summary>
        /// Resolves the cached column name mappings of the property.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        /// <param name="declaringType">The declaring type of the target property. Usually, the type of the parent derived class, not the base class.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        public string Resolve(PropertyInfo propertyInfo,
            Type declaringType) =>
            PropertyInfoExtension.GetMappedName(propertyInfo, declaringType ?? propertyInfo.DeclaringType);
    }
}
