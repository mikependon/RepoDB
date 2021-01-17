using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the list of <see cref="Field"/> objects of the data entity.
    /// </summary>
    public static class FieldCache
    {
        private static readonly ConcurrentDictionary<int, IEnumerable<Field>> cache = new ConcurrentDictionary<int, IEnumerable<Field>>();

        #region Methods

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the cached list of <see cref="Field"/> objects of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached list <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Get(Type entityType)
        {
            if (entityType.IsClassType() == false)
            {
                return null;
            }

            // Variables
            var key = GenerateHashCode(entityType);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = entityType.AsFields();
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="Field"/> objects.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        #endregion
    }
}
