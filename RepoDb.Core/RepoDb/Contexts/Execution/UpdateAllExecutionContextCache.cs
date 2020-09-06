using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// A class that is used to cache the context of the update-all executions.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public static class UpdateAllExecutionContextCache<TEntity>
        where TEntity : class
    {
        private static ConcurrentDictionary<int, UpdateAllExecutionContext<TEntity>> cache =
            new ConcurrentDictionary<int, UpdateAllExecutionContext<TEntity>>();

        /// <summary>
        /// Flushes all the cached execution text for update-all operation.
        /// </summary>
        public static void Flush()
        {
            cache.Clear();
        }

        /// <summary>
        /// Gets the cached execution context.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of the <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="callback">The callback function to be invoked.</param>
        /// <returns>The instance of the cached execution context.</returns>
        internal static UpdateAllExecutionContext<TEntity> Get(string tableName,
            IEnumerable<Field> fields,
            int batchSize,
            Func<int, UpdateAllExecutionContext<TEntity>> callback)
        {
            // Variables
            var key = tableName.GetHashCode() + batchSize.GetHashCode();
            var context = (UpdateAllExecutionContext<TEntity>)null;

            // The fields hashcodes
            if (fields?.Any() == true)
            {
                foreach (var field in fields)
                {
                    key += field.GetHashCode();
                }
            }

            // Get the cache
            if (cache.TryGetValue(key, out context) == false)
            {
                context = callback(batchSize);
                cache.TryAdd(key, context);
            }

            // Return the cache
            return context;
        }
    }
}
