using RepoDb.Interfaces;
using System;
using System.Linq;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the identity property of the data entity type.
    /// </summary>
    public class IdentityResolver : IResolver<Type, ClassProperty>
    {
        /// <summary>
        /// Resolves the identity <see cref="ClassProperty"/> of the data entity type.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The instance of the identity <see cref="ClassProperty"/> object.</returns>
        public ClassProperty Resolve(Type entityType)
        {
            var properties = PropertyCache.Get(entityType);

            // Get the first entry with Identity attribute
            var property = properties?
                .FirstOrDefault(p => p.GetIdentityAttribute() != null);

            // Get from the implicit mapping
            if (property == null)
            {
                property = IdentityMapper.Get(entityType);
            }

            // Return the instance
            return property;
        }
    }
}
