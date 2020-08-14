using RepoDb.Contexts.Execution;
using System.Collections.Concurrent;

namespace RepoDb.Cachers.ExecutionContext
{
    /// <summary>
    /// A class that is used to cache the execution context of the MergeAll operation.
    /// </summary>
    public static class MergeAllExecutionContextCache
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

        internal static void Add<TEntity>(MergeAllExecutionContext<TEntity> context)
            where TEntity : class
        {
            cache.TryAdd(context.GetHashCode(), context);
        }
    }
}
