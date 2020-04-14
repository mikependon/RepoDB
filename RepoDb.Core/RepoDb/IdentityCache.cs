using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to cache the identity property of the data entity.
    /// </summary>
    public static class IdentityCache
    {
        private static readonly ConcurrentDictionary<int, ClassProperty> m_cache = new ConcurrentDictionary<int, ClassProperty>();

        #region Methods

        /// <summary>
        /// Gets the cached identity property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached identity property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached identity property of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached identity property.</returns>
        public static ClassProperty Get(Type entityType)
        {
            // Variables for the cache
            var key = GenerateHashCode(entityType);
            var property = (ClassProperty)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out property) == false)
            {
                // Get all with IsPrimary() flags
                var properties = PropertyCache.Get(entityType).Where(p => p.IsIdentity() == true);

                // Check if there is forced [Identity] attribute
                property = properties.FirstOrDefault(p => p.GetIdentityAttribute() != null);

                // Otherwise, get the first one
                if (property == null)
                {
                    property = properties?.FirstOrDefault();
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
        /// Flushes all the existing cached identity <see cref="ClassProperty"/> objects.
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
