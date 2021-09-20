using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the associated <see cref="PropertyValueAttribute"/> objects of the property.
    /// </summary>
    public class PropertyValueAttributeResolver : IResolver<PropertyInfo, IEnumerable<PropertyValueAttribute>>
    {
        /// <summary>
        /// Resolves the associated <see cref="PropertyValueAttribute"/> objects of the property.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be resolved.</param>
        /// <returns>The list of associated <see cref="PropertyValueAttribute"/> objects on the property.</returns>
        public IEnumerable<PropertyValueAttribute> Resolve(PropertyInfo propertyInfo) =>
            propertyInfo.GetPropertyValueAttributes(); // PropertyValueAttributeMapper.Get(propertyInfo.DeclaringType, propertyInfo);
    }
}
