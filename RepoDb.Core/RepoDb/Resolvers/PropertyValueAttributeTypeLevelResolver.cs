using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the associated <see cref="PropertyValueAttribute"/> objects of the .NET CLR type.
    /// </summary>
    public class PropertyValueAttributeTypeLevelResolver : IResolver<Type, IEnumerable<PropertyValueAttribute>>
    {
        /// <summary>
        /// Resolves the associated <see cref="PropertyValueAttribute"/> objects of the property.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The list of associated <see cref="PropertyValueAttribute"/> objects on the property.</returns>
        public IEnumerable<PropertyValueAttribute> Resolve(Type type) =>
            type.GetPropertyValueAttributes();
    }
}
