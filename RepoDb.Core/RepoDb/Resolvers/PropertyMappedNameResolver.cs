using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is used to resolve the database object name mappingss of the data entity type.
    /// </summary>
    public class PropertyMappedNameResolver : IResolver<PropertyInfo, string>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyMappedNameResolver"/> object.
        /// </summary>
        public PropertyMappedNameResolver() { }

        /// <summary>
        /// Resolves the cached column name mappings of the property.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        public string Resolve(PropertyInfo propertyInfo) =>
            PropertyInfoExtension.GetMappedName(propertyInfo);
    }
}
