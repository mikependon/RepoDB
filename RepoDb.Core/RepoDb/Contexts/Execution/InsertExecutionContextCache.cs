using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// A class used to cache the context of the insert executions.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal static class InsertExecutionContextCache<TEntity>
        where TEntity : class
    {
        private static ConcurrentDictionary<int, InsertExecutionContext<TEntity>> m_cache =
            new ConcurrentDictionary<int, InsertExecutionContext<TEntity>>();

        /// <summary>
        /// Gets the cached execution context.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of the <see cref="Field"/> objects.</param>
        /// <param name="callback">The callback function to be invoked.</param>
        /// <returns>The instance of the cached execution context.</returns>
        public static InsertExecutionContext<TEntity> Get(string tableName,
            IEnumerable<Field> fields,
            Func<InsertExecutionContext<TEntity>> callback)
        {
            // Variables
            var key = tableName.GetHashCode();
            var context = (InsertExecutionContext<TEntity>)null;

            // The fields hashcodes
            if (fields?.Any() == true)
            {
                foreach (var field in fields)
                {
                    key += field.GetHashCode();
                }
            }

            // Get the cache
            if (m_cache.TryGetValue(key, out context) == false)
            {
                context = callback();
                m_cache.TryAdd(key, context);
            }

            // Return the cache
            return context;
        }
    }
}
