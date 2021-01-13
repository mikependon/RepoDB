using RepoDb.Interfaces;
using System;
using System.Linq;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the primary property of the data entity type.
    /// </summary>
    public class PrimaryResolver : IResolver<Type, ClassProperty>
    {
        /// <summary>
        /// Resolves the primary <see cref="ClassProperty"/> of the data entity type.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The instance of the primary <see cref="ClassProperty"/> object.</returns>
        public ClassProperty Resolve(Type entityType)
        {
            var properties = PropertyCache.Get(entityType);

            // Check for the properties
            if (properties == null)
            {
                return null;
            }

            // Get the first entry with Primary attribute
            var property = properties
                .FirstOrDefault(p => p.GetPrimaryAttribute() != null);

            // Get from the implicit mapping
            if (property == null)
            {
                property = PrimaryMapper.Get(entityType);
            }

            // Id Property
            if (property == null)
            {
                property = properties
                    .FirstOrDefault(p =>
                        string.Equals(p.PropertyInfo.Name, "id", StringComparison.OrdinalIgnoreCase));
            }

            // Type.Name + Id
            if (property == null)
            {
                property = properties
                    .FirstOrDefault(p =>
                        string.Equals(p.PropertyInfo.Name, string.Concat(p.GetDeclaringType().Name, "id"), StringComparison.OrdinalIgnoreCase));
            }

            // Mapping.Name + Id
            if (property == null)
            {
                property = properties
                    .FirstOrDefault(p =>
                        string.Equals(p.PropertyInfo.Name, string.Concat(ClassMappedNameCache.Get(p.GetDeclaringType()), "id"), StringComparison.OrdinalIgnoreCase));
            }

            // Return the instance
            return property;
        }
    }
}
