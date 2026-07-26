using System;
using System.Collections.Concurrent;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// An in-process cache of generated SQL command texts, keyed by a hash of the inputs that shaped them
    /// (table names, field lists, qualifiers, identity behavior). Avoids re-running the <see cref="QueryBuilder"/>
    /// pipeline for the same shape of bulk call more than once per process lifetime.
    /// </summary>
    internal static class LocalCommandTextCache
    {
        private static readonly ConcurrentDictionary<int, string> cache = new();

        /// <summary>
        /// Adds a command text to the cache.
        /// </summary>
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
        /// Gets a previously cached command text, or <c>null</c> if not present.
        /// </summary>
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
