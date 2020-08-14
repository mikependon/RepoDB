using RepoDb.Contexts.Execution;
using System.Collections.Concurrent;

namespace RepoDb.Contexts.Cachers
{
    /// <summary>
    /// A class that is used to cache the execution context of the MergeAll operation.
    /// </summary>
    public static class InsertAllExecutionContextCache
    {
        private static ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Flushes all the cached execution context.
        /// </summary>
        public static void Flush()
        {
            cache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="context"></param>
        internal static void Add<TEntity>(string key,
            InsertAllExecutionContext<TEntity> context)
            where TEntity : class
        {
            cache.TryAdd(key, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static InsertAllExecutionContext<TEntity> Get<TEntity>(string key)
            where TEntity : class
        {
            var result = (object)null;
            if (cache.TryGetValue(key, out result))
            {
                return result as InsertAllExecutionContext<TEntity>;
            }
            return null;
        }
    }
}
