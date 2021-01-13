using RepoDb.Contexts.Execution;
using System.Collections.Concurrent;

namespace RepoDb.Contexts.Cachers
{
    /// <summary>
    /// A class that is being used to cache the execution context of the Merge operation.
    /// </summary>
    public static class MergeExecutionContextCache
    {
        private static ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Flushes all the cached execution context.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="context"></param>
        internal static void Add<TEntity>(string key,
            MergeExecutionContext<TEntity> context)
            where TEntity : class =>
            cache.TryAdd(key, context);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static MergeExecutionContext<TEntity> Get<TEntity>(string key)
            where TEntity : class
        {
            if (cache.TryGetValue(key, out var result))
            {
                return result as MergeExecutionContext<TEntity>;
            }
            return null;
        }
    }
}
