using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to cache the primary property of the data entity.
    /// </summary>
    public static class PrimaryCache
    {
        private static readonly ConcurrentDictionary<int, ClassProperty> m_cache = new ConcurrentDictionary<int, ClassProperty>();

        #region Methods

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get(Type entityType)
        {
            // Variables for the cache
            var property = (ClassProperty)null;
            var key = GenerateHashCode(entityType);

            // Try get the value
            if (m_cache.TryGetValue(key, out property) == false)
            {
                var properties = PropertyCache.Get(entityType);

                // Get all with IsIdentity() flags
                property = properties?
                    .FirstOrDefault(p => p.GetPrimaryAttribute() != null);

                // Otherwise, get the first one
                if (property == null)
                {
                    property = PrimaryMapper.Get(entityType);
                }

                // Id Property
                if (property == null)
                {
                    property = properties?
                       .FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, "id", StringComparison.OrdinalIgnoreCase));
                }

                // Type.Name + Id
                if (property == null)
                {
                    property = properties?
                       .FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, string.Concat(p.GetDeclaringType().Name, "id"), StringComparison.OrdinalIgnoreCase));
                }

                // Mapping.Name + Id
                if (property == null)
                {
                    property = properties?
                       .FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, string.Concat(ClassMappedNameCache.Get(p.GetDeclaringType()), "id"), StringComparison.OrdinalIgnoreCase));
                }

                // Add to the cache (whatever)
                m_cache.TryAdd(key, property);
            }

            // Return the value
            return property;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached primary <see cref="ClassProperty"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type)
        {
            return TypeExtension.GenerateHashCode(type);
        }

        #endregion
    }
}
