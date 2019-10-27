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

        #region

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get<TEntity>()
            where TEntity : class
        {
            return Get(typeof(TEntity));
        }

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get(Type type)
        {
            var key = type.FullName.GetHashCode();
            var result = (IEnumerable<Field>)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = type.AsFields();
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

        #endregion
    }
}
