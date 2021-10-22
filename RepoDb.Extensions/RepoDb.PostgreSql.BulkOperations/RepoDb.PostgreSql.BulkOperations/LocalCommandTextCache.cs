using System;
using System.Collections.Concurrent;

namespace RepoDb.PostgreSql.BulkOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal static class LocalCommandTextCache
    {
        private static ConcurrentDictionary<int, string> cache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandText"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static bool Add(int key,
            string commandText,
            bool force)
        {
            if (cache.TryGetValue(key, out var _) && force == false)
            {
                throw new InvalidOperationException();
            }

            return cache.TryAdd(key, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(int key)
        {
            if (cache.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
