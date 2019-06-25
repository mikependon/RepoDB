using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// A class used to cache the context of the merge executions.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public static class MergeExecutionContextCache<TEntity>
        where TEntity : class
    {
        private static ConcurrentDictionary<int, MergeExecutionContext<TEntity>> m_cache =
            new ConcurrentDictionary<int, MergeExecutionContext<TEntity>>();

        /// <summary>
        /// Flushes all the cached execution text for merge operation.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Gets the cached execution context.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of the <see cref="Field"/> objects.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="callback">The callback function to be invoked.</param>
        /// <returns>The instance of the cached execution context.</returns>
        internal static MergeExecutionContext<TEntity> Get(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Func<MergeExecutionContext<TEntity>> callback)
        {
            // Variables
            var key = tableName.GetHashCode();
            var context = (MergeExecutionContext<TEntity>)null;

            // The fields hashcodes
            if (fields?.Any() == true)
            {
                foreach (var field in fields)
                {
                    key ^= field.GetHashCode();
                }
            }

            // The qualifiers hashcodes
            if (qualifiers?.Any() == true)
            {
                foreach (var field in qualifiers)
                {
                    key ^= field.GetHashCode();
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
