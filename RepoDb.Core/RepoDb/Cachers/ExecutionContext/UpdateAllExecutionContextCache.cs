using RepoDb.Contexts.Execution;
using System.Collections.Concurrent;

namespace RepoDb.Cachers.ExecutionContext
{
    /// <summary>
    /// A class that is used to cache the execution context of the UpdateAll operation.
    /// </summary>
    public static class UpdateAllExecutionContextCache
    {
        private static ConcurrentDictionary<int, object> cache =
            new ConcurrentDictionary<int, object>();

        /// <summary>
        /// Flushes all the cached execution context.
        /// </summary>
        public static void Flush()
        {
            cache.Clear();
        }

        internal static void Add<TEntity>(UpdateAllExecutionContext<TEntity> context)
            where TEntity : class
        {
            cache.TryAdd(context.GetHashCode(), context);
        }


        //internal static UpdateAllExecutionContext<TEntity> Get<TEntity>(int key)
        //    where TEntity : class
        //{
        //    // Variables
        //    var key = tableName.GetHashCode() + batchSize.GetHashCode();
        //    var context = (UpdateAllExecutionContext<TEntity>)null;

        //    // The fields hashcodes
        //    if (fields?.Any() == true)
        //    {
        //        foreach (var field in fields)
        //        {
        //            key += field.GetHashCode();
        //        }
        //    }

        //    // Get the cache
        //    if (cache.TryGetValue(key, out context) == false)
        //    {
        //        context = callback(batchSize);
        //        cache.TryAdd(key, context);
        //    }

        //    // Return the cache
        //    return context;
        //}
    }
}
