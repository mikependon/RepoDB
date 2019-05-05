using System;
using System.Collections.Concurrent;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// A class used to cache the context of the insert-all executions.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal static class InsertAllExecutionContextCache<TEntity>
        where TEntity : class
    {
        private static ConcurrentDictionary<string, InsertAllExecutionContext<TEntity>> m_cache =
            new ConcurrentDictionary<string, InsertAllExecutionContext<TEntity>>();

        /// <summary>
        /// Gets the cached execution context.
        /// </summary>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="callback">The callback function to be invoked.</param>
        /// <returns>The instance of the cached execution context.</returns>
        public static InsertAllExecutionContext<TEntity> Get(int batchSize,
            Func<int, InsertAllExecutionContext<TEntity>> callback)
        {
            var key = string.Concat(typeof(TEntity).FullName, ".", batchSize);
            var context = (InsertAllExecutionContext<TEntity>)null;
            if (m_cache.TryGetValue(key, out context) == false)
            {
                context = callback(batchSize);
                m_cache.TryAdd(key, context);
            }
            return context;
        }
    }
}
