using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the primary property of the entity.
    /// </summary>
    public static class PrimaryCache
    {
        private static readonly ConcurrentDictionary<int, ClassProperty> m_cache = new ConcurrentDictionary<int, ClassProperty>();

        #region Methods

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached primary property of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached primary property.</returns>
        public static ClassProperty Get(Type type)
        {
            // Variables for the cache
            var property = (ClassProperty)null;
            var key = type.FullName.GetHashCode();

            // Try get the value
            if (m_cache.TryGetValue(key, out property) == false)
            {
                // Get all with IsPrimary() flags
                var properties = PropertyCache.Get(type).Where(p => p.IsPrimary() == true);

                // Check if there is forced [Primary] attribute
                property = properties.FirstOrDefault(p => p.GetPrimaryAttribute() != null);

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
        /// Flushes all the existing cached primary <see cref="ClassProperty"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
