using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the properties of the data entity as a list <see cref="Field"/> objects.
    /// </summary>
    public static class FieldCache
    {
        private static readonly ConcurrentDictionary<int, IEnumerable<Field>> m_cache = new ConcurrentDictionary<int, IEnumerable<Field>>();

        #region Methods

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get(Type entityType)
        {
            var key = GenerateHashCode(entityType);
            var result = (IEnumerable<Field>)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = entityType.AsFields();
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="Field"/> objects.
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
