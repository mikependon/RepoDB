using RepoDb.Contexts.Execution;
using System;
using System.Collections.Concurrent;

namespace RepoDb.Contexts.Cachers
{
    /// <summary>
    /// A class that is being used to cache the execution context of the Insert operation.
    /// </summary>
    public static class InsertExecutionContextCache
    {
        private static ConcurrentDictionary<string, InsertExecutionContext> cache = new();

        /// <summary>
        /// Flushes all the cached execution context.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="context"></param>
        internal static void Add(Type type,
            string key,
            InsertExecutionContext context) =>
            cache.TryAdd(key, context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static InsertExecutionContext Get(string key)
        {
            if (cache.TryGetValue(key, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
